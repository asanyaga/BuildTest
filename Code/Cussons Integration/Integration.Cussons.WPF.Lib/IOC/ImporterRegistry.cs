using Integration.Cussons.WPF.Lib.ExportService;
using Integration.Cussons.WPF.Lib.ExportService.Orders;
using Integration.Cussons.WPF.Lib.ImportService.CostCentres;
using Integration.Cussons.WPF.Lib.ImportService.CostCentres.Impl;
using Integration.Cussons.WPF.Lib.ImportService.DistributrSalesmen;
using Integration.Cussons.WPF.Lib.ImportService.Products;
using Integration.Cussons.WPF.Lib.ImportService.Products.Impl;
using Integration.Cussons.WPF.Lib.ImportService.Shipping;
using StructureMap.Configuration.DSL;

namespace Integration.Cussons.WPF.Lib.IOC
{
   public class ImporterRegistry:Registry
    {
       public ImporterRegistry()
       {
           For<IProductImportService>().Use<ProductImportService>();
           For<IProductBrandImportService>().Use<ProductBrandImportService>();
           For<IShipToAddressImportService>().Use<ShipToAddressImportService>();
           For<IOutletImportService>().Use<OutletImportService>();
           For<IDistributorSalesmanImportService>().Use<DistributorSalesmanImportService>();
           For<IAfcoPricingImportService>().Use<AfcoPricingImportService>();
           For<IOrderExportService>().Use<OrderExportService>();
           For<IDistributrIntegrationService>().Use<DistributrIntegrationService>();

       }
    }
}
