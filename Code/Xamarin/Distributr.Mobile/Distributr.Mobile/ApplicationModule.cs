using System;
using System.IO;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Login;
using Distributr.Mobile.Data;
using Mobile.Common.Core;
using Ninject.Modules;
using SQLite.Net.Platform.XamarinAndroid;

namespace Distributr.Mobile
{
    public class CoreModule : NinjectModule
    {
        private readonly BaseApplication<User> app;

        public CoreModule(BaseApplication<User> app)
        {
            this.app = app;
        }

        public override void Load()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Bind<BaseApplication<User>>().ToConstant(app);
            Bind<Database>().ToConstant(new Database(new SQLitePlatformAndroid(), Path.Combine(path, "masterdata.db")));
            Bind<IConnectivityMonitor>().To<ConnectivityMonitor>();
        }
    }
}