using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Repository.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.Data.Repository.Payment.Response
{
    public class AsynchronousPaymentNotificationResponseRepository : IAsynchronousPaymentNotificationResponseRepository
    {
        private DistributrLocalContext _ctx;

        public AsynchronousPaymentNotificationResponseRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public Guid Save(PaymentNotificationResponse entity)
        {

            PaymentNotificationResponse existingEntity = _ctx.AsynchronousPaymentNotificationResponse.FirstOrDefault
                                                               (n => n.Id == entity.Id);

            if (existingEntity == null)
            {
                existingEntity = new PaymentNotificationResponse();
                existingEntity.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                existingEntity.DateCreated = DateTime.Now;
                existingEntity.TimeStamp = DateTime.Now;
                existingEntity.Id = entity.Id;
                _ctx.AsynchronousPaymentNotificationResponse.Add(existingEntity);
            }
           // existingEntity.PaymentNotificationDetails
            foreach (var item in entity.PaymentNotificationDetails)
            {
                var i =_ctx.PaymentNotificationListItems.FirstOrDefault(s => s.Id == item.Id);
                if (i == null)
                {
                    i= new PaymentNotificationListItem();
                    i.Id = item.Id;
                    _ctx.PaymentNotificationListItems.Add(i);
                }
                i.BalanceDue = item.BalanceDue;
                i.PaidAmount = item.PaidAmount;
                i.Status = item.Status;
                i.TimeStamp = item.TimeStamp;
                i.TotalAmount = item.TotalAmount;
                i.ResponseId = existingEntity.Id;
            }
            existingEntity.DistributorCostCenterId = entity.DistributorCostCenterId;
            existingEntity.TransactionRefId = entity.TransactionRefId;
            existingEntity.BalanceDue = entity.BalanceDue;
            existingEntity.Currency = entity.Currency;
            existingEntity.PaidAmount = entity.PaidAmount;
            existingEntity.SDPReferenceId = entity.SDPReferenceId;
          
            //_ctx.AsynchronousPaymentNotificationResponse.Add(existingEntity);
            _ctx.SaveChanges();
            return existingEntity.Id;

        }

        public PaymentNotificationResponse GetById(Guid id)
        {
            return _ctx.AsynchronousPaymentNotificationResponse.FirstOrDefault(n => n.Id == id);
        }

        public IEnumerable<PaymentNotificationResponse> GetAll()
        {
            IEnumerable<PaymentNotificationResponse> all = null;
            all = _ctx.AsynchronousPaymentNotificationResponse.ToList();
            return all;
        }

        public void ConfirmNotificationItem(Guid notifationItemId)
        {
            var i = _ctx.PaymentNotificationListItems.FirstOrDefault(s => s.Id == notifationItemId);
            if (i != null)
            {
                i.IsUsed = true;
                _ctx.SaveChanges();
            }
        }
    }
}
