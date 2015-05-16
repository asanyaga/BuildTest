using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr_Middleware.WPF.Lib.Utils;
using PzIntegrations.Lib.MasterDataImports;
using PzIntegrations.Lib.MasterDataImports.Outlets;
using PzIntegrations.Lib.MasterDataImports.Products;
using PzIntegrations.Lib.MasterDataImports.Salesmen;
using PzIntegrations.Lib.MasterDataImports.Shipping;
using StructureMap;
using log4net;
using Timer = System.Timers.Timer;

namespace PzIntegrations.Lib
{
    internal class PzIntegrationService : IPzIntegrationService
    {
        readonly Timer _timer;
        private readonly IOrderExportService _orderExportService = null;
        private readonly IInventoryService _inventoryService;
        private static readonly ILog _log = LogManager.GetLogger("Distributr_Integration_Service Logger");
        private bool isOnMission = false;
        public bool IsStarted;
        private Control logViewer { get; set; }
        private bool isTaskFinished = false;

        private List<ImportValidationResultInfo> Errors;

        public PzIntegrationService(IOrderExportService orderExportService,IInventoryService inventoryService)
        {
            Errors=new List<ImportValidationResultInfo>();
           
            _orderExportService = orderExportService;
            _inventoryService = inventoryService;
           // TestJob();
            _timer = new Timer(120000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) =>
            {
                if (!isOnMission)
                {
                    GetNextOrders();

                    isOnMission = false;

                }
                IsStarted = true;

            };
        }

      public void ImportMasterData(string[] masterdata, bool isAll = false)
        {
            if(isAll)
            {
              var list=  masterdata.Where(p => p != "Inventory").ToList();
                Array.Sort(masterdata);
                list.Add("Inventory");
                masterdata = list.ToArray();
            }
               

            foreach (var item in masterdata)
            {
                ImportMasterData(item);
            }
            
        }

        public void Start()
        {
            _timer.Start();
            GetNextOrders();
        }

        public void Stop()
        {
            _timer.Stop();
            isOnMission = false;
            IsStarted = false;
            ObjectFactory.Container.Dispose();
            _log.Info(string.Format("Integration service stopped at {0}", DateTime.Now));
        }

        private string GenerateResults(IEnumerable<ValidationResult> list)
        {
            string results = "";
            int count = 1;
            foreach (var result in list)
            {
                results += count + " . " + result + " ";
                count++;
            }
            return results;
        }

        public List<string> GetImportErrors()
        {
           if(Errors.Any())
           {
               var list = Errors.Select(error => GenerateResults(error.Results)).ToList();
               return list;
           }
            return new List<string>();
        }

        public bool IsTaskCompleted()
        {
            return isTaskFinished;
        }

        public void FindAndExportOrder(string externalRef)
        {
            try
            {

                FileUtility.LogCommandActivity(string.Format("Search order Ref {0} from HQ",externalRef), logViewer);

                Task.Run(async () =>
                                   {
                                       await _orderExportService.GetAndExportOrders(externalRef);
                                   });




            }
            catch (Exception ex)
            {
                _log.Info("Error=>" + ex.Message);
                FileUtility.LogCommandActivity(string.Format("Error {0}", ex.Message), logViewer);

            }
        }

        async void GetNextOrders()
        {
            try
            {

                FileUtility.LogCommandActivity(string.Format("Getting Next orders from HQ=>{0} ", DateTime.Now), logViewer);
                
                isOnMission = true;
                var itemsFound = await _orderExportService.GetAndExportOrders();

                FileUtility.LogCommandActivity(string.Format("Files Fetched=>{0} as at {1} health Ok,Next HQ trip is at {2}", itemsFound,
                                  DateTime.Now.ToShortTimeString(),
                                  DateTime.Now.AddMilliseconds(_timer.Interval).ToShortTimeString()), logViewer);


            }
            catch (Exception ex)
            {
                _log.Info("Error=>" + ex.Message);
                FileUtility.LogCommandActivity(string.Format("Error {0}", ex.Message), logViewer);

            }
            finally
            {
                isOnMission = false;
            }
        }

        #region MasterData

        private void ImportMasterData(string masterdata)
        {
            if (string.IsNullOrEmpty(masterdata))
            {
                FileUtility.LogCommandActivity("Master data not selected", logViewer);
                return;
            }

            switch (masterdata)
            {

                case "Customer":
                    ImportCustomers(masterdata);
                    break;
                case "Products":
                    ImportProducts(masterdata);
                    break;
                case "Brands":
                    ImportProductBrands(masterdata);
                    break;
                case "Shipping Addresses":
                    ImportShippingAddresses();
                    break;
                case "Salesmen":
                    ImportSalesmen();
                    break;
                case "Inventory":
                    ImportInventory();
                    break;
            }
        }

        private async void ImportInventory()
        {
            FileUtility.LogCommandActivity("Executing Inventory from file");
            var hqUser = ObjectFactory.GetInstance<IUserRepository>().GetByUserType(UserType.HQAdmin).FirstOrDefault();
            if(hqUser==null)
            {
                FileUtility.LogCommandActivity("Unable to set HQ user credentials..task aborted");
                return;
            }
            _inventoryService.SetCredentials(hqUser.Username,hqUser.Password);
          var res= await  _inventoryService.ImportInventoty();
            FileUtility.LogCommandActivity(res ? "Done..successful" : "Done..with errors..see error logs for details");
        }

         private async void ImportSalesmen()
        {
            FileUtility.LogCommandActivity("Fetching Salemen", logViewer);
            var importervice = ObjectFactory.GetInstance<ISalesmanImportService>();
            string path = FileUtility.GetWorkingDirectory("masterdataimportpath");

            var file = Path.Combine(path, string.Concat("users", ".txt"));
            if (string.IsNullOrEmpty(path) || !File.Exists(file))
            {
                MessageBox.Show("Set masterdata import path");
                return;
            }
            var result = await importervice.Import(file);
            FileUtility.LogCommandActivity(string.Format("Done loading {0} users", result.Count()), logViewer);

            FileUtility.LogCommandActivity(string.Format("Validating data....."), logViewer);
            var errors = importervice.ValidateAndSave(result.ToList());
            if(errors.Any())
                Errors.AddRange(errors);
            FileUtility.LogCommandActivity(string.Format("Done processing {0} salesmen", result.Count()), logViewer);
        }
      
      

        private async void ImportShippingAddresses()
        {
            FileUtility.LogCommandActivity("Fetching Addresses");
            var importervice = ObjectFactory.GetInstance<IShipToAddressImportService>();
            string path = FileUtility.GetWorkingDirectory("masterdataimportpath");

            var file = Path.Combine(path, string.Concat("shipto", ".txt"));
            if (string.IsNullOrEmpty(path) || !File.Exists(file))
            {
                MessageBox.Show("Set masterdata import path");
                return;
            }
            var result = await importervice.Import(file);
            FileUtility.LogCommandActivity(string.Format("Done loading {0} addresses", result.Count()));

            FileUtility.LogCommandActivity(string.Format("Validating data....."));
            var errors = importervice.ValidateAndSave(result.ToList());
            if (errors.Any())
                Errors.AddRange(errors);
            FileUtility.LogCommandActivity(string.Format("Done processing {0} address", result.Count()));
        }

        private async void ImportProductBrands(string masterdata)
        {
            FileUtility.LogCommandActivity("Fetching Brands");
            var importervice = ObjectFactory.GetInstance<IProductBrandImportService>();
            string path = FileUtility.GetWorkingDirectory("masterdataimportpath");
            var file = Path.Combine(path, string.Concat(masterdata, ".txt"));
            if (string.IsNullOrEmpty(path) || !File.Exists(file))
            {
                MessageBox.Show("Set masterdata import path");
                return;
            }
            var result = await importervice.Import(file);
            FileUtility.LogCommandActivity(string.Format("Done loading {0} Product Brands", result.Count()));

            FileUtility.LogCommandActivity(string.Format("Validating data....."));
            var errors = importervice.ValidateAndSave(result.ToList());
            if (errors.Any())
                Errors.AddRange(errors);
            FileUtility.LogCommandActivity(string.Format("Done processing {0} Brands", result.Count()));
        }

        private async void ImportCustomers(string masterdata)
        {
            FileUtility.LogCommandActivity("Fetching customers");
            var importervice = ObjectFactory.GetInstance<IOutletImportService>();
            string path = FileUtility.GetWorkingDirectory("masterdataimportpath");
            var file = Path.Combine(path, string.Concat(masterdata, ".txt"));
            if (string.IsNullOrEmpty(path) || !File.Exists(file))
            {
                MessageBox.Show("Set masterdata import path");
                return;
            }
            var result = await importervice.Import(file);
            FileUtility.LogCommandActivity(string.Format("Done loading {0} customers", result.Count()));
            FileUtility.LogCommandActivity(string.Format("Validating data....."));
            var errors = importervice.ValidateAndSave(result.ToList());
            if (errors.Any())
                Errors.AddRange(errors);
            FileUtility.LogCommandActivity(string.Format("Done processing {0} customerss", result.Count()));
        }
        private async void ImportProducts(string masterdata)
        {
            FileUtility.LogCommandActivity("Fetching Products");
            var _importervice = ObjectFactory.GetInstance<IProductImportService>();
            string path = FileUtility.GetWorkingDirectory("masterdataimportpath");
            var file = Path.Combine(path, string.Concat(masterdata, ".txt"));
            if (string.IsNullOrEmpty(path) || !File.Exists(file))
            {
                MessageBox.Show("Set masterdata import path");
                return;
            }
            var result = await _importervice.Import(file);
            FileUtility.LogCommandActivity(string.Format("Done loading {0} Products", result.Count()));

            FileUtility.LogCommandActivity(string.Format("Validating data....."));
            var errors = _importervice.ValidateAndSave(result.ToList());
            if (errors.Any())
                Errors.AddRange(errors);
            FileUtility.LogCommandActivity(string.Format("Done processing {0} Products and prices", result.Count()));
        }
        #endregion


        
        

    }


}
