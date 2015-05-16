using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.WSAPI.Lib.Integrations
{
    public class IntegrationResponse : ResponseBasic
    {

    }
    public class MasterDataValidationAndImportResponse:IntegrationResponse
    {
        public MasterDataValidationAndImportResponse()
        {
            ValidationResults = new List<StringifiedValidationResult>();
        }

        public int UploadedItemsCount { get; set; }
        public List<StringifiedValidationResult> ValidationResults { get; set; }
    }
    public class MasterdataExportResponse:IntegrationResponse
    {
        public IEnumerable<ImportEntity> MasterData { get; set; }
        public bool HasNextPage { get; set; }
        public int CurrentPage { get; set; }
        public int Skip { get; set; }

    }
    public class MasterDataImportResponse : IntegrationResponse
    {
        public MasterDataImportResponse()
        {
            ValidationResults = new List<ImportValidationResultInfo>();
            
        }
        
        public List<ImportValidationResultInfo> ValidationResults { get; set; }
    }

    public class TransactionExportResponse : ResponseBool
    {
        //TODO:GO=>Figure out a better way to pull orders/sales/payments using a single web api response
        public TransactionExportResponse()
        {
           
        }
        public string TransactionData { get; set; }
        public string Info { get; set; }
        public bool Status { get; set; }
        
    }
    public class TransactionResponse : IntegrationResponse
    {
        //TODO:GO=>Figure out a better way to pull orders/sales/payments using a single web api response
        public TransactionResponse()
        {
            DocRefs = new List<string>();
        }
        public string TransactionData { get; set; }
        public List<string> DocRefs { get; set; }
    }

    public class TransactionsAcknowledgementResponse:IntegrationResponse
    {
        
    }
    public class InventoryImportAcknowledgment : IntegrationResponse
    {
        public InventoryImportAcknowledgment()
        {
            ImportedDocumentRefs=new List<string>();
        }
        public List<string> ImportedDocumentRefs { get; set; } 
    }
    public class StringifiedValidationResult
    {
        public StringifiedValidationResult()
        {
            Results=new List<string>();
        }

        public List<string> Results { get; set; }
        public string Description { get; set; }
        public string Entity { get; set; }
        public bool IsValid { get; set; }
    }

    public class ImportValidationResultInfo : ValidationResultInfo
    {
        public string Description { get; set; }
        public MasterEntity Entity { get; set; }
        public string EntityItem { get; set; }
    }
  
    public interface IDistributrIntegrationService
    {
        Task<MasterDataImportResponse> ImportMasterData(IEnumerable<ImportEntity> importData);
        TransactionResponse ExportTransactions(string integrationModule, string documentRef = "",OrderType orderType=OrderType.OutletToDistributor, bool includeInvoiceAndReceipts = false,DocumentStatus documentStatus=DocumentStatus.Closed);

        TransactionResponse ExportReturnsTransactions(string integrationModule, string documentRef = "");
        TransactionsAcknowledgementResponse MarkAsExported(string integrationModule, IEnumerable<string> orderReferences);

        MasterdataExportResponse ExportMasterData(ThirdPartyMasterDataQuery query);
        IntegrationResponse ProcessInventory(InventoryTransferDTO dto);

        List<string> GetInventoryAcknowledgements(IntegrationModule integrationModule, DateTime date);
    }

    
    public class ImportEntity
    {
        public ImportEntity()
        {
            Fields=new string[100];
        }

        public string MasterDataCollective { get; set; }
        public  string[] Fields { get; set; } 
    }

   
    public class InventoryTransferDTO
    {
        public InventoryTransferDTO()
        {
            SalesmanInventoryList=new List<Dictionary<string, List<InventoryLineItemDto>>>();
            DistributorInventory=new List<InventoryLineItemDto>();
            Credentials=new IntegrationCredential();
            ExternalDocumentRefList=new List<string>();
        }

        public IntegrationCredential Credentials { get; set; }

        /// <summary>
        /// Contains list of inventory to be issued/adjusted positively to a distributor 
        /// </summary>
        public List<InventoryLineItemDto> DistributorInventory { get; set; }

        /// <summary>
        /// Distributor whose inventory is to be adjusted
        /// </summary>
        public string DistributorCode { get; set; }

        /// <summary>
        /// This contain list of document refs from Inventory Source to avoid double pulling from external source
        /// </summary>
        public List<string> ExternalDocumentRefList { get; set; }

        /// <summary>
        /// When issuing inventory to salesman at the same time...issue salesman whose code is dictionary key,inventory in list.
        /// First adjust distributor inventory upwards,..then issue.
        /// </summary>
        public List<Dictionary<string, List<InventoryLineItemDto>>> SalesmanInventoryList { get; set; } //salsmancode,list of inventory to be issued
   
    }
    public class IntegrationCredential
    {
        public string ApiUserName { get; set; }
        public string Password  { get; set; }
        public IntegrationModule IntegrationModule { get; set; }
    }
    public class InventoryLineItemDto
    {
       public string ProductCode { get; set; }
       public decimal Quantity { get; set; }
    }
}
