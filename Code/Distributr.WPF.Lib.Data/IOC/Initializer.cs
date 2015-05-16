using Distributr.Core.Utility.Mapping.impl;
using GalaSoft.MvvmLight.Threading;
using StructureMap;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class Initializer
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                DispatcherHelper.Initialize();
                DTOToEntityMapping.SetupAutomapperMappings();
                MasterDataToDTOMapping.SetupMapper();
                ObjectFactory.Initialize(x =>
                    {
                        x.AddRegistry<WPFRegistry>();
                        x.AddRegistry<RepositoryRegistry>();
                        x.AddRegistry<ServiceRegistry>();
                        x.AddRegistry<CommandHandlerRegistry>();
                        x.AddRegistry<WorkflowRegistry>();
                    });
            }
        }

     
    }
}
