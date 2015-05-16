using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportEntities
{
    public class AfcoProductPricingImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string ProductBrandCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string ProductCode { get; set; }
        
        [CsvColumn(FieldIndex = 3)]
        public decimal SellingPrice { get; set; }

       
    }
}
