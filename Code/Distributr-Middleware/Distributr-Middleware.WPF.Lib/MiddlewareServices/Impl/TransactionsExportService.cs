using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WSAPI.Lib.CommandResults;
using Distributr.WSAPI.Lib.Integrations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl
{
    internal class TransactionsDownloadService :MiddleWareBase, ITransactionsExportService
    {
        private string _userName;
        private string _password;
        public TransactionsDownloadService()
        {
            
        }
        public TransactionsDownloadService(string username,string password)
        {
            _userName = username;
            _password = password;
        }
        public string ExportSaleOrOrder(string orderExternalRef)
        {

            string _url = MiddlewareHttpClient.BaseAddress +
                          "Integrations/Transaction?Username={0}&Password={1}&integrationModule={2}&documentRef={3}";

            string url = string.Format(_url, _userName,_otherUtilities.MD5Hash(_password),IntegrationModule.Sage.ToString(),orderExternalRef);

            Uri uri = new Uri(url, UriKind.Absolute);
            WebClient wc = new WebClient();

            string response = wc.DownloadString(uri);
            TransactionResponse _response =
                JsonConvert.DeserializeObject<TransactionResponse>(response,new IsoDateTimeConverter
                                                                           ());

           if(_response !=null && _response.Transactions.Any())
           {
               
           }
           throw new NotImplementedException();
        }

        async Task<int> DownloadTransactionAsync(CancellationToken ct, string orderExternalRef)
        {

            var httpClient = MiddlewareHttpClient;

            string _url = httpClient.BaseAddress +
                          "Integrations/Transaction?Username={0}&Password={1}&integrationModule={2}&documentRef={3}";

            string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password), IntegrationModule.Sage.ToString(), orderExternalRef);
            // GetAsync returns a Task<HttpResponseMessage>.  
            // ***The ct argument carries the message if the Cancel button is chosen.
            HttpResponseMessage response = await httpClient.GetAsync(url, ct);

            // Retrieve the website contents from the HttpResponseMessage. 
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            // The result of the method is the length of the downloaded website. 
            return urlContents.Length;
        }

        public string ExportSalesOrOrders()
        {
            try
            {
                string _url = MiddlewareHttpClient.BaseAddress +
                        "Integrations/Transaction?Username={0}&Password={1}&integrationModule={2}";

                string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password), IntegrationModule.Sage.ToString());

                Uri uri = new Uri(url, UriKind.Absolute);
                WebClient wc = new WebClient();

                string response = wc.DownloadString(uri);
                TransactionResponse _response =
                    JsonConvert.DeserializeObject<TransactionResponse>(response, new IsoDateTimeConverter
                                                                               ());

                if (_response != null && _response.Transactions.Any())
                {

                }
            }catch(Exception ex)
            {
                
            }
           
            throw new NotImplementedException();
        }
    }
}
