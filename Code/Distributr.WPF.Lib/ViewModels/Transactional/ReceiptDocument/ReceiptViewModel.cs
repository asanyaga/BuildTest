using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument
{
   public class ReceiptViewModel:DistributrViewModelBase
    {
    }

    public class ReceiptHeader
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhysicalAddress { get; set; }
        public string TelNo { get; set; }
        public string FaxNo { get; set; }
        public string MobileNo { get; set; }
        public string PinNo { get; set; }
    }
}
