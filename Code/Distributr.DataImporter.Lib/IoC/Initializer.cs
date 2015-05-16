using Distributr.Core.Data.IOC;
using Distributr.WPF.Lib.Data.IOC;
using StructureMap;

namespace Distributr.DataImporter.Lib.IoC
{
    public class Initializer
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {
               
               
                ObjectFactory.Initialize(x =>
                {
                    x.AddRegistry<DataRegistry>();
                    x.AddRegistry<ImporterRegistry>();
                    x.AddRegistry<ServiceRegistry>();
                    x.AddRegistry<WPFRegistry>();
                    x.AddRegistry<RepositoryRegistry>();
                    x.AddRegistry<CommandHandlerRegistry>();
                    x.AddRegistry<WorkflowRegistry>();
                  
                });
                _isInitialized = true;
            }
        }

    }
}
