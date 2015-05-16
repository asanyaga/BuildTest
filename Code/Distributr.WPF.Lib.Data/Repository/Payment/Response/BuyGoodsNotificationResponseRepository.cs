using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;

using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Services.Repository.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;


namespace Distributr.WPF.Lib.Data.Repository.Payment.Response
{
    public class BuyGoodsNotificationResponseRepository : IBuyGoodsNotificationResponseRepository
    {
        private DistributrLocalContext _ctx;

        public BuyGoodsNotificationResponseRepository(DistributrLocalContext _ctx)
        {
            this._ctx = _ctx;
        }

        public Guid Save(BuyGoodsNotificationResponse entity)
        {
            /*BuyGoodsNotificationResponse existing = GetById(entity.Id);
            if (existing != null)
                _ctx.Delete(existing);
            siaqodb.StoreObject(entity);
            siaqodb.Flush();
            */
            BuyGoodsNotificationResponse exist = GetById(entity.Id);
            if (exist == null)
            {
                exist = new BuyGoodsNotificationResponse();
                _ctx.BuyGoodsNotificationResponse.Add(exist);
            }
            exist.Id = entity.Id;
            exist.DistributorCostCenterId = entity.DistributorCostCenterId;
            exist.TransactionRefId = entity.TransactionRefId;
            exist.ClientRequestResponseType = entity.ClientRequestResponseType;
            exist.DateCreated = entity.DateCreated;
            exist.SDPTransactionRefId = entity.SDPTransactionRefId;
            exist.SubscriberName = entity.SubscriberName;
            exist.ReceiptNumber = entity.ReceiptNumber;
            exist.Currency = entity.Currency;
            exist.PaidAmount = entity.PaidAmount;
            exist.MerchantBalance = entity.MerchantBalance;
            exist.Date = entity.Date;
            exist.Time = entity.Time;
            exist.StatusCode = entity.StatusCode;
            exist.StatusDetail = entity.StatusDetail;
            
            _ctx.SaveChanges();

            return entity.Id;
        }

        public BuyGoodsNotificationResponse GetById(Guid id)
        {
            return _ctx.BuyGoodsNotificationResponse.Cast<BuyGoodsNotificationResponse>().FirstOrDefault(n => n.Id == id);
        }

        public IEnumerable<BuyGoodsNotificationResponse> GetAll()
        {
            IEnumerable<BuyGoodsNotificationResponse> all = _ctx.BuyGoodsNotificationResponse;
           /* all = siaqodb.LoadAll<BuyGoodsNotificationResponse>().ToList();*/
            return all;
        }

        public List<BuyGoodsNotificationResponse> GetByReceiptId(string receiptId)
        {
            var all = _ctx.BuyGoodsNotificationResponse.Where(n => n.ReceiptNumber == receiptId).ToList();
            return all;
        }
    }
}
