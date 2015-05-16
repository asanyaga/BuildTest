using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;
using StructureMap.Configuration.DSL;

namespace Distributr_Middleware.WPF.Lib.IOC
{
   public class ImporterRegistry:Registry
    {
       public ImporterRegistry()
       {
           For<IMasterDataImportService>().Use<MasterDataImportService>();
           For<IMiddlewareService>().Use<MiddlewareService>();
           For<ITransactionsDownloadService>().Use<TransactionsDownloadService>();
           For<IInventoryTransferService>().Use<InventoryTransferService>();
       }
    }
}
