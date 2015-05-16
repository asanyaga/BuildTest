using Distributr_Middleware.WPF.Lib.IOC;
using SAPUtilityLib.Masterdata;
using SAPUtilityLib.Masterdata.Impl;
using SAPUtilityLib.Proxies;
using SAPUtilityLib.Proxies.Impl;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace SAPUtilityLib.IoC
{
   public class SApRegistry:Registry
    {
       public SApRegistry()
       {
           For<IExportTransactionsService>().Use<ExportTransactionsService>();
           For<IPullMasterdataService>().Use<PullMasterdataService>();
           For<ISapWebProxy>().Use<SapWebProxy>();
           For<IOrderExportTransactionsService>().Use<OrderExportTransactionsService>();
           
        
       }
    }
}
