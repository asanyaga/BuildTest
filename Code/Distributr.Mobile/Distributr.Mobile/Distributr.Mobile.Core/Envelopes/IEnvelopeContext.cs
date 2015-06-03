using System;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Mobile.Core.Envelopes
{
    public interface IEnvelopeContext
    {
        Guid VisitId { get; set; }
        int OrderTypeId { get; }
        User User { get; }
        string ShipToAddress { get; }
        decimal SaleDiscount { get; }
        Guid ParentDocumentId { get; }
        Guid IssuedOnBehalfOfCostCentre { get; }
        Guid GeneratedByCostCentreId { get; }
        Guid RecipientCostCentreId { get; }
        Guid GeneratedByCostCentreApplicationId { get; }
        DateTime Timestamp { get; }
        Guid GeneratedByUserId { get; }
        Guid InvoiceId { get; }
        int InventoryAdjustmentNoteType { get; }
        string InventoryAdjustmentNoteReason { get; }
        bool IsClosable();

        string ExternalDocumentReference();
        string OrderSaleReference();
        string ReceiptReference();
        string InvoiceReference();

    }
}
