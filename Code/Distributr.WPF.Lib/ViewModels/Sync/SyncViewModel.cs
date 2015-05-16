using System;
using System.Linq;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using GalaSoft.MvvmLight.Command;
using log4net;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Sync
{
    public class SyncViewModel : DistributrViewModelBase
    {
        private ILog _logger = LogManager.GetLogger("SyncViewModel");
        public SyncViewModel()
        {
            RunMasterDataSync = new RelayCommand(BeginMasterDataSync);
            ListUploadCommandInfo = new RelayCommand(BeginListUploadCommandInfo);
            RunDownloadCommandSync = new RelayCommand(RestartAutoSync);
        }
        public void ReceiveMessage(string msg)
        {
            SyncStatusInfo += "\n" + msg;

        }
        private void RestartAutoSync()
        {
            SyncStatusInfo = "";
            var _autoSyncService = ObjectFactory.GetInstance<IAutoSyncService>();
            _autoSyncService.RestartAutomaticSync();

        }

        public virtual void MasterDataSyncCompleted()
        {

        }

        public void BeginMasterDataSync()
        {
            SyncStatusInfo = "Starting Master Data Sync";
           // UpdateInventory();
            UpdateMasterData();
        }

        public void BeginInventorySync()
        {
            SyncStatusInfo += "\n\nStarting Inventory Sync";
            UpdateInventory();
        }

        public void BeginPaymentSync()
        {
            SyncStatusInfo += "\n\nStarting Payment Sync";
            UpdatePayment();
        }

        private async void UpdateInventory()
        {
            using (IContainer c = NestedContainer)
            {
               
                ISyncService syncService = c.GetInstance<ISyncService>();
                var progress = new Progress<string>(ReportProgress);
                bool inventorySuccess = await syncService.UpdateInventory(progress);
                if (inventorySuccess)
                {
                    SyncStatusInfo += "\n " + "==>  Successful update of inventory";
                    InventorySyncCompleted();
                }
                else
                { 
                    SyncStatusInfo += "\n " + "==>  Failed to update inventory";
                    InventorySyncCompleted();
                }

                
            }
        }

        private async void UpdatePayment()
        {
            using (IContainer c = NestedContainer)
            {
                ISyncService syncService = c.GetInstance<ISyncService>();
                var progress = new Progress<string>(ReportProgress);
                bool paymentSuccess = await syncService.UpdatePayments(progress);

                if (paymentSuccess)
                {
                    SyncStatusInfo += "\n " + "==>  Successful update of payments";
                    PaymentSyncCompleted();
                }
                else
                    SyncStatusInfo += "\n " + "==>  Failed to update payments";

                PaymentSyncCompleted();
            }
        }

        public virtual void InventorySyncCompleted()
        {

        }

        public virtual void PaymentSyncCompleted()
        {

        }

        public void BeginListUploadCommandInfo()
        {
            using (IContainer c = NestedContainer)
            {
                SyncStatusInfo = "Show Upload info";
                var itemsToGo = Using<IOutgoingCommandEnvelopeQueueRepository>(c).GetUnSentEnvelope();
                SyncStatusInfo += "\n There are " + itemsToGo.Count() + " items to send";
            }
        }

        public void ReportProgress(string progress)
        {
            SyncStatusInfo += progress;
        }

        public async void UpdateMasterData()
        {

            _syncEnabled = false;
            Guid appId;
            bool needTo = true;
            _logger.Debug("start creating=> UpdateMasterDataService ");
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                if (!needTo)
                {
                    SyncStatusInfo += "\n Masterdata sync not required Data is updated" + " : ";
                }
                else
                {
                    var progress = new Progress<string>(ReportProgress);
                    ISyncService syncService = c.GetInstance<ISyncService>();
                    bool result = await syncService.UpdateMasterData(progress);
                    MasterDataSyncCompleted();
                }
            }
        }

        public virtual void DownloadCommandSyncCompleted()
        {

            SyncEnabled = true;
        }

        public RelayCommand RunMasterDataSync { get; set; }
        public RelayCommand RunUploadCommandSync { get; set; }
        public RelayCommand ListUploadCommandInfo { get; set; }
        public RelayCommand RunDownloadCommandSync { get; set; }

        public const string SyncStatusInfoPropertyName = "SyncStatusInfo";
        private string _syncStatusInfo = "";
        public string SyncStatusInfo
        {
            get
            {
                return _syncStatusInfo;
            }

            set
            {
                if (_syncStatusInfo == value)
                {
                    return;
                }

                var oldValue = _syncStatusInfo;
                _syncStatusInfo = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(SyncStatusInfoPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SyncEnabled" /> property's name.
        /// </summary>
        public const string SyncEnabledPropertyName = "SyncEnabled";
        private bool _syncEnabled = true;
        public bool SyncEnabled
        {
            get
            {
                return _syncEnabled;
            }

            set
            {
                if (_syncEnabled == value)
                    return;
                var oldValue = _syncEnabled;
                _syncEnabled = value;
                RaisePropertyChanged(SyncEnabledPropertyName);
            }
        }


    }
}
