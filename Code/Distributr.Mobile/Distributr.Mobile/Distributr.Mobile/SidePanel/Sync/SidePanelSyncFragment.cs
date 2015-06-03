using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Distributr.Mobile.Core;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Distributr.Mobile.Sync;
using Distributr.Mobile.Sync.Incoming;
using Distributr.Mobile.Sync.Outgoing;
using Mobile.Common.Core;

namespace Distributr.Mobile.SidePanel.Sync
{
    public class SidePanelSyncFragment : NestedFragment<User>
    {
        private SyncLogRepository syncLogRepository;
        private SyncWidget inboundTransactions;
        private SyncWidget inboundMasterData;
        private SyncWidget outboundTransactions;
        private new const string Paused = "Waiting for network";

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            var app = (DistributrApplication) App;
            syncLogRepository = Resolve<SyncLogRepository>();

            inboundTransactions = parent.FindViewById<SyncWidget>(Resource.Id.inbound_transactions_sync);
            inboundTransactions.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof (DownloadEnvelopeRequest)));
            inboundTransactions.OnSyncNow(delegate
            {
                Activity.StartService(new Intent(Activity, typeof(CommandEnvelopeDownloadService)));
            });

            if (app.IsSyncPaused(typeof (DownloadEnvelopeRequest)))
            {
                inboundTransactions.SetPaused();
                inboundTransactions.UpdateStatusMessage(Paused);
            }

            inboundMasterData = parent.FindViewById<SyncWidget>(Resource.Id.inbound_masterdata_sync);
            inboundMasterData.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof (MasterDataUpdate)));
            inboundMasterData.SetTitle("Reference Data");
            inboundMasterData.OnSyncNow(delegate
            {
                Activity.StartService(new Intent(Activity, typeof(MasterDataDownloadService)));
            });

            if (app.IsSyncPaused(typeof(MasterDataUpdate)))
            {
                inboundMasterData.SetPaused();
                inboundMasterData.UpdateStatusMessage(Paused);
            }

            outboundTransactions = parent.FindViewById<SyncWidget>(Resource.Id.outbound_transactions_sync);
            outboundTransactions.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof(LocalCommandEnvelope)));
            outboundTransactions.OnSyncNow(delegate
            {
                Activity.StartService(new Intent(Activity, typeof(CommandEnvelopeUploadService)));
            });

            if (app.IsSyncPaused(typeof(LocalCommandEnvelope)))
            {
                outboundTransactions.SetPaused();
                outboundTransactions.UpdateStatusMessage(Paused);
            }

        }

        //Inbound Transactions
        public void OnEvent(SyncUpdateEvent<DownloadEnvelopeRequest> update)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundTransactions.SetActive();
                inboundTransactions.UpdateStatusMessage(update.Message);
            });          
        }

        public void OnEvent(SyncPausedEvent<DownloadEnvelopeRequest> paused)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundTransactions.SetPaused();
                inboundTransactions.UpdateStatusMessage(paused.Message);
            });            
        }

        public void OnEvent(SyncCompletedEvent<DownloadEnvelopeRequest> completed)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundTransactions.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof (DownloadEnvelopeRequest)));
                inboundTransactions.SetInacvtive();                
            });            
        }

        public void OnEvent(SyncFailedEvent<DownloadEnvelopeRequest> failed)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundTransactions.SetError();
                Dialog alert = new ErrorAlertBuilder(Activity).Build(failed.Message, failed.Exception,
                    Resource.Style.AppTheme_AlertDialog);
                alert.Show();
            });                        
        }

        //Master Data
        public void OnEvent(SyncUpdateEvent<MasterDataUpdate> update)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundMasterData.SetActive();
                inboundMasterData.UpdateStatusMessage(update.Message);
            });
        }

        public void OnEvent(SyncPausedEvent<MasterDataUpdate> paused)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundMasterData.SetPaused();
                inboundMasterData.UpdateStatusMessage(paused.Message);
            });
        }

        public void OnEvent(SyncCompletedEvent<MasterDataUpdate> completed)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundMasterData.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof (MasterDataUpdate)));
                inboundMasterData.SetInacvtive();
            });
        }

        public void OnEvent(SyncFailedEvent<MasterDataUpdate> failed)
        {
            Activity.RunOnUiThread(() =>
            {
                inboundMasterData.SetError();
                Dialog alert = new ErrorAlertBuilder(Activity).Build(failed.Message, failed.Exception, 
                    Resource.Style.AppTheme_AlertDialog);
                alert.Show();
            });
        }

        //Outbound Transactions
        public void OnEvent(SyncUpdateEvent<LocalCommandEnvelope> update)
        {
            Activity.RunOnUiThread(() =>
            {
                outboundTransactions.SetActive();
                outboundTransactions.UpdateStatusMessage(update.Message);
            });
        }

        public void OnEvent(SyncPausedEvent<LocalCommandEnvelope> paused)
        {
            Activity.RunOnUiThread(() =>
            {
                outboundTransactions.SetPaused();
                outboundTransactions.UpdateStatusMessage(paused.Message);
            });
        }

        public void OnEvent(SyncCompletedEvent<LocalCommandEnvelope> completed)
        {
            Activity.RunOnUiThread(() =>
            {
                outboundTransactions.SetLastUpdateTime(syncLogRepository.LastSyncTime(typeof(LocalCommandEnvelope)));
                outboundTransactions.SetInacvtive();
            });
        }
    }
}