using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.DataImporter.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportService.Orders.Impl
{
    public class OrderExportService : IOrderExportService
    {
       internal List<FclExportOrderDto> OrdersPendingExport;
        private IIntegrationDocumentRepository _integrationDocumentRepository;
        private IMainOrderRepository _mainOrderRepository;
        private IOrderWorkflow _orderWorkflow;
    


        public OrderExportService(IIntegrationDocumentRepository integrationDocumentRepository, IMainOrderRepository mainOrderRepository, IOrderWorkflow orderWorkflow)
        {
           _integrationDocumentRepository = integrationDocumentRepository;
            _mainOrderRepository = mainOrderRepository;
            _orderWorkflow = orderWorkflow;
            OrdersPendingExport = new List<FclExportOrderDto>();
           
        }

        public void ExportOrders()
        {
            //1.Get the orders
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Fetching orders pending export");
           GetOrders();
          //2.Export the orders
            Export();
            if (!exportfailed)
            {  //3. Close the orders
                CloseFclExportedOrders();
            }
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Export terminated due to error on exporting");  
            }
        }

        private bool exportfailed = false;
        private  void Export()
        {
            if(OrdersPendingExport.Any())
            { 
                var orders = OrdersPendingExport.Select(OrderExportHelper.MapExport).ToList();
                var orderToCsv=new List<ExportOrderItem>();
                foreach (var order in orders)
                {
                    orderToCsv.AddRange(order);
                }
                var outputFileDescription = new CsvFileDescription
                {
                    // cool - I can specify my own separator!
                    SeparatorChar = ',',
                    FirstLineHasColumnNames =
                        false,
                    QuoteAllFields = true,
                    EnforceCsvColumnAttribute =
                        true
                };
              try
              {
                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Attempting to export {0} orders",orders.Count);
                  //var cc = new CsvContext();
                  //cc.Write(orderToCsv, OrderExportHelper.GetExportFileName("Orders"), outputFileDescription);
                  DumpExportFilesAsync(orderToCsv.ToCsv(), OrderExportHelper.GetExportFileName("Orders"));

                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Export compeleted for={0} orders", orders.Count));
              }catch(Exception ex)
              {
                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + "Error occured while exporting..See error logs for details");
                  exportfailed = true;
                  FileUtility.LogError(ex.Message);

              }
                
                
                
            }
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " No  orders exported");
            }
        }


        private void GetOrders()
        {

            var orders = _integrationDocumentRepository.GetFclOrdersPendingExport(OrderType.OutletToDistributor, "");
            OrdersPendingExport.AddRange(orders);
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                   string.Format(" Fetched {0} orders for export", orders.Count));

        }

        public void CloseFclExportedOrders()
        {
            if(!OrdersPendingExport.Any())return;
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                  string.Format(" Attempting to close {0} exported orders", OrdersPendingExport.Count));
            var orderRefs =
                OrdersPendingExport.OrderBy(p => p.GenericOrderReference).Where(p=>!string.IsNullOrEmpty(p.GenericOrderReference)).Select(p => p.GenericOrderReference)
                    .ToList();
            var orders = orderRefs.Distinct().Select(orderRef => _mainOrderRepository.GetByDocumentReference(orderRef)).ToList();
            bool exportedWithError = false;
            foreach (var order in orders)
            {
                try
                {
                    var removed =
                       OrdersPendingExport.FirstOrDefault(p => p.GenericOrderReference == order.DocumentReference);
                    if(removed !=null)
                    OrderExportHelper.MarkAsExported(removed);
                    order.Close();
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                  string.Format(" Attempting to submit order => {0} ", string.IsNullOrEmpty(order.ExternalDocumentReference)
                                                             ? order.DocumentReference
                                                             : order.ExternalDocumentReference));
                    _orderWorkflow.Submit(order);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Order {0} closing",
                                                         string.IsNullOrEmpty(order.ExternalDocumentReference)
                                                             ? order.DocumentReference
                                                             : order.ExternalDocumentReference));
                   
                    if(removed !=null)
                    {
                       OrderExportHelper.MarkAsExported(removed);
                        OrdersPendingExport.Remove(removed);
                        Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Order {0} closed",
                                                         string.IsNullOrEmpty(order.ExternalDocumentReference)
                                                             ? order.DocumentReference
                                                             : order.ExternalDocumentReference));
                    }
                    
                }
                catch (Exception ex)
                {
                    exportedWithError = true;
                    FileUtility.LogError(ex.Message);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Order with ref=>{0} not closed ", order.DocumentReference));
                }
            }
            if(exportedWithError)
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Export orders task done..=>there we errors..See logs for details"));
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                          string.Format(" Export orders task done.."));
            }
        }


        private async void DumpExportFilesAsync(string file, string selectedPath)
        {
            try
            {
                using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(selectedPath, false))
                    {
                        await wr.WriteLineAsync(file);

                    }
                }


            }
            catch (IOException ex)
            {

            }
        }
       

        
    }

    public class ExportOrderItem
    {
        [CsvColumn(FieldIndex = 1)]
        public string OrderReference { get; set; }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string OrderDate { get; set; }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public string SalesmanCode { get; set; }

        [CsvColumn(FieldIndex = 4, CanBeNull = false)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 5, CanBeNull = false)]
        public string ShiptoAddressCode { get; set; }

        [CsvColumn(FieldIndex = 6, CanBeNull = false)]
        public string ProductCode { get; set; }

        [CsvColumn(FieldIndex = 7, CanBeNull = false)]
        public decimal ApprovableQuantity { get; set; }
    }
}
