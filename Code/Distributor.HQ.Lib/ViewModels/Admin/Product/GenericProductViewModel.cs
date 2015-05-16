using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
    public class GenericProductViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string BrandsName { get; set; }
        public string SubBrandsName { get; set; }
        public string ProductTypeName { get; set; }
        public string PackagingName { get; set; }
        public string ProductCategory { get; set; }
        public string ProductVATClass { get; set; }
    }
}
