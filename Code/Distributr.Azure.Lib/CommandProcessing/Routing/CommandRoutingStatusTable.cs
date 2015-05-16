using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Distributr.Azure.Lib.CommandProcessing.Routing
{
    //PK ccappid - RK CommandRouteOnRequest
    public class CommandRoutingStatusTable : TableEntity
    {
        public Guid Id { get; set; }
        public long CommandRouteOnRequestId { get; set; }
        public Guid CommandId { get; set; }
        public Guid DestinationCostCentreApplicationId { get; set; }
        public bool Delivered { get; set; }
        public DateTime DateDelivered { get; set; }
        public bool Executed { get; set; }
        public DateTime DateExecuted { get; set; }
        public DateTime DateAdded { get; set; }
    }

    //PK ccappid RK (DateTime.MaxValue.Ticks - CommandRouteOnRequestId)
    /*
     * RK to facilitate ordering
     */

    public class CommandRoutingStatusRouteOnRequestIndexTable : TableEntity
    {
        public long CommandRouteOnRequestId { get; set; }
    }

}
