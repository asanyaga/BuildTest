namespace Distributr.WPF.Lib.Services.Payment
{
    public enum ClientRequestResponseType
    {
        PaymentInstrument = 1,
        AsynchronousPayment = 2,
        AsynchronousPaymentNotification = 3,
        AsynchronousPaymentQuery = 4,
        BuyGoodsNotification = 5,
        ExceptionReport = 6
    }
}
