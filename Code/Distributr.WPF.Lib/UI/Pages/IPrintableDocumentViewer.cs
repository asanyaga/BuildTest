using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.UI.Pages
{
    public interface IPrintableDocumentViewer
    {
        void ViewDocument(Guid documentId, DocumentType docType);
    }
}
