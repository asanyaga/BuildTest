using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Repository.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;


namespace Distributr.WPF.Lib.Data.Repository.Payment.Request
{
    public class AsynchronousPaymentRequestRepository : IAsynchronousPaymentRequestRepository
    {
        private DistributrLocalContext _ctx;

        public AsynchronousPaymentRequestRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public Guid Save(PaymentRequest entity)
        {

            PaymentRequest existingEntity = _ctx.AsynchronousPaymentRequest.FirstOrDefault
                                                               (n => n.Id == entity.Id);

            if (existingEntity == null)
            {
                existingEntity = new PaymentRequest();
                existingEntity.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                existingEntity.DateCreated = DateTime.Now;
                existingEntity.Id = entity.Id;
                _ctx.AsynchronousPaymentRequest.Add(existingEntity);
            }

            existingEntity.DistributorCostCenterId = entity.DistributorCostCenterId;
            existingEntity.TransactionRefId = entity.TransactionRefId;
            existingEntity.Currency = entity.Currency;
            existingEntity.AccountId = entity.AccountId;
            existingEntity.AllowOverPayment = entity.AllowOverPayment;
            existingEntity.AllowPartialPayments = entity.AllowPartialPayments;
            existingEntity.Amount = entity.Amount;
            existingEntity.ApplicationId = entity.ApplicationId;

            _ctx.AsynchronousPaymentRequest.Add(existingEntity);
            _ctx.SaveChanges();
            return existingEntity.Id;

        }

        public PaymentRequest GetById(Guid id)
        {
            return _ctx.AsynchronousPaymentRequest.FirstOrDefault(n => n.Id == id);
        }

        public IEnumerable<PaymentRequest> GetAll()
        {
            IEnumerable<PaymentRequest> all = null;
            all = _ctx.AsynchronousPaymentRequest.ToList();
            return all;

        }

        public List<PaymentRequest> GetByTransactionRefId(string tranRefId)
        {
            var all = _ctx.AsynchronousPaymentRequest.Where(n => n.TransactionRefId == tranRefId).ToList();
            return all.ToList();
        }
    }
}
