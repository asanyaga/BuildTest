using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Repository.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;


namespace Distributr.WPF.Lib.Data.Repository.Payment.Request
{
    public class AsynchronousPaymentNotificationRequestRepository : IAsynchronousPaymentNotificationRequestRepository
    {
        private DistributrLocalContext _ctx;
        public AsynchronousPaymentNotificationRequestRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public Guid Save(PaymentNotificationRequest entity)
        {
             PaymentNotificationRequest existingEntity = _ctx.AsynchronousPaymentNotificationRequest.FirstOrDefault
                                                                (n => n.Id == entity.Id);

            if(existingEntity ==null)
            {
                existingEntity = new PaymentNotificationRequest();
                existingEntity.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                existingEntity.DateCreated = DateTime.Now;
                existingEntity.Id = entity.Id;
               _ctx.AsynchronousPaymentNotificationRequest.Add(existingEntity);
            }

            existingEntity.DateCreated = DateTime.Now;
            existingEntity.DistributorCostCenterId = entity.DistributorCostCenterId;
            existingEntity.TransactionRefId = entity.TransactionRefId;
    
            _ctx.AsynchronousPaymentNotificationRequest.Add(existingEntity);
            _ctx.SaveChanges();
            return entity.Id;

        }

        public PaymentNotificationRequest GetById(Guid id)
        {

            return _ctx.AsynchronousPaymentNotificationRequest.FirstOrDefault(n => n.Id == id);
          
        }

        public IEnumerable<PaymentNotificationRequest> GetAll()
        {
            IEnumerable<PaymentNotificationRequest> all = null;
            all = _ctx.AsynchronousPaymentNotificationRequest.ToList();
            return all;
        
        }
    }
}
