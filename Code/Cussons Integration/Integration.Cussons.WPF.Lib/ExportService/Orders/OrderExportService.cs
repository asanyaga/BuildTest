using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using GalaSoft.MvvmLight.Messaging;
using Integration.Cussons.WPF.Lib.Utils;
using StructureMap;

namespace Integration.Cussons.WPF.Lib.ExportService.Orders
{
    internal class OrderExportService : IOrderExportService
    {
        private readonly IIntegrationDocumentRepository _integrationDocumentRepository;
        int _itemsFetched = 0;
        public OrderExportService(IIntegrationDocumentRepository integrationDocumentRepository)
        {
            _integrationDocumentRepository = integrationDocumentRepository;
           
        }

        public int GetItemsFetched
        {
            get { return _itemsFetched; }
        }

        public async Task GetAndExportOrders(DateTime startDate, DateTime endDate)
        {
           await Task.Run(() =>
                                      {
                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Fetching orders pending export between {0} and {1}",startDate,endDate));
                                          var orders = _integrationDocumentRepository
                                              .GetPzOrdersPendingExport(startDate, endDate);

                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Fetching done for {0} orders ",orders.Count));
                                          if(orders.Any())
                                          {
                                              ExportOrders(orders);
                                              Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Exports task completed,{0} orders exported",orders.Count));
                                          }
                                          else
                                          {
                                              Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Exports task completed,no orders to export"));
                                          }

                                          
                                      }


                             );
        }

        public async Task GetAndExportOrders( string externalRef)
        {
            await Task.Run(() =>
                               {
                                   var
                                       orders = _integrationDocumentRepository
                                           .GetPzOrdersPendingExport(externalRef);
                                   if (orders != null)
                                   {
                                       ExportOrders(new List<PzCussonsOrderIntegrationDto> { orders });
                                   }
                                   else
                                   {
                                       Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                              string.Format(
                                                                  " Exports task completed,order with ref={0} doesn't exist",externalRef));
                                   }

                               });


        }

        public async Task<int> GetAndExportOrders()
        {
           return await Task.Factory.StartNew(() =>
                                      {
                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                 string.Format(" Fetching orders pending export...."));
                                         
                                          var orders = _integrationDocumentRepository
                                              .GetPzOrdersPendingExport();
                                          _itemsFetched = orders.Count;

                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                 string.Format(" Fetching done for {0} orders ",
                                                                               orders.Count));
                                          if (orders.Any())
                                          {
                                              ExportOrders(orders);
                                              Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                     string.Format(
                                                                         " Exports task completed,{0} orders exported",
                                                                         orders.Count));
                                          }
                                          else
                                          {
                                              Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                     string.Format(
                                                                         " Exports task completed,no orders to export"));
                                          }
                                          return orders.Count;
                                      });
        }

        private async void ExportOrders(IEnumerable<PzCussonsOrderIntegrationDto> toExport)


        {
            var exportrootDirectoryPath = Path.Combine(FileUtility.GetWorkingDirectory("exportpath"),
                                             DateTime.Now.ToString("yyy-MM-dd"));
          var localExportPath =!Directory.Exists(exportrootDirectoryPath)?Path.GetFullPath(Directory.CreateDirectory(exportrootDirectoryPath).FullName):exportrootDirectoryPath;

            var mfgDestinationPath = FileUtility.GetWorkingDirectory("MFGDestinationPath");
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Validating export folders: Local={0} and MFG destination={1}",localExportPath,mfgDestinationPath));
            
            foreach (var order in toExport)
            {
                string externalOrderRef = GetExternalOrderRef(order);
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Export started for order =>{0}", externalOrderRef));

                var sb = new StringBuilder();
                
                sb.AppendLine(string.Format("01::{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|",
                                            externalOrderRef,
                                            order.OutletCode,
                                            order.ShiptoAddressCode,
                                            order.DocumentDateIssued,
                                            order.OrderDateRequired,
                                            order.OutletCode,
                                            order.Currency,
                                            order.Note,
                                            order.ChequeReferenceNo
                                  ));
                int cnt = 1;
                foreach (var lineItem in order.LineItems)
                {
                    sb.AppendLine(string.Format("02::{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                                                cnt,
                                                lineItem.ProductCode,
                                                lineItem.Site, //Site
                                                lineItem.ApprovedQuantity,
                                                lineItem.Value,
                                                lineItem.Location, //Location
                                                order.OrderDateRequired
                                      ));
                    cnt++;
                }
                sb.AppendLine(string.Format("03::{0}", externalOrderRef));

                var orderString = sb.ToString();
                var exportFileName = GenerateExportFileName(cnt);
                string localFileName = Path.Combine(localExportPath, exportFileName);
                string mfgFileName = Path.Combine(mfgDestinationPath, exportFileName);
                if (WriteLocal(localFileName, orderString, externalOrderRef))
                {
                    WriteToMfgPath(mfgFileName, orderString, externalOrderRef, localFileName);
                    MarkAsExported(order);
                }
                else
                {
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Export FAILED for order =>{0}", externalOrderRef));
                }

            }

        }
        void MarkAsExported(PzCussonsOrderIntegrationDto dto)
        {
            var exportedItem = new ExportImportAudit(IntegrationModule.PZCussons, DocumentAuditStatus.Exported,
                                                     Guid.NewGuid(), dto.GenericOrderReference,
                                                     dto.ExternalOrderReference)
                                   {

                                       AuditStatus = DocumentAuditStatus.Exported,
                                       ExternalDocumentRef = dto.ExternalOrderReference,
                                       DateUploaded = DateTime.Now
                                       


                                   };

            
            ObjectFactory.GetInstance<IExportImportAuditRepository>().Save(exportedItem);
        }

        private bool WriteLocal(string localPath,string order,string orderRef)
        {
            try
            {
                File.WriteAllText(localPath, order);
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Write local done for =>{0}", orderRef));
                return true;
            }catch(IOException ex)
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" I/O Error while copying order to  local folder path=>{0}", localPath));
                FileUtility.LogError("WriteLocal=> " + ex.Message);
            }
            catch(Exception ex)
            {
                FileUtility.LogError("WriteLocal=> " + ex.Message);
            }

            return false;
        }

        private void WriteToMfgPath(string mfgPath, string order, string orderRef,string localFileName)
        {
            int trialCounter = 3;
            try
            {
                CopyLocalToMFG(localFileName, mfgPath);
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Copying from local to ERP done for =>{0}", orderRef));
                if(!File.Exists(mfgPath))
                {
                    using (var sw = new StreamWriter(mfgPath))
                    {
                        sw.Write(order);
                        Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Copying from local to ERP done for =>{0}", orderRef));
                        sw.Close();
                    }
                }
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Write to MFG done for =>{0}", orderRef));
            }
            catch (IOException ex)
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" I/O Error while copying order to  MFG path=>{0}",mfgPath));
                FileUtility.LogError("WriteToMfgPath=> " + ex.Message);

                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Attempting to copy from local folder to  MFG path=>{0}", mfgPath));
                bool copySuccess = false;
                while (trialCounter > 0 && !copySuccess)
                {
                   copySuccess= CopyLocalToMFG(localFileName,mfgPath);
                    trialCounter--;
                }
            }
            catch (Exception ex)
            {
                FileUtility.LogError("WriteToMfgPath=> " + ex.Message);
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" UNKNOWN Error while copying order to  MFG path=>{0}", mfgPath));
            }

        }


        bool CopyLocalToMFG(string localPath, string mfgPath)
        {
            if (File.Exists(localPath))
            {
                try
                {
                    var order = File.ReadAllText(localPath);
                    using (var sw = new StreamWriter(mfgPath))
                    {
                        sw.Write(order);
                        Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                               string.Format(" Retry after attempt to ERP done"));
                        sw.Close();
                    }
                    return File.Exists(mfgPath);
                }
                catch (Exception ex)
                {
                    FileUtility.LogError("CopyLocalToMFG=> " + ex.Message);

                }
                return false;
            }
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                   string.Format(" Local file =>{0} doesn't exist", localPath));
            return false;
        }

        string GetExternalOrderRef(PzCussonsOrderIntegrationDto order)
        {
            var orderref = order.ExternalOrderReference;
            return orderref.Length > 8 ? orderref.Substring(0, 8) : orderref;
        }

        string GenerateExportFileName(int randcount)
        {
            DateTime dt = DateTime.Now;
            var rand = new Random();
            var count = Environment.TickCount;
            var range = rand.Next(0, count + randcount);

            var dateStamp = string.Format("{0}{1}{2}{3}", range, dt.Ticks, dt.Millisecond, dt.Day);

            var teststring = string.Format("{0}{1}", "PDA", dateStamp);
            if (teststring.Length > 11)
                teststring = teststring.Substring(0, 11);
            return teststring;
        }
    }
}
