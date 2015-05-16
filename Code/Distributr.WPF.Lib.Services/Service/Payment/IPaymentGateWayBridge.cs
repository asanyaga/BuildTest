using System;
using System.Threading.Tasks;
using Distributr.Core.Workflow;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Services.Service.Payment
{
    public interface IPaymentGateWayBridge
    {

        Task<PaymentNotificationResponse> GetNotification(PaymentInfo paymentInfo);
        void ConfirmNotification(Guid notifationId);
    }
}