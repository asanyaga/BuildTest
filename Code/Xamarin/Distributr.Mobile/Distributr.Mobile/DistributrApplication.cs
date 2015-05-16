using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Login;
using Distributr.Mobile.Login.Settings;
using Distributr.Mobile.MakeSale;
using Distributr.Mobile.Data;
using Distributr.Mobile.Routes;
using Distributr.Mobile.Sync.Incoming;
using Mobile.Common.Core;
using Ninject;

namespace Distributr.Mobile.Core
{
    [Application(Label = "Distributr", Theme = "@style/AppTheme")]
    public class DistributrApplication : BaseApplication<User>
    {
        private readonly StandardKernel container;
        private bool loginSyncComplete;

        public DistributrApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(typeof(Resource.Layout), typeof(Resource.Menu), javaReference, transfer)
        {
            container = BuildContainer();
        }

        private StandardKernel BuildContainer()
        {
            return new StandardKernel(
                new CoreModule(this),
                new LoginModule(),
                new LoginSettingsModule(),
                new RoutesModule(),
                new MakeSaleModule());
        }

        public override void InitialiseFor(User user)
        {
            User = user;
            Register(this);

            if (User.IsNewUser)
            {
                Resolve<Database>().ClearTables();
            }

            StartServices();
        }

        public override bool Initialised()
        {
            return (User != null && !User.IsNewUser) || loginSyncComplete;
        }

        public override void Logout()
        {
            User = null;
            loginSyncComplete = false;
            Publish(new SyncCancelledEvent());
        }

        private void StartServices()
        {
            StartService(new Intent(this, typeof(MasterDataDownloadService)));
        }

        public void OnEvent(SyncCompletedEvent completed)
        {
            loginSyncComplete = true;
            if (User.IsNewUser)
            {
                Resolve<ILoginRepository>().SetLastUser(User.Username);
                User.IsNewUser = false;
            }
        }

        protected override T _Resolve<T>()
        {
            return container.Get<T>();
        }
    }
}