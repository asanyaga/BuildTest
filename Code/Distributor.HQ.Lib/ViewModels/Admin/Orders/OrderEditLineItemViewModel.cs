using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Orders
{
    public class OrderEditLineItemViewModel
    {
        public string DocumentRef { get; set; }
        public string LineItemId { get; set; }
        public string DocumentId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductDesc { get; set; }
        public string Qty { get; set; }
    }
}
