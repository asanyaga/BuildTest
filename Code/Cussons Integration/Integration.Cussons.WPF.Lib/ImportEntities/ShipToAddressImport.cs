using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportEntities
{
   public class ShipToAddressImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string OutletName { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string ShipToCode { get; set; }


        [CsvColumn(FieldIndex = 4)]
        public string ShipToName { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public string PostalAddress { get; set; }

        //[CsvColumn(FieldIndex = 6)]
        //public string Longitude { get; set; }

        //[CsvColumn(FieldIndex = 7)]
        //public string Latitude { get; set; }
    }
}
