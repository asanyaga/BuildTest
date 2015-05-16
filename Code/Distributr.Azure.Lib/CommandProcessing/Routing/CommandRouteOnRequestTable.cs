using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Distributr.Azure.Lib.CommandProcessing.Routing
{
    //PK Costcentre RK = Id 
    public class CommandRouteOnRequestTable : TableEntity
    {
        public long Id { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateCommandInserted { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        public string CommandType { get; set; }
        public string JsonCommand { get; set; }
        public Guid DocumentParentId { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRetired { get; set; }
    }

    //PK "idlu" - RK Id
    public class CommandRouteOnRequestIdIndex : TableEntity
    {
        public Guid GeneratedByCostCentreId { get; set; }
    }

    //PK - documentid  RK - ID 
    public class CommandRouteOnRequestDocumentIndex : TableEntity
    {
        public Guid GeneratedByCostCentreId { get; set; }
    }

    //PK "commandlu" 
    public class CommandRouteOnRequestCommandIndex : TableEntity
    {
        public long Id { get; set; }
        public Guid GeneratedByCostCentreId { get; set; }
    }

    //PK parentdocid RK id
    public class CommandRouteOnRequestParentDocIndex : TableEntity
    {
        public Guid GeneratedByCostCentreId { get; set; }
    }

}
