﻿using System;
using Android.App;
using Java.Lang;
using Exception = System.Exception;
using Uri = Android.Net.Uri;

namespace Mobile.Common.Core
{
    public abstract class BaseFileDownloadService<U> : BaseIntentService<U>
    {
        private DownloadManager downloadManager;
        private long downloadId;
        protected DownloadManager.Request CurrentRequest;

        protected void Download(string title, string path)
        {
            downloadManager = (DownloadManager)GetSystemService(DownloadService);
            Console.WriteLine("Downloading Path {0}", path);
            CurrentRequest = new DownloadManager.Request(Uri.Parse(path));
            CurrentRequest.SetTitle(title);
            CurrentRequest.SetDestinationInExternalFilesDir(this, "", CreateLocalFileName());
            downloadId = downloadManager.Enqueue(CurrentRequest);
            
            MonitorStatus();
        }

        protected abstract string CreateLocalFileName();

        private void MonitorStatus()
        {
            var statusQuery = new DownloadManager.Query();
            statusQuery.SetFilterById(downloadId);
            var status = GetStatus(statusQuery);

            while (!status.Finished)
            {
                Thread.Sleep(500);
                status = GetStatus(statusQuery);
                OnStatusUpdate(status);
            }
        }

        private DownloadStatusUpdate GetStatus(DownloadManager.Query statusQuery)
        {
            var cursor = downloadManager.InvokeQuery(statusQuery);
            if (!cursor.MoveToNext())
            {
                return new DownloadStatusUpdate("In progress", 0);
            }

            var status = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));
            var reason = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnReason));
            var bytesDownloaded = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnBytesDownloadedSoFar));
            var totalBytes = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnTotalSizeBytes));
            var percentDone = (double) bytesDownloaded/(double) totalBytes;

            var downloadStatus = (DownloadStatus) status;

            switch (downloadStatus)
            {
                case DownloadStatus.Failed:
                    return CreateFailureUpdate(reason);
                case DownloadStatus.Pending:
                case DownloadStatus.Running:
                    return new DownloadStatusUpdate("In progress", percentDone);
                case DownloadStatus.Paused:
                    return new DownloadStatusUpdate("Waiting for network", percentDone, paused:true);
                case DownloadStatus.Successful:
                    return new DownloadStatusUpdate("Download Complete", 100);
                default : throw new Exception("Status not handled " +status );
            }
        }

        private DownloadStatusUpdate CreateFailureUpdate(int reason)
        {
            var reasonText = "Unknown error";

            switch ((DownloadError)reason)
            {
                case DownloadError.CannotResume:
                    reasonText = "Can not resume download. Please try again";
                    break;
                case DownloadError.DeviceNotFound:
                    reasonText = "SD Card unavailable or not mounted";
                    break;
                case DownloadError.FileAlreadyExists:
                    reasonText = "A file with the same name already exists";
                    break;
                case DownloadError.FileError:
                    reasonText = "Unable to store file";
                    break;
                case DownloadError.HttpDataError:
                    reasonText = "Error when communciation with server (HTTP Error)";
                    break;
                case DownloadError.InsufficientSpace:
                    reasonText = "Can not complete download due to insuffienct space";
                    break;
                case DownloadError.TooManyRedirects:
                    reasonText = "Server is incorrectly configured (too many redirects)";
                    break;
                case DownloadError.UnhandledHttpCode:
                    reasonText = "Server responded with an invalid HTTP code";
                    break;
                case DownloadError.Unknown:
                    break;
            }

            Console.WriteLine("Received error code during download {0} - {1}", reason, reasonText);

            return new DownloadStatusUpdate(reasonText, failed: true);
        }

        public abstract void OnStatusUpdate(DownloadStatusUpdate statud);

    }

    public class DownloadStatusUpdate
    {
        public bool Failed { get; private set; }
        public string Message { get; private set; }
        public double Progress { get; private set; }
        public bool Finished { get; private set; }
        public bool Paused { get; private set; }

        public DownloadStatusUpdate(string message, double progress = -1,  bool failed = false, bool paused = false)
        {
            Failed = failed;
            Message = message;
            Progress = progress;
            Finished = Failed || progress == 100;
            Paused = paused;
        }
    }
}