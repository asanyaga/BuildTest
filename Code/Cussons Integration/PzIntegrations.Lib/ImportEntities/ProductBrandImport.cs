using LINQtoCSV;

namespace PzIntegrations.Lib.ImportEntities
{
  public class ProductBrandImport
    {
        [CsvColumn(FieldIndex = 1)]
        public string Code { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string Name { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string Description { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string SupplierCode { get; set; }
        
    }
}
