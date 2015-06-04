using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Core.MakeSale
{
    public class MakeSaleProductWrapper : ProductWrapper
    {
        public static string InventoryProductSearch(string searchText)
        {
            return string.Format(
                        @" SELECT 
                          SaleProduct.Description, Inventory.ProductMasterID AS MasterId, Inventory.Balance
                        FROM 
                          Inventory                  
                        INNER JOIN 
                          SaleProduct
                        ON 
                          Inventory.ProductMasterID = SaleProduct.MasterId 
                        WHERE 
                          LOWER(SaleProduct.Description) LIKE LOWER('%{0}%') 
                        ORDER BY 
                                Inventory.Balance DESC", searchText.Trim());
        }
    }
}
