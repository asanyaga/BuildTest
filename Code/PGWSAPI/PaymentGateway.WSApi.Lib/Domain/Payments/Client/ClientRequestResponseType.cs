using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client
{
    public enum ClientRequestResponseType
    {
        PaymentInstrument = 1,
        AsynchronousPayment = 2,
        AsynchronousPaymentNotification = 3,
        AsynchronousPaymentQuery = 4,
        BuyGoodsNotification = 5,
        ExceptionReport = 6,
        SMS = 7
    }
}
