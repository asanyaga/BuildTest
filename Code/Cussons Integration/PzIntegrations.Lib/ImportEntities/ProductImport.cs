using LINQtoCSV;

namespace PzIntegrations.Lib.ImportEntities
{
    //BrandCode, ProductCode,ProductName,Active(Status),Selling Price, ExFactoryPrice
  public  class ProductImport
    {
      [CsvColumn(FieldIndex = 1)]
      public string BrandCode { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string ProductCode { get; set; }
        [CsvColumn(FieldIndex = 3)]
        public string Description { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string Status { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public decimal SellingPrice { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public decimal ExFactoryPrice { get; set; }

      
      
    }
}
