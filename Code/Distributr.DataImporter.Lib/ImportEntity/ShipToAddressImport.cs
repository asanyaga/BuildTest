using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    //OutletCode, Name, Description,PostalAddress,PhysicalAddress,Longitude,Latitude 
   public class ShipToAddressImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string Code { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string Description { get; set; }


         [CsvColumn(FieldIndex = 4)]
        public string PostalAddress { get; set; }

        [CsvColumn(FieldIndex = 5)]
         public string PhysicalAddress { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public string Longitude { get; set; }

        [CsvColumn(FieldIndex = 7)]
        public string Latitude { get; set; }
       
    }
}
