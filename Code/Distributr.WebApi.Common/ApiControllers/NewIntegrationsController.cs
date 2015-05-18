using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Import.Entities;
using Distributr.WSAPI.Lib.Services.AgrimanagrImports;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Lib.Services.Imports;
using Distributr.WebApi.ApiControllers;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WebApi.Common.ApiControllers
{
    public class NewIntegrationsController : BaseApiController
    {

        ILog _log = LogManager.GetLogger("NewIntegrationsController");
        private readonly IOrderExportDocumentRepository _orderExportDocumentRepository;
        private readonly ICostCentreApplicationService _costCentreApplicationService;
        private readonly IReceiptExportDocumentRepository _receiptExportDocumentRepository;
        private readonly IInvoiceExportDocumentRepository _invoiceExportDocumentRepository;
        private readonly IReturnInventoryExportDocumentRepository _returnInventoryExportDocumentRepository;
        private readonly ICountryImporterService _countryImporterService;
        private readonly IRegionImporterService _regionImporterService;
        private readonly IAreaImporterService _areaImporterService;
        private readonly IBankImporterService _bankImporterService;
        private readonly IBankBranchImporterService _bankBranchImporterService;
        private readonly ISupplierImporterService _supplierImporterService;
        private readonly IVATClassImporterService _vatClassImporterService;
        private readonly ISaleProductImporterService _saleProductImporterService;
        private readonly IProductBrandImporterService _productBrandImporterService;
        private readonly IPricingImporterService _pricingImporterService;
        private readonly IPricingTierImporterService _pricingTierImporterService;
        private readonly IDistributorImporterService _distributorImporterService;
        private readonly IDistributorSalesmanImporterService _distributorSalesmanImporterService;
        private readonly IRouteImporterService _routeImporterService;
        private readonly IOutletImporterService _outletImporterService;
        private readonly IProductFlavourImporterService _productFlavourImporterService;
        private readonly IProductTypeImporterService _productTypeImporterService;
        private readonly IProductPackagingTypeImporterService _productPackagingTypeImporterService;
        private readonly IProductPackagingImporterService _productPackagingImporterService;
        private readonly IInventoryImporterService _inventoryImporterService;
        private readonly IProductDiscountGroupImporterService _productDiscountGroupImporterService;
        private readonly IDiscountGroupImporterService _discountGroupImporterService;
        private readonly IOutletCategoryImporterService _outletCategoryImporterService;
        private readonly IOutletTypeImporterService _outletTypeImporterService;
        private IProductDiscountImporterService _productDiscountImporterService;

        #region Agrimanagr
        private readonly ICommodityTypeImporterService _commodityTypeImporterService;
        private readonly ICommodityImporterService _commodityImporterService;
        private readonly ICommoditySupplierImporterService _commoditySupplierImporterService;
        private readonly ICommodityOwnerImporterService _commodityOwnerImporterService;
        private readonly ICommodityOwnerTypeImporterService _commodityOwnerTypeImporterService;
        #endregion


        public NewIntegrationsController(ICountryImporterService countryImporterService, IRegionImporterService regionImporterService, 
            IBankImporterService bankImporterService, IBankBranchImporterService bankBranchImporterService, ISupplierImporterService 
            supplierImporterService, IVATClassImporterService vatClassImporterService, ISaleProductImporterService saleProductImporterService, 
            IProductBrandImporterService productBrandImporterService, IPricingImporterService pricingImporterService, 
            IDistributorImporterService distributorImporterService, IDistributorSalesmanImporterService distributorSalesmanImporterService, 
            IRouteImporterService routeImporterService, IOutletImporterService outletImporterService, 
            IProductFlavourImporterService productFlavourImporterService, IProductTypeImporterService productTypeImporterService, 
            IAreaImporterService areaImporterService, IInventoryImporterService inventoryImporterService, 
            IPricingTierImporterService pricingTierImporterService, IProductDiscountGroupImporterService productDiscountGroupImporterService, 
            IDiscountGroupImporterService discountGroupImporterService, IProductPackagingTypeImporterService productPackagingTypeImporterService, 
            IOutletCategoryImporterService outletCategoryImporterService, IOutletTypeImporterService outletTypeImporterService, 
            ICostCentreApplicationService costCentreApplicationService, IOrderExportDocumentRepository orderExportDocumentRepository, 
            IReceiptExportDocumentRepository receiptExportDocumentRepository, IInvoiceExportDocumentRepository invoiceExportDocumentRepository, 
            IProductDiscountImporterService productDiscountImporterService, IReturnInventoryExportDocumentRepository returnInventoryExportDocumentRepository, 
            IProductPackagingImporterService productPackagingImporterService, ICommodityTypeImporterService commodityTypeImporterService, ICommodityImporterService commodityImporterService, ICommoditySupplierImporterService commoditySupplierImporterService, ICommodityOwnerImporterService commodityOwnerImporterService, ICommodityOwnerTypeImporterService commodityOwnerTypeImporterService)
        {
            _countryImporterService = countryImporterService;
            _regionImporterService = regionImporterService;
            _bankImporterService = bankImporterService;
            _bankBranchImporterService = bankBranchImporterService;
            _supplierImporterService = supplierImporterService;
            _vatClassImporterService = vatClassImporterService;
            _saleProductImporterService = saleProductImporterService;
            _productBrandImporterService = productBrandImporterService;
            _pricingImporterService = pricingImporterService;
            _distributorImporterService = distributorImporterService;
            _distributorSalesmanImporterService = distributorSalesmanImporterService;
            _routeImporterService = routeImporterService;
            _outletImporterService = outletImporterService;
            _productFlavourImporterService = productFlavourImporterService;
            _productTypeImporterService = productTypeImporterService;
            _areaImporterService = areaImporterService;
            _inventoryImporterService = inventoryImporterService;
            _pricingTierImporterService = pricingTierImporterService;
            _productDiscountGroupImporterService = productDiscountGroupImporterService;
            _discountGroupImporterService = discountGroupImporterService;
            _productPackagingTypeImporterService = productPackagingTypeImporterService;
            _outletCategoryImporterService = outletCategoryImporterService;
            _outletTypeImporterService = outletTypeImporterService;
            _costCentreApplicationService = costCentreApplicationService;
            _orderExportDocumentRepository = orderExportDocumentRepository;
            _receiptExportDocumentRepository = receiptExportDocumentRepository;
            _invoiceExportDocumentRepository = invoiceExportDocumentRepository;
            _productDiscountImporterService = productDiscountImporterService;
            _returnInventoryExportDocumentRepository = returnInventoryExportDocumentRepository;
            _productPackagingImporterService = productPackagingImporterService;
            _commodityTypeImporterService = commodityTypeImporterService;
            _commodityImporterService = commodityImporterService;
            _commoditySupplierImporterService = commoditySupplierImporterService;
            _commodityOwnerImporterService = commodityOwnerImporterService;
            _commodityOwnerTypeImporterService = commodityOwnerTypeImporterService;
        }

        public ImportResponse CommodityOwnerDelete(List<string> deletedCodes)
        {
            try
            {
                return _commodityOwnerImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommoditySupplierDelete(List<string> deletedCodes)
        {
            try
            {
                return _commoditySupplierImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommodityDelete(List<string> deletedCodes)
        {
            try
            {
                return _commodityImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommodityTypeDelete(List<string> deletedCodes)
        {
            try
            {
                return _commodityTypeImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse CommodityOwnerTypeImport(List<CommodityOwnerTypeImport> imports)
        {
            try
            {
                var results = _commodityOwnerTypeImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommodityOwnerImport(List<CommodityOwnerImport> imports)
        {
            try
            {
                var results = _commodityOwnerImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommoditySupplierImport(List<CommoditySupplierImport> imports)
        {
            try
            {
                var results = _commoditySupplierImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CommodityImport(List<CommodityImport> imports)
        {
            try
            {
                var results = _commodityImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        public ImportResponse CommodityTypeImport(List<CommodityTypeImport> imports)
        {
            try
            {
                var results = _commodityTypeImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        
        public ImportResponse CountriesImport(List<CountryImport> imports)
        {
            try
            {
                var results = _countryImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }



        public ImportResponse CountriesDelete(List<string> deletedCodes)
        {
            try
            {
                var results = _countryImporterService.Delete(deletedCodes);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse AreasImport(List<AreaImport> imports)
        {
            try
            {
                return _areaImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse AreasDelete(List<string> deletedCodes)
        {
            try
            {
                return _areaImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse RegionsImport(List<RegionImport> imports)
        {
            try
            {
                return _regionImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse RegionsDelete(List<string> deletedCodes)
        {
            try
            {
                return _regionImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }


        public ImportResponse BanksImport(List<BankImport> imports)
        {
            try
            {
                return _bankImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse BanksDelete(List<string> deletedCodes)
        {
            try
            {
                return _bankImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }

        public ImportResponse BankBranchesDelete(List<string> deletedCodes)
        {
            try
            {
                return _bankBranchImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }


        public ImportResponse BankBranchesImport(List<BankBranchImport> imports)
        {
            try
            {
                return _bankBranchImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }

        }
        public ImportResponse SupplierImport(List<SupplierImport> imports)
        {
            try
            {
                return _supplierImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse SupplierDelete(List<string> deletedCodes)
        {
            try
            {
                return _supplierImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse VATClassImport(List<VATClassImport> imports)
        {
            try
            {
                return _vatClassImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse VATClassDelete(List<string> deletedCodes)
        {
            try
            {
                return _vatClassImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse ProductBrandImport(List<ProductBrandImport> imports)
        {
            try
            {
                var results = _productBrandImporterService.Save(imports);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductBrandDelete(List<string> deletedCodes)
        {
            try
            {
                var results = _productBrandImporterService.Delete(deletedCodes);
                return results;
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductFlavorImport(List<ProductFlavourImport> imports)
        {
            try
            {
                return _productFlavourImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }



        public ImportResponse ProductFlavorDelete(List<string> deletedCodes)
        {
            try
            {
                return _productFlavourImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductTypeImport(List<ProductTypeImport> imports)
        {
            try
            {
                return _productTypeImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse ProductTypeDelete(List<string> deletedCodes)
        {
            try
            {
                return _productTypeImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse SaleProductImport(List<SaleProductImport> imports)
        {
            try
            {
                return _saleProductImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse SaleProductDelete(List<string> deletedCodes)
        {
            try
            {
                return _saleProductImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse PricingTierImport(List<PricingTierImport> imports)
        {
            try
            {
                return _pricingTierImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse PricingTierDelete(List<string> deletedCodes)
        {
            try
            {
                return _pricingTierImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse PricingImport(List<PricingImport> imports)
        {
            try
            {
                return _pricingImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse PricingDelete(List<string> deletedCodes)
        {
            try
            {
                return _pricingImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse DistributorImport(List<DistributorImport> imports)
        {
            try
            {
                return _distributorImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse DistributorDelete(List<string> deletedCodes)
        {
            try
            {
                return _distributorImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }



        public ImportResponse DistributorSalesmanImport(List<DistributorSalesmanImport> imports)
        {
            try
            {
                return _distributorSalesmanImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse DistributorSalesmanDelete(List<string> deletedCodes)
        {
            try
            {
                return _distributorSalesmanImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse RouteImport(List<RouteImport> imports)
        {
            try
            {
                return _routeImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse RouteDelete(List<string> deletedCodes)
        {
            try
            {
                return _routeImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse OutletCategoryImport(List<OutletCategoryImport> imports)
        {
            try
            {
                return _outletCategoryImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse OutletCategoryDelete(List<string> deletedCodes)
        {
            try
            {
                return _outletCategoryImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse OutletTypeImport(List<OutletTypeImport> imports)
        {
            try
            {
                return _outletTypeImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse OutletTypeDelete(List<string> deletedCodes)
        {
            try
            {
                return _outletTypeImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse OutletImport(List<OutletImport> imports)
        {
            try
            {
                return _outletImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse OutletDelete(List<string> deletedCodes)
        {
            try
            {
                return _outletImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        public ImportResponse InventoryImport(List<InventoryImport> imports)
        {
            try
            {
                return _inventoryImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductPackagingTypeImport(List<ProductPackagingTypeImport> imports)
        {
            try
            {
                return _productPackagingTypeImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductPackagingTypeDelete(List<string> deletedCodes)
        {
            try
            {
                return _productPackagingTypeImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductPackagingImport(List<ProductPackagingImport> imports)
        {
            try
            {
                return _productPackagingImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse ProductPackagingDelete(List<string> deletedCodes)
        {
            try
            {
                return _productPackagingImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        public ImportResponse DiscountGroupImport(List<DiscountGroupImport> imports)
        {
            try
            {
                return _discountGroupImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse DiscountGroupDelete(List<string> deletedCodes)
        {
            try
            {
                return _discountGroupImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

       
        public ImportResponse ProductGroupDiscountImport(List<ProductDiscountGroupItemImport> imports)
        {
            try
            {
                return _productDiscountGroupImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }

        
        public ImportResponse ProductGroupDiscountDelete(List<string> deletedCodes)
        {
            try
            {
                return _productDiscountGroupImporterService.Delete(deletedCodes);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }
        public ImportResponse ProductDiscountImport(List<ProductDiscountImport> imports)
        {
            try
            {
                return _productDiscountImporterService.Save(imports);
            }
            catch (Exception ex)
            {
                return new ImportResponse() { Status = false, Info = ex.Message };
            }
        }


        [HttpGet]
        public HttpResponseMessage GetNextOrderToExport(string userName, string password, OrderType ordertype = OrderType.OutletToDistributor, DocumentStatus documentStatus = DocumentStatus.Confirmed)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextOrder", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _orderExportDocumentRepository.GetDocument(ordertype, documentStatus);
                    if (data != null)
                    {
                        transactionResponse.TransactionData = JsonConvert.SerializeObject(data);
                        transactionResponse.Status = true;
                    }
                    else
                    {
                        string documents = ordertype == OrderType.OutletToDistributor ? "Orders" : "Sales Orders";
                        transactionResponse.Info = string.Format("No {0} to import", documents);
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n", ex));
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpPost]
        public HttpResponseMessage MarkOrderAsExported(string userName, string password, string orderExternalRef)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - MarkOrderAsExported", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _orderExportDocumentRepository.MarkAsExported(orderExternalRef);

                    if (data)
                    {

                        transactionResponse.Status = true;
                    }
                    else
                    {
                        transactionResponse.Info = "Failed to mark as exported";
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpGet]
        public HttpResponseMessage GetNextReceiptToExport(string userName, string password)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextReceiptToExport", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _receiptExportDocumentRepository.GetPayment();
                    if (data != null)
                    {
                        transactionResponse.TransactionData = JsonConvert.SerializeObject(data);
                        transactionResponse.Status = true;
                    }
                    else
                    {
                        string documents =   "Receipt" ;
                        transactionResponse.Info = string.Format("No {0} to import", documents);
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n", ex));
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpPost]
        public HttpResponseMessage MarkRecieptAsExported(string userName, string password, Guid receiptId)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - MarkReceiptAsExported", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _receiptExportDocumentRepository.MarkAsExported(receiptId);

                    if (data)
                    {

                        transactionResponse.Status = true;
                    }
                    else
                    {
                        transactionResponse.Info = "Failed to mark as exported";
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");

            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpGet]
        public HttpResponseMessage GetNextInvoiceToExport(string userName, string password)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextInvoiceToExport", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _invoiceExportDocumentRepository.GetDocument();
                    if (data != null)
                    {
                        transactionResponse.Info = "1 Invoice Exported";
                        transactionResponse.TransactionData = JsonConvert.SerializeObject(data);
                        transactionResponse.Status = true;
                    }
                    else
                    {
                        const string documents = "Invoice";
                        transactionResponse.Info = string.Format("No {0} to import", documents);
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n", ex));
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpPost]
        public HttpResponseMessage MarkInvoiceAsExported(string userName, string password, Guid invoiceId)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - MarkInvoiceAsExported", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _invoiceExportDocumentRepository.MarkAsExported(invoiceId);

                    if (data)
                    {

                        transactionResponse.Status = true;
                    }
                    else
                    {
                        transactionResponse.Info = "Failed to mark as exported";
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");

            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }


        [HttpGet]
        public HttpResponseMessage GetNextReturnToExport(string userName, string password)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextReturnInventoryToExport", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _returnInventoryExportDocumentRepository.GetDocument();
                    if (data != null)
                    {
                        transactionResponse.Info = "1 Return Inventory Exported";
                        transactionResponse.TransactionData = JsonConvert.SerializeObject(data);
                        transactionResponse.Status = true;
                    }
                    else
                    {
                        const string documents = "Return Inventory";
                        transactionResponse.Info = string.Format("No {0} to import", documents);
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n"), ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpPost]
        public HttpResponseMessage MarkReturnAsExported(string userName, string password, Guid returnInventoryId)
        {
            var transactionResponse = new ImportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - MarkReturnInventoryAsExported", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.Info = "Invalid user credentials";
                    transactionResponse.Status = false;
                }
                else
                {
                    var data = _returnInventoryExportDocumentRepository.MarkAsExported(returnInventoryId);

                    if (data)
                    {

                        transactionResponse.Status = true;
                    }
                    else
                    {
                        transactionResponse.Info = "Failed to mark as exported";
                        transactionResponse.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Status = false;
                transactionResponse.Info = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");

            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

    }
}
