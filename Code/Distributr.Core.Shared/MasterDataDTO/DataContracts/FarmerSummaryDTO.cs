using System;
using System.Collections.Generic;
using System.Text;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public enum FarmerQueryStatusCode { Success=1,Failure=2, CodeNotFound=10,NumberNotAuthenticated=11}
    public class FarmerSummaryDetail
    {
        public string FarmerName { get; set; }
        public string FarmerCode { get; set; }
        public string AsAtDate { get; set; }
        public int StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public List<FarmerSummaryDTO> SummaryDetail { get; set; } 
    }

    public class FarmerSummaryDTO
    {
        public string CommodityName { get; set; }

        public decimal CummWeight { get; set; }
    }
}
