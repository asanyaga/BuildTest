using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    //OrderReference,OrderDate,SalesmanCode,OutletCode,ProductCode,ApprovedQuantity,DistributrCode


   public class ApprovedOrderImport
    {
       [CsvColumn(FieldIndex = 1)]
       public string OrderReference { get; set; }
       [CsvColumn(FieldIndex = 2)]
       public string OrderDate{ get; set; }
        [CsvColumn(FieldIndex = 3)]
       public string SalesmanCode { get; set; }
        [CsvColumn(FieldIndex = 4)]
        public string OutletCode { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public string ProductCode { get; set; }
        [CsvColumn(FieldIndex = 6)]
        public decimal ApprovedQuantity { get; set; }
        [CsvColumn(FieldIndex = 7)]
        public string DistributrCode { get; set; }
       
    }
}
