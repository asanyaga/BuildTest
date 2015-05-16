using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public class CostCentreRouteOnRequestDetail
    {
        public long CommandRouteOnRequestId { get; set; }
        public bool IsValid { get; set; }
        public bool IsRetired { get; set; }
        public DateTime DateAdded { get; set; }
        public string DocumentId { get; set; }
        public string ParentDocumentId { get; set; }
        public string JsonCommand { get; set; }
        public bool Delivered { get; set; }
        public string CommandType { get; set; }
        public DateTime? DateDelivered { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        
        
    }
}
