using Distributr.Mobile.Data;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Data
{
    public class DataModule : Registry
    {
        public DataModule ()
        {
            For<MasterDataUpdater>().Use<MasterDataUpdater>();
            For<Database>().Singleton().Use<Database>();
        }
    }
}
