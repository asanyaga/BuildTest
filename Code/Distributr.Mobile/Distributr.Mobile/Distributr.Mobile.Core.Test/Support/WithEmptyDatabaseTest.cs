using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Data;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Support
{
    //Provides support for sub-classes to clear a database after tests
    public class WithEmptyDatabaseTest : WithDependenciesTest
    {
        protected Database Database;
        protected IFileSystem FileSystem;

        [SetUp]
        public virtual void SetupDatabase()
        {
            Database = Resolve<Database>();
            FileSystem = Resolve<IFileSystem>();
        }
       
        [TearDown]
        public virtual void DeleteDatabase()
        {
            Database.ClearTables();
        }
    }
}