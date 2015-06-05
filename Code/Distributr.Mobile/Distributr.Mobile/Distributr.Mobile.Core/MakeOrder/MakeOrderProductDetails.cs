using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Core.MakeOrder
{
    public class MakeOrderProductDetails : ProductDetails
    {
        public static string ProductSearch(string searchText)
        {
            return string.Format(
                        @" SELECT 
                          SaleProduct.Description, SaleProduct.MasterId
                        FROM 
                          SaleProduct
                       WHERE 
                          LOWER(SaleProduct.Description) LIKE LOWER('%{0}%') 
                        ORDER BY
                          SaleProduct.Description", searchText.Trim());
        }
    }
}
