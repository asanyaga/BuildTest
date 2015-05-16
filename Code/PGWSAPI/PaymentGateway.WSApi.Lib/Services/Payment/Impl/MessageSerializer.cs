using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Services.Payment.Impl
{
    public class MessageSerializer : IMessageSerializer
    {
        private IMessageValidation _messageValidation;
        private IAuditLogRepository _auditLogRepository;

        public MessageSerializer(IMessageValidation messageValidation, IAuditLogRepository auditLogRepository)
        {
            _messageValidation = messageValidation;
            _auditLogRepository = auditLogRepository;
        }

        public ClientRequestResponseBase DeserializeClientRequest(string requestType, string jsonRequest)
        {
            ClientRequestResponseType clientRequestResponseType = GetMessageType(requestType);
            try
            {
                switch (clientRequestResponseType)
                {
                    case ClientRequestResponseType.AsynchronousPayment:
                        PaymentRequest apr = null;
                        _messageValidation.CanDeserializeMessage(jsonRequest, out apr);
                        apr.TransactionRefId = apr.TransactionRefId.Replace("-", "");
                        return apr;
                    case ClientRequestResponseType.AsynchronousPaymentNotification:
                        PaymentNotificationRequest apn = null;
                        _messageValidation.CanDeserializeMessage(jsonRequest, out apn);
                        apn.TransactionRefId = apn.TransactionRefId.Replace("-", "");
                        return apn;
                    case ClientRequestResponseType.PaymentInstrument:
                        PaymentInstrumentRequest pir = null;
                        _messageValidation.CanDeserializeMessage(jsonRequest, out pir);
                        pir.TransactionRefId = pir.TransactionRefId.Replace("-", "");
                        return pir;
                    case ClientRequestResponseType.BuyGoodsNotification:
                        BuyGoodsNotificationRequest bgnr = null;
                        _messageValidation.CanDeserializeMessage(jsonRequest, out bgnr);
                        bgnr.TransactionRefId = bgnr.TransactionRefId.Replace("-", "");
                        return bgnr;
                    default:
                        {
                            string ex = "Failed to deserialize Message in DeserializeClientRequest.";
                            _auditLogRepository.AddLog(Guid.Empty, requestType, "Client", ex + "\n" + jsonRequest);
                            throw new Exception(ex);
                        }
                }
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(Guid.Empty, requestType, "Client", "Error deserializing jsonRequest " + jsonRequest + "\nException details: \n" +
                    ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : "");
                return null;
            }
        }

        public ServerRequestBase DeserializeSDPRequest (string requestType, string jsonRequest)
        {
            ClientRequestResponseType clientRequestResponseType = GetMessageType(requestType);
            try
            {
                switch (clientRequestResponseType)
                {
                    case ClientRequestResponseType.AsynchronousPaymentNotification:
                        SDPPaymentNotificationRequest sdpapn = null;
                        _messageValidation.CanDeserializeMessage(jsonRequest, out sdpapn);
                        return sdpapn;
                    default:
                        {
                            string ex = "Failed to deserialize Message in DeserializeSDPRequest.";
                            _auditLogRepository.AddLog(Guid.Empty, requestType, "Client", ex + "\n" + jsonRequest);
                            throw new Exception(ex);
                        }
                }
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(Guid.Empty, requestType, "Client", "Error deserializing jsonRequest " + jsonRequest + "\nException details: \n" +
                    ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : "");
                return null;
            }
        }

        ClientRequestResponseType GetMessageType(string messageType)
        {
            ClientRequestResponseType _messageType;
            Enum.TryParse(messageType, out _messageType);
            return _messageType;
        }

        public ClientRequestResponseBase DeserializeSDPResponse(string jsonResponse, ClientRequestResponseType responseType)
        {
            ClientRequestResponseBase crr = null;
            try
            {
                JObject jo = JObject.Parse(jsonResponse);

                string statusCode = (string) jo["statusCode"];
                if (statusCode == "E1340" || statusCode == "E1304")
                    return null;

                switch (responseType)
                {
                    case ClientRequestResponseType.PaymentInstrument:
                        SDPPaymentInstrumentResponse sdppir = null;
                        _messageValidation.CanDeserializeMessage(jsonResponse, out sdppir);
                        PaymentInstrumentResponse pir = new PaymentInstrumentResponse();

                        pir.Id = Guid.NewGuid();
                        pir.DateCreated = DateTime.Now;
                        pir.ClientRequestResponseType = ClientRequestResponseType.PaymentInstrument;
                        pir.PaymentInstrumentList = sdppir.paymentInstrumentList;
                        pir.StatusCode = sdppir.statusCode;
                        pir.StatusDetail = sdppir.statusDetail;

                        crr = pir;
                        break;

                    case ClientRequestResponseType.AsynchronousPayment:
                        SDPPaymentResponse sdppr = null;
                        PaymentResponse apr = new PaymentResponse();
                        _messageValidation.CanDeserializeMessage(jsonResponse, out sdppr);

                        apr.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPayment;
                        apr.Id = Guid.NewGuid();
                        apr.SDPTransactionRefId = sdppr.internalTrxId;
                        if (sdppr.externalTrxId != null)
                            apr.TransactionRefId = ConstructMyGuid(sdppr.externalTrxId).ToString();
                        apr.BusinessNumber = sdppr.businessNumber;
                        apr.DateCreated = sdppr.timeStamp;
                        apr.SDPReferenceId = sdppr.referenceId.ToString();
                        apr.StatusCode = sdppr.statusCode;
                        apr.StatusDetail = sdppr.statusDetail;
                        apr.ShortDescription = sdppr.statusDetail;
                        apr.LongDescription = sdppr.longDescription;
                        apr.TimeStamp = sdppr.timeStamp.ToString() == "1/1/0001 12:00:00 AM" ? DateTime.Now : sdppr.timeStamp;
                        apr.AmountDue = Convert.ToDouble(sdppr.amountDue);
                        crr = apr;
                        break;
                }
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(Guid.Empty, responseType.ToString(), "Client", "Error deserializing jsonResponse " + jsonResponse + " in DeserializeSDPResponse.\nException details: \n" +
                   ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : "");
                return null;
            }

            return crr;
        }

        public string SerializeMessage(ClientRequestResponseBase enitity, ClientRequestResponseType entityType)
        {
            throw new NotImplementedException();
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
