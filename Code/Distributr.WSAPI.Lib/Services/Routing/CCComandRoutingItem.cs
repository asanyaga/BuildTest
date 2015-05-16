using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public class CCComandRoutingItem
    {
        public Guid CommandId { get; set; }
        public long CommandAsInteger { get; set; }
        public Guid DocumentId { get; set; }
        public string CommandType { get; set; }
        public bool Delivered { get; set; }
        public DateTime? DateDelivered { get; set; }
        public DateTime? DateProcessed { get; set; }
    }

    public class CommandRef
    {
        public Guid CommandId { get; set; }
        public long CommandIdAsInteger { get; set; }
        public string CommandType { get; set; }
        public Guid DocumentId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        public Guid CommandGeneratedByApplicationId { get; set; }
    }
}
