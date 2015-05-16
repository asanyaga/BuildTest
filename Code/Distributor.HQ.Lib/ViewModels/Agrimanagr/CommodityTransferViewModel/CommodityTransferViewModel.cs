using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel
{
    public class CommodityTransferViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Current Store: ")]
        public string HubName { get; set; }
        [Display(Name = "Transfer Date: ")]
        public DateTime TransferDate { get; set; }
        public decimal Weight { get; set; }
    }

    public class CommodityTransferStoreAssignmentViewModel
    {
        [Required(ErrorMessage = "Store is a Required Field!"), Display(Name = "Transfer To: ")]
        public Guid StoreId { get; set; }
        public Guid Id { get; set; }
        public CommodityTransferViewModel Note { get; set; }
        public IList<CommodityTransferDetailsViewModel> Items { get; set; }
    }
}
