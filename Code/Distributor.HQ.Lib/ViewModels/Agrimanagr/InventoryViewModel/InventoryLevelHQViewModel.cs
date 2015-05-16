using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.InventoryViewModel
{
    public class InventoryLevelHQViewModel
    {
        public Warehouse Warehouse { get; set; }
        public Commodity Commodity { get; set; }
        public CommodityGrade Grade { get; set; }
        public decimal Balance { get; set; }
        public decimal UnavailableBalance { get; set; }
        public decimal Value { get; set; }
    }
}
