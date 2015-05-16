using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.IOC;
using Distributr.WPF.Lib.Data.IOC;
using GalaSoft.MvvmLight.Threading;
using StructureMap;

namespace Integration.Cussons.WPF.Lib.IOC
{
   public class PzInitializer
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
                    
                });
                DispatcherHelper.Initialize();
                _isInitialized = true;
            }
        }
    }
}
