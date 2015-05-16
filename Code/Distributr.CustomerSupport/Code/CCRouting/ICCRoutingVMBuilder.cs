using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Code.CCRouting
{
    public interface ICCRoutingVMBuilder
    {
        CCRoutingSummaryViewModel GetRoutingSummary();
        CCRoutingDetailViewModel RoutingDetail(Guid costCentreId, int dayOfYear, int year);
    }

    public class CCRoutingDetailViewModel
    {
        public CCRoutingDetailViewModel()
        {
            Items = new List<CCRoutingDetailItem>();
        }
        public DateTime Date { get; set; }
        public string ShortDate { get; set; }
        public string CostCentreId { get; set; }
        public string CostCentreName { get; set; }

        public List<CCRoutingDetailItem> Items { get; set; }

        public class CCRoutingDetailItem : CostCentreRouteOnRequestDetail
        {
            public Guid CommandGeneratedByCostCentreId { get; set; }
            public string CommandGeneratedByCostCentre { get; set; }
        }
    }
}