using System;
using System.IO;
using System.Text;

using Android.App;
using Android.Content;
using Distributr.Mobile.Login;
using Distributr.Mobile.Data;
using Distributr.Mobile.Sync.Incoming;
using Java.Util.Zip;
using Mobile.Common.Core;

namespace Distributr.Mobile.Core.Sync
{
    [Service]
    public class MasterDataDownloadService : BaseFileDownloadService<User>
    {
        private Database database;

        public override void Created()
        {
            database = Resolve<Database>();
        }

        public override void OnStatusUpdate(DownloadStatusUpdate status)
        {
            Console.WriteLine(status.Message);
            if (status.Failed)
            {
                Publish(new SyncFailedEvent(status.Message));
            }
            else if (status.Finished)
            {
                UpdateDatabase(status);
            }
            else
            {
                Publish(new SyncUpdateEvent(status.Progress));
            }
        }

        private void UpdateDatabase(DownloadStatusUpdate status)
        {
            try
            {
                database.BeginTransaction();
                ApplyDownload();
            }
            catch (Exception e)
            {
                Publish(new SyncFailedEvent(exception: e));
                database.Rollback();
            }
            finally
            {
                database.Commit();
            }
        }

        protected override void OnHandleIntent(Intent intent)
        {
            //Download("Master Data Download", "http://mirror.catn.com/pub/apache/tomcat/tomcat-8/v8.0.20/bin/apache-tomcat-8.0.20.zip");
            ApplyDownload();
        }

        private void ApplyDownload()
        {
            Publish(new SyncUpdateEvent("Applying updates.."));
            using (ZipInputStream zipInputStream = new ZipInputStream(Assets.Open("masterdata.zip")))
            {
                ZipEntry zipEntry;
                while ((zipEntry = zipInputStream.NextEntry) != null)
                {
                    string fileName = Path.GetFileName(zipEntry.Name);

                    if (fileName != String.Empty)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var size = 4096;
                            var data = new byte[size];
                            while (true)
                            {
                                size = zipInputStream.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    stream.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            var contents = Encoding.Default.GetString(stream.ToArray());
                            ProcessFile(fileName, contents);
                        }
                    }
                }
                Publish(new SyncCompletedEvent());
            }
        }

        private void ProcessFile(string fileName, string contents)
        {
            string tableName = fileName.Substring(0, fileName.LastIndexOf('_')).Replace("_", "");

            var records = contents.Trim().Split(new string[] {"\r\n", "\n"}, options: StringSplitOptions.None);
            if (records.Length > 1)
            {
                var header = records[0];
                for (var i = 1; i < records.Length; i++)
                {
                    Console.WriteLine("Inserting " + i);
                    InsertRecord(tableName, header, records[i]);
                }
            }
        }

        private void InsertRecord(string tableName, string header, string record)
        {
            //TODO switch between insert or inset and replace depending on context
            StringBuilder builder = new StringBuilder("INSERT OR REPLACE INTO ")
                .Append(tableName)
                .Append("(")
                .Append(header)
                .Append(")")
                .Append(" VALUES")
                .Append("(")
                .Append(record)
                .Append(");");
            Console.WriteLine(builder.ToString());
            database.Execute(builder.ToString());
        }
    }
}