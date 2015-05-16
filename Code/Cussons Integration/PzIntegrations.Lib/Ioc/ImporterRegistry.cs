using PzIntegrations.Lib.MasterDataImports.Outlets;
using PzIntegrations.Lib.MasterDataImports.Products;
using PzIntegrations.Lib.MasterDataImports.Salesmen;
using PzIntegrations.Lib.MasterDataImports.Shipping;
using PzIntegrations.Lib.TransactionServices;
using StructureMap.Configuration.DSL;

namespace PzIntegrations.Lib.Ioc
{
    public class ImporterRegistry : Registry
    {
        public ImporterRegistry()
        {
            For<IOutletImportService>().Use<OutletImportService>();
            For<IProductImportService>().Use<ProductImportService>();
            For<IProductBrandImportService>().Use<ProductBrandImportService>();
            For<IShipToAddressImportService>().Use<ShipToAddressImportService>();
            For<ISalesmanImportService>().Use<SalesmanImportService>();
            For<IOrderExportService>().Use<OrderExportService>();
            For<IPzIntegrationService>().Use<PzIntegrationService>();

        }
    }
}
