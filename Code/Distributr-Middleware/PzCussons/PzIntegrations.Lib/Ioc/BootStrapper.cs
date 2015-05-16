using Distributr.Core.Data.IOC;
using Distributr.WPF.Lib.Data.IOC;
using StructureMap;

namespace PzIntegrations.Lib.Ioc
{
   public class BootStrapper
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {
               // Distributr_Middleware.WPF.Lib.IOC.Initializer.Init();
                ObjectFactory.Initialize(x =>
                {
                    x.AddRegistry<DataRegistry>();
                    x.AddRegistry<ImporterRegistry>();
                    x.AddRegistry<RepositoryRegistry>();

                });
                
                _isInitialized = true;
            }
        }
    }
}
