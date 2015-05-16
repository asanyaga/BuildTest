using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.HSenid.Domain;

namespace PaymentGateway.WSApi.Lib.Services.Webservice
{
    public interface IResolveRequestService
    {
        void ProcessRequest(PgNotification notification, out RequestMessage sms);
    }
}
