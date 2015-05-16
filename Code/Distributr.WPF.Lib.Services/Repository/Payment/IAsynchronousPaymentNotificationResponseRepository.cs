using System;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Services.Repository.Payment
{
    public interface IAsynchronousPaymentNotificationResponseRepository : IPaymentRepositoryBase<PaymentNotificationResponse>
    {
      void  ConfirmNotificationItem(Guid notifationItemId);
    }
}
