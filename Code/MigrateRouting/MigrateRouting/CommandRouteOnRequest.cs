using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MigrateRouting
{
    public class CommandRouteOnRequest
    {
        public long Id { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateCommandInserted { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        //TODO Convert to CommandType
        public string CommandType { get; set; }
        public string JsonCommand { get; set; }

        //[BsonIgnore][Obsolete]
        //public List<CommandRouteCentre> CommandRouteCentre { get; set; }

        public Guid DocumentParentId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateAdded { get; set; }

        public bool IsRetired { get; set; }
    }

    public class CommandRouteOnRequestDTO
    {
        public CommandRouteOnRequestDTO()
        {
            CommandRouteCentres = new List<CommandRouteOnRequestCostcentre>();
            RouteOnRequest = new CommandRouteOnRequest();
        }
        public CommandRouteOnRequest RouteOnRequest { get; set; }
        public List<CommandRouteOnRequestCostcentre> CommandRouteCentres { get; set; }
    }
    public class CommandRouteOnRequestCostcentre
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid CostCentreId { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRetired { get; set; }
        public string CommandType { get; set; }
    }
    public class CommandRoutingStatus
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid CommandId { get; set; }
        public Guid DestinationCostCentreApplicationId { get; set; }
        public bool Delivered { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateDelivered { get; set; }

        public bool Executed { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateExecuted { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateAdded { get; set; }


    }
    public class IdCounterHelper : MongoBase
    {
        private string idCounterCollectionName = "idcounter";
        private MongoCollection<BsonDocument> _counterCollection;
        public IdCounterHelper(string connectionString)
            : base(connectionString)
        {
            _counterCollection = CurrentMongoDB[idCounterCollectionName];
        }

        public int GetNextId(string collectionName)
        {
            if (_counterCollection.FindOne(Query.EQ("_id", collectionName)) == null)
                _counterCollection.Insert(new BsonDocument { { "_id", collectionName }, { "c", 0 } });
            var query = Query.EQ("_id", collectionName);
            var sortBy = SortBy.Descending("_id");
            var update = Update.Inc("c", 1);
            var result = _counterCollection.FindAndModify(query, sortBy, update, true);
            return result.ModifiedDocument["c"].AsInt32;
        }

    }
    public enum CommandType : int
    {
        //Order
        CreateOrder = 100,
        ApproveOrder = 101,
        AddOrderLineItem = 102,
        ConfirmOrder = 103,
        RejectOrder = 104,
        ChangeOrderLineItem = 105,
        RemoveOrderLineItem = 106,
        CloseOrder = 107,
        BackOrder = 108,
        OrderPendingDispatch = 109,
        DispatchToPhone = 110,


        //IAN
        CreateInventoryAdjustmentNote = 115,
        AddInventoryAdjustmentNoteLineItem = 116,
        ConfirmInventoryAdjustmentNote = 117,

        //DN
        CreateDispatchNote = 120,
        AddDispatchNoteLineItem = 121,
        ConfirmDispatchNote = 122,

        //IRN
        CreateInventoryReceivedNote = 130,
        AddInventoryReceivedNoteLineItem = 131,
        ConfirmInventoryReceivedNote = 132,

        //ITN
        CreateInventoryTransferNote = 140,
        AddInventoryTransferNoteLineItem = 141,
        ConfirmInventoryTransferNote = 142,

        //Invoice
        CreateInvoice = 150,
        AddInvoiceLineItem = 151,
        ConfirmInvoice = 152,
        CloseInvoice = 153,

        //Receipt
        CreateReceipt = 160,
        AddReceiptLineItem = 161,
        ConfirmReceiptLineItem = 178,
        ConfirmReceipt = 162,

        //DisbursementNote
        CreateDisbursementNote = 163,
        AddDisbursementNoteLineItem = 164,
        ConfirmDisbursementNote = 165,

        //Credit Note
        CreateCreditNote = 166,
        AddCreditNoteLineItem = 167,
        ConfirmCreditNote = 168,

        //Returns Note
        CreateReturnsNote = 169,
        AddReturnsNoteLineItem = 170,
        ConfirmReturnsNote = 171,
        CloseReturnsNote = 179,
        //PaymentNote
        CreatePaymentNote = 172,
        AddPaymentNoteLineItem = 173,
        ConfirmPaymentNote = 174,

        //Discount
        CreateDiscount = 175,
        AddDiscountLineItem = 176,
        ConfirmDiscount = 177,
        RetireDocument = 180,

        CreateInventorySerials = 181
    }
}
