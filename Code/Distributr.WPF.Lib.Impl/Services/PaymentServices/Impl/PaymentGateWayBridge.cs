using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Impl.Services.PaymentServices.Impl
{
  public  class PaymentGateWayBridge : IPaymentGateWayBridge
  {
      private IPaymentGatewayProxy _paymentGatewayProxy;
      private IConfigService _configService;
      private IAsynchronousPaymentNotificationResponseRepository _paymentNotification;

      public PaymentGateWayBridge(IPaymentGatewayProxy paymentGatewayProxy, IConfigService configService, IAsynchronousPaymentNotificationResponseRepository paymentNotification)
      {
          _paymentGatewayProxy = paymentGatewayProxy;
          _configService = configService;
          _paymentNotification = paymentNotification;
      }

      public async Task<PaymentNotificationResponse> GetNotification(PaymentInfo paymentInfo)
        {
            var pnr = new PaymentNotificationRequest()
            {
                Id = Guid.NewGuid(),
                DistributorCostCenterId =_configService.Load().CostCentreId,
                ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification,
                DateCreated = DateTime.Now,
                TransactionRefId = paymentInfo.Id.ToString(),
            };

          var data = await _paymentGatewayProxy.GetPaymentNotificationAsync(pnr);
          PaymentNotificationResponse response = null; 
          if (data != null && data.StatusCode == "S1000")
          {
             _paymentNotification.Save(data);
             response = _paymentNotification.GetById(data.Id);
          }
          return response;
        }

      public void ConfirmNotification(Guid notifationId)
      {
          _paymentNotification.ConfirmNotificationItem(notifationId);
      }
  }
}
