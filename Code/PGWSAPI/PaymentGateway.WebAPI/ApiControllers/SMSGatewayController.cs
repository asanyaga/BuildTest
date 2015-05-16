using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Domain.SMS;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;
using PaymentGateway.WSApi.Lib.Domain.SMS.SDP;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Clients;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Repository.SMS;
using PaymentGateway.WSApi.Lib.Services.Payment;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WebAPI.ApiControllers
{
    public class SMSGatewayController : ApiController
    {
        private IMessageValidation _messageValidation;
        private IAuditLogRepository _auditLogRepository;
        private IResolveMessageService _resolveMessageService;
        private IDocSMSRepository _docSMSRepository;
        private ISmsQueryResolverService _smsQueryResolver;
        private IClientRepository _clientRepository;

        public SMSGatewayController(IMessageValidation messageValidation, IAuditLogRepository auditLogRepository, IResolveMessageService resolveMessageService, IDocSMSRepository docSmsRepository, ISmsQueryResolverService smsQueryResolver, IClientRepository clientRepository)
        {
            _messageValidation = messageValidation;
            _auditLogRepository = auditLogRepository;
            _resolveMessageService = resolveMessageService;
            _docSMSRepository = docSmsRepository;
            _smsQueryResolver = smsQueryResolver;
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public HttpResponseMessage SmsNotificationSend()
        {
            var sms = new SDPSMSRequest();
            ServiceProvider sp = _resolveMessageService.GetServiceProvider(new Guid("ae19c448-47a4-4044-843c-102085e0157f"));
            if (sp == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "This service provider is not registered.");
            }
            sms = new SDPSMSRequest
            {
                applicationId = sp.SdpAppId,
                password = sp.SdpPassword,
                destinationAddresses = new List<string>(){"254722557538"}.Distinct().Select(n => "tel:" + n).ToList(),
                deliveryStatusRequest = 1,
                encoding = SDPSmsEncoding.Text,
                message = "Test",
                sourceAddress = "20358",
                binaryHeader = "",
                chargingAmount = 0,
                version = "1.0",


            };
            SDPSMSResponse sdpResponse = HSenidSendNotification(sms);
            return Request.CreateResponse(HttpStatusCode.OK, sdpResponse.statusCode + " " + sdpResponse.statusDetail);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpPost]
        public HttpResponseMessage PostSmsNotification(PGNotificationSMS notification)
        {
            var sms = new SDPSMSRequest();
            ServiceProvider sp = _resolveMessageService.GetServiceProvider(notification.HubId);
            if (sp == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "This service provider is not registered.");
            }
            sms = new SDPSMSRequest
            {
                applicationId = sp.SdpAppId,
                password = sp.SdpPassword,
                destinationAddresses = notification.Recipitents.Distinct().Select(n => "tel:" + n).ToList(),
                deliveryStatusRequest = 1,
                encoding = SDPSmsEncoding.Text,
                message = notification.SmsBody,
                sourceAddress = "20358",
                binaryHeader = "",
                chargingAmount = 0,
                version = "1.0",
                
                
            };
            SDPSMSResponse sdpResponse = HSenidSendNotification(sms);
            return Request.CreateResponse(HttpStatusCode.OK, sdpResponse.statusCode +" "+ sdpResponse.statusDetail);
        }

        [HttpPost]
        public HttpResponseMessage SendSMS(DocSMS clientSms)
        {
            DocSMSResponse pgresponse = new DocSMSResponse();
            pgresponse.SmsStatus = SmsStatuses.Pending;
            try
            {
                ServiceProvider sp = _resolveMessageService.GetServiceProvider(clientSms.DistributorCostCenterId);
                if (sp == null)
                {
                    pgresponse.SdpResponseStatus = "This service provider is not registered.";
                }
                else
                {
                    ServerRequestBase sdpSms;
                    _resolveMessageService.ProcessClientRequest(clientSms, sp, out sdpSms);
                    SDPSMSResponse sdpResponse = HSenidSend(sdpSms as SDPSMSRequest, clientSms);

                    _docSMSRepository.Save(clientSms);

                    pgresponse.SmsStatus = SmsStatuses.Sent;
                    pgresponse.ClientRequestResponseType = ClientRequestResponseType.SMS;
                    pgresponse.DateCreated = DateTime.Now;
                    pgresponse.DistributorCostCenterId = clientSms.DistributorCostCenterId;
                    pgresponse.TransactionRefId = sdpResponse.requestId;
                    pgresponse.Id = clientSms.Id;
                    pgresponse.SdpVersion = sdpResponse.version;
                    pgresponse.SdpResponseStatus = sdpResponse.statusDetail;
                    pgresponse.SdpResponseCode = sdpResponse.statusCode;
                    pgresponse.SdpDestinationResponses = sdpResponse.destinationResponses;
                    pgresponse.SdpRequestId = sdpResponse.requestId;
                    pgresponse.SmsStatus = SmsStatuses.Sent;

                    _docSMSRepository.SaveResponse(pgresponse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return Request.CreateResponse(HttpStatusCode.OK, pgresponse);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage SmsQuery(string code, string clientKeyWord, string actionKeyWord, string phoneNumber)
        {

            string msg = "";
                
            var sms = new SDPSMSRequest();
            var client = _clientRepository.GetByCode(clientKeyWord);
            if (client == null)
            {
                msg= "This Client is not registered.";
            }
            else
            {
                msg = _smsQueryResolver.SmsQuery(new SmsQuery
                {
                    Code = code,
                    ActionKeyWord = actionKeyWord,
                    ClientKeyWord = clientKeyWord,
                    PhoneNumber = phoneNumber
                });
            }
          
            sms = new SDPSMSRequest
            {
                applicationId = client.ApplicationId,
                password = client.ApplicationPassword,
                destinationAddresses = new List<string>{"tel:" +phoneNumber},
                deliveryStatusRequest = 1,
                encoding = SDPSmsEncoding.Text,
                message = msg,
                sourceAddress = "20358",
                binaryHeader = "",
                chargingAmount = 0,
                version = "1.0",
            };
            SDPSMSResponse sdpResponse = HSenidSendNotification(sms);

            return Request.CreateResponse(HttpStatusCode.OK, "Sent");
        }

        SDPSMSResponse HSenidSendNotification(SDPSMSRequest sdpSms)
        {
            SDPSMSResponse response;
            string jsonRequest = JsonConvert.SerializeObject(sdpSms);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = "";// Request.Url.AbsoluteUri.Substring(0, (Request.Url.AbsoluteUri.Length - 1));
            hsenidUrl = SdpHost.GetSdpsmsUri();
            Uri uri = new Uri(hsenidUrl, UriKind.Absolute);

           
            string jsonResponse = wc.UploadString(uri, "POST", jsonRequest);
            _messageValidation.CanDeserializeMessage(jsonResponse, out response);
           return response;
        }

        SDPSMSResponse HSenidSend(SDPSMSRequest sdpSms, DocSMS clientSms)
        {
            SDPSMSResponse response;
            string jsonRequest = JsonConvert.SerializeObject(sdpSms);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = "";// Request.Url.AbsoluteUri.Substring(0, (Request.Url.AbsoluteUri.Length - 1));
            hsenidUrl = SdpHost.GetSdpsmsUri();
            Uri uri = new Uri(hsenidUrl , UriKind.Absolute);

            _auditLogRepository.AddLog(clientSms.DistributorCostCenterId, "SMS Request", "To HSenid", string.Format("Json SMS Request: {0}", jsonRequest));
            string jsonResponse = wc.UploadString(uri, "POST", jsonRequest);
            _messageValidation.CanDeserializeMessage(jsonResponse, out response);
            _auditLogRepository.AddLog(clientSms.DistributorCostCenterId,"SMS Response","From HSenid", string.Format("Json SMS Response: {0}", jsonResponse));
            
            return response;
        }
    }
}
