using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Distributr.Azure.Lib.CommandProcessing.Routing
{
    //PK rorid  RK costcenteid
    public class CommandRouteOnRequestCostCentreTable : TableEntity
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid DestinationCostCentreId { get; set; }
        public Guid SourceCostCentreId { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRetired { get; set; }
        public string CommandType { get; set; }
    }
}
