using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Admin.Orders
{
    public class OrderAddLineItemViewModel
    {
        public string DocumentRef { get; set; }
        public string DocumentId { get; set; }
        public Guid ProductId { get; set; }
        public string Qty { get; set; }
        public string QuantityType { get; set; }
        public SelectList ProductLookup { get; set; }
    }
}
