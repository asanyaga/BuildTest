using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Transactional.DocumentEntities;

using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using SAPUtilityLib.Util;

namespace SAPUtilityLib.Proxies.Impl
{
    public class SapWebProxy : ISapWebProxy
    {
        
        protected HttpClient SapHttpClient
        {
            get
            {
                string url = ConfigurationManager.AppSettings["WSURL"];
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("Invalid Url");
                }

                return new HttpClient { BaseAddress = new Uri(url), Timeout = TimeSpan.FromMinutes(30) };

            }
        }
        public  TransactionExportResponse GetNextOrder(OrderType orderType, DocumentStatus status)
        {
            TransactionExportResponse _response= new TransactionExportResponse();
            string _url = SapHttpClient.BaseAddress +
                          "api/Integrations/GetNextOrderToExport?username={0}&password={1}&orderType={2}&documentStatus={3}";

            string url = string.Format(_url, CredentialsManager.GetUserName(), Md5Hash.GetMd5Hash(CredentialsManager.GetPassword()), orderType, status);
            try
            {
                var response =  SapHttpClient.GetAsync(url).Result;

                _response =  response.Content.ReadAsAsync<TransactionExportResponse>().Result;
            }
            catch (Exception ex)
            {
                _response = new TransactionExportResponse {ErrorInfo = ex.Message};
            }

            return _response;
        }

        public ResponseBool MarkOrderAsExported(string orderExternalRef)
        {
            ResponseBool _response = new ResponseBool();
            string _url = SapHttpClient.BaseAddress +
                          "api/Integrations/MarkOrderAsExported";

            string url = string.Format(_url, CredentialsManager.GetUserName(), Md5Hash.GetMd5Hash(CredentialsManager.GetPassword()), orderExternalRef);
            try
            {
                string data = string.Format("?username={0}&password={1}&orderExternalRef={2}",
                    CredentialsManager.GetUserName(), Md5Hash.GetMd5Hash(CredentialsManager.GetPassword()),
                    orderExternalRef);
                HttpContent content = new StringContent(data);
                var response = SapHttpClient.PostAsync(url+data, content).Result;

                _response = response.Content.ReadAsAsync<ResponseBool>().Result;
            }
            catch (Exception ex)
            {
                _response = new ResponseBool { ErrorInfo = ex.Message };
            }

            return _response;
        }
    }
}
