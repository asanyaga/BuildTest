using System;
using System.IO;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Util;
using NUnit.Framework;
using SQLite.Net.Interop;
using SQLite.Net.Platform.Win32;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Test.Support
{
    //Provides the same dependencies as Distributr.Mobile.Core, plus some enviroment specific-stuff
    public class WithDependenciesTest
    {
        private DependencyContainer container;

        [TestFixtureSetUp]
        public void CreateDependencies()
        {           
            container = new DependencyContainerBuilder()
                .AddModule(new TestModule())
                .Build();
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public void CheckResult(Result<object> result)
        {
            if (!result.WasSuccessful())
            {
                Console.WriteLine("{0}, {1}", result.Message, result.Exception);
                throw result.Exception;
            }
        }

    }

    public class TestModule : Registry
    {
        public TestModule()
        {
            For<ISQLitePlatform>().Use<SQLitePlatformWin32>();
            For<IFileSystem>().Use(new WindowsFileSystem());
            For<IConnectivityMonitor>().Use(new AlwaysOnConnectivityMonitor());
        }
    }

    public class WindowsFileSystem : IFileSystem
    {
        public string GetDatabasePath()
        {
            return Path.Combine( GetRootStorageFolder(), "distributr_test.db");
        }

        public string GetRootStorageFolder()
        {
            var path = "TestData";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public class AlwaysOnConnectivityMonitor : IConnectivityMonitor
    {
        public bool IsNetworkAvailable()
        {
            return true;
        }

        public void WaitForNetwork()
        {
            //Never wait
        }
    }
}
