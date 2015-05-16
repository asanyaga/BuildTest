using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.Integrations.MasterData;
using Distributr.WSAPI.Lib.Integrations.MasterData.Exports;
using Distributr.WSAPI.Lib.Integrations.MasterData.Impl;
using Distributr.WSAPI.Lib.Integrations.Transactions;
using Newtonsoft.Json;
using StructureMap;

namespace Distributr.WSAPI.Lib.Integrations
{
   public class DistributrIntegrationService:IDistributrIntegrationService
    {
       private ITerritoryImportService _territoryImportService;
       private ICountryImportService _countryImportService;
       private IRegionImportService _regionImportService;
       private IAreaImportService _areaImportService;
       private IAssetCategoryImportService _assetCategoryImportService;
       private IBankBranchImportService _bankBranchImportService;
       private IBankImportService _bankImportService;
       private IProductBrandImportService _productBrandImportService;
       private IVatClassImportService _vatClassImportService;
       private ISupplierImportService _supplierImportService;
       private IProductFlavourImportService _productFlavourImportService;
       private IProductPackagingTypeImportService _productPackagingTypeImportService;
       private IProductTypeImportService _productTypeImportService;
       private IProductPackagingImportService _productPackagingImportService;
       private IPricingTierImportService _pricingTierImportService;
       private IOutletCategoryImportService _outletCategoryImportService;
       private IOutletTypeImportService _outletTypeImportService;
       private IProvinceImportService _provinceImportService;
       private IDistrictImportService _districtImportService;
       private IContactTypeImportService _contactTypeImportService;
       private IProductImportService _saleProductImportService;
       private IPricingImportService _pricingImportService;
       private IDiscountGroupImportService _discountGroupImportService;
       private IPromotionDiscountImportService _promotionDiscountImportService;
       private ISaleValueDiscountImportService _saleValueDiscountImportService;
       private IProductGroupDiscountImportService _productGroupDiscountImportService;
       private IDistributorsImportService _distributorsImportService;

       private ISalesmanImportService _salesmanImportService;
       private IOutletImportService _outletImportService;
       private IRouteImportService _routeImportService;
       private IShiptoAddressesImportService _shiptoAddressesImportService;

       private ISageTransactionsExportService _sageTransactionsExportService;
       private ISapTransactionsDownloadService _sapTransactionsDownloadService;
       private IQuickBooksTransactionsDownloadService _quickBooksTransactionsDownloadService;
       private IMasterDataExportService _masterDataExportService;
       private IInventoryTransferService _inventoryTransferService;

       private ICommodityImportService _commodityImportService;
       private ICommodityOwnerTypeImportService _commodityOwnerTypeImportService;
       private ICommoditySupplierImportService _commoditySupplierImportService;
       private ICommodityTypeImportService _commodityTypeImportService;
       private ICommodityOwnerImportService _commodityOwnerImportService;


       public DistributrIntegrationService(ITerritoryImportService territoryImportService, ICountryImportService countryImportService, IRegionImportService regionImportService, IAreaImportService areaImportService, IAssetCategoryImportService assetCategoryImportService, IBankBranchImportService bankBranchImportService, IBankImportService bankImportService, IProductBrandImportService productBrandImportService, IVatClassImportService vatClassImportService, ISupplierImportService supplierImportService, IProductFlavourImportService productFlavourImportService, IProductPackagingTypeImportService productPackagingTypeImportService, IProductTypeImportService productTypeImportService, IProductPackagingImportService productPackagingImportService, IPricingTierImportService pricingTierImportService, IOutletCategoryImportService outletCategoryImportService, IOutletTypeImportService outletTypeImportService, IProvinceImportService provinceImportService, IDistrictImportService districtImportService, IContactTypeImportService contactTypeImportService, IProductImportService saleProductImportService, IPricingImportService pricingImportService, IDiscountGroupImportService discountGroupImportService, IPromotionDiscountImportService promotionDiscountImportService, ISaleValueDiscountImportService saleValueDiscountImportService, IProductGroupDiscountImportService productGroupDiscountImportService, IDistributorsImportService distributorsImportService, ISalesmanImportService salesmanImportService, IOutletImportService outletImportService, IRouteImportService routeImportService, IShiptoAddressesImportService shiptoAddressesImportService, ISageTransactionsExportService sageTransactionsExportService, ISapTransactionsDownloadService sapTransactionsDownloadService, IQuickBooksTransactionsDownloadService quickBooksTransactionsDownloadService, IMasterDataExportService masterDataExportService, IInventoryTransferService inventoryTransferService, ICommodityImportService commodityImportService, ICommodityOwnerTypeImportService commodityOwnerTypeImportService, ICommoditySupplierImportService commoditySupplierImportService, ICommodityTypeImportService commodityTypeImportService, ICommodityOwnerImportService commodityOwnerImportService)
       {
           _territoryImportService = territoryImportService;
           _countryImportService = countryImportService;
           _regionImportService = regionImportService;
           _areaImportService = areaImportService;
           _assetCategoryImportService = assetCategoryImportService;
           _bankBranchImportService = bankBranchImportService;
           _bankImportService = bankImportService;
           _productBrandImportService = productBrandImportService;
           _vatClassImportService = vatClassImportService;
           _supplierImportService = supplierImportService;
           _productFlavourImportService = productFlavourImportService;
           _productPackagingTypeImportService = productPackagingTypeImportService;
           _productTypeImportService = productTypeImportService;
           _productPackagingImportService = productPackagingImportService;
           _pricingTierImportService = pricingTierImportService;
           _outletCategoryImportService = outletCategoryImportService;
           _outletTypeImportService = outletTypeImportService;
           _provinceImportService = provinceImportService;
           _districtImportService = districtImportService;
           _contactTypeImportService = contactTypeImportService;
           _saleProductImportService = saleProductImportService;
           _pricingImportService = pricingImportService;
           _discountGroupImportService = discountGroupImportService;
           _promotionDiscountImportService = promotionDiscountImportService;
           _saleValueDiscountImportService = saleValueDiscountImportService;
           _productGroupDiscountImportService = productGroupDiscountImportService;
           _distributorsImportService = distributorsImportService;
           _salesmanImportService = salesmanImportService;
           _outletImportService = outletImportService;
           _routeImportService = routeImportService;
           _shiptoAddressesImportService = shiptoAddressesImportService;
           _sageTransactionsExportService = sageTransactionsExportService;
           _sapTransactionsDownloadService = sapTransactionsDownloadService;
           _quickBooksTransactionsDownloadService = quickBooksTransactionsDownloadService;
           _masterDataExportService = masterDataExportService;
           _inventoryTransferService = inventoryTransferService;
           _commodityImportService = commodityImportService;
           _commodityOwnerTypeImportService = commodityOwnerTypeImportService;
           _commoditySupplierImportService = commoditySupplierImportService;
           _commodityTypeImportService = commodityTypeImportService;
           _commodityOwnerImportService = commodityOwnerImportService;
       }

       public Task<MasterDataImportResponse> ImportMasterData(IEnumerable<ImportEntity> importData)
       {
           return ImportEntity(importData.ToList());
       }

       
       private async Task<MasterDataImportResponse> ImportEntity(List<ImportEntity> importData)
       {
           var response = new MasterDataImportResponse();
           var entityType = (MasterDataCollective)Enum.Parse(typeof(MasterDataCollective), importData.FirstOrDefault().MasterDataCollective);
           switch (entityType)
           {
               case MasterDataCollective.Territory:
                   response = await _territoryImportService.ValidateAndSaveAsync(importData);
                   
                   break;

               case MasterDataCollective.Country:
                   response = await _countryImportService.ValidateAndSaveAsync(importData);
                  break;

               case MasterDataCollective.Region:
                   response = await _regionImportService.ValidateAndSaveAsync(importData);
                
                   break;
               case MasterDataCollective.Area:
                   response = await _areaImportService.ValidateAndSaveAsync(importData);

                   break;

               case MasterDataCollective.AssetCategory:
                   response = await _assetCategoryImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.Bank:
                   response = await _bankImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.BankBranch:
                   response = await _bankBranchImportService.ValidateAndSaveAsync(importData);
                   break;

               case MasterDataCollective.ProductBrand:
                   response = await _productBrandImportService.ValidateAndSaveAsync(importData);
                  
                   break;
               case MasterDataCollective.VatClass:
                   response = await _vatClassImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.Supplier:
                   response = await _supplierImportService.ValidateAndSaveAsync(importData);
                   break;

               case MasterDataCollective.ProductFlavour:
                   response = await _productFlavourImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.ProductPackagingType:
                   response = await _productPackagingTypeImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.ProductType:
                   response = await _productTypeImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.PricingTier:
                   response = await _pricingTierImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.OutletCategory:
                   response = await _outletCategoryImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.OutletType:
                   response = await _outletTypeImportService.ValidateAndSaveAsync(importData);
                   break;
               
               case MasterDataCollective.ContactType:
                   response = await _contactTypeImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.Province:
                   response = await _provinceImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.District:
                   response = await _districtImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.SaleProduct:
                   response = await _saleProductImportService.ValidateAndSaveAsync(importData);
                   break;

               case MasterDataCollective.Pricing:
                   response = await _pricingImportService.ValidateAndSaveAsync(importData);
                   break;

               case MasterDataCollective.DiscountGroup:
                   response = await _discountGroupImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.PromotionDiscount:
                   response = await _promotionDiscountImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.ProductGroupDiscount:
                   response = await _productGroupDiscountImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.SaleValueDiscount:
                   response = await _saleValueDiscountImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.DistributorSalesman:
                   response =await  _salesmanImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.Outlet:
                   response = await _outletImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.Route:
                   response = await _routeImportService.ValidateAndSaveAsync(importData);
                   break;
               case MasterDataCollective.ProductPackaging:
                   response = await _productPackagingImportService.ValidateAndSaveAsync(importData);
                   break;
                   case MasterDataCollective.Distributor:
                   response = await _distributorsImportService.ValidateAndSaveAsync(importData);
                   break;

                   #region Agrimanagr
                   case MasterDataCollective.Commodity:
                   response = await _commodityImportService.ValidateAndSaveAsync(importData);
                   break;
                   case MasterDataCollective.CommodityType:
                   response = await _commodityTypeImportService.ValidateAndSaveAsync(importData);
                   break;
                   case MasterDataCollective.CommodityOwnerType:
                   response = await _commodityOwnerTypeImportService.ValidateAndSaveAsync(importData);
                   break;
                   case MasterDataCollective.CommodityOwner:
                   response = await _commodityOwnerImportService.ValidateAndSaveAsync(importData);
                   break;
                   case MasterDataCollective.CommoditySupplier:
                   response = await _commoditySupplierImportService.ValidateAndSaveAsync(importData);
                   break;
                   #endregion

                   default:
                   var shipto = importData.FirstOrDefault();
                   if (shipto != null && shipto.MasterDataCollective.Contains("shipto"))
                   {
                       response = await _shiptoAddressesImportService.ValidateAndSaveAsync(importData);
                   }
                   else
                   {
                       response.Result = "Success";
                       response.ResultInfo = "No entity to be imported";
                       response.ErrorInfo = "Success"; 
                   }
                   break;
           }
           int count = response.ValidationResults.Where(n => !n.IsValid).Count();
           response.ResultInfo = response.ValidationResults.Any(n => !n.IsValid) ? "Completed with errors" : "Success";
           response.Result = "success";
           return response;
       }

       public TransactionResponse ExportTransactions(string integrationModule, string documentRef = "", OrderType orderType = OrderType.OutletToDistributor, bool includeInvoiceAndReceipts = false,DocumentStatus documentStatus=DocumentStatus.Closed)
       {
           var response = new TransactionResponse();
           if(string.IsNullOrEmpty(integrationModule))
           {
               response.ErrorInfo = "Integration Module is not provided";
               response.Result = "Cannot generate export orders";
               response.ResultInfo = "Invalid request";
               return response;
           }
           var requestingIntegrator = (IntegrationModule)Enum.Parse(typeof(IntegrationModule), integrationModule);
           switch (requestingIntegrator)
           {
               case IntegrationModule.Sage:
                   response = GenerateSageResponse(documentRef);
                   break;
                   case IntegrationModule.SAP:
                   response = GenerateSapResponse(documentRef,orderType);
                   break;
                   case IntegrationModule.QuickBooks:
                   response = GenerateQuickBooksResponse(documentRef, includeInvoiceAndReceipts,documentStatus);
                   break;
               default:
                   response.ErrorInfo = "Integration Module provided is not recognised";
           response.Result = "Cannot generate export orders";
           response.ResultInfo = "Invalid request";
                   break;
           }
           return response;
       }

       public TransactionResponse ExportReturnsTransactions(string integrationModule, string documentRef = "")
       {
           var response = new TransactionResponse();
           if (string.IsNullOrEmpty(integrationModule))
           {
               response.ErrorInfo = "Integration Module is not provided";
               response.Result = "Cannot generate export orders";
               response.ResultInfo = "Invalid request";
               return response;
           }
          
           response = GenerateQuickBooksResponseForReturnsTransactions(documentRef);
           return response;
       }

       private TransactionResponse GenerateQuickBooksResponseForReturnsTransactions(string documentRef)
       {
           var response = new TransactionResponse();

           var orders = _quickBooksTransactionsDownloadService.GetReturnsPendingExport(documentRef);

           if (orders == null || !orders.Any())
           {
               response.ErrorInfo = "Success";
               response.Result = "Order not found";
               response.ResultInfo = "No transaction to export";
               return response;
           }
           response.DocRefs =
               orders.Where(p => p.DocumentType == DocumentType.ReturnsNote).Select(n=>n.SalesmanCode).ToList
                   ();
           response.TransactionData = JsonConvert.SerializeObject(orders);
           response.ErrorInfo = "Success";
           response.Result = "success";
           return response;
       }

       private TransactionResponse GenerateQuickBooksResponse(string documentRef,bool includeInvoiceAndReceipts,DocumentStatus documentStatus=DocumentStatus.Closed)
       {
           var response = new TransactionResponse();

           var orders = _quickBooksTransactionsDownloadService.GetOrdersPendingExport(documentRef,
                                                                                      includeInvoiceAndReceipts, documentStatus);

           if (orders == null || !orders.Any())
           {
               response.ErrorInfo = "Success";
               response.Result = "Order not found";
               response.ResultInfo = "No transaction to export";
               return response;
           }
           response.DocRefs =
               orders.Where(p => p.DocumentType == DocumentType.Order).Select(n => n.GenericReference).Distinct().ToList
                   ();
           response.TransactionData = JsonConvert.SerializeObject(orders);
           response.ErrorInfo = "Success";
           response.Result = "success";
           return response;
       }
       
       public TransactionsAcknowledgementResponse MarkAsExported(string integrationModule, IEnumerable<string> orderReferences)
       {
           TransactionsAcknowledgementResponse response;
           IntegrationModule module = (IntegrationModule)Enum.Parse(typeof(IntegrationModule), integrationModule);
           bool done = false;
           switch (module)
           {
               case IntegrationModule.Sage:
                   done=_sageTransactionsExportService.MarkAsExported(orderReferences);
                   break;
                   case IntegrationModule.SAP:
                   done = _sapTransactionsDownloadService.MarkAsExported(orderReferences);
                   break;
                   case IntegrationModule.QuickBooks:
                   done = _quickBooksTransactionsDownloadService.MarkAsExported(orderReferences);
                   break;
           }
           if(done)
           {
               response=new TransactionsAcknowledgementResponse
                            {
                                Result = "Success",
                                ResultInfo = "All orders acknowledged successfully"
                            };
           }
           else
           {
               response = new TransactionsAcknowledgementResponse
               {
                   Result = "Error",
                   ResultInfo = "Unknown Server Error Occured while acknowledging"
               };
           }
           return response;
       }

       public MasterdataExportResponse ExportMasterData(ThirdPartyMasterDataQuery query)
       {
           return _masterDataExportService.GetResponse(query);
       }

       public  IntegrationResponse ProcessInventory(InventoryTransferDTO dtos)
       {
           return  _inventoryTransferService.Process(dtos);
       }

       public List<string> GetInventoryAcknowledgements(IntegrationModule integrationModule, DateTime date)
       {
          return ObjectFactory.GetInstance<IIntegrationDocumentRepository>().GetInventoryAcknowledgements(integrationModule,
                                                                                                    date);
       }

       private TransactionResponse GenerateSageResponse(string documentRef)
       {
           TransactionResponse response;
           if (!string.IsNullOrEmpty(documentRef))
           {
               var item = _sageTransactionsExportService.GetShellOrderByRef(documentRef);
               response = MapTransactionResponse(item);
               if(item !=null && item.Any())
                   response.DocRefs = new List<string>() { documentRef};
           }
           else
           {
               var transactions = _sageTransactionsExportService.GetShellOrdersPendingExport();
               response = MapTransactionResponse(transactions);
               if (transactions != null && transactions.Any())
                   response.DocRefs = transactions.Select(n => n.ExternalOrderReference).Distinct().ToList();
           }
           response.Result = "success";
           return response;
       }

       #region Sage response mapping
       private TransactionResponse MapTransactionResponse(List<ShellOrderExportDto> orders)
       {
           
           var response = new TransactionResponse();
           if(orders==null ||!orders.Any())
           {
               response.ErrorInfo = "Success";
               response.Result = "Order not found";
               response.ResultInfo = "No transaction to export";
               return response;
           }
           response.TransactionData = orders.ToCsv();
           return response;
       }
       #endregion

       #region SAP Response
       private TransactionResponse GenerateSapResponse(string docref="",OrderType orderType=OrderType.OutletToDistributor)
       {
           var response=new TransactionResponse();

           var transactions = _sapTransactionsDownloadService.GetOrdersPendingExport(docref, orderType);
           if (transactions != null && transactions.Any())
           {
               response.DocRefs = transactions.Select(n => n.OrderRef).Distinct().ToList();//I use it for acknowledging transaction only
               response.TransactionData = transactions.ToCsv();
           }
           return response;
       }

       
      #endregion

       
#region inventory Transfer

       private void AdjustDistributorInventory()
       {
           InventoryTransferNote inventoryTransferDoc = null;
           InventoryAdjustmentNote inventoryAdjustmentNote = null;
       }

       private void IssueInventoryToSalesman()
       {
           
       }
       //public void IssueInventoryAsync(Dictionary<string, IEnumerable<ImportInvetoryIssueToSalesman>> stockLines)
       //{
       //    return await Task.Run(() =>
       //    {
       //        var errors = new List<string>();
       //        var applicationId = Guid.Empty;// _configService.Load().CostCentreApplicationId;

       //        foreach (var stockLine in stockLines.GroupBy(n => n.Key)) //key=>salesmancode
       //        {


       //            InventoryTransferNote inventoryTransferDoc = null;
       //            InventoryAdjustmentNote inventoryAdjustmentNote = null;
       //            var salsmancode = stockLine.Key;

       //            var distributor =
       //                _repositoryHelpers.MapDistributor(
       //                    _ctx.tblCostCentre.FirstOrDefault(
       //                        n => n.CostCentreType == (int)CostCentreType.Distributor));
       //            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Loading Salesman...."));
       //            var transferTo = _costCentreRepository.GetByCode(salsmancode, CostCentreType.DistributorSalesman, true);

       //            var salesmanUser = _userRepository.GetByCostCentre(distributor.Id).FirstOrDefault();
       //            if (distributor != null && transferTo != null && salesmanUser != null)
       //            {

       //                #region create inventory adjustment note

       //                inventoryAdjustmentNote =
       //                    _inventoryAdjustmentNoteFactory.Create(distributor, applicationId,
       //                                                           distributor,
       //                                                           salesmanUser, "",
       //                                                           InventoryAdjustmentNoteType
       //                                                               .Available, Guid.Empty);
       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("AInventory adjustment note created for distribuor=>{0}", distributor.CostCentreCode));

       //                #endregion

       //                #region create inventory transfer note

       //                inventoryTransferDoc =
       //                    _inventoryTransferNoteFactory.Create(distributor, applicationId,
       //                                                         salesmanUser, transferTo,
       //                                                         distributor, "");
       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("AInventory transfer note created to salesman=>{0}", transferTo.CostCentreCode));

       //                #endregion

       //                foreach (var stockitem in stockLine.SelectMany(n => n.Value.ToList()))
       //                {
       //                    string msg = string.Empty;
       //                    try
       //                    {
       //                        var product =
       //                            _ctx.tblProduct.FirstOrDefault(
       //                                p =>
       //                                p.ProductCode != null &&
       //                                p.ProductCode == stockitem.ProductCode);

       //                        if (product != null)
       //                        {
       //                            var lineitem =
       //                                _inventoryAdjustmentNoteFactory.CreateLineItem(
       //                                    stockitem.
       //                                        ApprovedQuantity, product.id,
       //                                    0,
       //                                    0,
       //                                    "Inventory Adjustment");

       //                            inventoryAdjustmentNote.AddLineItem(lineitem);

       //                            InventoryTransferNoteLineItem itnLineitem =
       //                                _inventoryTransferNoteFactory.CreateLineItem(
       //                                    product.id,
       //                                    stockitem.
       //                                        ApprovedQuantity,
       //                                    0, 0,
       //                                    "");
       //                            if (itnLineitem != null)
       //                                inventoryTransferDoc.AddLineItem(itnLineitem);



       //                            if (msg != "")
       //                            {
       //                                FileUtility.LogError(msg);
       //                                errors.Add(msg);
       //                            }
       //                        }
       //                        else
       //                        {
       //                            var error = string.Format("{0} doest exist=>",
       //                                                      stockitem.ProductCode);
       //                            if (!errors.Any(p => p.Contains(error)))
       //                                errors.Add(error);
       //                        }

       //                    }
       //                    catch (Exception ex)
       //                    {
       //                        Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Error occured while adjusting inventory..See error logs"));
       //                        FileUtility.LogError(ex.Message);
       //                    }
       //                }
       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Confirming inventory adjustment note for distributr=>{0}", distributor.CostCentreCode));
       //                inventoryAdjustmentNote.Confirm();
       //                _inventoryAdjustmentNoteWfManager.SubmitChanges(inventoryAdjustmentNote);
       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Inventory adjustment note CONFIRMED for distributr=>{0}", distributor.CostCentreCode));

       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Confirming inventory transfer to salesman=>{0}", transferTo.CostCentreCode));
       //                inventoryTransferDoc.Confirm();
       //                _inventoryTransferNoteWfManager.SubmitChanges(inventoryTransferDoc);
       //                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Inventory  transfer CONFIRMED to salesman=>{0}", transferTo.CostCentreCode));



       //            }
       //            else
       //            {
       //                FileUtility.LogError(
       //                    string.Format("Inventory issue failed for salesman=>{0}",
       //                                  salsmancode));
       //                if (distributor == null)
       //                {
       //                    FileUtility.LogError(
       //                        string.Format("Distributor is null=> doest exist"));
       //                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Distributor is null=> doest exist"));
       //                }

       //                if (transferTo == null)
       //                {
       //                    FileUtility.LogError(
       //                        string.Format("salesman is with code=>{0} doest exist",
       //                                      salsmancode));
       //                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("salesman is with code=>{0} doest exist",
       //                                      salsmancode));

       //                }

       //                if (salesmanUser == null)
       //                {
       //                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("user is null=> doest exist"));
       //                    FileUtility.LogError(string.Format("user is null=> doest exist"));
       //                }


       //                FileUtility.LogError("----------------------------------------------");

       //            }
       //        }

       //        return errors;
       //    });
       //}

#endregion
    }
}
