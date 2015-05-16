using System;
using System.Configuration;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Script;
using Distributr.Core.Resources.Util;
using Distributr.DatabaseSetup;
using Distributr.WSAPI.Lib.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using StructureMap;

namespace RebuildDB
{
    class Program
    {
        static void Main(string[] args)
        {
            new ResetDB().Run();
        }
    }

    public class ResetDB
    {
        [Test]
        public void Run()
        {
            string conn = ConfigurationManager.AppSettings["directconnectionstring"];
            string scriptlocation = ConfigurationManager.AppSettings["createtablesscriptlocation"];
            //Console.Write("Connection string: {0}",conn);
            //Console.ReadKey();

            RebuildDb.dropalltables(conn);
            RebuildDb.RecreateTables(conn, scriptlocation);

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<InsertDataRegistry>();
                x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("win"));
            });
            DistributrDataHelper.Migrate();

            //InsertTestData td = ObjectFactory.GetInstance<InsertTestData>();
            IInsertTestData td = ObjectFactory.GetInstance<IInsertTestData>();
            td.InsertTestMasterData();
        }
        ////[Test]
        public void SerializeDateTimeOffset()
        {
            IsoDateTimeConverter converter = new IsoDateTimeConverter();

            DateTimeOffset d = new DateTimeOffset(2000, 12, 15, 22, 11, 3, 55, TimeSpan.Zero);
            string result;

            result = JsonConvert.SerializeObject(DateTime.Now, converter);
           
            DateTimeOffset xx = JsonConvert.DeserializeObject<DateTimeOffset>(result, converter);
            DateTime datetime = xx.DateTime;
            Assert.AreEqual(d, xx);
        }

        [Test] 
        public void ResetLocalDbs()
        {
            string connLocal = @"data source=(local);initial catalog=distributrlocal;uid=sa;pwd=P@ssw0rd;";
            string scriptlocation = ConfigurationManager.AppSettings["createtablesscriptlocation"];
            RebuildDb.dropalltables(connLocal);
            RebuildDb.RecreateTables(connLocal, scriptlocation);

            RebuildDb.DropLocaSetupDb(connLocal);
        }
    }
}
