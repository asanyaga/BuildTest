
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.ViewModels.Utils.Payment
{
    public interface IPaymentService
    {
        string GetPGWSUrl(ClientRequestResponseType clientRequesType);
        PaymentInstrument GetPaymentInstrumentList();
        AsynchronousPaymentResponse SendPaymentRequest();
        AsynchronousPaymentNotificationResponse SendPaymentNoficationRequest();
    }
}
