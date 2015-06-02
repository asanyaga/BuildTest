using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Login;
using Distributr.Mobile.Data;
using Distributr.Mobile.Net;
using Distributr.Mobile.Util;
using Mobile.Common.Core;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using StructureMap.Configuration.DSL;


namespace Distributr.Mobile
{
    public class ApplicationModule : Registry
    {
        public ApplicationModule(BaseApplication<User> app)
        {
            For<BaseApplication<User>>().Use(app);
            For<IFileSystem>().Use(new AndroidFileSystem());
            For<ISQLitePlatform>().Use(new SQLitePlatformAndroid());
            For<IConnectivityMonitor>().Use<ConnectivityMonitor>();
            For<IZipStreamProcessor>().Use<ZipStreamProcessorWrapper>();
        }
    }
}