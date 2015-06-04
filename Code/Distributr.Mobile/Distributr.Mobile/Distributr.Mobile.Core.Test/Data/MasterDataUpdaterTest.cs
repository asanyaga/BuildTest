using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Test.Support;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Data
{
    [TestFixture]
    [Category("Database")]
    public class MasterDataUpdaterTest : WithEmptyDatabaseTest
    {
        [SetUp]
        public override void SetupDatabase()
        {
            base.SetupDatabase();
        }

        [TearDown]
        public override void DeleteDatabase()
        {
            base.DeleteDatabase();
        }


        public const string FullMasterUpdatePath = @"../../Data/MasterDataFiles/Full/";
        private const string PartialMasterUpdatePath = @"../../Data/MasterDataFiles/Partial/";

        //This test can be used to verify MasterData Sample Data provided by the Server devs. It checks
        // 1) That most of the MasterDataUpdater Code works
        // 2) That we have the same number of local masterdata tables as CSV files
        // 3) The CSV file is correct and can be turned into valid SQL
        //
        // It is written in a generic way so that you can replace MasterData/*.csv with a new batch 
        // and then run this test to verify things are OK. 
        [Test]
        public void CanApplyLocalCopyOfFullMasterDataUpdate()
        {
            //Given 
            var zipStreamProcesser = new FakeZipStreamProcesser(FullMasterUpdatePath);
            var masterDataUpdater = new MasterDataUpdater(Database, zipStreamProcesser);
            
            //When
            var result = masterDataUpdater.ApplyUpdate(false, default(Stream));
            
            //Then
            CheckResult(result);
            
            var actualTableNames = new List<string>();
            var emptyTables = new List<string>();

            foreach (var entity in DatabaseConfig.GetMasteDataTypes())
            {                
                var tableName = Database.GetTableName(entity);              
                actualTableNames.Add(tableName);

                var count = Database.Count(entity);
                if (count == 0) emptyTables.Add(tableName);
            }

            var fail = CheckEmpty("Local Tables that have no rows:\n{0}", emptyTables);

            var difference = actualTableNames.Except(zipStreamProcesser.TableNames);
            fail +=  CheckEmpty("Local Tables that have no Master Data CSV File:\n{0}", difference);

            difference = zipStreamProcesser.TableNames.Except(actualTableNames);
            fail += CheckEmpty("Master Data CSV Files without Local Tables:\n {0}", difference);

            Assert.IsTrue(fail == 0, "\n\nOne or more errors occurred");
        }

        [Test]
        public void CanApplyLocalCopyOfPartialUpdateToExistingRecord()
        {
            //Given 
            var zipStreamProcesser = new FakeZipStreamProcesser(PartialMasterUpdatePath);
            var masterDataUpdater = new MasterDataUpdater(Database, zipStreamProcesser);
            var assetType = new AssetType()
            {
                Id = new Guid("202fc04f-40be-4297-96ff-a375630c805c"),
                Name = "Current Name"
            };
            
            Database.Update(assetType);

            //When
            var result = masterDataUpdater.ApplyUpdate(true, default(Stream));

            //Then
            CheckResult(result);

            var count = Database.Count(typeof(AssetType));

            Assert.AreEqual(1, count, "should only have one record for Asset Type");

            var assetTypeFromDb = Database.Table<AssetType>().FirstOrDefault();

            Assert.AreEqual(assetType.Id, assetTypeFromDb.Id, "Guids do not match");

            //COOLER TYPE is present in the record in MasterDataFiles/Partial/AssetType_0.csv
            Assert.AreEqual("COOLER TYPE", assetTypeFromDb.Name, "Name has not been updated");
        }

        [Test]
        public void ConvertsFileNameIntoCorrectTableName()
        {
            Assert.AreEqual("MyCsvFile", MasterDataUpdater.ConvertToTableName("MyCsv_File_9"), "check 1");
            Assert.AreEqual("MyCsvFile", MasterDataUpdater.ConvertToTableName("MyCsvFile_07"), "check 2");
        }

        private int CheckEmpty(string message, IEnumerable<string> items)
        {
            if (items.Any())
            {
                var empty = string.Join(",", items);
                Console.WriteLine(message, empty);
                return 1;
            }
            return 0;
        }
    }

    //Serves up MasterData files that are already unzipped. There doesn't appear to any unzipping APIs that work across all project types. 
    public class FakeZipStreamProcesser : IZipStreamProcessor
    {
        private readonly string path;

        public FakeZipStreamProcesser(string path)
        {
            this.path = path;
        }

        public List<string> TableNames = new List<string>();

        public IEnumerable<Tuple<string, string>> ProcessZipStream(Stream zipStream)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var content = File.ReadAllText(file);
                var name = file.Substring(file.LastIndexOf("/", StringComparison.Ordinal) + 1);
                
                TableNames.Add(MasterDataUpdater.ConvertToTableName(name));
                
                yield return Tuple.Create(name, content);
            }
        }
    }
}
