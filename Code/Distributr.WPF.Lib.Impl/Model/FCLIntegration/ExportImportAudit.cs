using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Impl.Model.FCLIntegration
{
   public class ExportImportAudit
    {
       public Guid Id { get; set; }
       public string DocumentReference { get; set; }
       public string ExternalDocumentReference { get; set; }
       public DocumentAuditStatus AuditStatus { get; set; }
    }

   public enum DocumentAuditStatus
    {
       Exported=2,
       Imported=1,
       New=0
    }
}
