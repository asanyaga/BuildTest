using System.Collections.Generic;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;

namespace Distributr.WPF.Lib.Services.Repository.Payment
{
    public interface IAsynchronousPaymentRequestRepository : IPaymentRepositoryBase<PaymentRequest>
    {
        List<PaymentRequest> GetByTransactionRefId(string tranRefId);
    }
}
