using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository
{
    public interface IIntegrationDocumentRepository : IDocumentRepository<MainOrder>
    {
        List<PzCussonsOrderIntegrationDto> GetPzOrdersPendingExport();
       List<PzCussonsOrderIntegrationDto> GetPzOrdersPendingExport(DateTime startDate, DateTime endDate);
        PzCussonsOrderIntegrationDto GetPzOrdersPendingExport(string externalDocRef);

        List<FclExportOrderDto> GetFclOrdersPendingExport(OrderType orderType, string search = "");
        List<FclPaymentExportDto> GetFclPaymentsPendingExport();

        #region shell Sage =>Customised for shell


        List<ShellOrderExportDto> GetShellOrderByRef(string orderRef);
        List<ShellOrderExportDto> GetShellOrdersPendingExport();

        #endregion

        #region SAP
        List<SapDocumentExportDto> GetOrdersPendingExport(string orderref="",OrderType orderType=OrderType.OutletToDistributor);
        #endregion

        #region QuickBooks
        List<QuickBooksOrderDocumentDto> GetPendingExport(bool includeReceiptsAndInvoice=false, string docRef = "",DocumentStatus documentStatus=DocumentStatus.Closed);
        List<QuickBooksOrderDocumentDto> GetTransactionPendingExport(bool includeReceiptsAndInvoice = false, DocumentStatus documentStatus = DocumentStatus.Closed);
       
        List<QuickBooksReturnInventoryDocumentDto> GetReturnsPendingExport(string documentRef);
        #endregion

        bool MarkAsExported(IEnumerable<string> orderReferences,IntegrationModule module);
       bool MarkInventoryDocumentAsImported(IEnumerable<string> docrefs, IntegrationModule module);

       List<string> GetInventoryAcknowledgements(IntegrationModule integrationModule, DateTime date);
       
    }

    public class ExportedOrderSummary
    {
        public string GenericOrderReference { get; set; }
        public string ExternalOrderReference { get; set; }
        public string DocumentDateExported { get; set; }
    }

    #region PZCussons MFG Integration
    public class PzCussonsOrderIntegrationDto
    {
        public PzCussonsOrderIntegrationDto()
        {
            LineItems=new List<PzCussonsIntegrationDtoOrderLineItem>();
        }

        public string GenericOrderReference { get; set; }
        public string ExternalOrderReference { get; set; }
        public string ShiptoAddressCode { get; set; }
        public string SalesmanCode { get; set; }
        public string OutletCode { get; set; }
        public string OrderDateRequired { get; set; }
        public string DocumentDateIssued { get; set; }
        public string Currency { get; set; }
        public string TotalNet   { get; set; }
        public string Note { get; set; }
        public string ChequeReferenceNo { get; set; }
        public List<PzCussonsIntegrationDtoOrderLineItem> LineItems { get; set; }

    }

    public class PzCussonsIntegrationDtoOrderLineItem
    {
        public int Count { get; set; }
        public string ProductCode { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal Value { get; set; }
        public string OrderDateRequired { get; set; }
        public string Location { get; set; }
        public string Site { get; set; }
    }
    #endregion

    #region FCL NAvision Integration
    public class FclPaymentExportDto
    {
        public string GenericOrderReference { get; set; }
        public string ExternalOrderReference { get; set; }
        public string PaymentDate { get; set; }
        public string SalesmanCode { get; set; }
        public string OutletCode { get; set; }
        public string ShiptoAddressCode { get; set; }
        public string Salevalue { get; set; }
        public string AmountPaid { get; set; }
        public string Balance { get; set; }
    }

    public class FclExportOrderDto
    {
        public FclExportOrderDto()
        {
            LineItems=new List<FclExportOrderLineItem>();
        }

        public string GenericOrderReference { get; set; }
        public string ExternalOrderReference { get; set; }
       
        public string OrderDate { get; set; }
       
        public string SalesmanCode { get; set; }
       
        public string OutletCode { get; set; }
        public string ShiptoAddressCode { get; set; }

        public List<FclExportOrderLineItem> LineItems { get; set; } 
        
    }

    public class FclExportOrderLineItem
    {
         public string ProductCode { get; set; }
       
        public decimal ApprovableQuantity { get; set; }
    }

#endregion

  #region Shell SAGE Integration
   
    public class ShellOrderExportDto
    {
        public string OrderDate { get; set; }
        public string ExternalOrderReference { get; set; }
        public string OutletCode { get; set; }
        public string PricingTierCode { get; set; }
        public string SalesmanCode { get; set; }
        public string ShiptoAddressCode { get; set; }
        public string ChequeNo { get; set; }
        public string SalesmanCodeTwo { get; set; }
        public string ModeOfpayment { get; set; }
        public string AmountPaid { get; set; }
       
        //lineitem
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string Note { get; set; }
        public string OrderDateRequired { get; set; }
        public string NetAmountExlVAT { get; set; }
        public string TotalVATAmount { get; set; }
        public string NetAmountIncVAT { get; set; }
        
    }

    public class ShellSalesOrderLineItemExport
    {
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string OrderDateRequired { get; set; }
        public string PricingTierCode { get; set; }
        public string NetAmountExlVAT { get; set; }
        public string NetAmountIncVAT { get; set; }
        public string TOtalVATAmount { get; set; }
        
    }
 
#endregion

#region SAP Transaction integration

    public class SapDocumentExportDto
    {
       //doc
        public string OutletCode { get; set; }
        public string OrderRef { get; set; }
        public string OrderDate { get; set; }
        public string OrderDueDate { get; set; }
       
        //lineitem
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string VatClass { get; set; }

        public string SalesmanCode { get; set; }
    }
   
#endregion

#region Quick Books
      
    [Serializable]
    public class QuickBooksOrderDocumentDto
    {
        
        public QuickBooksOrderDocumentDto()
        {
            LineItems=new List<QuickBooksOrderDocLineItem>();
        }

     
        public DocumentType DocumentType { get; set; }
        public OrderType OrderType { get; set; }
        public string GenericReference { get; set; }
        public string ExternalReference { get; set; }
        public string OutletName { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Used to lookup  outlets from distributr and import if not present in quick books at order export
        /// </summary>
        public string OutletCode { get; set; } 
        public string OrderDateRequired { get; set; }
        public string DocumentDateIssued { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalDiscount { get; set; }

        public List<QuickBooksOrderDocLineItem> LineItems { get; set; }


        public string SalesmanName { get; set; }
        public string SalesmanCode { get; set; }
    }
    
    public class QuickBooksOrderDocLineItem
    {
        public string ProductDescription { get; set; }
        /// <summary>
        /// Used to lookup  products from distributr and import if not present in quick books at order export
        /// </summary>
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal LineItemValue { get; set; }
        public decimal GrossValue { get; set; }
        public decimal TotalVat { get; set; }
        public string PaymentType { get; set; }
        public string PaymentRef { get; set; }
        public string VATClass { get; set; }
       
    }

    public class QuickBooksReturnInventoryDocumentDto
    {
        public QuickBooksReturnInventoryDocumentDto()
        {
            LineItems = new List<QuickBooksReturnInventoryDocLineItemDto>();
        }

        public List<QuickBooksReturnInventoryDocLineItemDto> LineItems { get; set; }

        public DocumentType DocumentType { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string GenericReference { get; set; }
       
    }

    public class QuickBooksReturnInventoryDocLineItemDto
    {
        public Guid LineItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }

   }
#endregion

   
}
