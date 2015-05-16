using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel
{
    public class CommodityTransferDetailsViewModel
    {
        public Guid Id { get; set; }
        public string CommodityName { get; set; }
        public string CommodityGradeName { get; set; }
        public decimal Weight { get; set; }
        public string HubName { get; set; }
    }
}
