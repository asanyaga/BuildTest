using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    //ProductCode, Description, ExFactoryPrice, PackagingTypeCode, DiscountGroupCode ,CustomerDiscount , VATClass, BrandCode,Weight,PackagingCode,ProductTypeCode,ProductFlavourCode
    public class ProductImport
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal ExFactoryPrice { get; set; }
        public string PackagingTypeCode { get; set; }
        public string DiscountGroup { get; set; }
        public string CustomerDiscount { get; set; }
        public string VATClass { get; set; }
        public string BrandCode { get; set; }
        public string PackagingCode { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductFlavourCode { get; set; }
    }
}
