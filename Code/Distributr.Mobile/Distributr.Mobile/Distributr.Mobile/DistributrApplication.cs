using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Login;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Sync;
using Distributr.Mobile.Sync.Incoming;
using Distributr.Mobile.Sync.Outgoing;
using Mobile.Common.Core;

namespace Distributr.Mobile.Core
{
    [Application(Label = "Distributr", Theme = "@style/AppTheme")]
    public class DistributrApplication : BaseApplication<User>
    {
        private readonly DependencyContainer container;
        private readonly Dictionary<Type, ISyncStatusWatcher> syncWatchers = new Dictionary<Type, ISyncStatusWatcher>();
        private bool loginSyncComplete;

        public DistributrApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(typeof(Resource.Layout), typeof(Resource.Menu), javaReference, transfer)
        {
            container = BuildContainer();
            RegisterSyncWatchers();
        }

        private void RegisterSyncWatchers()
        {
            syncWatchers[typeof(MasterDataUpdate)] = SyncStatusWatchers.MasterDataSyncWatcher;
            syncWatchers[typeof(LocalCommandEnvelope)] = SyncStatusWatchers.OutboundTransactionSyncWatcher;
            syncWatchers[typeof(DownloadEnvelopeRequest)] = SyncStatusWatchers.InboundTransactionSyncWatcher;
            foreach (var watcher in syncWatchers.Values)
            {
                Register(watcher);
            }
        }

        public bool IsSyncPaused(Type type)
        {
            return syncWatchers[type].IsPaused;
        }

        private DependencyContainer BuildContainer()
        {
            return new DependencyContainerBuilder()
                .AddModule(new ApplicationModule(this))
                .Build();
        }

        public override void InitialiseFor(User user)
        {
            User = user;
            if (User.IsNewUser)
            {
                Register(this);
                Resolve<Database>().ClearTables();
                StartService(new Intent(this, typeof (MasterDataDownloadService)));
            }
            else
            {
                user.CostCentreApplicationId = Resolve<ILoginRepository>().GetLastUser().CostCentreApplicationId;

                //Send any pending commands
                StartService(new Intent(this, typeof (CommandEnvelopeUploadService)));
            }
        }

        public override bool Initialised()
        {
            return (User != null && !User.IsNewUser) || loginSyncComplete;
        }

        public override void Logout()
        {
            User = null;
            loginSyncComplete = false;
        }

        public void OnEvent(SyncCompletedEvent<MasterDataUpdate> completed)
        {
            Unregister(this);
            loginSyncComplete = true;
            if (User.IsNewUser)
            {
                User.IsNewUser = false;
                User = UpdateLocalUser();
                UpdateLastLoggedInUser();
            }
        }

        private User UpdateLocalUser()
        {
            var user = Resolve<ILoginRepository>().FindUser(User.Username);
            user.CostCentreApplicationId = User.CostCentreApplicationId;
            Resolve<Database>().Update(user);
            return user;
        }

        private void UpdateLastLoggedInUser()
        {
            Resolve<ILoginRepository>().SetLastUser(User.Username, User.CostCentreApplicationId);
        }

        protected override T ResolveDependency<T>()
        {
            return container.Resolve<T>();
        }
    }
}