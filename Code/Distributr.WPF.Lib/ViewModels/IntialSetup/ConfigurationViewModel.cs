using System;
using System.Windows;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Sync;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.IntialSetup
{

    public class ConfigurationViewModel : SyncViewModel
    {


        public ConfigurationViewModel()
        {

            GetApplicationIdCommand = new RelayCommand(GetCostCentreAppId);
            SaveSettingCommand = new RelayCommand(SaveSetting);
            SyncCommand = new RelayCommand(Sync);
            using (StructureMap.IContainer c = NestedContainer)
            {
                Config con = Using<IConfigService>(c).Load();
                ServerUrl = con.WebServiceUrl;
                CostCentreId = con.CostCentreId;
            }

            FinishCommand = new RelayCommand(Finish);
            AppDescription = "Distributr-MDC-WPF-APP-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-";

            //GetAppId();

        }

        #region Properties
        public const string CostCentreIdPropertyName = "CostCentreId";
        private Guid _costCentreId = Guid.Empty;
        public Guid CostCentreId
        {
            get
            {
                return _costCentreId;
            }

            set
            {
                if (_costCentreId == value)
                {
                    return;
                }

                var oldValue = _costCentreId;
                _costCentreId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(CostCentreIdPropertyName);
               
            }
        }
     
        public const string ServerUrlPropertyName = "ServerUrl";
        private string _serverUrl = "";
        public string ServerUrl
        {
            get
            {
                return _serverUrl;
            }

            set
            {
                if (_serverUrl == value)
                {
                    return;
                }

                var oldValue = _serverUrl;
                _serverUrl = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ServerUrlPropertyName);

              
            }
        }

        public const string CostCentreApplicationIdPropertyName = "CostCentreApplicationId";
        private Guid _costcentreApplicationId = Guid.Empty;
        public Guid CostCentreApplicationId
        {
            get
            {
                return _costcentreApplicationId;
            }

            set
            {
                if (_costcentreApplicationId == value)
                {
                    return;
                }

                var oldValue = _costcentreApplicationId;
                _costcentreApplicationId = value;

               
                // Update bindings, no broadcast
                RaisePropertyChanged(CostCentreApplicationIdPropertyName);

                
            }
        }

        public const string AppDescriptionPropertyName = "AppDescription";
        private string _appdescription = "Test";
        public string AppDescription
        {
            get
            {
                return _appdescription;
            }

            set
            {
                if (_appdescription == value)
                {
                    return;
                }

                var oldValue = _appdescription;
                _appdescription = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(AppDescriptionPropertyName);

             
            }
        }

        public RelayCommand GetApplicationIdCommand { get; set; }
        public RelayCommand SaveSettingCommand { get; set; }
        public RelayCommand SyncCommand { get; set; }
        public RelayCommand FinishCommand { get; set; }

        #endregion

        #region Methods

        public async override void MasterDataSyncCompleted()
        {
            using (IContainer c = NestedContainer)
            {
                IUpdateMasterDataService updateMasterDataService = c.GetInstance<IUpdateMasterDataService>();
                IConfigService configService = c.GetInstance<IConfigService>();
                Guid appId = configService.Load().CostCentreApplicationId;
                bool UnderBankingpaymentSuccess = await updateMasterDataService.GetAndUpdateUnderBankingAsync(appId);

            }
            BeginInventorySync();
        }

        public override void InventorySyncCompleted()
        {
            BeginPaymentSync();
           
            base.InventorySyncCompleted();

        }

        public override void PaymentSyncCompleted()
        {
            base.PaymentSyncCompleted();
            Finish();
        }

        public async  void GetCostCentreAppId()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {

                var config = Using<IConfigService>(c).Load();
              //  CreateCostCentreApplicationResponse response = Using<ISetupApplication>(c).CreateCostCentreApplication(config.CostCentreId, AppDescription);
                CreateCostCentreApplicationResponse response =await Using<IWebApiProxy>(c).CreateCostCentreApplicationAsync(config.CostCentreId, AppDescription);
                if (response !=null && response.CostCentreApplicationId != Guid.Empty)
                {
                    CostCentreApplicationId = response.CostCentreApplicationId;
                    SaveSetting();
                   
                }
                else
                {
                    MessageBox.Show("Get application Id from server first");
                }

               
            }
        }

        public void SaveSetting()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (CostCentreApplicationId != Guid.Empty)
                {
                    Config config = c.GetInstance<ISyncService>()
                        .AppInitialize(CostCentreApplicationId, CostCentreId, ServerUrl.Trim());
                    Sync();
                    
                }
                else
                {
                    MessageBox.Show("Get application Id from server first");
                }
            }
        }

        public void Sync()
        {
            Config config = null;
            using (StructureMap.IContainer c = NestedContainer)
            {
                config = Using<IConfigService>(c).Load();
            }
            if (config.IsApplicationInitialized)
            {
                SyncStatusInfo = "Starting Master Data Sync";
                UpdateMasterData();
            }
            else
            {
                MessageBox.Show("Register Appliaction first");
            }

        }

        public void Finish()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Config config = Using<IConfigService>(c).Load();
                if (config.IsApplicationInitialized)
                {

                    SendNavigationRequestMessage(new Uri("/Views/LoginViews/LoginPage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Register Appliaction first");
                }
            }
        }
        //protected void SendNavigationRequestMessage(Uri uri) { Messenger.Default.Send<Uri>(uri, "NavigationRequest"); }
        #endregion
    }
}