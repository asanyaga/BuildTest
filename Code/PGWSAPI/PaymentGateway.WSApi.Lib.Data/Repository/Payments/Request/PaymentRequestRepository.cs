using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.Payments;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Data.Repository.Payments.Request
{
    public class PaymentRequestRepository : IPaymentRequestRepository
    {
        private PGDataContext _ctx;
        private IAuditLogRepository _auditLogRepository;
        public PaymentRequestRepository(PGDataContext ctx, IAuditLogRepository auditLogRepository)
        {
            _ctx = ctx;
            _auditLogRepository = auditLogRepository;
        }

        public Guid Save(ClientRequestResponseBase entity)
        {
            if (entity == null)
            {
                _auditLogRepository.AddLog(Guid.Empty, "Null Request", "DB", "entity == null");
            }
            tblPaymentRequest toSave = _ctx.tblPaymentRequest.FirstOrDefault(n => n.Id == entity.Id);
            string log = "";
                if (toSave == null)
                {
                    toSave = new tblPaymentRequest();
                    toSave.Id = entity.Id;
                    toSave.DistributorCostCenterId = entity.DistributorCostCenterId;
                    toSave.DateCreated = DateTime.Now;
                    toSave.ClientRequestResponseTypeId = (int)entity.ClientRequestResponseType;
                }

            try
            {
                if (entity is PaymentRequest)
                {
                    PaymentRequest apr = entity as PaymentRequest;

                    toSave.AccountId             = apr.AccountId;
                    toSave.Amount                = apr.Amount;
                    toSave.ApplicationId         = apr.ApplicationId;
                    toSave.SubscriberId          = apr.SubscriberId;
                    toSave.Currency              = apr.Currency;
                    toSave.Extra                 = apr.Extra.ToString();
                    toSave.InvoiceNumber         = apr.InvoiceNumber;
                    toSave.OrderNumber           = apr.OrderNumber;
                    toSave.PaymentInstrumentName = apr.PaymentInstrumentName;
                    toSave.TransactionRefId      = apr.TransactionRefId;
                    log                          =
                        string.Format(
                            "New AsynchronousPaymentRequest Id: {0}; TransactionrefId: {1}; AccountId: {2}; Amount: {3}; ApplicationId {4};"
                            +" SubscriberId: {5}; Currency: {6}; Extra: {7}; InvoiceNumber: {8}; OrderNumber: {9}; PaymentInstrumentName: {10};",
                            toSave.Id, toSave.TransactionRefId, toSave.AccountId, toSave.Amount, toSave.ApplicationId,
                            toSave.SubscriberId, toSave.Currency, toSave.Extra, toSave.InvoiceNumber, toSave.OrderNumber,
                            toSave.PaymentInstrumentName);
                }

                if (entity is PaymentNotificationRequest)
                {
                    PaymentNotificationRequest apnr = entity as PaymentNotificationRequest;

                    toSave.TransactionRefId    = apnr.TransactionRefId;
                    toSave.SDPTransactionRefId = apnr.SDPTransactionRefId;
                    toSave.SDPReferenceId      = apnr.SDPReferenceId ?? "";
                    toSave.TimeStamp           = apnr.SDPTimeStamp;
                    toSave.StatusCode          = apnr.SDPStatusCode ?? "";
                    toSave.StatusDetail        = apnr.SDPStatusDetail ?? "";
                    toSave.Currency            = apnr.SDPCurrency ?? "";
                    toSave.Amount              = apnr.SDPPaidAmount;
                    toSave.BalanceDue          = apnr.SDPBalanceDue;
                    toSave.TotalAmount         = apnr.SDPTotalAmount;

                    log =
                        string.Format(
                            "New AsynchronousPaymentNotificationRequest Id: {0}; TransactionrefId: {1}; SDPTransactionRefId: {2}; SDPReferenceId: {3}; TimeStamp {4};"
                            + " StatusCode: {5}; StatusDetail: {6}; Currency: {7}; Amount: {8}; BalanceDue: {9}; TotalAmount: {10};",
                            toSave.Id, toSave.TransactionRefId, toSave.SDPTransactionRefId, toSave.SDPReferenceId, toSave.TimeStamp,
                            toSave.StatusCode, toSave.StatusDetail, toSave.Currency, toSave.Amount, toSave.BalanceDue,
                            toSave.TotalAmount);
                }

                if (entity is BuyGoodsNotificationRequest)
                {
                    BuyGoodsNotificationRequest bgr = entity as BuyGoodsNotificationRequest;

                    toSave.TransactionRefId         = bgr.TransactionRefId ?? Guid.NewGuid().ToString();
                    toSave.SDPTransactionRefId      = bgr.SDPTransactionRefId;
                    toSave.SubscriberName           = bgr.SubscriberName ?? "";
                    toSave.ReceiptNumber            = bgr.ReceiptNumber ?? "";
                    toSave.Currency                 = bgr.Currency;
                    toSave.Amount                   = bgr.PaidAmount;
                    toSave.MerchantBalance          = bgr.MerchantBalance;
                    toSave.Date                     = bgr.Date;
                    toSave.Time                     = bgr.Time;
                    toSave.StatusCode               = bgr.StatusCode ?? "";
                    toSave.StatusDetail             = bgr.StatusDetail ?? "";
                    log                             =
                        string.Format(
                            "New BuyGoodsNotificationRequest Id: {0}; TransactionRefId: {1}; SDPTransactionRefId: {2}; SubscriberName: {3};"
                            + " ReceiptNumber: {4}; Currency: {5}; Amount: {6}; MerchantBalance: {7}; Date: {8};"
                            + " Time: {9}; StatusCode: {10}; StatusDetail: {11}",
                            toSave.Id, toSave.TransactionRefId, toSave.SDPTransactionRefId, toSave.SubscriberName,
                            toSave.ReceiptNumber, toSave.Currency, toSave.Amount, toSave.MerchantBalance, toSave.Date,
                            toSave.Time, toSave.StatusCode, toSave.StatusDetail);
                }

                _ctx.tblPaymentRequest.Add(toSave);
                _ctx.SaveChanges();

                _auditLogRepository.AddLog(toSave.DistributorCostCenterId, entity.ClientRequestResponseType.ToString()+"Request", "DB", string.Format("Saved {0}", log));
            }
            catch(Exception ex)
            {
                _auditLogRepository.AddLog(toSave.DistributorCostCenterId, entity.ClientRequestResponseType.ToString() + "Request", "DB", string.Format("Error saving {0}\nDetails: {1}", log, ex.Message + ex.InnerException != null ? "\n" + ex.InnerException.Message : ""));
            }
            return toSave.Id;
        }

        public ClientRequestResponseBase GetById(Guid Id)
        {
            tblPaymentRequest tbl = _ctx.tblPaymentRequest.FirstOrDefault(n => n.Id == Id);

            return Map(tbl);
        }

        public IEnumerable<ClientRequestResponseBase> GetByTransactionRefId(string id)
        {
            var items = _ctx.tblPaymentRequest.Where(n => n.TransactionRefId == id);
            return items.Select(Map);
        }

        public IEnumerable<ClientRequestResponseBase> GetByReceiptNumber(string receiptNumber)
        {
            var items = _ctx.tblPaymentRequest.Where(n => n.ReceiptNumber.ToLower() == receiptNumber.ToLower());
            return items.Select(Map);
        }

        public IEnumerable<ClientRequestResponseBase> GetAll()
        {
            var items = _ctx.tblPaymentRequest.ToList();
            return items.Select(Map);
        }

        public IEnumerable<ClientRequestResponseBase> GetAllTimedOutPayments(Guid serviceProviderId, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            IQueryable<tblPaymentRequest> items = null;
            if (!startDate.Equals(new DateTime()) && !endDate.Equals(new DateTime()))
            {
                endDate = (endDate.Date).AddDays(1);
                items = _ctx.tblPaymentRequest
                    .Where(n => n.DistributorCostCenterId == serviceProviderId)
                    .Where(n => n.ClientRequestResponseTypeId == (int)ClientRequestResponseType.AsynchronousPaymentNotification)
                    .Where(n => n.StatusCode.ToLower() == "e1403")
                    .Where(n => n.DateCreated >= startDate.Date && n.DateCreated <= endDate);
            }
            else if (startDate.Equals(new DateTime()) && endDate.Equals(new DateTime()))
                items = _ctx.tblPaymentRequest
                    .Where(n => n.DistributorCostCenterId == serviceProviderId)
                    .Where(n => n.ClientRequestResponseTypeId == (int)ClientRequestResponseType.AsynchronousPaymentNotification)
                    .Where(n => n.StatusCode.ToLower() == "e1403");
            var retItems = items.Select(Map);

            return retItems;
        }

        ClientRequestResponseBase Map(tblPaymentRequest tbl)
        {
            ClientRequestResponseBase crr = null;

            ClientRequestResponseType type = (ClientRequestResponseType) tbl.ClientRequestResponseTypeId;
            switch(type)
            {
                case ClientRequestResponseType.AsynchronousPayment:
                    crr = new PaymentRequest();
                    break;
                case ClientRequestResponseType.AsynchronousPaymentNotification:
                    crr = new PaymentNotificationRequest();
                    break;
                case ClientRequestResponseType.BuyGoodsNotification:
                    crr = new BuyGoodsNotificationRequest();
                    break;
            }

            if (crr != null)
            {
                crr.Id = tbl.Id;
                crr.DistributorCostCenterId = tbl.DistributorCostCenterId;
                crr.ClientRequestResponseType = type;
                crr.DateCreated = tbl.DateCreated;

                if (crr.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPayment)
                {
                    PaymentRequest apr = crr as PaymentRequest;

                    apr.AccountId                  = tbl.AccountId.Trim();
                    //apr.Amount                   = tbl.Amount.Value;
                    apr.ApplicationId              = tbl.ApplicationId.Trim();
                    apr.SubscriberId               = tbl.SubscriberId.Trim();
                    apr.Currency                   = tbl.Currency.Trim();
                    //apr.Extra                      = tbl.Extra.ToDictionary();
                    apr.InvoiceNumber              = tbl.InvoiceNumber.Trim();
                    apr.OrderNumber                = tbl.OrderNumber.Trim();
                    apr.PaymentInstrumentName      = tbl.PaymentInstrumentName.Trim();
                    apr.TransactionRefId           = tbl.TransactionRefId.Trim();
                }
                if (crr.ClientRequestResponseType == ClientRequestResponseType.AsynchronousPaymentNotification)
                {
                    PaymentNotificationRequest apn = crr as PaymentNotificationRequest;

                    apn.TransactionRefId    = tbl.TransactionRefId.Trim();
                    apn.SDPTransactionRefId = tbl.SDPTransactionRefId ?? "";
                    apn.SDPStatusCode       = tbl.StatusCode.Trim();
                    apn.SDPStatusDetail     = tbl.StatusDetail.Trim();
                    if (tbl.Amount         != null) apn.SDPPaidAmount = tbl.Amount.Value;
                    if (tbl.TotalAmount    != null) apn.SDPTotalAmount = tbl.TotalAmount.Value;
                    if (tbl.BalanceDue     != null) apn.SDPBalanceDue = tbl.BalanceDue.Value;
                    if (tbl.TimeStamp      != null) apn.SDPTimeStamp = tbl.TimeStamp.Value;
                    apn.SDPTransactionRefId = tbl.SDPTransactionRefId;
                    apn.SDPCurrency         = tbl.Currency.Trim();
                    apn.SDPReferenceId      = tbl.SDPReferenceId.Trim();
                }
                if (crr.ClientRequestResponseType == ClientRequestResponseType.BuyGoodsNotification)
                {
                    BuyGoodsNotificationRequest bgn = crr as BuyGoodsNotificationRequest;

                    bgn.Currency             = tbl.Currency.Trim();
                    bgn.Date                 = tbl.Date.Value;
                    if (tbl.MerchantBalance != null) bgn.MerchantBalance = tbl.MerchantBalance.Value;
                    if (tbl.Amount          != null) bgn.PaidAmount = tbl.Amount.Value;
                    bgn.ReceiptNumber        = tbl.ReceiptNumber.Trim();
                    bgn.SDPTransactionRefId  = tbl.SDPTransactionRefId.Trim();
                    bgn.StatusCode           = tbl.StatusCode.Trim();
                    bgn.StatusDetail         = tbl.StatusDetail.Trim();
                    bgn.SubscriberName       = tbl.SubscriberName.Trim();
                    bgn.Time                 = tbl.Time.Value;
                    bgn.TransactionRefId     = tbl.TransactionRefId.Trim();
                }

                return crr;
            }

            return null;
        }
    }
}
