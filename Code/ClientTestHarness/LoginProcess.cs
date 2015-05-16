using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Newtonsoft.Json.Linq;

namespace ClientTestHarness
{
    public class LoginProcess
    {
        public static bool Login()
        {
            IConfigService configService = Services.Using<IConfigService>();
            string username = "kameme";
            string pw = "12345678";
            string url = ConfigurationManager.AppSettings["WSURL"];
            bool isInitialised = configService.Load().IsApplicationInitialized;

            Config config = configService.Load();
            config.WebServiceUrl = url;
            configService.Save(config);

            if (isInitialised)
            {
                User user = null;
                user = Services.Using<IUserRepository>().Login(username, Services.Using<IOtherUtilities>().MD5Hash(pw));
                return user != null;
            }
            else
            {
               

                if (!CanConnectToServer(url))
                    return false;
                CostCentreLoginResponse response = Services.Using<ISetupApplication>().LoginOnServer(username, pw, UserType.WarehouseManager);
                if (response.ErrorInfo.Equals("Success"))
                    return true;
                return false;
            }

        }

        private static bool CanConnectToServer(string url)
        {
            bool canconnecto = false;
            try
            {
                string _url = url + "Test/gettestcostcentre";
                Uri uri = new Uri(_url, UriKind.Absolute);
                WebClient wc = new WebClient();
                string result = wc.DownloadString(uri);
                JObject jo = JObject.Parse(result);
                string costCentreId = (string)jo["costCentreId"];
                canconnecto = true;
                //MessageBox.Show("Connected Successfully", "Distributr Configuration", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Connection failed, check the server URL", "Distributr Configuration", MessageBoxButton.OK);
                canconnecto = false;
            }

            return canconnecto;
        }
    }
}
