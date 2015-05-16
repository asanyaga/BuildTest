using Distributr_Middleware.WPF.Lib.IOC;
using SAPUtilityLib.Masterdata;
using SAPUtilityLib.Masterdata.Impl;
using StructureMap;

namespace SAPUtilityLib.IoC
{
    public class BootStrapper
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {
                
                
                Initializer.Init();
                ObjectFactory.Configure(x => x.AddRegistry<SApRegistry>());
                _isInitialized = true;
            }
        }
    }
}
