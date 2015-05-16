using System.Collections.Generic;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Services.Repository.Payment
{
    public interface IAsynchronousPaymentResponseRepository : IPaymentRepositoryBase<PaymentResponse>
    {
        List<PaymentResponse> GetByTransactionRef(string transactionRef);
    }
}
