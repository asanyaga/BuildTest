using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributr.CustomerSupport.Code.CCRouting
{
    public class CCRoutingSummaryViewModel
    {

        public List<CCSummaryItem> Items { get; set; }

        public class CCSummaryItem
        {
            public CCSummaryItem()
            {
                CCAppStatusItems = new List<CCRoutingStatusSummaryItem>();
            }
            public string CostCentre { get; set; }
            public Guid DestinationCostCentreId { get; set; }
            public int TotalCount { get; set; }
            public int ValidCount { get; set; }
            public int RetiredCount { get; set; }
            public List<CCRoutingStatusSummaryItem> CCAppStatusItems { get; set; }
            public int DeliveredCount
            {
                get { return CCAppStatusItems.Sum(n => n.Delivered); }
            }
        }

        public class CCRoutingStatusSummaryItem
        {
            public Guid DestinationCostCentreId { get; set; }
            public Guid DestinationCostCentreApplicationId { get; set; }
            public int Delivered { get; set; }

        }
    }
}