using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class FarmerSummary
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public Guid CommodityId { get; set; }
        public decimal CummWeight { get; set; }

        public Guid FactoryId { get; set; }

        public string FactoryCode { get; set; }

        public decimal MonthlyCummWeight { get; set; }

        public decimal QtyLastDelivered { get; set; }

        public DateTime LastDeliverlyDate { get; set; }
    }
}
