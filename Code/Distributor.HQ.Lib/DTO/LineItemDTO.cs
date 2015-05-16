using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.DTO
{
    public class LineItemDTO
    {
        public Guid Id { get; set; }
        public string BatchNo { get; set; }
        public CommodityDTO Commodity { get; set; }

        public GradeDTO Grade { get; set; }

        public StoreDTO Store { get; set; }

        public decimal Weight { get; set; }

        public bool IsSelected { get; set; }
    }

    public class TransferLineItemDTO
    {
        public string ItemId { get; set; }
        public string BatchNo { get; set; }
        public string CommodityId { get; set; }

        public string GradeId { get; set; }

        public string StoreId { get; set; }

        public decimal Weight { get; set; }
    }
}
