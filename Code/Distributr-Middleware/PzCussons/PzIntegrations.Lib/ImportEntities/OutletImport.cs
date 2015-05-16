using LINQtoCSV;

namespace PzIntegrations.Lib.ImportEntities
{
   public class OutletImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string Name { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string PinNo { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string PostalAddress { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public string PhysicalAddress { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public string Status { get; set; }

        [CsvColumn(FieldIndex = 7)]
        public string Tax{ get; set; }//we'll not use this field

        [CsvColumn(FieldIndex = 8)]
        public string Currency { get; set; }//we'll not use this field

        [CsvColumn(FieldIndex = 9)]
        public string CreditLimit { get; set; }//we'll not use this field

        [CsvColumn(FieldIndex = 10)]
        public string SalesmanCode { get; set; }

        [CsvColumn(FieldIndex = 11)]
        public string RouteName { get; set; }//this is actually a route name

        
    }
}
