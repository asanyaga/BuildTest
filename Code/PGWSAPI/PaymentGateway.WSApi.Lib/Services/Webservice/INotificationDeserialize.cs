using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.Domain.ResultResponse;

namespace PaymentGateway.WSApi.Lib.Services.Webservice
{
    public interface INotificationDeserialize
    {
        PgNotification DeserializeNotification(string MessageType, string jsonNotification);
        RequestResponse SerializeResponse(string jsonResponse);
    }
}
