using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response;
using PaymentGateway.WSApi.Lib.Domain.SMS.SDP;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Services.Payment;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WebAPI.ApiControllers
{
    public class PaymentController : ApiController
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

        //public HttpResponseMessage


        public HttpResponseMessage GetAbout()
        {
            string version = "Payment Gateway Bridge version: " +
                             ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();

            return Request.CreateResponse(HttpStatusCode.OK, version);
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentInstrumentList(PaymentInstrumentRequest request)
        {
            var response = ClientRequest(request);
            var httpResponse = Request.CreateResponse(HttpStatusCode.OK, response.Response as PaymentInstrumentResponse);
            return httpResponse;
        }

        [HttpPost]
        public HttpResponseMessage PaymentRequest(PaymentRequest request)
        {
            var response = ClientRequest(request);
            if (response.Response is PaymentResponse)
            {
                var rs = response.Response as PaymentResponse;
                SendSMS(request.SubscriberId,request.DistributorCostCenterId,rs.LongDescription);
            }
            var httpResponse = Request.CreateResponse(HttpStatusCode.OK, response.Response as PaymentResponse);
            return httpResponse;
        }

        private void SendSMS(string recipient,Guid sip,string msg)
        {
            var sms = new SDPSMSRequest();
            ServiceProvider sp = _resolveMessageService.GetServiceProvider(sip);
            if (sp == null)
            {
                return;
            }
            sms = new SDPSMSRequest
            {
                applicationId = sp.SdpAppId,
                password = sp.SdpPassword,
                destinationAddresses = new List<string>(){recipient}.Distinct().ToList(),
                deliveryStatusRequest = 1,
                encoding = SDPSmsEncoding.Text,
                message = msg,
                sourceAddress = sp.SmsShortCode,
                binaryHeader = "",
                chargingAmount = 0,
                version = "1.0",


            };
           HSenidSendNotification(sms);
           // return Request.CreateResponse(HttpStatusCode.OK, sdpResponse.statusCode + " " + sdpResponse.statusDetail);
        }
        void HSenidSendNotification(SDPSMSRequest sdpSms)
        {
            SDPSMSResponse response;
            string jsonRequest = JsonConvert.SerializeObject(sdpSms);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = "";// Request.Url.AbsoluteUri.Substring(0, (Request.Url.AbsoluteUri.Length - 1));
            hsenidUrl = SdpHost.GetSdpsmsUri();
            Uri uri = new Uri(hsenidUrl, UriKind.Absolute);


            string jsonResponse = wc.UploadString(uri, "POST", jsonRequest);
           // _messageValidation.CanDeserializeMessage(jsonResponse, out response);
            //return response;
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentNotification(PaymentNotificationRequest request)
        {
            var r = ClientRequest(request);
           // var eeee = response.Response as PaymentNotificationResponse;
           // string data = "{\"SDPTransactionRefId\":\"913111810040322\",\"SDPReferenceId\":\"123303\",\"Currency\":\"KES\",\"StatusCode\":\"S1000\",\"StatusDetail\":\"Request was Successfully processed, Due amount fully paid.\",\"SubscriberId\":\"tel:254722557538\",\"AccountId\":\"254707102171\",\"PaidAmount\":0.0,\"BalanceDue\":0.0,\"TotalAmount\":0.0,\"TimeStamp\":\"0001-01-01T00:00:00\",\"PaymentNotificationDetails\":[{\"Id\":\"57ca4d94-9f0b-43f5-afdc-a0fc09d6f67d\",\"PaidAmount\":10.0,\"BalanceDue\":0.0,\"TotalAmount\":10.0,\"TimeStamp\":\"2013-11-18T10:05:00\",\"Status\":\"S1000; Request was Successfully processed, Due amount fully paid.\"}],\"Id\":\"57ca4d94-9f0b-43f5-afdc-a0fc09d6f67d\",\"DistributorCostCenterId\":\"2a17cd47-44f0-4ba1-b737-090adb5ab0fd\",\"TransactionRefId\":\"f10ad033-ca1c-421a-aebf-b425081a5684\",\"ClientRequestResponseType\":3,\"DateCreated\":\"2013-11-18T10:06:12.1645451\"}";
           // var r = JsonConvert.DeserializeObject<PaymentNotificationResponse>(data);
           //// r.SDPReferenceId = request.SDPReferenceId;
           // r.TransactionRefId = request.TransactionRefId;
           // r.Id = Guid.NewGuid();
           // var item = r.PaymentNotificationDetails.FirstOrDefault();
           // item.Id = Guid.NewGuid();
            var httpResponse = Request.CreateResponse(HttpStatusCode.OK, r);
            return httpResponse;
        }

        [HttpPost]
        public HttpResponseMessage GetBuyGoodsNotification(BuyGoodsNotificationResponse request)
        {
            var response = ClientRequest(request);
            var httpResponse = Request.CreateResponse(HttpStatusCode.OK, response.Response as PaymentNotificationResponse);
            return httpResponse;
        }

        private PGBResponse ClientRequest(ClientRequestResponseBase request)
        {
            PGBResponse pgresponse = new PGBResponse();pgresponse.Success = false;
            try
            {
                _auditLogRepository.AddLog(request.DistributorCostCenterId,
                                           request.ClientRequestResponseType.ToString() + "Request", "From client",
                                           "Json: " + JsonConvert.SerializeObject(request, new IsoDateTimeConverter()));
                
                if (request.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification)
                {
                    ClientRequestResponseBase paymentNotfResponse = _resolveMessageService.ProcessClientPaymentNotificationRequest(request);
                    pgresponse.Success = true;
                    pgresponse.Response = paymentNotfResponse;
                }
                else
                {
                    ServiceProvider sp = _resolveMessageService.GetServiceProvider(request.DistributorCostCenterId);
                    if (sp == null)
                    {
                        ClientRequestResponseBase errorResponse = Common.ThrowError("This service provider is not registered.", request);
                        pgresponse.Success = false;
                        pgresponse.ErrorInfo = "This service provider is not registered.";
                        pgresponse.Response = errorResponse;
                    }
                    else
                    {
                        ServerRequestBase msgToSDP = null;
                        _resolveMessageService.ProcessClientRequest(request, sp, out msgToSDP);
                        ClientRequestResponseBase sdpResponse = SaveNSend(request, msgToSDP);
                        pgresponse.Success = true;
                        pgresponse.Response = sdpResponse;
                    }
                }

            }
            catch (WebException we)
            {
                ClientRequestResponseBase errorResponse = Common.ThrowError("Unable to contact remote server.\n" + we.Message, request);
                pgresponse.Success = false;
                pgresponse.ErrorInfo = "Unable to contact remote server.\n" + we.Message;
                pgresponse.Response = errorResponse;
            }
            catch (Exception ex)
            {
                ClientRequestResponseBase errorResponse = Common.ThrowError(ex.Message, request);
                pgresponse.Success = false;
                pgresponse.ErrorInfo = "An error occurred while processing you request. Error details.\n" + ex.Message;
                pgresponse.Response = errorResponse;
            }

            return pgresponse;
        }

        private ClientRequestResponseBase SaveNSend(ClientRequestResponseBase crrMessage, ServerRequestBase serverMessage)
        {
            if (crrMessage.ClientRequestResponseType != ClientRequestResponseType.AsynchronousPaymentQuery)
                Save(crrMessage);

            ClientRequestResponseBase sdpResponse = null;
            SendToHSenid(serverMessage, crrMessage, out sdpResponse);
            return sdpResponse;
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
            if (sdpResponse is PaymentResponse && requestMessage is SDPPaymentRequest)
            {
                var sdpresp = sdpResponse as PaymentResponse;
                var sdpreq = requestMessage as SDPPaymentRequest;
                sdpresp.SubscriberId = sdpreq.subscriberId;
            }
           
            sdpResponse.DistributorCostCenterId = crrMessage.DistributorCostCenterId;

            if (sdpResponse.ClientRequestResponseType != ClientRequestResponseType.PaymentInstrument
                && sdpResponse.ClientRequestResponseType != ClientRequestResponseType.AsynchronousPaymentQuery
                )
            {
                _paymentResponseRepository.Save(sdpResponse);
            }
        }

        private void Save(ClientRequestResponseBase crrMessage)
        {
            if (crrMessage.ClientRequestResponseType != ClientRequestResponseType.PaymentInstrument)
                _paymentRequestRepository.Save(crrMessage);
        }

        //ClientRequestResponseBase ThrowError(string error, ClientRequestResponseBase enitity)
        //{
        //    ClientRequestResponseBase response = new ClientRequestResponseBase();
        //    if (enitity is PaymentNotificationRequest)
        //    {
        //        PaymentNotificationResponse apn = new PaymentNotificationResponse();
        //        apn.DistributorCostCenterId = enitity.DistributorCostCenterId;
        //        apn.StatusCode = "Error";
        //        apn.StatusDetail = error;
        //        response = apn;
        //    }
        //    else if (enitity is PaymentInstrumentRequest)
        //    {
        //        PaymentInstrumentResponse pi = new PaymentInstrumentResponse();
        //        pi.StatusCode = "Error";
        //        pi.StatusDetail = error;
        //        response = pi;
        //    }
        //    else if (enitity is PaymentRequest)
        //    {
        //        PaymentResponse apr = new PaymentResponse();
        //        apr.StatusCode = "Error";
        //        apr.StatusDetail = error;
        //        response = apr;
        //    }

        //    return response;
        //}

        [HttpPost]
        public SDPPaymentNotificationResponse ReceiveAsynchPaymentNotification(SDPPaymentNotificationRequest notification)
            //, SDPBuyGoodsNotificationRequest buyGoodsNotification
        {
            SDPPaymentNotificationResponse sdpapnResponse = new SDPPaymentNotificationResponse();
            try
            {
                ServerRequestBase sdpRequest = null;
                string serializedNotif = "";
                string requestType = "AsynchronousPaymentNotificationRequest";

                if (notification != null && notification.externalTrxId != null)
                {
                    serializedNotif = JsonConvert.SerializeObject(notification, new IsoDateTimeConverter());
                    sdpRequest = notification;
                }
                //else if (buyGoodsNotification != null && buyGoodsNotification.receiptNumber != null)
                //{
                //    requestType = "SDPBuyGoodsNotificationRequest";
                //    serializedNotif = JsonConvert.SerializeObject(buyGoodsNotification, new IsoDateTimeConverter());
                //    sdpRequest = buyGoodsNotification;
                //}
                _auditLogRepository.AddLog(Guid.Empty, requestType, "From HSenid",
                                           "Json: " + serializedNotif);

                /*_messageDeserialize.DeserializeSDPRequest(
                ClientRequestResponseType.AsynchronousPaymentNotification.ToString(), json);*/
                string subcriberId = "";
                Guid spid = Guid.Empty;
                ClientRequestResponseBase crrRequest = null;
                _resolveMessageService.ProcessSDPRequest(sdpRequest, out crrRequest,out subcriberId,out spid); //convert into our entity

                if (crrRequest is PaymentNotificationRequest)
                {
                    var noti = crrRequest as PaymentNotificationRequest;
                    string msg = string.Format("Distributr => : Payment for Ref. {0}. Received {1} {2}  from mpesa.Balance is {3}", noti.SDPReferenceId, noti.SDPPaidAmount, noti.SDPCurrency, noti.SDPBalanceDue);
                    SendSMS(subcriberId, spid, msg);

                }

                sdpapnResponse = ProcessSDPPaymentNotification(sdpRequest, crrRequest);

                string result = JsonConvert.SerializeObject(sdpapnResponse, new IsoDateTimeConverter());

                _auditLogRepository.AddLog(Guid.Empty, "AsynchronousPaymentNotificationResponse", "To HSenid",
                                           "Json: " + result);
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(Guid.Empty, "AsynchronousPaymentNotificationRequest", "From HSenid",
                                           "Error: " + ex.Message);
                sdpapnResponse.statusCode = "Error";
                sdpapnResponse.statusDetail = "Error processing notification";

            }
            return sdpapnResponse;
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
            catch (Exception ex)
            {
                //error
                _auditLogRepository.AddLog(Guid.Empty, crrRequest.ClientRequestResponseType.ToString(), "To/From HSenid",
                                           "Error: " + ex.Message + "\n" + ex.InnerException.Message);
                response = new SDPPaymentNotificationResponse
                {
                    statusCode = "Error",
                    statusDetail = "Error"
                };
            }

            return response;
        }

    }
}
