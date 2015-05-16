using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    //SequenceNo,SalesmanCode,ProductCode,ApprovedQuantity

    public class ImportInvetoryIssueToSalesman
    {
        [CsvColumn(FieldIndex = 1)]
        public int SequenceNo { get; set; }
        [CsvColumn(FieldIndex = 2)]
        public string ProductCode { get; set; }
        [CsvColumn(FieldIndex = 3)]
        public decimal ApprovedQuantity { get; set; }

    }
   
}
