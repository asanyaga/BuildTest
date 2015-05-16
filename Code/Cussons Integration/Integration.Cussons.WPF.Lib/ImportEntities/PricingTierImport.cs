using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportEntities
{
   public class PricingTierImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string TierCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string ProductCode { get; set; }
        
        [CsvColumn(FieldIndex = 5)]
        public decimal SellingPrice { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public decimal ExFactoryPrice { get; set; }
    }
}
