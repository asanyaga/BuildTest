using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository
{
    public  interface ITransactionRepository
    {
        List<Guid> SaveToLocal(OrderExportDocument document,TransactionType type);
        void SaveToQuickBooks(TransactionImport transactionImport);
        void MarkExportedLocal(Guid id);
        void MarkExportedLocal(string orderRef);
        //List<OrderExportDocument> LoadFromDB(TransactionType type);
        List<TransactionImport> LoadFromDB(TransactionType type);
    }
}
