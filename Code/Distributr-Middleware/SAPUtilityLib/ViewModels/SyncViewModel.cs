using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SAPUtilityLib.Masterdata;
using SAPUtilityLib.Masterdata.Impl;
using StructureMap;

namespace SAPUtilityLib.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SyncViewModel : MiddleWareViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SyncViewModel class.
        /// </summary>
        /// 
        private Dictionary<MasterDataItem, List<MasterDataValidationAndImportResponse>> _dictionary;

       

        private DispatcherTimer dtsyncCommand;
        private RelayCommand<Button> _startsyncCommand;
        public RelayCommand<Button> StartSyncCommand
        {
            get { return _startsyncCommand ?? (_startsyncCommand = new RelayCommand<Button>(StartSync)); }
        }
        private RelayCommand<Button> _stopsyncCommand;
        public RelayCommand<Button> StopSyncCommand
        {
            get { return _stopsyncCommand ?? (_stopsyncCommand = new RelayCommand<Button>(StopSync)); }
        }
        private RelayCommand _clearInfoCommand;
        public RelayCommand ClearInfoCommand
        {
            get { return _clearInfoCommand ?? (_clearInfoCommand = new RelayCommand(ClearInfo)); }
        }

        private void ClearInfo()
        {
            SyncInfo = "";
        }

        private void StopSync(Button btn)
        {
            if (btn != null)
            {
                btn.IsEnabled = true;
                btn.Foreground = new SolidColorBrush(Colors.Red);
                btn.Content = "Start";

            }
        
            Informer("Stopping");
            dtsyncCommand.Stop();
            IsTimeStopped = true;
            Informer("Stopped");
        }

        private void StartSync(Button btn)
        {
            if (btn != null)
            {
                btn.IsEnabled = false;
                btn.Foreground = new SolidColorBrush(Colors.GreenYellow);
                btn.Content = "Started";

            }
            SyncInfo = "";
            Informer("Starting");
            dtsyncCommand.Start();
            IsTimeStopped = false;
            Informer("Started");
        }
        public void Informer(string info)
        {
            SyncInfo += "\n\t " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +" --> " +info;
        }
        protected async override void Sync()
        {
            IntializeSyncMasterData();
         await   Task.Factory.StartNew(() =>
                                      {
                                          try
                                          {
                                              dtsyncCommand.Stop();
                                              
                                              Informer("Sap");
                                           
                                              var service = new PullMasterdataService();
                                              bool fileImported = true;
                                              foreach (var masterdata in GenerateMasterDataList)
                                              {

                                                  Informer("\t\t Pulling  " + masterdata);
                                                  if (fileImported)
                                                  {
                                                      var result = service.Import(masterdata);
                                                      fileImported = result.Status;
                                                      if (!fileImported)
                                                      {
                                                          Informer("\t\t Error  " + result.Info);
                                                      }
                                                  }
                                              }
                                              if (fileImported)
                                              {
                                                  Informer("Ready to import");
                                                  Informer("Done...!");
                                              }
                                              else
                                              {
                                                  Informer("Completed with errors...see logs for details!");
                                              }
                                             
                                              Informer("Distributor");

                                              foreach (var masterdata in GenerateMasterDataList)
                                              {
                                                  ImportMasterDataItem(masterdata);
                                              }
                                              Informer("Distributor --> Inventory");
                                                var config = InventoryConfiguration.Load();
                                              if (config == null)
                                              {
                                                  var info = new ExportTransactionsService().PullIntialInventory();
                                                  if (info.Status)
                                                      Informer("\t\tIntial Inventory uploaded successfully ");
                                                  else
                                                  {
                                                      Informer("\t\t " + info.Info);
                                                  }
                                              }
                                              else
                                              {
                                                  var info = new ExportTransactionsService().PullInventoryTransfer();
                                                  if (info.Status)
                                                      Informer("\t\tInventory transfer uploaded successfully ");
                                                  else
                                                  {
                                                      Informer("\t\t " + info.Info);
                                                  }
                                              }
                                              ExportOrders();
                                              ExportSaleOrders();

                                              if (!IsTimeStopped)
                                              {
                                                  dtsyncCommand.Interval = new TimeSpan(0, 10, 0);
                                                   dtsyncCommand.Start();
                                              }
                                                 
                                          }catch(Exception ex)
                                          {
                                              Informer(ex.Message);
                                          }

                                      });
        }

        private void IntializeSyncMasterData()
        {
            var synccon = MasterDataSyncConfiguration.Load();
            if (synccon == null)
            {
                synccon = new MasterDataSyncConfiguration();
                synccon.Item =
                    new List<MasterDataSyncConfiguration.MasterDataSyncItem>();
                foreach (var masterdata in GenerateMasterDataList)
                {
                    synccon.Item.Add(new MasterDataSyncConfiguration.MasterDataSyncItem
                                     {
                                         Collective = (MasterDataCollective)
                                             Enum.Parse(typeof (MasterDataCollective), masterdata),
                                         LastSyncDateTime = new DateTime(1940, 01, 01)
                                     });
                }
                synccon.Save();
            }
        }

        private void ExportOrders()
        {
            Informer("Distributor --> Import orders");
            var orderResponce = ObjectFactory.GetInstance<IOrderExportTransactionsService>().ExportToSap(OrderType.OutletToDistributor,DocumentStatus.Approved).Result;
            Informer("\t\t " + orderResponce.Info);
            if (orderResponce.Status)
            {
                ExportOrders();
            }
                                        
        }
        private void ExportSaleOrders()
        {
            Informer("Distributor --> Import Sales orders");
            var orderResponce = ObjectFactory.GetInstance<IOrderExportTransactionsService>().ExportToSap(OrderType.DistributorPOS,DocumentStatus.Closed).Result;
            Informer("\t\t " + orderResponce.Info);
            if (orderResponce.Status)
            {
                ExportSaleOrders();
            }

        }

        private void ImportMasterDataItem(string masterdata)
        {
             MasterDataFilepath = FileUtility.GetFilePath("masterdataimportpath");
             var syncTracker = MasterDataSyncConfiguration.Load();
             var servicesync = new PullMasterdataService();
                using (var c = NestedContainer)
                {
                    bool isSuccess = false;
                    var service = Using<IMasterDataImportService>(c);
                     try
                     {
                         var imports =

                             service.ImportAsync(MasterDataFilepath,
                                                 (MasterDataCollective)
                                                 Enum.Parse(typeof (MasterDataCollective), masterdata)).Result;

                         if (imports != null)
                         {

                             if (imports.Count <= 0)
                             {
                                 Informer(string.Format("\t\t{0} --> No master data items to upload", masterdata));
                                 return;
                             }


                             var responses = service.Upload(imports).Result;
                             if (responses.ValidationResults == null || responses.Result == null ||
                                 responses.Result.Contains("Error"))
                             {

                                 var error = responses != null && !string.IsNullOrEmpty(responses.ResultInfo)
                                                 ? responses.ResultInfo
                                                 : string.Format(
                                                     "{0} -> There is an unknown error response from server..import failed",
                                                     masterdata);

                                 Informer("\t\t" + error);

                                 return;

                             }
                             if (responses != null && responses.ValidationResults.Any())
                             {


                                 if (responses.ValidationResults.Any(n => !n.IsValid))
                                 {
                                     responses.ValidationResults.Where(n => !n.IsValid).SelectMany(s => s.Results).
                                         ToList
                                         ().ForEach(s => Informer(string.Format("\t\t{0} ->" + s, masterdata)));


                                 }
                                 else
                                 {

                                     isSuccess = true;
                                     Informer(string.Format("\t\t{0} -> Uploaded successfully", masterdata));

                                 }
                             }
                             else if (responses != null && responses.Result == "Success" &&
                                      (responses.ValidationResults.Count == 0 ||
                                       responses.ValidationResults.All(n => n.IsValid)))
                             {

                                 Informer(string.Format("\t\t{0} -> Uploaded successfully", masterdata));
                                 isSuccess = true;
                                 
                             }
                          var tracker=   syncTracker.Item.FirstOrDefault(s => s.Collective == (MasterDataCollective)
                                 Enum.Parse(typeof (MasterDataCollective), masterdata));
                             if (tracker != null)
                             {
                                 tracker.LastSyncDateTime =servicesync.GetCurrentDatetime().AddDays(-1);
                                 syncTracker.Save();
                             }
                         }
                     }
                     catch (AggregateException ex)
                            {
                                //var errors = ex.InnerExceptions.Aggregate(string.Empty,
                                //                                          (current, error) =>
                                //                                          current + error);
                                Informer(string.Format("\t\t{0} ->"+ex.Message,masterdata));
                                return  ;
                            }
                            catch (Exception ex)
                            {
                                Informer(string.Format("\t\t{0} ->Error=>" + ex.Message,masterdata));
                                return;
                            }



                    return;
                }
            


        }
        public SyncViewModel()
        {
            dtsyncCommand = new DispatcherTimer();
            dtsyncCommand.Interval = new TimeSpan(0, 0, 2); // 500 Milliseconds
            dtsyncCommand.Tick += new EventHandler(dtStartCommand_Tick);
            _dictionary = new Dictionary<MasterDataItem, List<MasterDataValidationAndImportResponse>>();
        }

        private void dtStartCommand_Tick(object sender, EventArgs e)
        {
            SyncMasterDataCommand.Execute(null);

        }
       
        public const string SyncInfoPropertyName = "SyncInfo";
        private string _syncInfo = "";
        public string SyncInfo
        {
            get
            {
                return _syncInfo;
            }

            set
            {
                if (_syncInfo == value)
                {
                    return;
                }

                RaisePropertyChanging(SyncInfoPropertyName);
                _syncInfo = value;
                RaisePropertyChanged(SyncInfoPropertyName);
            }
        }

       
        public const string IsTimeStoppedPropertyName = "IsTimeStopped";
        private bool _istimestopped = true;
        public bool IsTimeStopped
        {
            get
            {
                return _istimestopped;
            }

            set
            {
                if (_istimestopped == value)
                {
                    return;
                }

                RaisePropertyChanging(IsTimeStoppedPropertyName);
                _istimestopped = value;
                RaisePropertyChanged(IsTimeStoppedPropertyName);
            }
        }

        private IEnumerable<string> GenerateMasterDataList
        {
            get
            {
                return new List<string>
                           {
                               MasterDataCollective.Country.ToString(),
                               MasterDataCollective.Region.ToString(),
                               MasterDataCollective.Bank.ToString(),
                               MasterDataCollective.PricingTier.ToString(),
                               MasterDataCollective.VatClass.ToString(),
                               MasterDataCollective.Supplier.ToString(),
                               MasterDataCollective.ProductBrand.ToString(),

                               MasterDataCollective.SaleProduct.ToString(),


                               MasterDataCollective.Pricing.ToString(),

                               MasterDataCollective.Distributor.ToString(),

                               MasterDataCollective.DistributorSalesman.ToString(),
                               MasterDataCollective.Route.ToString(),
                               MasterDataCollective.Outlet.ToString(),
                               MasterDataCollective.ShipTo.ToString(),


                               
                           };


            }
        } 
    }
}