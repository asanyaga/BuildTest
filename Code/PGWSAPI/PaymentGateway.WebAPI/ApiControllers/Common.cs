using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;

namespace PaymentGateway.WebAPI.ApiControllers
{
    public static class Common
    {
        public static ClientRequestResponseBase ThrowError(string error, ClientRequestResponseBase entity)
        {
            ClientRequestResponseBase response = new ClientRequestResponseBase();
            if (entity is PaymentNotificationRequest)
            {
                PaymentNotificationResponse apn = new PaymentNotificationResponse();
                apn.DistributorCostCenterId = entity.DistributorCostCenterId;
                apn.StatusCode = "Error";
                apn.StatusDetail = error;
                response = apn;
            }
            else if (entity is PaymentInstrumentRequest)
            {
                PaymentInstrumentResponse pi = new PaymentInstrumentResponse();
                pi.StatusCode = "Error";
                pi.StatusDetail = error;
                response = pi;
            }
            else if (entity is PaymentRequest)
            {
                PaymentResponse apr = new PaymentResponse();
                apr.StatusCode = "Error";
                apr.StatusDetail = error;
                response = apr;
            }
            else if(entity is DocSMSResponse)
            {
                DocSMSResponse sms = new DocSMSResponse();
                sms.SdpResponseCode = "Error";
                sms.SdpResponseStatus = error;
                response = sms;
            }

            return response;
        }
    }
}