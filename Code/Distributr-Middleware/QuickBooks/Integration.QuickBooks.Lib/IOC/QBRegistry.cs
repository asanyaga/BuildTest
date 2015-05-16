using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.QuickBooks.Lib.Repository;
using Integration.QuickBooks.Lib.Repository.Impl;
using Integration.QuickBooks.Lib.Services;
using Integration.QuickBooks.Lib.Services.Impl;
using StructureMap.Configuration.DSL;

namespace Integration.QuickBooks.Lib.IOC
{
    public class QBRegistry : Registry
    {
        public QBRegistry()
        {
            For<ITransactionsDownloadService>().Use<TransactionsDownloadService>();
            For<ITransactionRepository>().Use<TransactionRepository>();

            For<IOrderImportRepository>().Use<OrderImportRepository>();
            For<IInvoiceImportRepository>().Use<InvoiceImportRepository>();


        }
    }
}
