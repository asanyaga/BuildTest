using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Distributr.Core.Commands;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public abstract class CommandQueueItemLocal 
    {
        
        //WILL BE THE COST CENTRE COMMAND SEQUENCE ID
        public CommandType CommandType { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        //[MaxLength(2000)]
        [Column(TypeName = "nvarchar(MAX)")]
        public string JsonCommand { get; set; }
        public DateTime DateInserted { get; set; }
        public int Id { get; set; }



    }
}
