using System.Collections.Generic;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class PurchasingClerkItem : CostCentreItem
    {
        public string CostCentreCode { get; set; }
        public UserItem UserItem { get; set; }
        public List<PurchasingClerkRouteItem> PurchasingClerkRouteItems { get; set; }
    }
}
