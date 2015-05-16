using Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Workflow;
using Distributr.DataImporter.Lib.Experimental;
using Distributr.DataImporter.Lib.Experimental.Sync;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups.Impl;
using Distributr.DataImporter.Lib.ImportService.Orders;
using Distributr.DataImporter.Lib.ImportService.Orders.Impl;
using Distributr.DataImporter.Lib.ImportService.Outlets;
using Distributr.DataImporter.Lib.ImportService.Outlets.Impl;
using Distributr.DataImporter.Lib.ImportService.PriceGroups;
using Distributr.DataImporter.Lib.ImportService.PriceGroups.Impl;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.DataImporter.Lib.ImportService.Products.Impl;
using Distributr.DataImporter.Lib.ImportService.Salesman;
using Distributr.DataImporter.Lib.ImportService.Salesman.Impl;
using Distributr.DataImporter.Lib.ImportService.Shipping;
using Distributr.DataImporter.Lib.ImportService.Shipping.Impl;
using Distributr.DataImporter.Lib.Workflows;
using StructureMap.Configuration.DSL;

namespace Distributr.DataImporter.Lib.IoC
{
    public class ImporterRegistry : Registry
    {
        public ImporterRegistry()
        {
            For<IProductImportService>().Use<ProductImportService>();
            For<IOutletImportService>().Use<OutletImportService>();
            For<IDistributorSalesmanImportService>().Use<DistributorSalesmanImportService>();
            For<IPricingImportService>().Use<PricingImportService>();
            For<IShipToAddressImportService>().Use<ShipToAddressImportService>();
            For<IProductDiscountGroupImportService>().Use<ProductDiscountGroupImportService>();
            For<IInvetoryIssueToSalesmanImportService>().Use<InvetoryIssueToSalesmanImportService>();
            For<IExternalOrderWorkflow>().Use<FCLImportOrderWorkFlow>();
            For<IExportImportAuditRepository>().Use<ExportImportAuditRepository>();
            For<IOrderExportService>().Use<OrderExportService>();
            For<ISalesExportService>().Use<SalesExportService>();

            //experimental

            For<IProductDiscountGroupMapper>().Use<ProductDiscountGroupMapper>();
            For<IGroupDiscountMapper>().Use<GroupDiscountMapper>();
            For<IPricingMapper>().Use<PricingMapper>();
            For<ISyncProductGroupDiscount>().Use<SyncProductGroupDiscount>();
            For<ISyncProductPricing>().Use<SyncProductPricing>();
        }
    }
}
