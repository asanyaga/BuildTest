using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository
{
    public interface IInvoiceExportDocumentRepository
    {
        //InvoiceExportDocument GetDocument(string orderRef); 
        //bool MarkAsExported(string orderExternalRef);

        InvoiceExportDocument GetDocument();
        bool MarkAsExported(Guid id);
    }
}
