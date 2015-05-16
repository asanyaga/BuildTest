using System;
using NUnit.Framework;
using RebuildDB;
using StructureMap;
using Distributr.Core.Data.IOC;

namespace Distributr.DatabaseSetup
{
    [TestFixture]
    public class RefreshDatabase 
    {

        [SetUp]
        public void setup()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<InsertDataRegistry>();
            });
        }

        [Test]
        public void RunClearAndInsert()
        {
            //new ClearDB().ClearDatabases();
            var td = ObjectFactory.GetInstance<IInsertTestData>();
            td.InsertTestMasterData();
        }

    }


}
