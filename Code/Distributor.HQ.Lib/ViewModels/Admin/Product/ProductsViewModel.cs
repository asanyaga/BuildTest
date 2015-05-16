using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{
   public class ProductsViewModel
    {
        public int Brands { get; set; }
        public int SubBrands { get; set; }
        public int PackageType { get; set; }
        public int Packaging { get; set; }
        public int ProductType { get; set; }
        public int Returnable { get; set; }
        public string Description { get; set; }
        public decimal UnitCases { get; set; }

        public string BrandsName { get; set; }
        public string SubBrandsName { get; set; }
        public string PackageTypeName { get; set; }
        public string PackagingName { get; set; }
        public string ProductTypeName { get; set; }
        public string ReturnableName { get; set; }
        public int Id { get; set; }
        public bool isActive { get; set; }
    }
}
