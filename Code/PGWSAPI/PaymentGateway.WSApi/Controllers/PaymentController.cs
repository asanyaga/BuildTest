using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Services.Payment;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WSApi.Controllers
{
    public class PaymentController : Controller
    {
        private IMessageSerializer _messageDeserialize;
        private IResolveMessageService _resolveMessageService;
        private IPaymentRequestRepository _paymentRequestRepository;
        private IPaymentResponseRepository _paymentResponseRepository;
        private IAuditLogRepository _auditLogRepository;

        public PaymentController(IMessageSerializer messageDeserialize, IResolveMessageService resolveMessageService, IPaymentRequestRepository paymentRequestRepository, IPaymentResponseRepository paymentResponseRepository, IAuditLogRepository auditLogRepository)
        {
            _messageDeserialize = messageDeserialize;
            _resolveMessageService = resolveMessageService;
            _paymentRequestRepository = paymentRequestRepository;
            _paymentResponseRepository = paymentResponseRepository;
            _auditLogRepository = auditLogRepository;
        }

        #region Tests

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPaymentInstrumentList()
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            string result = SendTest(ClientRequestResponseType.PaymentInstrument);

            cResult.Content = result;
            //cResult.Content = "Test change";

            return cResult;
        }

        public ActionResult PaymentRequest()
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            string result = SendTest(ClientRequestResponseType.AsynchronousPayment);

            cResult.Content = result;

            return cResult;
        }

        public ActionResult SDPAsynchPaymentNotification()
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            SDPPaymentNotificationRequest sdpreq = GenerateSampleSDPAsynchronousPaymentNotificationRequest();
            string msg = JsonConvert.SerializeObject(sdpreq, new IsoDateTimeConverter());

            //ActionResult ar = ReceiveAsynchPaymentNotification(msg);

            //cResult.Content = ((ContentResult) ar).Content;
            return cResult;
        }

        public ActionResult AsynchPaymentNotificationRequest()
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            string result = SendTest(ClientRequestResponseType.AsynchronousPaymentNotification);

            cResult.Content = result;

            return cResult;
        }

        #endregion

        [HttpPost]
        public ActionResult MSend(string messageType, string jsonMessage)//mobile
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;
            
            ClientRequestResponseBase crrRequestMessage = _messageDeserialize.DeserializeClientRequest(messageType, jsonMessage);

            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       crrRequestMessage.ClientRequestResponseType.ToString()+"Request", "From Mobile client",
                                       "Json: " + jsonMessage);
            object response = null;

            if (crrRequestMessage == null)
            {
                response = new PGResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    ClientRequestResponseBase serverResponse = null;
                    if (crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification
                        || crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.BuyGoodsNotification)
                    {
                        var notifList = _resolveMessageService.ProcessClientPaymentNotificationRequest(crrRequestMessage);
                        response = notifList;
                    }
                    else
                    {
                        ServiceProvider sp = _resolveMessageService.GetServiceProvider(crrRequestMessage.DistributorCostCenterId);
                        if (sp == null)
                            response = ThrowError("This service provider is not registered.", crrRequestMessage);
                        else
                        {
                            ServerRequestBase msgToSDP = null;
                            _resolveMessageService.ProcessClientRequest(crrRequestMessage, sp, out msgToSDP);
                            //StartTheSaveAndSendThread(crrMessage, msg);
                            serverResponse = SaveNSend(crrRequestMessage, msgToSDP);
                            response = serverResponse;
                        }
                    }
                }
                catch (WebException we)
                {
                    //response = new ResponseBasic { Result = "Failed", ErrorInfo = "Unable to contact remote server.\n" + we.Message };
                    response = ThrowError("Unable to contact remote server.\n" + we.Message, crrRequestMessage);
                }
                catch(Exception ex)
                {
                    //response = new ResponseBasic {Result = "Failed", ErrorInfo = ex.Message};
                    response = ThrowError(ex.Message, crrRequestMessage);
                }
            }

            string result = JsonConvert.SerializeObject(response, new IsoDateTimeConverter());
            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       messageType + "Response", "To Mobile client",
                                       "Json: " + result);
            cResult.Content = result;
            return cResult;
        }

        [HttpPost]
        [NotificationController.JsonFilter]
        public ActionResult SLSend(string messageType, string jsonMessage)//sl
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            ClientRequestResponseBase crrRequestMessage = _messageDeserialize.DeserializeClientRequest(messageType, jsonMessage);

            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       crrRequestMessage.ClientRequestResponseType.ToString() + "Request", "From SL client", "Json: " + jsonMessage);

            object response = null;

            if (crrRequestMessage == null)
            {
                response = new PGResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    ClientRequestResponseBase serverResponse = null;
                    if (crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification
                        || crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.BuyGoodsNotification)
                    {
                        var notifList = _resolveMessageService.ProcessClientPaymentNotificationRequest(crrRequestMessage);
                        response = notifList;
                    }
                    else
                    {
                        ServiceProvider sp = _resolveMessageService.GetServiceProvider(crrRequestMessage.DistributorCostCenterId);
                        if (sp == null)
                            response = ThrowError("This service provider is not registered.", crrRequestMessage);
                        else
                        {
                            ServerRequestBase msgToSDP = null;
                            _resolveMessageService.ProcessClientRequest(crrRequestMessage, sp, out msgToSDP);
                            //StartTheSaveAndSendThread(crrMessage, msg);
                            serverResponse = SaveNSend(crrRequestMessage, msgToSDP);
                            response = serverResponse;
                        }
                    }
                }
                catch (WebException we)
                {
                    //response = new ResponseBasic { Result = "Failed", ErrorInfo = "Unable to contact remote server.\n" + we.Message };
                    response = ThrowError("Unable to contact remote server.\n" + we.Message, crrRequestMessage);
                }
                catch (Exception ex)
                {
                    //response = new ResponseBasic {Result = "Failed", ErrorInfo = ex.Message};
                    response = ThrowError(ex.Message, crrRequestMessage);
                }
            }

            string result = JsonConvert.SerializeObject(response, new IsoDateTimeConverter());
            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       messageType + "Response", "To SL client",
                                       "Json: " + result);
            cResult.Content = result;
            return cResult;
        }

        [HttpPost]
        [NotificationController.JsonFilter]
        public ActionResult PaymentReport(string messageType, string jsonMessage)
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;
            string result = "";
            try
            {
                var exparams = JsonConvert.DeserializeObject<ExReportRequestParams>(jsonMessage,new IsoDateTimeConverter());

                var response = _resolveMessageService.ProcessPaymentReportRequest(exparams.ServiceProviderId, exparams.StartDate, exparams.EndDate);

                result = JsonConvert.SerializeObject(response, new IsoDateTimeConverter());
            }
            catch (Exception ex)
            {
                result = "Error occurred while fetching report.\nError Details;\n" + ex.Message;
            }
            cResult.Content = result;
            return cResult;
        }

        [HttpPost]
        [NotificationController.JsonFilter]
        public ActionResult SLSendSimulator(string messageType, string jsonMessage)//sl
        {
            ContentResult cResult = new ContentResult();
            cResult.ContentType = "application/json";
            cResult.ContentEncoding = Encoding.UTF8;
            cResult.Content = null;

            ClientRequestResponseBase crrRequestMessage = _messageDeserialize.DeserializeClientRequest(messageType, jsonMessage);

            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       crrRequestMessage.ClientRequestResponseType.ToString() + "Request", "From SL client", "Json: " + jsonMessage);

            object response = null;

            if (crrRequestMessage == null)
            {
                response = new PGResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    ClientRequestResponseBase serverResponse = null;
                    if (crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification
                        || crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.BuyGoodsNotification)
                    {
                        var notifList = _resolveMessageService.ProcessClientPaymentNotificationRequest(crrRequestMessage);
                        response = notifList;
                    }
                    else
                    {
                        ServiceProvider sp = _resolveMessageService.GetServiceProvider(crrRequestMessage.DistributorCostCenterId);
                        if (sp == null)
                            response = ThrowError("This service provider is not registered.", crrRequestMessage);
                        else
                        {
                            ServerRequestBase msgToSDP = null;
                            _resolveMessageService.ProcessClientRequest(crrRequestMessage, sp, out msgToSDP);
                            //StartTheSaveAndSendThread(crrMessage, msg);
                            if (crrRequestMessage.ClientRequestResponseType == ClientRequestResponseType.PaymentInstrument)
                                serverResponse = SaveNSend(crrRequestMessage, msgToSDP);
                            else
                                serverResponse = SaveNSendSimulator(crrRequestMessage, msgToSDP);
                            response = serverResponse;
                        }
                    }
                }
                catch (WebException we)
                {
                    //response = new ResponseBasic { Result = "Failed", ErrorInfo = "Unable to contact remote server.\n" + we.Message };
                    response = ThrowError("Unable to contact remote server.\n" + we.Message, crrRequestMessage);
                }
                catch (Exception ex)
                {
                    //response = new ResponseBasic {Result = "Failed", ErrorInfo = ex.Message};
                    response = ThrowError(ex.Message, crrRequestMessage);
                }
            }

            string result = JsonConvert.SerializeObject(response, new IsoDateTimeConverter());
            _auditLogRepository.AddLog(crrRequestMessage.DistributorCostCenterId,
                                       messageType + "Response", "To SL client",
                                       "Json: " + result);
            cResult.Content = result;
            return cResult;
        }

        ClientRequestResponseBase ThrowError(string error, ClientRequestResponseBase enitity)
        {
            ClientRequestResponseBase response = new ClientRequestResponseBase();
            if (enitity is PaymentNotificationRequest)
            {
                PaymentNotificationResponse apn = new PaymentNotificationResponse();
                apn.DistributorCostCenterId = enitity.DistributorCostCenterId;
                apn.StatusCode = "Failed";
                apn.StatusDetail = error;
                response = apn;
            }
            else if (enitity is PaymentInstrumentRequest)
            {
                PaymentInstrumentResponse pi = new PaymentInstrumentResponse();
                pi.StatusDetail = "Failed: " + error;
                response = pi;
            }
            else if (enitity is PaymentRequest)
            {
                PaymentResponse apr = new PaymentResponse();
                apr.StatusCode = "Failed";
                apr.StatusDetail = error;
                response = apr;
            }

            return response;
        }

        private void SendToHSenid(ServerRequestBase requestMessage, ClientRequestResponseBase crrMessage, out ClientRequestResponseBase sdpResponse)
        {
            string hsenidUrl = "";
            WebClient wc = new WebClient();

            string mssg = JsonConvert.SerializeObject(requestMessage);
            wc.Encoding = Encoding.UTF8;

            hsenidUrl = SdpHost.GetSdpPaymentUri(crrMessage.ClientRequestResponseType);

            _auditLogRepository.AddLog(crrMessage.DistributorCostCenterId,
                                       crrMessage.ClientRequestResponseType.ToString() + "Request", "To HSenid",
                                       string.Format("Url: {0}; JsonRequest: {1}.",
                                                     hsenidUrl, mssg));

            Uri uri = new Uri(hsenidUrl, UriKind.Absolute);

            string strResponse = wc.UploadString(uri, "POST", mssg);

            _auditLogRepository.AddLog(crrMessage.DistributorCostCenterId,
                                       crrMessage.ClientRequestResponseType.ToString() + "Response", "From HSenid",
                                       string.Format("Url: {0}; JsonRequest: {1}.",
                                                     hsenidUrl, strResponse));

            sdpResponse = _messageDeserialize.DeserializeSDPResponse(strResponse, crrMessage.ClientRequestResponseType);

            sdpResponse.DistributorCostCenterId = crrMessage.DistributorCostCenterId;

            //LogThis(sdpResponse);

            if (sdpResponse == null) return;

            if (sdpResponse.ClientRequestResponseType != ClientRequestResponseType.PaymentInstrument
                && sdpResponse.ClientRequestResponseType != ClientRequestResponseType.AsynchronousPaymentQuery
                )
            {
                _paymentResponseRepository.Save(sdpResponse);
            }
        }

        //when SDP sends us payment notification
        [HttpPost]
        //[HttpGet]
        [SDPJsonFilter]
        public ActionResult ReceiveAsynchPaymentNotification(SDPPaymentNotificationRequest notification, SDPBuyGoodsNotificationRequest buyGoodsNotification)
        {
            //SDPAsynchronousPaymentNotificationRequest
            ContentResult cResult = new ContentResult();
            try
            {
                cResult.ContentType = "application/json";
                cResult.ContentEncoding = Encoding.UTF8;
                cResult.Content = null;

                ServerRequestBase sdpRequest = null;
                string serializedNotif = "";
                string requestType = "AsynchronousPaymentNotificationRequest";
                
                if (notification != null && notification.externalTrxId != null)
                {
                    serializedNotif = JsonConvert.SerializeObject(notification, new IsoDateTimeConverter());
                    sdpRequest = notification;
                }
                else if (buyGoodsNotification != null && buyGoodsNotification.receiptNumber != null)
                {
                    requestType = "SDPBuyGoodsNotificationRequest";
                    serializedNotif = JsonConvert.SerializeObject(buyGoodsNotification, new IsoDateTimeConverter());
                    sdpRequest = buyGoodsNotification;
                }
                _auditLogRepository.AddLog(Guid.Empty, requestType, "From HSenid",
                                           "Json: " + serializedNotif);

                /*_messageDeserialize.DeserializeSDPRequest(
                ClientRequestResponseType.AsynchronousPaymentNotification.ToString(), json);*/

                SDPPaymentNotificationResponse sdpapnResponse = null;
                ClientRequestResponseBase crrRequest = null;
                Guid spid = Guid.Empty;
                string subscriber = "";
                _resolveMessageService.ProcessSDPRequest(sdpRequest, out crrRequest, out subscriber, out spid); //convert into our entity

                sdpapnResponse = ProcessSDPPaymentNotification(sdpRequest, crrRequest);

                string result = JsonConvert.SerializeObject(sdpapnResponse, new IsoDateTimeConverter());

                _auditLogRepository.AddLog(Guid.Empty, "AsynchronousPaymentNotificationResponse", "To HSenid",
                                           "Json: " + result);
                cResult.Content = result;
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(Guid.Empty, "AsynchronousPaymentNotificationRequest", "From HSenid",
                                           "Error: " + ex.Message);
                cResult.Content = "Error processing notification";

            }
            return cResult;
        }

        SDPPaymentNotificationResponse ProcessSDPPaymentNotification(ServerRequestBase serverRequest, ClientRequestResponseBase crrRequest)
        {
            SDPPaymentNotificationResponse response = null;
            SDPPaymentNotificationRequest req = serverRequest as SDPPaymentNotificationRequest;
            try
            {
                _paymentRequestRepository.Save(crrRequest);
                response = new SDPPaymentNotificationResponse
                               {
                                   statusCode = "Success",
                                   statusDetail = "Success"
                               };
            }
            catch(Exception ex)
            {
                //error
                _auditLogRepository.AddLog(Guid.Empty, crrRequest.ClientRequestResponseType.ToString(), "To/From HSenid",
                                           "Error: " + ex.Message+"\n"+ex.InnerException.Message);
                response = new SDPPaymentNotificationResponse
                {
                    statusCode = "Failed",
                    statusDetail = "Failed"
                };
            }

            return response;
        }

        private ClientRequestResponseBase SaveNSend(ClientRequestResponseBase crrMessage, ServerRequestBase serverMessage)
        {
            if (crrMessage.ClientRequestResponseType != ClientRequestResponseType.AsynchronousPaymentQuery)
                Save(crrMessage);

            ClientRequestResponseBase sdpResponse = null;
            SendToHSenid(serverMessage, crrMessage, out sdpResponse);
            return sdpResponse;
        }

        private ClientRequestResponseBase SaveNSendSimulator(ClientRequestResponseBase crrMessage, ServerRequestBase serverMessage)
        {
            if (crrMessage.ClientRequestResponseType != ClientRequestResponseType.AsynchronousPaymentQuery)
                Save(crrMessage);

            ClientRequestResponseBase sdpResponse = null;
            var paymentResponse = _paymentResponseRepository.GetByTransRefId(new Guid(crrMessage.TransactionRefId)).OfType<PaymentResponse>().FirstOrDefault();
            if (paymentResponse != null)
            {
                sdpResponse = paymentResponse;
            }
            return sdpResponse;
        } 

        private void Save(ClientRequestResponseBase crrMessage)
        {
            if (crrMessage.ClientRequestResponseType != ClientRequestResponseType.PaymentInstrument)
                _paymentRequestRepository.Save(crrMessage);
        }

        #region Samples
        string LogThis(ClientRequestResponseBase response)
        {
            string log = "";
            if (response is PaymentInstrumentResponse)
            {
                var res = response as PaymentInstrumentResponse;
                string pi = res.PaymentInstrumentList.Aggregate("", (current, item) => current + ("Name: " + item.name + "Type: " + item.type + (item.accountId != null ? "Account: " + item.accountId : "")));
                log = string.Format("From HSenid; PaymentInstrumentResponse; PaymentInstrumentList: {0}; Status Detail: {1}", pi, res.StatusDetail);
            }
            return log;
        }

        private Guid sampleExternalTrxId = new Guid("f970dee1-fb32-40ca-9399-04f58b5e846c");
        private Guid sampleInternalTrxId = new Guid("b2eab1f0-9a2a-4d56-a6e4-04d812a0cfba");
        string SendTest(ClientRequestResponseType messageType)
        {
            string mssg = "";
            NameValueCollection param = new NameValueCollection();
            DateTime now = DateTime.Now;

            #region PaymentInstrument
            if (messageType == ClientRequestResponseType.PaymentInstrument)
            {
                mssg = JsonConvert.SerializeObject(new PaymentInstrumentRequest
                                                       {
                                                           Id                             = Guid.NewGuid(),
                                                           TransactionRefId               = Guid.NewGuid().ToString(),
                                                           ClientRequestResponseType      = ClientRequestResponseType.PaymentInstrument,
                                                           SubscriberId                   = "tel:254701234563",
                                                           DateCreated                    = now,
                                                           DistributorCostCenterId        = new Guid("500B3503-03B0-4BE5-A397-AB617B32B038")
                                                       }
                    , new IsoDateTimeConverter());
                param.Add("messageType", "PaymentInstrument");
            }
            #endregion

            #region AsynchronousPayment
            if (messageType == ClientRequestResponseType.AsynchronousPayment)
            {
                mssg = JsonConvert.SerializeObject(new PaymentRequest
                                                       {
                                                           Id                        = Guid.NewGuid(),
                                                           AccountId                 = "123",
                                                           SubscriberId              = "tel:0724556667",
                                                           AllowOverPayment          = true,
                                                           PaymentInstrumentName     = "M-Pesa",
                                                           OrderNumber               = "Ord_John_O003_2012.2.29.16:21_7",
                                                           InvoiceNumber             = "Inv_John_O003_2012.2.29.16:21_7",
                                                          
                                                           TransactionRefId          = sampleExternalTrxId.ToString(),
                                                           Currency                  = "KSH",
                                                           ApplicationId             = Guid.Parse("36EDE98E-714D-4E8E-88E1-5E96A9B2EF67").ToString(),
                                                           Amount                    = 5,
                                                           AllowPartialPayments      = false,
                                                           ClientRequestResponseType = ClientRequestResponseType.AsynchronousPayment,
                                                           DateCreated               = DateTime.Now,
                                                           smsDescription            = "pil",
                                                           DistributorCostCenterId        = new Guid("500B3503-03B0-4BE5-A397-AB617B32B038")
                                                       }, new IsoDateTimeConverter());
                param.Add("messageType", "AsynchronousPayment");
            }
            #endregion 

            #region AsynchronousPaymentNotification
            if (messageType == ClientRequestResponseType.AsynchronousPaymentNotification)
            {
                mssg = JsonConvert.SerializeObject(new PaymentNotificationRequest
                                                       {
                                                           TransactionRefId = sampleExternalTrxId.ToString(),
                                                           ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification,
                                                           DistributorCostCenterId        = new Guid("500B3503-03B0-4BE5-A397-AB617B32B038")
                                                       }, new IsoDateTimeConverter());
                param.Add("messageType", "AsynchronousPaymentNotification");

            }
            #endregion

            param.Add("jsonMessage", mssg);

            string pgBridgeUrl = ConfigurationSettings.AppSettings["PgBridgeUrl"];

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            //string pgbridgeUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            _auditLogRepository.AddLog(Guid.Empty,messageType.ToString(),"Test PGBridge", string.Format("PGBridge Url: {0}; Json : {1}", pgBridgeUrl, mssg));
            Uri uri = new Uri(pgBridgeUrl + "Payment/Msend", UriKind.Absolute);

           

            var bytes = wc.UploadValues(uri, "POST", param);

            string result = ByteArrayToString(bytes, EncodingType.UTF8);

            return result;
        }

        public static string ByteArrayToString(byte[] bytes, EncodingType encodingType)
        {
            System.Text.Encoding encoding = null;
            switch (encodingType)
            {
                case EncodingType.ASCII:
                    encoding = new System.Text.ASCIIEncoding();
                    break;
                case EncodingType.Unicode:
                    encoding = new System.Text.UnicodeEncoding();
                    break;
                case EncodingType.UTF7:
                    encoding = new System.Text.UTF7Encoding();
                    break;
                case EncodingType.UTF8:
                    encoding = new System.Text.UTF8Encoding();
                    break;
            }
            return encoding.GetString(bytes);
        }

        ClientRequestResponseBase GenerateSampleResponce(ClientRequestResponseBase crrMessage)
        {
            ClientRequestResponseType messageType = crrMessage.ClientRequestResponseType;
            switch(messageType)
            {
                case ClientRequestResponseType.PaymentInstrument:
                    return GetSamplePaymentList();
                case ClientRequestResponseType.AsynchronousPayment:
                    return GenerateSamplePaymentResponse(crrMessage as PaymentRequest);
            }

            return null;
        }

        PaymentInstrumentResponse GetSamplePaymentList()
        {
            PaymentInstrumentResponse resp          = new PaymentInstrumentResponse();
            List<SDPPaymentInstrument> paymentInst     = SamplePaymentList();
            resp.PaymentInstrumentList              = paymentInst;
            resp.Id                                 = Guid.NewGuid();
            resp.TransactionRefId                   = Guid.NewGuid().ToString();
            resp.ClientRequestResponseType          = ClientRequestResponseType.PaymentInstrument;
            resp.DateCreated                        = DateTime.Now;
            resp.StatusDetail                       = "Success";

            return resp;
        }

        List<SDPPaymentInstrument> SamplePaymentList()
        {
            List<SDPPaymentInstrument> retList = new List<SDPPaymentInstrument>();
            SDPPaymentInstrument _1 = new SDPPaymentInstrument
                                           {
                                               //accountId = "123",
                                               name = "Operator",
                                               //type = Lib.Domain.Payments.PaymentInstrumentType.async
                                           };
            retList.Add(_1);
            SDPPaymentInstrument _2 = new SDPPaymentInstrument
                                           {
                                               //accountId = "456",
                                               name = "BuyGoods",
                                               //type = Lib.Domain.Payments.PaymentInstrumentType.all
                                           };
            retList.Add(_2);
            SDPPaymentInstrument _3 = new SDPPaymentInstrument
                                           {
                                               //accountId = "789",
                                               name = "Equity",
                                               //type = Lib.Domain.Payments.PaymentInstrumentType.all
                                           };
            retList.Add(_3);

            return retList;
        }

        PaymentResponse GenerateSamplePaymentResponse(PaymentRequest request)
        {
            PaymentResponse apr = new PaymentResponse();
            Random ran = new Random();

            apr.BusinessNumber            = Guid.NewGuid().ToString();
            apr.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPayment;
            apr.DateCreated               = DateTime.Now;
            apr.TransactionRefId          = request.TransactionRefId;
            apr.Id                        = Guid.NewGuid();
            apr.SDPTransactionRefId = sampleInternalTrxId.ToString();
            apr.LongDescription           = "Long description";
            apr.SDPReferenceId            = "12345678";// ran.Next(10000000, 99999999);
            apr.ShortDescription          = "Short Desc.";
            apr.StatusDetail              = "OK";

            return apr;
        }

        SDPPaymentNotificationRequest GenerateSampleSDPAsynchronousPaymentNotificationRequest()
        {
            var existingPaymentRequest =
                _paymentRequestRepository.GetAll().OfType<PaymentRequest>()
                    .Where(n => n.TransactionRefId == sampleExternalTrxId.ToString()).LastOrDefault();

            SDPPaymentNotificationRequest sample = new SDPPaymentNotificationRequest();
            sample.externalTrxId = existingPaymentRequest.TransactionRefId.ToString();
            sample.paidAmount = existingPaymentRequest.Amount.ToString();
            sample.currency = existingPaymentRequest.Currency;
            sample.internalTrxId = Guid.NewGuid().ToString();
            sample.statusCode = "Success";
            sample.statusDetail = "Success";
            //sample.referenceId = existingPaymentResponse.SDPReferenceId;
            sample.timestamp = DateTime.Now;

            return sample;
        }

        SDPPaymentNotificationResponse GenerateSampleSDPAsynchronousPaymentNotificationResponse(SDPPaymentNotificationRequest request)
        {
            SDPPaymentNotificationResponse sample = new SDPPaymentNotificationResponse();
            sample.statusCode = "Success";
            sample.statusDetail = "Success";

            return sample;
        }

        private Thread StartTheSaveAndSendThread(ClientRequestResponseBase crrMessage, ServerRequestBase serverMessage)
        {
            Thread thread = new Thread(() => SaveNSend(crrMessage, serverMessage));
            return thread;
        }

        public enum EncodingType
        {
            ASCII,
            Unicode,
            UTF7,
            UTF8
        }
        #endregion

    }

    public class SDPJsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var incomingData = new StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
            filterContext.ActionParameters["jsonMessage"] = incomingData.Replace("jsonMessage=", "");

            base.OnActionExecuting(filterContext);
        }
    }

    public class ExReportRequestParams
    {
        public Guid ServiceProviderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
