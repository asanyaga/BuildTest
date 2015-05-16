using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;
using PaymentGateway.WSApi.Lib.Domain.SMS.SDP;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Services.Payment.Impl
{
    public class ResolveMessageService : IResolveMessageService
    {
        private IPaymentRequestRepository _paymentRequestRepository;
        private IPaymentResponseRepository _paymentResponseRepository;
        private IAuditLogRepository _auditLogRepository;
        private IServiceProviderRepository _serviceProviderRepository;

        public ResolveMessageService(IPaymentRequestRepository paymentRequestRepository, IAuditLogRepository auditLogRepository, IServiceProviderRepository serviceProviderRepository, IPaymentResponseRepository paymentResponseRepository)
        {
            _paymentRequestRepository = paymentRequestRepository;
            _paymentResponseRepository = paymentResponseRepository;
            _auditLogRepository = auditLogRepository;
            _serviceProviderRepository = serviceProviderRepository;
        }

        public ServiceProvider GetServiceProvider(Guid distributorCCId)
        {
            ServiceProvider sp = _serviceProviderRepository.GetByServiceProviderId(distributorCCId.ToString());
                

            return sp;
        }

        public void ProcessClientRequest(ClientRequestResponseBase crrMessage, ServiceProvider serviceProvider, out ServerRequestBase serverReqBase)
        {
            if (crrMessage == null)
            {
                throw new Exception("Message null");
            }

            serverReqBase = new ServerRequestBase();
            string userName = "";
            string password = "f2b9e361c13bc54b86d3c8180b0fd242";
            string subscriberId = "tel:254701234563";
            string applicationId = "APP_000007";
            bool allowOverPayment = true, allowPartialPayment = true;
            string version = "1.0";
            string sourcesAddress = "hewani";
            string binaryHeader = "Content-Type:application/json";

            if (serviceProvider != null)
            {
                applicationId = serviceProvider.SdpAppId;
                subscriberId = serviceProvider.SubscriberId;
                password = serviceProvider.SdpPassword;
                allowOverPayment = serviceProvider.AllowOverPayment;
                allowPartialPayment = serviceProvider.AllowPartialPayment;
            }
            else
            {
                throw new Exception("This service provider is not registered.");
            }
            //
            if (crrMessage is PaymentInstrumentRequest)
            {
                PaymentInstrumentRequest pir = crrMessage as PaymentInstrumentRequest;
                SDPPaymentInstrumentRequest paymentIstReq = new SDPPaymentInstrumentRequest
                                                                {
                                                                    applicationId     = applicationId,
                                                                    password          = password,
                                                                    type              = pir.paymentInstrumentType,
                                                                    //subscriberId    = subscriberId
                                                                    subscriberId      = pir.SubscriberId
                                                                };

                serverReqBase = paymentIstReq;
            }
            if (crrMessage is PaymentRequest)
            {
                PaymentRequest apr         = crrMessage as PaymentRequest;
                SDPPaymentRequest sdpapr   = new SDPPaymentRequest();
                sdpapr.accountId                       = apr.AccountId.ToString();
                sdpapr.allowOverPayments = allowOverPayment ? AllOverPayment.Allow.ToString() : AllOverPayment.Disallow.ToString();
                sdpapr.allowPartialPayments = allowPartialPayment ? AllowPartialPayments.Allow.ToString() : AllowPartialPayments.Disallow.ToString();
                sdpapr.amount                          = apr.Amount.ToString();
                sdpapr.applicationId                   = applicationId;
                sdpapr.currency                        = apr.Currency;
                sdpapr.externalTrxId                   = apr.TransactionRefId.Replace("-","");
                sdpapr.extra = apr.Extra; //new Dictionary<string, string>();// {new string("tilNo","66363" )};
               // sdpapr.extra.Add("tillNo","66361"); 
                sdpapr.invoiceNo                       = apr.InvoiceNumber;
                sdpapr.orderNo                         = apr.OrderNumber;
                sdpapr.password                        = password;
                sdpapr.paymentInstrumentName           = apr.PaymentInstrumentName;
                //sdpapr.subscriberId                  = subscriberId;
                sdpapr.subscriberId                    = apr.SubscriberId;
                sdpapr.smsDescription                  = apr.smsDescription;
                
                serverReqBase = sdpapr;
            }
            if (crrMessage is PaymentQueryRequest)
            {
                PaymentQueryRequest apq = crrMessage as PaymentQueryRequest;
                SDPPaymentQueryRequest sdpapq = new SDPPaymentQueryRequest();

                sdpapq.applicationId = applicationId;
                sdpapq.internalTrxId = apq.TransactionRefId.ToString();
                sdpapq.password      = password;

                serverReqBase = sdpapq;
            }
            if(crrMessage is DocSMS)
            {
                DocSMS sms = crrMessage as DocSMS;
                SDPSMSRequest reqSMS = new SDPSMSRequest
                                        {
                                            applicationId = applicationId,
                                            password = password,
                                            destinationAddresses = sms.Recipitents.Select(n => "tel:" + n).ToList(),
                                            deliveryStatusRequest = 1,
                                            encoding = SDPSmsEncoding.Text,
                                            message = sms.SmsBody,
                                        };
            }
        }

        public void ProcessSDPRequest(ServerRequestBase sdpRequest, out ClientRequestResponseBase crrRequest, out string subcriberid, out Guid sip)
        {
            subcriberid = "";
            sip = Guid.Empty;
            ClientRequestResponseBase paymentRequest = null;
            Guid serviceProviderId = Guid.Empty;
            crrRequest = new ClientRequestResponseBase();
            if (sdpRequest is SDPPaymentNotificationRequest)
            {
                paymentRequest = _paymentResponseRepository.GetByTransRefId(
                    new Guid(((SDPPaymentNotificationRequest)sdpRequest)
                        .externalTrxId))
                        .OfType<PaymentResponse>().FirstOrDefault();

                if (paymentRequest != null)
                {
                    serviceProviderId = paymentRequest.DistributorCostCenterId;
                   
                }
                if (paymentRequest  is PaymentResponse)
                {
                    var respo = ((PaymentResponse) paymentRequest);
                    subcriberid = respo.SubscriberId;
                    sip = respo.DistributorCostCenterId;
                }

                SDPPaymentNotificationRequest sdpapn =
                    sdpRequest as SDPPaymentNotificationRequest;

                PaymentNotificationRequest apn = new PaymentNotificationRequest();

                apn.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                apn.DateCreated               = sdpapn.timestamp;
                apn.Id                        = Guid.NewGuid();
                apn.TransactionRefId          =ConstructMyGuid(sdpapn.externalTrxId).ToString();
                apn.SDPStatusCode             = sdpapn.statusCode;
                apn.SDPStatusDetail           = sdpapn.statusDetail;
                apn.SDPTimeStamp              = sdpapn.timestamp;
                apn.SDPTransactionRefId       = sdpapn.internalTrxId;
                apn.SDPPaidAmount             = Convert.ToDouble(sdpapn.paidAmount);
                apn.SDPTotalAmount            = Convert.ToDouble(sdpapn.totalAmount);
                apn.SDPBalanceDue             = Convert.ToDouble(sdpapn.balanceDue);
                apn.SDPCurrency               = sdpapn.currency;
                apn.SDPReferenceId            = sdpapn.referenceId;
                apn.DistributorCostCenterId   = serviceProviderId;
               
                crrRequest = apn;
            }
            else if (sdpRequest is SDPBuyGoodsNotificationRequest)
            {
                throw new Exception("Buy goods not in use");
                SDPBuyGoodsNotificationRequest sdpBg = sdpRequest as SDPBuyGoodsNotificationRequest;
                BuyGoodsNotificationRequest bgNotif = new BuyGoodsNotificationRequest();

                bgNotif.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                bgNotif.Currency = sdpBg.currency;
                bgNotif.Date = sdpBg.date;
                bgNotif.DateCreated = DateTime.Now;
                bgNotif.Id = Guid.NewGuid();
                bgNotif.MerchantBalance = Convert.ToDouble(sdpBg.merchantBalance);
                bgNotif.PaidAmount = Convert.ToDouble(sdpBg.paidAmount);
                bgNotif.ReceiptNumber = sdpBg.receiptNumber;
                bgNotif.SDPTransactionRefId = sdpBg.internalTrxId;
                bgNotif.StatusCode = sdpBg.statusCode;
                bgNotif.StatusDetail = sdpBg.statusDetail;
                bgNotif.SubscriberName = sdpBg.subscriberName;
                bgNotif.Time = sdpBg.time;

                crrRequest = bgNotif;
            }
        }

        public ClientRequestResponseBase ProcessClientPaymentNotificationRequest(ClientRequestResponseBase crrReqMsg)
        {
            ClientRequestResponseBase response = new ClientRequestResponseBase();

            try
            {
                IEnumerable<ClientRequestResponseBase> sdpRequests = new List<ClientRequestResponseBase>();
                if (crrReqMsg is PaymentNotificationRequest)
                {
                    sdpRequests = _paymentRequestRepository.GetByTransactionRefId(crrReqMsg.TransactionRefId).OfType
                        <PaymentNotificationRequest>().ToList();//.FirstOrDefault();

                    if (sdpRequests.Count() > 0)
                    {
                        var any = sdpRequests.OrderBy(n => ((PaymentNotificationRequest)n).SDPTimeStamp).FirstOrDefault(s=>!string.IsNullOrEmpty(s.TransactionRefId)) as PaymentNotificationRequest;

                        var paymentResp = _paymentRequestRepository.GetByTransactionRefId(crrReqMsg.TransactionRefId).OfType
                                <PaymentRequest>().FirstOrDefault();

                        var paymentInfoDetails = new List<PaymentNotificationListItem>();
                        foreach (var re in sdpRequests)
                        {
                            var apnr = re as PaymentNotificationRequest;
                           
                            var detail = new PaymentNotificationListItem
                                            {
                                                Id          = apnr.Id,
                                                PaidAmount  = apnr.SDPPaidAmount,
                                                TotalAmount = apnr.SDPTotalAmount,
                                                BalanceDue  = apnr.SDPBalanceDue,
                                                TimeStamp   = apnr.SDPTimeStamp,
                                                Status      = apnr.SDPStatusCode +"; "+apnr.SDPStatusDetail,
                                                ResponseId = apnr.Id
                                                
                                            };
                            paymentInfoDetails.Add(detail);
                        }
                        response = new PaymentNotificationResponse
                                                        {
                                                            Id                        = any.Id,
                                                            TransactionRefId = any.TransactionRefId,
                                                            DistributorCostCenterId   = any.DistributorCostCenterId,
                                                            ClientRequestResponseType = any.ClientRequestResponseType,

                                                            SDPReferenceId             = any.SDPReferenceId,
                                                            SDPTransactionRefId        = any.SDPTransactionRefId,
                                                            Currency                   = any.SDPCurrency,
                                                            StatusCode                 = any.SDPStatusCode,
                                                            StatusDetail               = any.SDPStatusDetail,
                                                            DateCreated                = any.DateCreated,
                                                            PaymentNotificationDetails = paymentInfoDetails,
                                                            AccountId                  = paymentResp.AccountId,
                                                            SubscriberId               = paymentResp.SubscriberId,
                                                            Items = paymentInfoDetails,
                                                        };

                        return response;
                    }
                    goto pending;
                }

                #region BuyGoodsNotificationRequest
                if (crrReqMsg is BuyGoodsNotificationRequest)
                {
                    var sdpBgRequests = _paymentRequestRepository.GetByReceiptNumber(crrReqMsg.TransactionRefId).OfType
                        <BuyGoodsNotificationRequest>().LastOrDefault();
                    if (sdpBgRequests == null)
                        goto pending;
                    BuyGoodsNotificationRequest bgnr = sdpBgRequests as BuyGoodsNotificationRequest;
                    response = new BuyGoodsNotificationResponse
                                   {
                                       Id = bgnr.Id,
                                       ClientRequestResponseType = ClientRequestResponseType.BuyGoodsNotification,
                                       Currency = bgnr.Currency,
                                       Date = bgnr.Date,
                                       DateCreated = bgnr.DateCreated,
                                       DistributorCostCenterId = crrReqMsg.DistributorCostCenterId,
                                       MerchantBalance = bgnr.MerchantBalance,
                                       PaidAmount = bgnr.PaidAmount,
                                       ReceiptNumber = bgnr.ReceiptNumber,
                                       SDPTransactionRefId = bgnr.SDPTransactionRefId,
                                       StatusCode = bgnr.StatusCode,
                                       StatusDetail = bgnr.StatusDetail,
                                       SubscriberName = bgnr.SubscriberName ?? "",
                                       Time = bgnr.Time,
                                       TransactionRefId = bgnr.TransactionRefId
                                   };
                    return response;
                }
                #endregion
            }
            catch (Exception ex)
            {
                string msg = string.Format("Notification; Id: {0}; TransactionRefId: " + crrReqMsg.Id, crrReqMsg.TransactionRefId);

                _auditLogRepository.AddLog(crrReqMsg.DistributorCostCenterId,
                                           crrReqMsg.ClientRequestResponseType.ToString(),
                                           "To Mobile",
                                           "Error in ProcessClientPaymentNotificationRequest \n"
                                           + msg + "\nException details: \n"
                                           + ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : "");
                goto pending;
            }

        pending:
            response = new PaymentNotificationResponse
            {
                Id = Guid.NewGuid(),
                ClientRequestResponseType = crrReqMsg.ClientRequestResponseType,
                TransactionRefId = crrReqMsg.TransactionRefId,
                StatusDetail = "Pending",
            };

            return response;
        }

        public List<PaymentNotification> ProcessPaymentReportRequest(Guid serviceProviderId, DateTime startDate, DateTime endDate)
        {
            var items = _paymentRequestRepository.GetAllTimedOutPayments(serviceProviderId, startDate, endDate);
            var notifications = new List<PaymentNotification>();

            foreach (var item in items)
            {
                var pn = item as PaymentNotificationRequest;
                var notification = new PaymentNotification 
                {
                    Id = pn.Id,
                    TransactionRefId = pn.TransactionRefId,
                    DistributorCostCenterId = pn.DistributorCostCenterId,
                    ClientRequestResponseType = pn.ClientRequestResponseType,
                    SDPReferenceId = pn.SDPReferenceId,
                    SDPTransactionRefId = pn.SDPTransactionRefId,
                    Currency = pn.SDPCurrency,
                    StatusCode = pn.SDPStatusCode,
                    StatusDetail = pn.SDPStatusDetail,
                    DateCreated = pn.DateCreated,
                    PaidAmount = pn.SDPPaidAmount, 
                    TimeStamp = pn.SDPTimeStamp,
                    BalanceDue = pn.SDPBalanceDue,
                    TotalAmount = pn.SDPTotalAmount
                };
                notifications.Add(notification);
            }

            return notifications;
        }

         Guid ConstructMyGuid(string externalIdFromHSenid)
        {
            externalIdFromHSenid = externalIdFromHSenid.Insert(8, "-");
            externalIdFromHSenid = externalIdFromHSenid.Insert(13, "-");
            externalIdFromHSenid = externalIdFromHSenid.Insert(18, "-");
            externalIdFromHSenid = externalIdFromHSenid.Insert(23, "-");

            return new Guid(externalIdFromHSenid);
        }
    }
}
