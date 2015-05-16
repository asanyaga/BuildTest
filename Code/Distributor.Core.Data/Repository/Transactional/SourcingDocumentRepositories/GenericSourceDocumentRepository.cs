using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
    public class GenericSourceDocumentRepository : IGenericSourceDocumentRepository
    {
        protected CokeDataContext _ctx;

        public GenericSourceDocumentRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public GenericSourcingDocument GetById(Guid id)
        {
            var doc = _ctx.tblSourcingDocument.FirstOrDefault(s => s.Id == id);
            if(doc!=null)
            {
                var info = new GenericSourcingDocument();
                info.DocumentIssuer = doc.DocumentIssuerCostCentreId;
                info.DocumentRecepient = doc.DocumentRecipientCostCentreId;
                if (doc.DocumentOnBehalfOfCostCentreId != null)
                    info.OnBehalf = doc.DocumentOnBehalfOfCostCentreId.Value;
                return info;
            }
            return null;
        }

        public GenericSourcingDocument GetActivityById(Guid id)
        {
            var doc = _ctx.tblActivityDocument.FirstOrDefault(s => s.Id == id);
            if (doc != null)
            {
                var info = new GenericSourcingDocument();
                info.DocumentIssuer = doc.ClerkId;
                info.DocumentRecepient = doc.hubId;
                
                return info;
            }
            return null;
        }
    }
}
