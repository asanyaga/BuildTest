using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr_Middleware.WPF.Lib.IOC;
using Integration.QuickBooks.Lib.Repository;
using Integration.QuickBooks.Lib.Repository.Impl;
using Integration.QuickBooks.Lib.Services;
using Integration.QuickBooks.Lib.Services.Impl;
using StructureMap;

namespace Integration.QuickBooks.Lib.IOC
{
    public class BootStrapper
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {

                ObjectFactory.Initialize(x => x.For<ITransactionsDownloadService>().Use<TransactionsDownloadService>());
                ObjectFactory.Initialize(x =>x.For<ITransactionRepository>().Use<TransactionRepository>());
                Initializer.Init();
                
                _isInitialized = true;
            }
        }
    }
}
