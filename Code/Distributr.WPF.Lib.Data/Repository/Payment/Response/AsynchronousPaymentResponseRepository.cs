using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Repository.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;


namespace Distributr.WPF.Lib.Data.Repository.Payment.Response
{
    public class AsynchronousPaymentResponseRepository : IAsynchronousPaymentResponseRepository
    {
        private DistributrLocalContext _ctx;

        public AsynchronousPaymentResponseRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public Guid Save(PaymentResponse entity)
        {

            PaymentResponse existingEntity = _ctx.AsynchronousPaymentResponse.FirstOrDefault
                                                               (n => n.Id == entity.Id);

            if (existingEntity == null)
            {
                existingEntity = new PaymentResponse();
                existingEntity.ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification;
                existingEntity.DateCreated = DateTime.Now;
                existingEntity.TimeStamp = DateTime.Now;
                existingEntity.Id = entity.Id;
                _ctx.AsynchronousPaymentResponse.Add(existingEntity);
            }

            existingEntity.DistributorCostCenterId = entity.DistributorCostCenterId;
            existingEntity.TransactionRefId = entity.TransactionRefId;
            existingEntity.AmountDue = entity.AmountDue;
            existingEntity.BusinessNumber = entity.BusinessNumber;
            existingEntity.LongDescription = entity.LongDescription;
            existingEntity.SDPReferenceId = entity.SDPReferenceId;
            existingEntity.StatusDetail = entity.StatusDetail;
            existingEntity.StatusCode = entity.StatusCode;
            existingEntity.ShortDescription = entity.ShortDescription;
            existingEntity.SDPTransactionRefId = entity.SDPTransactionRefId;
            

          //  _ctx.AsynchronousPaymentResponse.Add(existingEntity);
            _ctx.SaveChanges();
            return existingEntity.Id;
         
        }

        public PaymentResponse GetById(Guid id)
        {
            return _ctx.AsynchronousPaymentResponse.FirstOrDefault(n => n.Id == id);
        }

        public IEnumerable<PaymentResponse> GetAll()
        {
            IEnumerable<PaymentResponse> all = null;
            all = _ctx.AsynchronousPaymentResponse.ToList();
            return all;
        }

        public List<PaymentResponse> GetByTransactionRef(string transactionRef)
        {
            var all =_ctx.AsynchronousPaymentResponse.Where(n => n.TransactionRefId == transactionRef).ToList();
            return all;
        }
    }
}
