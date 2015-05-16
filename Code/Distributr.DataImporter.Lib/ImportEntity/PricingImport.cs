using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportEntity
{
    //ProductCode, PricingTireCode,SellingPrice,ExFactoryRate, Value
   public class PricingImport
    {
       
       public string ProductCode { get; set; }

       public string PricingTireCode { get; set; }

       public decimal SellingPrice { get; set; }

       public decimal ExFactoryRate { get; set; }
       
    }
}
