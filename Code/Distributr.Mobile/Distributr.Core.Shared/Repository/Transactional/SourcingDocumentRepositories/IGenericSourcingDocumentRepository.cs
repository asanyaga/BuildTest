using System;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{

    public class GenericSourcingDocument 
    {
        public Guid DocumentRecepient { get; set; }
        public Guid DocumentIssuer { get; set; }
        public Guid OnBehalf { get; set; }


       
    }

   
    public interface IGenericSourceDocumentRepository
    {
        GenericSourcingDocument GetById(Guid id);
        GenericSourcingDocument GetActivityById(Guid id);
        
    }
}
