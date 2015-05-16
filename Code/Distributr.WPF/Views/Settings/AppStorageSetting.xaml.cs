using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StructureMap;

namespace Distributr.WPF.UI.Views.Settings
{
    /// <summary>
    /// Interaction logic for AppStorageSetting.xaml
    /// </summary>
    public partial class AppStorageSetting : Page
    {
       IConfigService _configService;
        public AppStorageSetting()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Home_Loaded);

        }

        void Home_Loaded(object sender, RoutedEventArgs e)
        {
            SetStorageData();
            loadConfig();
        }

        private void loadConfig()
        {
            _configService = ObjectFactory.GetInstance<IConfigService>();
            isRegistred.Content = _configService.Load().IsApplicationInitialized;
            CurrentCC.Content = _configService.Load().CostCentreId;
            dateRegistered.Content = _configService.Load().DateInitialized.ToString();
            ccAppId.Content = _configService.Load().CostCentreApplicationId.ToString();
            if (string.IsNullOrEmpty(_configService.Load().WebServiceUrl.Trim()))
            {
                txtWebServiceUrl.Text = "http://localhost:50759/";
                SaveUrl();
            }
            txtWebServiceUrl.Text = _configService.Load().WebServiceUrl;

        }
        private void SetStorageData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                SpacedUsed.Text = "Current Spaced Used = " + (isf.Quota - isf.AvailableFreeSpace).ToString() + " bytes";
                SpaceAvaiable.Text = "Current Space Available=" + isf.AvailableFreeSpace.ToString() + " bytes";
                CurrentQuota.Text = "Current Quota=" + isf.Quota.ToString() + " bytes";
            }
        }
        private void IncreaseStorage(long spaceRequest)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                long newSpace = isf.Quota + spaceRequest;
                try
                {
                    if (true == isf.IncreaseQuotaTo(newSpace))
                    {
                        Results.Text = "Quota successfully increased.";
                    }
                    else
                    {
                        Results.Text = "Quota increase was unsuccessfull.";
                    }
                }
                catch (Exception e)
                {
                    Results.Text = "An error occured: " + e.Message;
                }
                SetStorageData();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long spaceRequest = Convert.ToInt64(SpaceRequest.Text);
                IncreaseStorage(spaceRequest);
            }
            catch
            { // User put bad data in text box 
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            string _url = _configService.Load().WebServiceUrl + "Test/gettestcostcentre";
            Uri uri = new Uri(_url, UriKind.Absolute);
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadStringAsync(uri);
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = e.Result;
                JObject jo = JObject.Parse(result);
                Guid ccid = Guid.Parse(jo["costCentreId"].ToString());
                Config config = _configService.Load();
                config.CostCentreId = ccid;
                
                _configService.Save(config);
                loadConfig();
                RegisterLocalAppId();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void RegisterLocalAppId()
        {
            var config = _configService.Load();
            string _url = _configService.Load().WebServiceUrl + "CostCentreApplication/createcostcentreapplication?costcentreid={0}&applicationdescription={1}";
            string url = string.Format(_url, config.CostCentreId, "Test_App_for_CC_" + config.CostCentreId );
            Uri uri = new Uri(url, UriKind.Absolute);
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_RegisterLocalAppIdDownloadStringCompleted);
            wc.DownloadStringAsync(uri);
        }

        void wc_RegisterLocalAppIdDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try 
            {
                string result = e.Result;
                CreateCostCentreApplicationResponse r = JsonConvert.DeserializeObject<CreateCostCentreApplicationResponse>(result, new IsoDateTimeConverter());
                Config config = _configService.Load();
                config.CostCentreApplicationId = r.CostCentreApplicationId;
                config.DateInitialized = DateTime.Now;
                config.IsApplicationInitialized = true;
                _configService.Save(config);
                loadConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveUrl_Click(object sender, RoutedEventArgs e)
        {
            SaveUrl();
        }

        private void SaveUrl()
        {
            Config config = _configService.Load();
            config.WebServiceUrl = txtWebServiceUrl.Text;
            _configService.Save(config);
            loadConfig();
        }

        private void ClearDB_Click(object sender, RoutedEventArgs e)
        {
            _configService.CleanLocalDB();
            SaveUrl();
            MessageBox.Show("Local DB Cleared");
        }
        
        private void cmdFP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                //dynamic outlook = AutomationFactory.GetObject("Outlook.Application");

                ////dynamic com = AutomationFactory.CreateObject("Zfpcom.ZekaFP");
                ////dynamic zfp = AutomationFactory.CreateObject("Zfpcom.ZekaFP");
                ////zfp.Setup(4, 9600, 3, 1000);
                ////string sdsd = zfp.GetVersion();
                ////byte oper = byte.Parse("1");
                ////zfp.OpenFiscalBon(oper, "0000", 0, 1);
                ////if (0 != zfp.errorCode)
                ////{
                ////    string err = zfp.GetErrorString(zfp.errorCode, 0);
                ////    MessageBox.Show(err);
                ////}
                ////zfp.SellFree("Cooking fat", Convert.ToByte("1"), 5, 3, 0);
                ////if (0 != zfp.errorCode)
                ////{
                ////    string err = zfp.GetErrorString(zfp.errorCode, 0);
                ////    MessageBox.Show(err);
                ////}
                ////zfp.Payment(Single.Parse("200"), Convert.ToByte("0"), 0);
                ////if (0 != zfp.errorCode)
                ////{
                ////    string err = zfp.GetErrorString(zfp.errorCode, 0);
                ////    MessageBox.Show(err);
                ////}
                ////zfp.CloseFiscalBon();
                ////if (0 != zfp.errorCode)
                ////{
                ////    string err = zfp.GetErrorString(zfp.errorCode, 0);
                ////    MessageBox.Show(err);
                ////}
                //bool isopen = com.Open("COM4");
               // string ss = com.Read((char)(3));
                //bool isClosed = com.Close();
               // dynamic com1 = AutomationFactory.CreateObject("FPRinter");
               ////string dg= com1.test();
               // com1.OpenPort("COM4", 9600);
               // //test.PrintLogo();
               // //test.TerminateReceipt(false);
               // com1.PrintText("Test 1...");
               // com1.PrintText("Test Silverlight...");
               // com1.ClosePort();
             
                //dynamic count = com.GetDeviceCount();
                //StringBuilder sb = new StringBuilder();
                //SerialPort c = new SerialPort();
                //List<dynamic> devices = new List<dynamic>();
                //for (int i = 1; i <= 9; i++)
                //{
                //    devices.Add("COM" + i);
                //}

                //for (int i = 0; i < count; i++)
                //{
                //    devices.Add(com.GetDevice(i));

                //}
                //com.device = devices[9];
                //com.Open();
                //MessageBox.Show(com.GetErrorDescription(com.LastError));
                //string buffer = "";
                //string tb = "";
                //com.WriteLine("dsdsdsdsdsadsad");
                //System.Threading.Thread t = new Thread(new ThreadStart(delegate()
                //                                                           {

                //    while (1 == 1)
                //    {
                //        com.Sleep(200);

                //        buffer = com.ReadString();
                //        if (buffer == "") { com.Close(); return; }

                //            tb += "\r\n" + com.ReadString();

                //    }
                //}));
                //t.Start();
                //com.Close();





               using (FP test = new FP())
               {
                   test.OpenPort("COM4", 9600);
                   //test.PrintLogo();
                   //test.TerminateReceipt(false);
                   test.PrintText("Test 1...");

                   test.ClosePort();
               }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FPException))
                {
                    int err = ((FPException)ex).ErrorCode;
                    MessageBox.Show(ex.Message, "Error " + err.ToString(), MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }
    }
}
