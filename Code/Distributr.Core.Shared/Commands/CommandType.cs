using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands
{
    public enum CommandType : int
    {
        //Order
        //[Obsolete("Do not use")]
        //CreateOrder = 100,
        //[Obsolete("Do not use")]
        //ApproveOrder = 101,
        //[Obsolete("Do not use")]
        //AddOrderLineItem = 102,
        //[Obsolete("Do not use")]
        //ConfirmOrder = 103,
        //[Obsolete("Do not use")]
        //RejectOrder = 104,
        //[Obsolete("Do not use")]
        //ChangeOrderLineItem = 105,
        //[Obsolete("Do not use")]
        //RemoveOrderLineItem = 106,
        
        CloseOrder = 107,
        //[Obsolete("Do not use")]
        //BackOrder = 108,
        //[Obsolete("Do not use")]
        //OrderPendingDispatch = 109,
        [Obsolete("Do not use")]
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
        CreatePaymentNote=172,
        AddPaymentNoteLineItem=173,
        ConfirmPaymentNote= 174,

         //Discount
        CreateDiscount=175,
        AddDiscountLineItem=176,
        ConfirmDiscount=177,
        RetireDocument=180,

        CreateInventorySerials = 181,

        CreateCommodityPurchase=182,
        AddCommodityPurchaseLineItem=183,
        ConfirmCommodityPurchase=184,
        CreateCommodityReception=185,
        AddCommodityReceptionLineItem = 186,
        ConfirmCommodityReception = 187,
        CreateCommodityStorage=188,
        AddCommodityStorageLineItem = 189,
        ConfirmCommodityStorage = 190,
        CreateCommodityDelivery=191,
        AddCommodityDeliveryLineItem=192,
        ConfirmCommodityDelivery=193,
        AddReceivedDeliveryLineItem = 194,
        ConfirmReceivedDelivery = 195,
        CreateReceivedDelivery=196,
        WeighedCommodityDeliveryLineItem=197,
        ApproveDelivery = 198,
        StoredCommodityReceptionLineItem=199,
        StoredReceivedDeliveryLineItem=200,

        //CommodityTransfer
        CreateCommodityTransfer = 201,
        AddCommodityTransferLineItem = 202,
        ConfirmCommodityTransfer = 203,
        ApproveCommodityTransfer = 204,
        //transfered commoditystorage
        TransferedCommodityStorage = 205,

        CreateMainOrder=300,
        AddMainOrderLineItem = 301,
        ConfirmMainOrder = 302,

        ApproveOrderLineItem=303,
        ApproveMainOrder=304,
        OrderDispatchApprovedLineItems=305,
        ChangeMainOrderLineItem=306,
        RemoveMainOrderLineItem=307,
        RejectMainOrder=308,
        OrderPaymentInfo=309,

        CreateActivity=310,
        AddActivityInputItem = 311,
        AddActivityServiceItem = 312,
        AddActivityProduceItem = 313,
        AddActivityInfectionItem = 314,
        ConfirmActivity = 315,


        CreateCommodityWarehouseStorage = 316,
        AddCommodityWarehouseStorageLineItem = 317,
        ConfirmCommodityWarehouseStorage = 318,
        UpdateCommodityWarehouseStorageLineItem=319,
        ApproveCommodityWarehouseStorage = 321,
        StoreCommodityWarehouseStorage = 322,
        GenerateReceiptCommodityWarehouseStorage=323,

         CreateOutletVisitNote=320,

        CreateCommodityReleaseNote=324,
        AddCommodityReleaseNoteLineItem = 325,
        ConfirmCommodityReleaseNote = 326,

        ReRouteDocument=1000,
        AddExternalDocRef=1001,
        ReCollection=1002,

       
    }
}
