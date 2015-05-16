using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace Integration.QuickBooks.Lib.Services.Impl
{
    internal class TransactionsDownloadService : MiddleWareBase, ITransactionsDownloadService
    {
        private string _userName;
        private string _password;
        private IntegrationModule module;
        private List<QuickBooksOrderDocumentDto> _transactionData=null;
        public TransactionsDownloadService()
        {
            module=IntegrationModule.QuickBooks;
            _userName = CredentialsManager.GetUserName();
            _password = CredentialsManager.GetPassword();
            _transactionData=new List<QuickBooksOrderDocumentDto>();
        }

        public TransactionResponse DownloadAllAsync(string orderref = "", bool includeInvoiceAndReceipts = true, DocumentStatus documentStatus = DocumentStatus.Closed)
        {
            var time =new TimeSpan(0,0,5,0);
            var time1 = TimeSpan.FromMinutes(5).Seconds;
            var task= Task.Run(async () =>
                                     {
                                          TransactionResponse res=new TransactionResponse();
                                          try
                                          {
                                              string url = string.Format(MiddlewareHttpClient.BaseAddress +
                                                                         "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&documentRef={3}&includeInvoiceAndReceipts={4}&documentStatus={5}",
                                                                         _userName,
                                                                         _otherUtilities.MD5Hash(_password),
                                                                         module, orderref,
                                                                         includeInvoiceAndReceipts,documentStatus);
                                              Messenger.Default.Send(
                                                  string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                                                ":Contacting Server =>" +
                                                                MiddlewareHttpClient.BaseAddress));

                                              HttpResponseMessage response =
                                                  await MiddlewareHttpClient.GetAsync(url);
                                              Messenger.Default.Send(
                                                  string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                                                ":Server Response=>" + response.StatusCode));
                                              res=await response.Content.ReadAsAsync<TransactionResponse>();
                                          }catch(Exception ex)
                                          {
                                              
                                          }

                                          return res;
                                          
                                      });

             //Task.Delay(TimeSpan.FromMinutes(5).Seconds);

            task.Wait(time);
            return task.Result;

        }

        public async Task<TransactionExportResponse> GetNextOrder(OrderType orderType = OrderType.OutletToDistributor, DocumentStatus documentStatus = DocumentStatus.Closed)
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/GetNextOrderToExport?username={0}&password={1}&orderType={2}&documentStatus={3}";

                string url = string.Format(urlSuffix, _userName,_otherUtilities.MD5Hash(_password), orderType, documentStatus);

                var response = client.GetAsync(url).Result;
                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;
                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse { Info = ex.Message };
            }
        }

        public async Task<TransactionExportResponse> GetNextInvoice()
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/GetNextInvoiceToExport?username={0}&password={1}";

                string url = string.Format(urlSuffix, _userName, _otherUtilities.MD5Hash(_password));

                var response = client.GetAsync(url).Result;
                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;
                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse { Info = ex.Message };
            }
        }

        public async Task<TransactionExportResponse> GetNextReceipt()
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/GetNextReceipt?username={0}&password={1}";

                string url = string.Format(urlSuffix, _userName, _otherUtilities.MD5Hash(_password));

                var response = client.GetAsync(url).Result;
                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;
                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse { Info = ex.Message };
            }
        }

        public async Task<TransactionExportResponse> MarkAsExported(string orderExternalRef)
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/MarkOrderAsExported";

                string url = string.Format(urlSuffix, _userName, _otherUtilities.MD5Hash(_password));


                string data = string.Format("?username={0}&password={1}&orderExternalRef={2}", _userName, _otherUtilities.MD5Hash(_password), orderExternalRef);
                HttpContent content = new StringContent(data);
                var response =  client.PostAsync(url + data, content).Result;

                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;


                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse() { Info = ex.Message };
            }
        }

        public async Task<TransactionExportResponse> MarkInvoiceAsExported(Guid invoceId)
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/MarkInvoiceAsExported";

                string url = string.Format(urlSuffix, _userName, _otherUtilities.MD5Hash(_password));


                string data = string.Format("?username={0}&password={1}&invoiceId={2}", _userName, _otherUtilities.MD5Hash(_password), invoceId);
                HttpContent content = new StringContent(data);
                var response = client.PostAsync(url + data, content).Result;

                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;


                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse() { Info = ex.Message };
            }
        }

        public async Task<TransactionExportResponse> MarkReceiptAsExported(Guid receiptId)
        {
            HttpClient client = MiddlewareHttpClient;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string urlSuffix = client.BaseAddress + "api/new/Integration/MarkReceiptAsExported";

                string url = string.Format(urlSuffix, _userName, _otherUtilities.MD5Hash(_password));


                string data = string.Format("?username={0}&password={1}&receiptId={2}", _userName, _otherUtilities.MD5Hash(_password),receiptId);
                HttpContent content = new StringContent(data);
                var response = client.PostAsync(url + data, content).Result;

                TransactionExportResponse _response = response.Content.ReadAsAsync<TransactionExportResponse>().Result;


                if (_response == null)
                {
                    return new TransactionExportResponse() { Success = false };
                }


                return _response;
            }
            catch (Exception ex)
            {
                return new TransactionExportResponse() { Info = ex.Message };
            }
        }

        public TransactionResponse DownloadTransactionAsync(string orderref = "", bool includeInvoiceAndReceipts = true, DocumentStatus documentStatus = DocumentStatus.Closed)
        {
            var time = new TimeSpan(0, 0, 5, 0);
            var time1 = TimeSpan.FromMinutes(5).Seconds;
            var task = Task.Run(async () =>
            {
                TransactionResponse res = new TransactionResponse();
                try
                {
                    string url = string.Format(MiddlewareHttpClient.BaseAddress +
                                               "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&documentRef={3}&includeInvoiceAndReceipts={4}&documentStatus={5}",
                                               _userName,
                                               _otherUtilities.MD5Hash(_password),
                                               module, orderref,
                                               includeInvoiceAndReceipts, documentStatus);
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ":Contacting Server =>" +
                                      MiddlewareHttpClient.BaseAddress));

                    HttpResponseMessage response =
                        await MiddlewareHttpClient.GetAsync(url);
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ":Server Response=>" + response.StatusCode));
                    res = await response.Content.ReadAsAsync<TransactionResponse>();
                }
                catch (Exception ex)
                {

                }

                return res;

            });

            //Task.Delay(TimeSpan.FromMinutes(5).Seconds);

            task.Wait(time);
            return task.Result;

        }

        public async Task<List<QuickBooksOrderDocumentDto>> GetOrdersPendingExport(string orderref = "", bool includeInvoiceAndReceipts = true)
        {
            List<QuickBooksOrderDocumentDto> data=new List<QuickBooksOrderDocumentDto>();
            try
            {
            
            TransactionResponse _response;
            string _url = MiddlewareHttpClient.BaseAddress +
                          "api/Integrations/Transaction?username={0}&password={1}&integrationModule={2}&documentRef={3}&includeInvoiceAndReceipts={4}";

            string url = string.Format(_url, _userName,
                                       _otherUtilities.MD5Hash(_password),
                                       module, orderref,
                                       includeInvoiceAndReceipts);
            Messenger.Default.Send(
                string.Format(DateTime.Now.ToString("hh:mm:ss") +
                              ":Contacting Server =>" +
                              MiddlewareHttpClient.BaseAddress));

            HttpResponseMessage response = await MiddlewareHttpClient.GetAsync(url);
            Messenger.Default.Send(
                string.Format(DateTime.Now.ToString("hh:mm:ss") +
                              ":Server Response=>" + response.StatusCode));
            _response =
                await response.Content.ReadAsAsync<TransactionResponse>();
            if (_response != null)
            {
                if (_response.Result == "Error")
                {
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ":Server Response=>" +
                                      _response.ErrorInfo));
                    return data;
                }
                if (!string.IsNullOrEmpty(_response.TransactionData))
                {
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ": Server response: items downloaded"));
                    data = JsonConvert.DeserializeObject
                        <List<QuickBooksOrderDocumentDto>>(
                            _response.TransactionData);


                }
            }

          
            return data;
            }
            catch (Exception ex)
            {
                FileUtility.LogError(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public List<QuickBooksOrderDocumentDto> GetTransactions()
        {
            return _transactionData;
        }

        internal  async Task<bool> AcknowledgeDocuments(List<string> orderRefs)
        {
            bool taskDone = false;
            //await Task.Factory.StartNew(async () =>
            //{
        await Task.Run(async ()=>
                {
                    TransactionsAcknowledgementResponse _response = null;

                    var httpClient = MiddlewareHttpClient;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string _url = MiddlewareHttpClient.BaseAddress +
                                  "api/Integrations/Acknowledge?username={0}&password={1}&integrationModule={2}";

                    string url = string.Format(_url, _userName,
                                               _otherUtilities.MD5Hash(_password), module);
                    var response = await httpClient.PostAsJsonAsync(url, orderRefs);
                    _response = await response.Content.ReadAsAsync<TransactionsAcknowledgementResponse>();

                    int attemps = 5; //if we failed retry for 5 minutes then give up..next request will download everything again
                    while ((_response == null || _response.Result == "Error") &&
                           attemps > 0)
                    {
                        if (_response != null)
                            FileUtility.LogError(_response.ErrorInfo);
                        attemps--;
                        AcknowledgeDocuments(orderRefs);
                        // Task.Delay(TimeSpan.FromSeconds(60));
                    
                    }

                    taskDone = _response != null && _response.Result == "Success";
                });
          

            return taskDone;
        }

        public TransactionResponse DownloadReturnsAsync(string orderref = "")
        {
            var task = Task.Run(async () =>
            {
                TransactionResponse res = new TransactionResponse();
                try
                {
                    string url = string.Format(MiddlewareHttpClient.BaseAddress +
                                               "api/client/Integrations/InventoryReturnTransaction/{0}/{1}/{2}/{3}",
                                               _userName,
                                               _otherUtilities.MD5Hash(_password),
                                               module, orderref);
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ":Contacting Server =>" +
                                      MiddlewareHttpClient.BaseAddress));

                    HttpResponseMessage response =
                        await MiddlewareHttpClient.GetAsync(url);
                    Messenger.Default.Send(
                        string.Format(DateTime.Now.ToString("hh:mm:ss") +
                                      ":Server Response=>" + response.StatusCode));
                    res = await response.Content.ReadAsAsync<TransactionResponse>();
                }
                catch (Exception ex)
                {

                }

                return res;

            });
            task.Wait(TimeSpan.FromMinutes(5).Seconds);
            return task.Result;

        }
        
    }
}
