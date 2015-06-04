using System;
using System.IO;
using System.Net;
using Android.App;
using Android.Content;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Login;
using Distributr.Mobile.Login.Settings;
using Mobile.Common.Core;

namespace Distributr.Mobile.Sync.Incoming
{
    [Service]
    public class MasterDataDownloadService : BaseFileDownloadService<User>
    {
        private MasterDataUpdater masterDataUpdater;
        public const string MasterDataDownloadEndpoint = "downloadmasterdata/GetZipped/{0}";
        private string fileName;

        public override void Created()
        {
            masterDataUpdater = Resolve<MasterDataUpdater>();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            var settings = Resolve<LoginSettingsRepository>().GetSettings();
            var url = Path.Combine(settings.ServerUrl, string.Format(MasterDataDownloadEndpoint, User.CostCentreApplicationId));

            Publish(new SyncUpdateEvent<MasterDataUpdate>("Downloading..."));

            if (UrlIsValid(url))
            {
                Download("Reference Data Download", url);
            }
            else
            {
                var message = "Unable to complete Reference Data download";
                var exception = new WebException(string.Format("URL invalid or server unavailable: {0}", url));
                Publish(new SyncFailedEvent<MasterDataUpdate>(message:message, exception:exception));
            }
        }

        protected override string CreateLocalFileName()
        {
            fileName = string.Format("masterdata_{0}_{1}.zip", User.CostCentreApplicationId, DateTime.Now.Ticks);
            return fileName;
        }

        public override void OnStatusUpdate(DownloadStatusUpdate status)
        {
            if (status.Failed)
            {
                Publish(new SyncFailedEvent<MasterDataUpdate>(status.Message));
            }
            else if (status.Finished)
            {
                Publish(new SyncUpdateEvent<MasterDataUpdate>("Updating database"));
                UpdateDatabase(Path.Combine(GetExternalFilesDir(null).Path, fileName));
            }
            else if (status.Paused)
            {
                Publish(new SyncPausedEvent<MasterDataUpdate>(status.Message));
            } else
            {
                Publish(new SyncUpdateEvent<MasterDataUpdate>(status.Progress));
            }
        }

        private void UpdateDatabase(string filePath)
        {
            var result = masterDataUpdater.ApplyUpdate(!User.IsNewUser, new FileStream(filePath, FileMode.Open));

            Resolve<SyncLogRepository>().UpdateLastSyncTime(typeof (MasterDataUpdate));

            if (result.WasSuccessful())
            {
                Publish(new SyncCompletedEvent<MasterDataUpdate>());
                File.Delete(filePath);
            }
            else
            {
                Publish(new SyncFailedEvent<MasterDataUpdate>(result.Message, result.Exception));
            }            
        }

        //Validate the URL before Calling Android's Download Manager. 
        public bool UrlIsValid(string url)
        {
            try
            {
                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "HEAD";

                var response = request.GetResponse() as HttpWebResponse;

                int statusCode = (int) response.StatusCode;
                if (statusCode >= 100 && statusCode < 406) //Good requests including 405 - method now allowed
                {
                    return true;
                }
                else if (statusCode >= 500 && statusCode <= 510) //Server Errors
                {
                    Console.WriteLine("The remote server has thrown an internal error. Url is not valid: {0}", url);
                    return false;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) 
                {
                    //The web service exists but it doesn't support the method we used (head)
                    return true;
                }
                Console.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}