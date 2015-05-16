using System;
using System.ComponentModel.DataAnnotations.Schema;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public abstract  class CommandEnvelopeQueueItemLocal
    {
       
        //WILL BE THE COST CENTRE COMMAND SEQUENCE ID
        public  DocumentType DocumentType { get; set; }
        public Guid DocumentId { get; set; }
        public Guid EnvelopeId { get; set; }
        
        //[MaxLength(2000)]
        [Column(TypeName = "nvarchar(MAX)")]
        public string JsonEnvelope { get; set; }
        public DateTime DateInserted { get; set; }
        public int Id { get; set; }
      
       


    }
}