using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.Domain.ResultResponse;

namespace PaymentGateway.WSApi.Lib.Services.Webservice.Impl
{
    public class NotificationDeserialize : INotificationDeserialize
    {
        private INotificationValidation _notificationValidation;
        public NotificationDeserialize(INotificationValidation notificationValidation)
        {
            _notificationValidation = notificationValidation;
        }
        PgNotification DeserializeCommand(PgMessageType ct, string jsonNotification)
        {
            switch (ct)
            {
                    
                case PgMessageType.SMSCN:
                    SmscNotification note = null;
                    _notificationValidation.CanDeserializeNotification(jsonNotification, out note);
                    return note;
                case PgMessageType.SMSCP:
                    SmscPaymentConfirmation smscp = null;
                    _notificationValidation.CanDeserializeNotification(jsonNotification, out smscp);
                    return smscp;
                case PgMessageType.Eazy247N:
                    Eazy247Notification easyn = null;
                    _notificationValidation.CanDeserializeNotification(jsonNotification, out easyn);
                    return easyn;
                case PgMessageType.Eazy247P:
                    Easy247Payment easyp = null;
                    _notificationValidation.CanDeserializeNotification(jsonNotification, out easyp);
                    return easyp;
                default:
                    throw new Exception("Failed to deserialize Notifiaction in Notification deserializer");
            }
            return null;
        }
        PgMessageType GetPgMessageType(string messageType)
        {
            PgMessageType _messageType;
            Enum.TryParse(messageType, out _messageType);
            return _messageType;
        }
        public PgNotification DeserializeNotification(string messageType, string jsonNotification)
        {
            PgMessageType ct = GetPgMessageType(messageType);
            if (ct == 0)
            {
                return null;
            }
            return DeserializeCommand(ct, jsonNotification);
        }

        public RequestResponse SerializeResponse(string jsonResponse)
        {
            JObject jo = JObject.Parse(jsonResponse);
            RequestResponse response = new RequestResponse();
            response.messageId = (string)jo["messageId"];
            JArray destinationResponses = null;
            try
            {
                destinationResponses = (JArray)jo["destinationResponses"];
            }catch
            {
                destinationResponses = null;
            }
            
            response.statusCode = (string) jo["statusCode"];
            response.statusDetail = (string)jo["statusDescription"];
            response.version = (string)jo["version"];
            if (destinationResponses != null)
            {
                foreach (JObject j in jo["destinationResponses"])
                {
                    DestinationResponse dresponse = new DestinationResponse();
                    dresponse.messageId = (string) j["messageId"];
                    dresponse.address = (string)j["address"];
                    dresponse.statusCode = (string) j["statusCode"];
                    dresponse.statusDetail = (string)j["statusDescription"];
                    dresponse.timeStamp = (string)j["timeStamp"];
                    response.DesinationResponses.Add(dresponse);
                }
            }

            return response;

        }
    }
}
