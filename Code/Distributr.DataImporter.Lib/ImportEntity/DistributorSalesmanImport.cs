using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    public class DistributorSalesmanImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string SalesmanCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string Name { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string PayRollNumber { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string DistributorCode { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public string SalesmanPhoneNumber { get; set; }
    }
}
