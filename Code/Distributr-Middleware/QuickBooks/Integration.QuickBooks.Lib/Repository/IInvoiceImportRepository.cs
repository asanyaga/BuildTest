using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository
{
    public interface IInvoiceImportRepository
    {
        List<Guid> SaveToLocal(InvoiceExportDocument document);
        void SaveToQuickBooks(InvoiceExportDocument invoiceExportDocument);
        void MarkExportedLocal(Guid id);
        void MarkExportedLocal(string orderRef);
        List<InvoiceExportDocument> LoadFromDB();
    }
}
