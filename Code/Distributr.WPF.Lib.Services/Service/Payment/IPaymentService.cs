using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Services.Service.Payment
{
    public interface IPaymentService
    {
        string GetPGWSUrl(ClientRequestResponseType clientRequesType);
        string GetPGWebApiUrl();
    }
}
