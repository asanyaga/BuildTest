using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public enum DocumentType
    {
        Order = 1,
        DispatchNote = 2,
        InventoryReceivedNote = 3,
        InventoryTransferNote = 4,
        Invoice = 5,
        SalesInvoice = 6,
        ReturnsNote = 7,
        Receipt = 8,
        InventoryAdjustmentNote = 9,
        CreditNote = 10, 
        DisbursementNote = 11,
        PaymentNote = 12,
        CommodityPurchaseNote=13,
        CommodityReceptionNote=14,
        CommodityStorageNote=15,
        CommodityDelivery=16,
        ReceivedDelivery=17,
        CommodityTransferNote=18,
       
        CommodityWarehouseStorage = 19,
        CommodityRelease=21,
        OutletVisitNote = 20,
        ActivityNote=22,
        RecollectionNote=23,
        
        
        SystemCommandPlaceholder = 97,
        CreateInventorySerialsPlaceholder = 98,
        RetirePlaceholder=99
    }

    public enum WeighType
    {
        Manual = 1,
        WeighScale = 2
    }
}
