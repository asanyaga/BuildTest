namespace Distributr.WPF.Lib.Services.Payment.Request
{
    public class PaymentInstrumentRequest : ClientRequestResponseBase
    {
        public string SubscriberId { get; set; }
        public PaymentInstrumentType paymentInstrumentType { get; set; }
    }
}
