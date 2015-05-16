using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Distributr.Core;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility.Caching;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using StructureMap;
using log4net;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class AutoSyncService : IAutoSyncService
    {
        private ILog _logger = LogManager.GetLogger("AutoSyncService");
        private DispatcherTimer dtDownloadCommand;
        private DispatcherTimer dtUploadCommand;
        private DispatcherTimer dtMasterDataDownload;
        public bool ShowMessageBox { get; set; }

        public AutoSyncService()
        {
            SetUpTimers();
            ShowMessageBox = true;
        }

        private void SetUpTimers()
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Intializing Auto sync");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Intializing Auto sync");
            // download  command 
            dtDownloadCommand = new DispatcherTimer();
            dtDownloadCommand.Interval = new TimeSpan(0, 0, 30); // 500 Milliseconds
            dtDownloadCommand.Tick += new EventHandler(dtDownloadCommand_Tick);
            // dtUploadCommand.Stop();



            dtUploadCommand = new DispatcherTimer();
            dtUploadCommand.Interval = new TimeSpan(0, 1, 0);
            dtUploadCommand.Tick += new EventHandler(dtUploadCommand_Tick);
            //dtUploadCommand.Stop();

            //Master data Download

            dtMasterDataDownload = new DispatcherTimer();
            dtMasterDataDownload.Interval = new TimeSpan(0, 1, 0);
            dtMasterDataDownload.Tick += new EventHandler(dtMasterDataDownload_Tick);
            //dtMasterDataDownload.Stop();


        }

        void dtMasterDataDownload_Tick(object sender, EventArgs e)
        {

            DispatcherTimer dt = sender as DispatcherTimer;
            dt.Stop();
            if (CanSync())
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Master data sync started");
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Master data sync started");
                BeginMasterDataSync();
            }
            else
            {
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
                dt.Start();
            }

        }

        void dtUploadCommand_Tick(object sender, EventArgs e)
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction started");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction started");
            DispatcherTimer dt = sender as DispatcherTimer;
            dt.Stop();

            if (CanSync())
                BeginUploadCommandSync();
            else
            {
                dt.Start();
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
            }


        }

        void dtDownloadCommand_Tick(object sender, EventArgs e)
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Downloading of transaction started");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Downloading of transaction started");
            DispatcherTimer dt = sender as DispatcherTimer;
            dt.Stop();
            if (CanSync())
                BeginBatchDownloadCommandSync();
            else
            {
                dt.Start();
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Application not set to sync");
            }

        }

        bool CanSync()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                return c.GetInstance<ISyncService>().CanSync();
            }
        }

        public void MasterDataSyncCompleted()
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Master data sync stopped");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "Master data sync stopped");
            //dtMasterDataDownload.Interval = new TimeSpan(5, 0, 0);
            //dtMasterDataDownload.Start();
            dtDownloadCommand.Interval = new TimeSpan(0, 0, 30);
            dtDownloadCommand.Start();
        }

        public void DownloadCommandSyncCompleted()
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Downloading of transaction stopped");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "Downloading of transaction stopped");
            dtUploadCommand.Interval = new TimeSpan(0, 0, 30);
            dtUploadCommand.Start();
        }

        public void SetShowMessageBox(bool canShow)
        {
            ShowMessageBox = canShow;
        }

        public void UploadCommandSyncCompleted()
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction stopped ");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction  stopped ");
            dtDownloadCommand.Interval = new TimeSpan(0, 0, 30);
            dtDownloadCommand.Start();
        }

        void StartTimers()
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Starting Auto sync ");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Starting Auto sync  ");
            //dtDownloadCommand.Start();
            //dtUploadCommand.Start();
            dtMasterDataDownload.Start();
        }

        void StopTimers()
        {

            dtDownloadCommand.Stop();
            dtUploadCommand.Stop();
            dtMasterDataDownload.Stop();
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sync service stopped ");
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "  Sync service stopped ");
        }

        public void StartAutomaticSync()
        {

            StartTimers();
        }

        public void StopAutomaticSync()
        {
            StopTimers();
        }

        public void RestartAutomaticSync()
        {

            SetUpTimers();
            StartTimers();
            _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Restart sync completed ");
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Restart sync completed");
        }

        public void BeginMasterDataSync()
        {

            Config config;
            ISyncService syncService;
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                config = c.GetInstance<IConfigService>().Load();
                syncService = c.GetInstance<ISyncService>();
            }
            if (config != null && config.ApplicationStatus == 1)
                UpdateMasterData();
            else
            {
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + " Master data sync Cancelled ,Check  you Configuration setting ");
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Master data sync Cancelled ,Check  you Configuration setting ");
                StopTimers();
            }

        }

        public void BeginUploadCommandSync()
        {
            Config config;
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                config = c.GetInstance<IConfigService>().Load();
            }
            if (config != null && config.ApplicationStatus == 1)
                BeginUploadCommandEnvelope();
            else
            {
                _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "Sending of transaction Cancelled : ");
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction Cancelled : ");
                StopTimers();
            }
        }
        [Obsolete("Command Envelope Refactoring")]
        private async void BeginUploadCommand()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                try
                {
                    ISendPendingLocalCommandsService sendPendingLocalCommandsService =
                        c.GetInstance<ISendPendingLocalCommandsService>();
                    IConfigService configService = c.GetInstance<IConfigService>();
                    Guid appId = configService.Load().CostCentreApplicationId;
                    int noofcmdsent = await sendPendingLocalCommandsService.SendPendingCommandsAsync(200);
                    string msg = "( " + noofcmdsent + ") sent";
                    if (noofcmdsent == -1)
                    {
                        msg = "(0)  sent, check application webservice";
                    }

                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction : " + msg);

                    _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "Sending of transaction : " + msg);

                    int noofNotification = await sendPendingLocalCommandsService.SendPendingNotificationAsync();
                }

                catch (Exception e)
                {
                    string error = GetError(e);
                    _logger.Error(e.Message + error);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending  failed " + error);
                    //throw new AutoSyncException(
                    //    "A problem occurred while uploading .\nDetails:\n\t" + e.Message);
                }

            }
            UploadCommandSyncCompleted();
        }
        private async void BeginUploadCommandEnvelope()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                try
                {
                    ISyncService syncService = c.GetInstance<ISyncService>();

                    int noofcmdsent = await syncService.UploadCommandEnvelopesSync();
                    string msg = "( " + noofcmdsent + ") sent";
                    if (noofcmdsent == -1)
                    {
                        msg = "(0)  sent, check application webservice";
                    }
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending of transaction : " + msg);
                    _logger.Info(DateTime.Now.ToString("hh:mm:ss") + "Sending of transaction : " + msg);
                }

                catch (Exception e)
                {
                    string error = GetError(e);
                    _logger.Error(e.Message + error);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Sending  failed " + error);
                }

            }
            UploadCommandSyncCompleted();
        }

        public void ReportProgress(string progress)
        {
            //log??
        }

        public async void UpdateMasterData()
        {

            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                try
                {
                    var progress = new Progress<string>(ReportProgress);
                    bool result = await c.GetInstance<ISyncService>().UpdateMasterData(progress);
                }

                catch (Exception e)
                {
                    string error = GetError(e);
                    _logger.Error(e.Message + error);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Master data sync  failed " + error);
                    //throw new AutoSyncException(
                    //    "A problem occurred while updating master data.\nDetails:\n\t" + e.Message);
                }
            }
            MasterDataSyncCompleted();
        }

        public async void BeginBatchDownloadCommandSync()
        {
            bool iscontinue = false;
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                try
                {
                    iscontinue = await c.GetInstance<ISyncService>().DownloadCommandEnvelopesSync();
                }

                catch (Exception e)
                {
                    string error = GetError(e);
                    _logger.Error(e.Message + error);
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Downloading  failed " + error);
                }
            }

            DownloadCommandSyncCompleted();
        }

        private static string GetError(Exception he)
        {
            string error = he.Message;
            string inner1 = he.InnerException != null ? he.InnerException.Message : "";
            string inner2 = he.InnerException != null && he.InnerException.InnerException != null ? he.InnerException.InnerException.Message : "";
            error += "\n\t" + inner1 + "\n\t " + inner2;
            return error;
        }
    }
}
