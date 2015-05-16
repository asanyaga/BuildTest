namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public enum DocumentSourcingStatus
    {
        New       = 0,
        Confirmed = 1,
        Received = 2,
        Cancelled = 3,
        Stored=4,
        Closed = 5,
        Approved = 6,
        ReceiptGenerated=7
    }
   
}
