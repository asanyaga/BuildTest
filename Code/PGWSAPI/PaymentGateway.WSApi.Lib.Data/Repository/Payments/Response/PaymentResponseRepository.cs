using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Data.Repository.Payments.Response
{
    public class PaymentResponseRepository : IPaymentResponseRepository
    {
        private PGDataContext _ctx;
        private IAuditLogRepository _auditLogRepository;

        public PaymentResponseRepository(PGDataContext ctx, IAuditLogRepository auditLogRepository)
        {
            _ctx = ctx;
            _auditLogRepository = auditLogRepository;
        }

        public Guid Save(ClientRequestResponseBase entity)
        {
            tblPaymentResponse toSave = _ctx.tblPaymentResponse.FirstOrDefault(n => n.Id == entity.Id);
            string log = "";
                if (toSave == null)
                {
                    toSave = new tblPaymentResponse();
                    toSave.Id = entity.Id;
                    toSave.DistributorCostCenterId = entity.DistributorCostCenterId;
                    toSave.DateCreated = entity.DateCreated.ToString() == "1/1/0001 12:00:00 AM" ? DateTime.Now : entity.DateCreated;
                    toSave.ClientRequestResponseTypeId = (int) entity.ClientRequestResponseType;
                }

            try
            {
                if (entity.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPayment)
                {
                    PaymentResponse ap = entity as PaymentResponse;
                    toSave.BusinessNumber = ap.BusinessNumber;
                    toSave.Amount = ap.AmountDue;
                    toSave.TransactionRefId = ap.TransactionRefId;
                    toSave.SDPTransactionRefId = ap.SDPTransactionRefId;
                    toSave.LongDescription = ap.LongDescription;
                    toSave.SDPReferenceId = ap.SDPReferenceId;
                    toSave.ShortDescription = ap.ShortDescription;
                    toSave.StatusCode = ap.StatusCode;
                    toSave.StatusDetail = ap.StatusDetail;
                    toSave.TimeStamp = ap.TimeStamp;
                    toSave.SubscriberId = ap.SubscriberId;
                    toSave.TimeStamp = ap.TimeStamp.ToString() == "1/1/0001 12:00:00 AM" ? DateTime.Now : ap.TimeStamp;

                    log =
                        string.Format(
                            "New AsynchronousPaymentResponse Id: {0}; TransactionrefId: {1}; SDPTransactionRefId: {2}; BusinessNumber: {3}; Amount {4};"
                            + " StatusCode: {5}; StatusDetail: {6}; ShortDescription: {7}; LongDescription: {8}; TimeStamp: {9};",
                            toSave.Id, toSave.TransactionRefId, toSave.SDPTransactionRefId, toSave.BusinessNumber,
                            toSave.Amount,
                            toSave.StatusCode, toSave.StatusDetail, toSave.ShortDescription, toSave.LongDescription,
                            toSave.TimeStamp);
                }

                _ctx.tblPaymentResponse.Add(toSave);
                _ctx.SaveChanges();
                _auditLogRepository.AddLog(toSave.DistributorCostCenterId, entity.ClientRequestResponseType.ToString() + "Response", "DB", string.Format("Saved {0}", log));
            }
            catch (Exception ex)
            {
                _auditLogRepository.AddLog(toSave.DistributorCostCenterId, entity.ClientRequestResponseType.ToString() + "Response", "DB", string.Format("Error saving {0}\nDetails: {1}", log, ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : ""));
            }

            return toSave.Id;
        }

        public ClientRequestResponseBase GetById(Guid id)
        {
            return Map(_ctx.tblPaymentResponse.FirstOrDefault(n => n.Id == id));
        }

        public IEnumerable<ClientRequestResponseBase> GetByTransRefId(Guid id)
        {
            string idstr = id.ToString();
            var retval = _ctx.tblPaymentResponse.Where(n => n.TransactionRefId == idstr).Select(Map);
            return retval; ;
        }

        public IEnumerable<ClientRequestResponseBase> GetAll()
        {
            return _ctx.tblPaymentResponse.Select(Map);
        }

        ClientRequestResponseBase Map(tblPaymentResponse tblApn)
        {
            ClientRequestResponseBase crr = null;
            ClientRequestResponseType type = (ClientRequestResponseType) tblApn.ClientRequestResponseTypeId;
            switch(type)
            {
                case ClientRequestResponseType.AsynchronousPayment:
                    crr = new PaymentResponse();
                    break;
                case ClientRequestResponseType.AsynchronousPaymentNotification:
                    crr = new PaymentNotificationResponse();
                    break;
            }

            if (crr != null)
            {
                crr.Id = tblApn.Id;
                crr.DistributorCostCenterId = tblApn.DistributorCostCenterId;
                crr.ClientRequestResponseType = (ClientRequestResponseType) tblApn.ClientRequestResponseTypeId;
                crr.DateCreated = tblApn.DateCreated.Value;
            }

            //if (crr.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification)
            //{
            //    AsynchronousPaymentNotificationResponse apn = crr as AsynchronousPaymentNotificationResponse;
            //    apn.PaidAmount                                  = tblApn.Amount.Value;
            //    apn.Currency                                = tblApn.Currency.Trim();
            //    apn.TransactionRefId                        = tblApn.TransactionRefId;
            //    apn.SDPTransactionRefId                     = tblApn.SDPTransactionRefId;
            //    apn.SDPReferenceId                          = tblApn.SDPReferenceId;
            //    apn.StatusCode                              = tblApn.StatusCode.Trim();
            //    apn.StatusDetail                            = tblApn.StatusDetail.Trim();
            //    apn.TimeStamp                               = tblApn.TimeStamp.Value;
            //}

            if (crr.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPayment)
            {
                PaymentResponse apr = crr as PaymentResponse;
                apr.BusinessNumber              = tblApn.BusinessNumber;
                apr.AmountDue                   = tblApn.Amount.Value;
                apr.TransactionRefId            = tblApn.TransactionRefId;
                apr.SDPTransactionRefId         = tblApn.SDPTransactionRefId;
                apr.LongDescription             = tblApn.LongDescription.Trim();
                apr.SDPReferenceId              = tblApn.SDPReferenceId;
                apr.ShortDescription            = tblApn.ShortDescription.Trim();
                apr.StatusDetail                = tblApn.StatusDetail.Trim();
                apr.StatusCode                  = tblApn.StatusCode.Trim();
                apr.TimeStamp                   = tblApn.TimeStamp.Value;
                apr.SubscriberId = tblApn.SubscriberId;
            }

            return crr;
        }
    }
}
