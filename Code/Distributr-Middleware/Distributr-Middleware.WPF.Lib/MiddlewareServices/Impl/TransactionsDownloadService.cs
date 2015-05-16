using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl
{
    internal class TransactionsDownloadService :MiddleWareBase, ITransactionsDownloadService
    {
        private string _userName;
        private string _password;
        private IntegrationModule module;
      
        public TransactionsDownloadService()
        {
            _userName = CredentialsManager.GetUserName();
            _password = CredentialsManager.GetPassword();
            module = CredentialsManager.GetIntegrator();
        }

        public async Task<TransactionResponse> GetTransactions(string orderExternalRef, OrderType orderType = OrderType.OutletToDistributor)
        {
            TransactionResponse _response;
            string _url = MiddlewareHttpClient.BaseAddress +
                          "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&documentRef={3}&orderType={4}";

            string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password),
                                       module, orderExternalRef, orderType);
            Messenger.Default.Send(
                string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Contacting Server =>" +
                              MiddlewareHttpClient.BaseAddress));
            HttpResponseMessage response =
                await MiddlewareHttpClient.GetAsync(url);
            Messenger.Default.Send(
                string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + response.StatusCode));
            _response =
                await response.Content.ReadAsAsync<TransactionResponse>();
            if (_response != null)
            {
                if (_response.Result == "Error")
                {
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + _response.ErrorInfo));
                    return null;
                }
                if (!string.IsNullOrEmpty(_response.TransactionData))
                {
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") + ": Server response: items downloaded"));
                    DumpExportFilesAsync(_response);

                }
            }
            return _response;
        }

        public async Task DownloadSaleOrOrder(string orderExternalRef, OrderType orderType = OrderType.OutletToDistributor)
        {
            await Task.Factory.StartNew(async () =>
                                            {
                                                TransactionResponse _response;
                                                string _url = MiddlewareHttpClient.BaseAddress +
                                                              "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&documentRef={3}&orderType={4}";

                                                string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password),
                                                                           module, orderExternalRef,orderType);
                                                Messenger.Default.Send(
                          string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Contacting Server =>" + MiddlewareHttpClient.BaseAddress));
                                                HttpResponseMessage response =
                                                    await MiddlewareHttpClient.GetAsync(url);
                                                Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + response.StatusCode));
                                                _response =
                                                    await response.Content.ReadAsAsync<TransactionResponse>();
                                                if (_response != null)
                                                {
                                                    if (_response.Result == "Error")
                                                    {
                                                        Messenger.Default.Send(
                                                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + _response.ErrorInfo));
                                                        return;
                                                    }
                                                    if (!string.IsNullOrEmpty(_response.TransactionData))
                                                    {
                                                        Messenger.Default.Send(
                                                            string.Format(DateTime.Now.ToString("hh:mm:ss") + ": Server response: items downloaded"));
                                                        DumpExportFilesAsync(_response);

                                                    }
                                                }
                                              
                                            });
           
            
        }

       

        public void  DownloadSalesOrOrders(OrderType orderType = OrderType.OutletToDistributor)
        {
           Task.Factory.StartNew(async() =>
            {
                TransactionResponse _response;

                string _url = MiddlewareHttpClient.BaseAddress +
                                                            "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&orderType={3}";

                string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password),
                                           module,  orderType);
                Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Contacting Server =>" + MiddlewareHttpClient.BaseAddress));
                HttpResponseMessage response =
                    await MiddlewareHttpClient.GetAsync(url);

                Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + response.StatusCode));

                _response =
                    await response.Content.ReadAsAsync<TransactionResponse>();
                if(_response !=null )
                {
                    if(_response.Result=="Error")
                    {
                        Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Server Response=>" + _response.ErrorInfo));
                        return;
                    }
                    if (!string.IsNullOrEmpty(_response.TransactionData))
                    {
                        Messenger.Default.Send(
                            string.Format(DateTime.Now.ToString("hh:mm:ss") + ": Server response:Items downloaded"));
                      DumpExportFilesAsync(_response);

                    }
                }else
                {
                    Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Unknown server error..check connection"));
                }
            });
           
         
        }

        private async void DumpExportFilesAsync(TransactionResponse response)
        {
            if (string.IsNullOrEmpty(response.TransactionData)) return;
            var orders = response.TransactionData;
            
            string fileName =string.Format("{0}_{1}{2}","export", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),".csv");
            string selectedPath = Path.Combine(TransactionsExportPath, fileName);
            if (string.IsNullOrEmpty(selectedPath))
            {
                Messenger.Default.Send(
                         string.Format(DateTime.Now.ToString("hh:mm:ss") + ":Undefined export file path..files not dumped"));
                return;
            }
            try
            {
               if(File.Exists(selectedPath))
                    File.Delete(selectedPath);
                using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(selectedPath, false))
                    {
                        await wr.WriteLineAsync(orders);

                    }
                }
                var refs = response.DocRefs;
                if (refs.Any())
                    await Acknowledge(refs.Distinct().ToList());
                Messenger.Default.Send(
                           string.Format(DateTime.Now.ToString("hh:mm:ss") + string.Format("Task completed,File damped on folder path {0}",selectedPath)));
               
            }
            catch (IOException ex)
            {
                Messenger.Default.Send(
                          string.Format(DateTime.Now.ToString("hh:mm:ss") + ": File System Error \n Details=>"+ex.Message));
                
            }
            catch(Exception ex)
            {
                Messenger.Default.Send(
                           string.Format(DateTime.Now.ToString("hh:mm:ss") + ": General Error \n Details=>" + ex.Message)); 
            }
           
        }

        async Task Acknowledge(IEnumerable<string> orderRefs)
        {
            await Task.Factory.StartNew(async () =>
                                                  {
                                                      TransactionsAcknowledgementResponse _response = null;
                                                      
                                                      var httpClient = MiddlewareHttpClient;
                                                      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                         string _url = MiddlewareHttpClient.BaseAddress +
                                                                       "api/Integrations/Acknowledge?username={0}&password={1}&integrationModule={2}";

                                                                             string url = string.Format(_url, _userName,
                                                                                    _otherUtilities.MD5Hash(_password),module);
                                                      var response = await httpClient.PostAsJsonAsync(url, orderRefs);
                                                      _response = await response.Content.ReadAsAsync<TransactionsAcknowledgementResponse>();
                                                      
                                                      int attemps = 5; //if we failed retry for 5 minutes then give up..next request will download everything again
                                                      while ((_response == null || _response.Result == "Error") &&
                                                             attemps > 0)
                                                      {
                                                          if(_response !=null)
                                                          FileUtility.LogError(_response.ErrorInfo);
                                                          attemps--;
                                                          Acknowledge(orderRefs);
                                                          Task.Delay(TimeSpan.FromSeconds(60));
                                                      }

                                                  });

        }

        private IEnumerable<string> ExtractExternalRefs(IEnumerable<string> orders)
        {
            var refsList = new HashSet<string>();
            foreach (var order in orders)
            {
                var splitter = new string[] { "\"," };
                var items = order.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                var orderRef = items.Length > 2 ? items.ElementAt(1) : null;

                if (orderRef != null)
                {
                    var splitter2 = new string[] { "\"" };
                    var test = orderRef.Split(splitter2, StringSplitOptions.RemoveEmptyEntries);
                    orderRef = test.Length >= 2 ? test.ElementAt(1) : null;
                }
                if (!string.IsNullOrEmpty(orderRef))
                    refsList.Add(orderRef);
            }
            return refsList.ToArray();
        }
    }
}
