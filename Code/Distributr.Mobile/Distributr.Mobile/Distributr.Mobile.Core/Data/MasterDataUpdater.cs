using System;
using System.IO;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Data
{
    public enum MasterDataUpdate
    {
        //Just used as a generic type parameter on Sync*Event classes to save duplicating events
    }

    public class MasterDataUpdater
    {
        private readonly Database database;
        private readonly IZipStreamProcessor zipStreamProcessor;

        public MasterDataUpdater(Database database, IZipStreamProcessor zipStreamProcessor)
        {
            this.database = database;
            this.zipStreamProcessor = zipStreamProcessor;
        }

        public Result<object> ApplyUpdate(bool isUpdate, Stream fileStream)
        {
            return new Transactor(database)
                .Transact(
                    () => ProcessZipStream(isUpdate, fileStream)
                );
        }

        // Process the ZipSteam. This Stream contains one or more files which 
        // are then processed separately. 
        private void ProcessZipStream(bool isUpdate, Stream fileStream)
        {

            foreach (var file in zipStreamProcessor.ProcessZipStream(fileStream))
            {
                ProcessFile(file.Item1, file.Item2, isUpdate);
            }            
        }

        private void ProcessFile(string fileName, string contents, bool isUpdate)
        {
            var tableName = ConvertToTableName(fileName);
            var records = SplitFileIntoRecords(contents);

            if (records.Length > 1)
            {
                var header = records[0];
                for (var i = 1; i < records.Length; i++)
                {
                    InsertRecord(tableName, header, records[i], isUpdate);
                }
            }
        }

        //Convert 'table_name_0' to 'tablename'
        public static string ConvertToTableName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('_')).Replace("_", "");
        }

        //Split file into lines
        private string [] SplitFileIntoRecords(string contents)
        {
            return contents.Trim().Split(new string[] { "\r\n", "\n" }, 
                options: StringSplitOptions.None);
        }

        private void InsertRecord(string tableName, string header, string record, bool isUpdate)
        {
            var orReplace = isUpdate ? " OR REPLACE " : "";
            var insertStatement = string.Format("INSERT {0} INTO {1} ({2}) VALUES ({3});", orReplace, tableName, header, record);
            //Console.WriteLine(insertStatement);
            database.Execute(insertStatement);
        }
    }


}
