using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.ApplicSettings
{
    public class GeneralSettingsViewModel : DistributrViewModelBase
    {
        bool report = true;
        public GeneralSettingsViewModel()
        {
            SaveReportUrlCommand = new RelayCommand<Object>(SaveReportUrl);
            SavePaymentWebServiceUrlCommand = new RelayCommand(SavePaymentWebServiceUrl);
            SaveAllCommand = new RelayCommand(SaveAll);
            SaveRecordsPerPageCommand = new RelayCommand(SaveRecordsPerPage);
            AboutPgBridgeCommand = new RelayCommand(AboutPgBridge);
            LoadCurrentAppsCommand = new RelayCommand(DoLoadCurrentApps);
            SaveAgriSyncSettingsCommand = new RelayCommand(DoSaveAgriSyncSettings);
          
            CurrentAppsList = new ObservableCollection<ClientApplication>();
        }

      

        
        #region methods

        public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {

                ReportUrl = "";
                var generalSettingsRepo = Using<IGeneralSettingRepository>(c);
                GeneralSetting reportUrl =generalSettingsRepo .GetByKey(GeneralSettingKey.ReportUrl);
                AppSettings paymentWebServiceUrl = Using<ISettingsRepository>(c).GetByKey(SettingsKeys.PaymentGatewayWSUrl);
                GeneralSetting recordsPerPageSetting = generalSettingsRepo.GetByKey(GeneralSettingKey.RecordsPerPage);

                ReportUrl = reportUrl != null ? reportUrl.SettingValue : "";
                PaymentWebServiceUrl = paymentWebServiceUrl != null ? paymentWebServiceUrl.Value : "";
                RecordsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
                DistributrId = Using<IConfigService>(c).Load().CostCentreId;
                var dist = Using<ICostCentreRepository>(c).GetById(DistributrId);
                if (dist != null)
                {
                    DistributorCode = dist.CostCentreCode;
                    DistributrName = dist.Name;
                }

                var ficsalprinterEnabled = generalSettingsRepo.GetByKey(GeneralSettingKey.FiscalPrinterEnabled);
                if(ficsalprinterEnabled !=null && !string.IsNullOrEmpty(ficsalprinterEnabled.SettingValue))
                {
                    FiscalPrinterIsAvailable = Boolean.Parse(ficsalprinterEnabled.SettingValue);
                    FiscalPrinterIsEnabled = FiscalPrinterIsAvailable ? Visibility.Visible
                                                   : Visibility.Collapsed;

                }
                var fiscalprinterPort = generalSettingsRepo.GetByKey(GeneralSettingKey.FiscalPrinterPort);
                if (fiscalprinterPort != null && !string.IsNullOrEmpty(fiscalprinterPort.SettingValue))
                {
                    FiscalPrinterPort = Convert.ToInt32(fiscalprinterPort.SettingValue); 
                    
                }
                var fiscalprinterPortSpeed = generalSettingsRepo.GetByKey(GeneralSettingKey.FiscalPrinterPortSpeed);
                if (fiscalprinterPortSpeed != null && !string.IsNullOrEmpty(fiscalprinterPortSpeed.SettingValue))
                {
                    FiscalPrinterSpeed = Convert.ToInt32(fiscalprinterPortSpeed.SettingValue);
                }

               
            }
        }

        private void SaveReportUrl(Object obj)
        {
            ReportPassword = ((PasswordBox) obj).Password;
            if (!IsValidUri(ReportUrl.Trim()))
            {
                MessageBox.Show("Enter a Valid Reports Url", "Distributr Settings", MessageBoxButton.OK);
                return;
            }
            if (!ReportUrl.EndsWith("/"))
            {
                MessageBox.Show("Add '/' at the end of the url", "Distributr Settings", MessageBoxButton.OK);
                return;
            }
            else
            {
                try
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        GeneralSetting setting = new GeneralSetting
                            {
                                SettingKey = GeneralSettingKey.ReportUrl,
                                SettingValue = ReportUrl.Trim()
                            };
                     Using<IGeneralSettingRepository>(c).Save(setting);
                     GeneralSetting username = new GeneralSetting
                     {
                         SettingKey = GeneralSettingKey.ReportUsername,
                         SettingValue = ReportUsername.Trim()
                     };
                     Using<IGeneralSettingRepository>(c).Save(username);
                     GeneralSetting password = new GeneralSetting
                     {
                         SettingKey = GeneralSettingKey.ReportPassword,
                         SettingValue =VCEncryption.EncryptString(ReportPassword),
                     };
                     Using<IGeneralSettingRepository>(c).Save(password);
                     GeneralSetting folder = new GeneralSetting
                     {
                         SettingKey = GeneralSettingKey.ReportFolder,
                         SettingValue = ReportFolder,
                     };
                     Using<IGeneralSettingRepository>(c).Save(folder);
                        if (report)
                            MessageBox.Show("Reports Url Saved", "Distributr Settings", MessageBoxButton.OK);
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("Error Occured", "Distributr Settings", MessageBoxButton.OK);
                }

            }
        }

        private void SavePaymentWebServiceUrl()
        {
            if (!IsValidUri(PaymentWebServiceUrl.Trim()))
            {
                MessageBox.Show("Enter a valid Url", "Distributr Settings", MessageBoxButton.OK);
                return;
            }
            if (!PaymentWebServiceUrl.EndsWith("/"))
            {
                MessageBox.Show("Add '/' at the end of the url", "Distributr Settings", MessageBoxButton.OK);
                return;
            }
            else
            {
                try
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        AppSettings pgwsSetting = new AppSettings(Guid.NewGuid())
                            {
                                Key = SettingsKeys.PaymentGatewayWSUrl,
                                Value = PaymentWebServiceUrl
                            };
                        //Using<ISettingsRepository>(c).SaveLocally(pgwsSetting);
                        Using<ISettingsRepository>(c).Save(pgwsSetting);

                        if (report)
                            MessageBox.Show("Payment Web Service Url Saved", "Distributr Settings", MessageBoxButton.OK);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Occured", "Distributr Settings", MessageBoxButton.OK);
                }
            }
        }

        private void SaveRecordsPerPage()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (RecordsPerPage < 5)
                {
                    MessageBox.Show("Must be at least 5 items per page", "Distributr: General Settings",
                                    MessageBoxButton.OK);
                    return;
                }

                var pgwsSetting = new GeneralSetting()
                                      {
                                          SettingKey = GeneralSettingKey.RecordsPerPage,
                                          SettingValue = RecordsPerPage.ToString()
                                      };
                Using<IGeneralSettingRepository>(c).Save(pgwsSetting);

                if (report)
                    MessageBox.Show("Records per page settings saved", "Distributr: General Settings",
                                    MessageBoxButton.OK);
            }
        }

        void SaveAll()
        {
            Config config = GetConfigParams();

            report = false;
            try
            {
               // SaveReportUrl();
                SavePaymentWebServiceUrl();
                SaveRecordsPerPage();
               // ToggleFiscalPrinterCommand.Execute(null);
                if (config.AppId == Core.VirtualCityApp.Ditributr)
                    SavePrinterConfig();
               
                MessageBox.Show("All settings were saved successfully.", "Distributr Settings", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured"+"\n"+ex.Message, "Distributr Settings", MessageBoxButton.OK);
            }
            report = true;
        }

        Boolean IsValidUri(String uri)
        {
            try
            {
                new Uri(uri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void AboutPgBridge()
        {
            if (string.IsNullOrEmpty(PaymentWebServiceUrl))
                MessageBox.Show("Set the Payment Gateway bridge Url first");
            else
            {
                AboutApiPGBridge(); return;
                try
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        string url = Using<IPaymentService>(c).GetPGWSUrl(0);

                        Uri uri = new Uri(url, UriKind.Absolute);
                        WebClient wc = new WebClient();
                        wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                        wc.DownloadStringAsync(uri);
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show("Error\n" + e.Message);
                }
            }
        }

        async void AboutApiPGBridge()
        {

            using (var c = NestedContainer)
            {
                var pgresponse = await Using<IPaymentGatewayProxy>(c).AboutPGBridgeWebAPI();
                MessageBox.Show(pgresponse, "Payment Gateway Bridge: About", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        void DoLoadCurrentApps()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                CurrentAppsList.Clear();
                var apps = Using<IConfigService>(c).GetClientApplications();
                    apps.ForEach(CurrentAppsList.Add);
            }

           
        }

       public  void DoLoadSyncSettings()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var generalSettingsRepo=  Using<IGeneralSettingRepository>(c);
                var syncRecordsPageSize = generalSettingsRepo.GetByKey(GeneralSettingKey.SyncPageSize);
                if (syncRecordsPageSize != null && !string.IsNullOrEmpty(syncRecordsPageSize.SettingValue))
                {
                    SyncPageSize = Convert.ToInt32(syncRecordsPageSize.SettingValue);
                }
            }


        }
        private void DoSaveAgriSyncSettings()
        {
             SaveSyncPageSize();

             using (StructureMap.IContainer c = NestedContainer)
             {
                 var configService = Using<IConfigService>(c);
                 if (SelectedApplication != null)
                 {
                     configService.SaveClientApplication(SelectedApplication);
                     var savedApp =configService.GetClientApplications().FirstOrDefault(p => p.Id == SelectedApplication.Id);

                     if (savedApp != null && savedApp.CanSync == SelectedApplication.CanSync)
                     {
                         MessageBox.Show(SelectedApplication.HostName + " is currently set to sync");
                     }
                     else
                     {
                         MessageBox.Show("Error setting sync  application");
                     }
                 }
                 DoLoadCurrentApps();
                 
             }

        }

        private void SaveSyncPageSize()
        {
             if (SyncPageSize < 100)
            {
                MessageBox.Show("Must be greater than 100 records", "Distributr: Sync Settings",
                                MessageBoxButton.OK);
                return;
            }

            var pagesizeSetting = new GeneralSetting()
            {
                SettingKey = GeneralSettingKey.SyncPageSize,
                SettingValue = SyncPageSize.ToString()
            };
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IGeneralSettingRepository>(c).Save(pagesizeSetting);
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = "";
            try
            {
                if (e.Error == null)
                {
                    string jsonResult = e.Result;
                    result = JsonConvert.DeserializeObject<String>(jsonResult, new IsoDateTimeConverter());
                }
                else
                {
                    result = e.Error.Message + "\n" + e.Error.StackTrace;
                }
            }
            catch
            {
                result = "Information not found.";
            }
            MessageBox.Show(result);
        }
        #endregion

        #region Properties
        public RelayCommand<Object> SaveReportUrlCommand { get; set; }
        public RelayCommand SavePaymentWebServiceUrlCommand { get; set; }
        public RelayCommand AboutPgBridgeCommand { get; set; }
        public RelayCommand SaveRecordsPerPageCommand { get; set; }
        public RelayCommand SaveAllCommand { get; set; }
        public RelayCommand LoadCurrentAppsCommand { get; set; }
        public RelayCommand SaveAgriSyncSettingsCommand { get; set; }
        private RelayCommand<CheckBox> _toggleFiscalPrinterCommand;
        public RelayCommand<CheckBox> ToggleFiscalPrinterCommand
        {
            get { return _toggleFiscalPrinterCommand ?? (new RelayCommand<CheckBox>(ToggleFiscalPrinter)); }
        }

        private RelayCommand _saveFiscalPrinterSettings;
        public RelayCommand SaveFiscalPrinterSettingCommand
        {
            get { return _saveFiscalPrinterSettings ?? (new RelayCommand(SavePrinterConfig)); }
        }

        private RelayCommand _reInitializeFiscalPrinterSettings;
        public RelayCommand ReIntializeFiscalPrinterSettingCommand
        {
            get { return _reInitializeFiscalPrinterSettings ?? (new RelayCommand(ReInitializeFiscalPrinter)); }
        }


        private void ToggleFiscalPrinter(CheckBox checkBox)
        {
            using (var c =NestedContainer)
            {
                var currentSetting =
                    Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.FiscalPrinterEnabled) ??
                    new GeneralSetting();
                var checkedItem=checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
                FiscalPrinterIsAvailable = checkedItem;
                currentSetting.SettingKey = GeneralSettingKey.FiscalPrinterEnabled;
                currentSetting.SettingValue = checkedItem.ToString();
                Using<IGeneralSettingRepository>(c).Save(currentSetting);
                FiscalPrinterIsEnabled = checkedItem ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ReInitializeFiscalPrinter()
        {
            var comport = FiscalPrinterPort;
            var printerSpeed = FiscalPrinterSpeed;
            FiscalPrinterUtility.FiscalPrinterHelper.Reinitialize(comport,printerSpeed);
        }

        private void SavePrinterConfig()
        {
            using (var c = NestedContainer)
            {
                var portSetting = Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.FiscalPrinterPort) ??
                                  new GeneralSetting();
               
                    portSetting.SettingKey = GeneralSettingKey.FiscalPrinterPort;
                    portSetting.SettingValue = FiscalPrinterPort.ToString();
                    Using<IGeneralSettingRepository>(c).Save(portSetting);

                var portSpeedSetting =
                    Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.FiscalPrinterPortSpeed) ??
                    new GeneralSetting();
                portSpeedSetting.SettingKey = GeneralSettingKey.FiscalPrinterPortSpeed;
                portSpeedSetting.SettingValue = FiscalPrinterSpeed.ToString();
                Using<IGeneralSettingRepository>(c).Save(portSpeedSetting);

                //var config = SimpleIoc.Default.GetInstance<OrderFormViewModel>();
                var config =SimpleIoc.Default.GetInstance<OrderFormViewModel>();
                if (config != null)
                    config.ConfigureFiscalPrinter();
            }
        }
        public ObservableCollection<ClientApplication> CurrentAppsList { get; set; }


        public const string SelectedApplicationPropertyName = "SelectedApplication";
        private ClientApplication _SelectedApplication;
        public ClientApplication SelectedApplication
        {
            get
            {
                return _SelectedApplication;
            }

            set
            {
                if (_SelectedApplication == value)
                {
                    return;
                }

                var oldValue = _SelectedApplication;
                _SelectedApplication = value;
                RaisePropertyChanged(SelectedApplicationPropertyName);
            }
        }

        public const string FiscalPrinterSpeedPropertyName = "FiscalPrinterSpeed";
        private int _fiscalprinterSpeed = 115200;
        public int FiscalPrinterSpeed
        {
            get
            {
                return _fiscalprinterSpeed;
            }

            set
            {
                if (_fiscalprinterSpeed == value)
                {
                    return;
                }
                _fiscalprinterSpeed = value;
                RaisePropertyChanged(FiscalPrinterSpeedPropertyName);
            }
        }
        public const string ReInitializePrinterPropertyName = "ReInitializePrinter";
        private bool _reInitializePrinter = false;
        public bool ReInitializePrinter
        {
            get
            {
                return _reInitializePrinter;
            }

            set
            {
                if (_reInitializePrinter == value)
                {
                    return;
                }
                _reInitializePrinter = value;
                RaisePropertyChanged(ReInitializePrinterPropertyName);
            }
        }

        public const string FiscalPrinterIsAvailablePropertyName = "FiscalPrinterIsAvailable";
        private bool _fiscalprinterIsAvailable = false;
        public bool FiscalPrinterIsAvailable
        {
            get
            {
                return _fiscalprinterIsAvailable;
            }

            set
            {
                if (_fiscalprinterIsAvailable == value)
                {
                    return;
                }
                _fiscalprinterIsAvailable = value;
                RaisePropertyChanged(FiscalPrinterIsAvailablePropertyName);
            }
        }
        //FiscalPrinterIsAvailable

        public const string FiscalPrinterPortPropertyName = "FiscalPrinterPort";
        private int _fiscalprinterPort=2;
        public int FiscalPrinterPort
        {
            get
            {
                return _fiscalprinterPort;
            }

            set
            {
                if (_fiscalprinterPort == value)
                {
                    return;
                }
                _fiscalprinterPort = value;
                RaisePropertyChanged(FiscalPrinterPortPropertyName);
            }
        }

        public const string FiscalPrinterIsEnabledPropertyName = "FiscalPrinterIsEnabled";
        private Visibility _fiscalprinterEnabled=Visibility.Collapsed;
        public Visibility FiscalPrinterIsEnabled
        {
            get
            {
                return _fiscalprinterEnabled;
            }

            set
            {
                if (_fiscalprinterEnabled == value)
                {
                    return;
                }
                _fiscalprinterEnabled = value;
                RaisePropertyChanged(FiscalPrinterIsEnabledPropertyName);
            }
        }

        public const string ReportUrlPropertyName = "ReportUrl";
        private string _reportUrl = "";
        public string ReportUrl
        {
            get
            {
                return _reportUrl;
            }

            set
            {
                if (_reportUrl == value)
                {
                    return;
                }

                var oldValue = _reportUrl;
                _reportUrl = value;
                RaisePropertyChanged(ReportUrlPropertyName);
            }
        }

        public const string ReportFolderPropertyName = "ReportFolder";
        private string _ReportFolder = "";
        public string ReportFolder
        {
            get
            {
                return _ReportFolder;
            }

            set
            {
                if (_ReportFolder == value)
                {
                    return;
                }

                var oldValue = _ReportFolder;
                _ReportFolder = value;
                RaisePropertyChanged(ReportFolderPropertyName);
            }
        }

        public const string ReportUsernamePropertyName = "ReportUsername";
        private string _ReportUsername = "";
        public string ReportUsername
        {
            get
            {
                return _ReportUsername;
            }

            set
            {
                if (_ReportUsername == value)
                {
                    return;
                }

                var oldValue = _ReportUsername;
                _ReportUsername = value;
                RaisePropertyChanged(ReportUsernamePropertyName);
            }
        }

        public const string ReportPasswordPropertyName = "ReportPassword";
        private string _ReportPassword = "";
        public string ReportPassword
        {
            get
            {
                return _ReportPassword;
            }

            set
            {
                if (_ReportPassword == value)
                {
                    return;
                }

                var oldValue = _ReportPassword;
                _ReportPassword = value;
                RaisePropertyChanged(ReportPasswordPropertyName);
            }
        }
        public const string PaymentWebServiceUrlPropertyName = "PaymentWebServiceUrl";
        private string _paymentWebServiceUrl = "";
        public string PaymentWebServiceUrl
        {
            get
            {
                return _paymentWebServiceUrl;
            }

            set
            {
                if (_paymentWebServiceUrl == value)
                {
                    return;
                }

                var oldValue = _paymentWebServiceUrl;
                _paymentWebServiceUrl = value;
                RaisePropertyChanged(PaymentWebServiceUrlPropertyName);
            }
        }

        public const string RecordsPerPagePropertyName = "RecordsPerPage";
        private int _recordsPerPage = 10;
        public int RecordsPerPage
        {
            get
            {
                return _recordsPerPage;
            }

            set
            {
                if (_recordsPerPage == value)
                {
                    return;
                }

                var oldValue = _recordsPerPage;
                _recordsPerPage = value;
                RaisePropertyChanged(RecordsPerPagePropertyName);
            }
        }

        public const string SyncPageSizePropertyName = "SyncPageSize";
        private int _syncPageSize = 100;
        public int SyncPageSize
        {
            get
            {
                return _syncPageSize;
            }

            set
            {
                if (_syncPageSize == value)
                {
                    return;
                }

                var oldValue = _syncPageSize;
                _syncPageSize = value;
                RaisePropertyChanged(SyncPageSizePropertyName);
            }
        }
         
        public const string DistributrIdPropertyName = "DistributrId";
        private Guid _distributorId = Guid.Empty;
        public Guid DistributrId
        {
            get
            {
                return _distributorId;
            }

            set
            {
                if (_distributorId == value)
                {
                    return;
                }

                var oldValue = _distributorId;
                _distributorId = value;
                RaisePropertyChanged(DistributrIdPropertyName);
            }
        }
         
        public const string DistributrNamePropertyName = "DistributrName";
        private string _distributrName = "";
        public string DistributrName
        {
            get
            {
                return _distributrName;
            }

            set
            {
                if (_distributrName == value)
                {
                    return;
                }

                var oldValue = _distributrName;
                _distributrName = value;
                RaisePropertyChanged(DistributrNamePropertyName);
            }
        }
         
        public const string DistributorCodePropertyName = "DistributorCode";
        private string _distributorCode = "";
        public string DistributorCode
        {
            get
            {
                return _distributorCode;
            }

            set
            {
                if (_distributorCode == value)
                {
                    return;
                }

                var oldValue = _distributorCode;
                _distributorCode = value;
                RaisePropertyChanged(DistributorCodePropertyName);
            }
        }
        #endregion
    }
}