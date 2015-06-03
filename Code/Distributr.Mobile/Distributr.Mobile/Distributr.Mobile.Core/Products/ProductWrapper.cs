using System;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Mobile.Core.Products
{
    public enum UnitType {Each, Case};

    public class ProductWrapper
    {
        public static string AllProductsByNameAscending
        {
            get
            {
                return @" SELECT 
                          SaleProduct.Description, SaleProduct.MasterId
                        FROM 
                          SaleProduct
                        ORDER BY
                          SaleProduct.Description";
            }
        }

        public static string AllProductsByNameDescending
        {
            get { return AllProductsByNameAscending + " DESC"; }
        }

        public static string InventoryProductsByBalanceAscending
        {
            get
            {
                return @" SELECT 
                          SaleProduct.Description, Inventory.ProductMasterID AS MasterId, Inventory.Balance
                        FROM 
                          Inventory
                        INNER JOIN 
                          SaleProduct
                        ON 
                          Inventory.ProductMasterID = SaleProduct.MasterId
                        ORDER BY 
                                Inventory.Balance ";
            }
        }

        public static string InventoryProductsByNameAscending
        {
            get
            {
                return @" SELECT 
                          SaleProduct.Description, Inventory.ProductMasterID AS MasterId, Inventory.Balance
                        FROM 
                          Inventory
                        INNER JOIN 
                          SaleProduct
                        ON 
                          Inventory.ProductMasterID = SaleProduct.MasterId
                        ORDER BY 
                                SaleProduct.Description ";
            }
        }

        public static string InventoryProductsByNameDescending
        {
            get { return InventoryProductsByNameAscending + " DESC"; }
        }

        public static string InventoryProductsByBalanceDescending
        {
            get { return InventoryProductsByBalanceAscending + " DESC"; }
        }

        public Guid MasterId { get; set; }
        public string Description { get; set; }
        public SaleProduct SaleProduct {get; set; }       
        
        public decimal Price { get; set; }
        public decimal EachQuantity { get; set; }
        public decimal MaxEachQuantity { get; set; }

        public decimal CasePrice
        {
            get { return SaleProduct.ContainerCapacity * Price; }
        }
        public decimal CaseQuantity { get; set; }
        public decimal MaxCaseQuantity { get; set; }

        public decimal EachReturnablePrice { get; set; }
        public decimal EachReturnableQuantity { get; set; }
        public decimal MaxEachReturnableQuantity { get; set; }

        public decimal CaseReturnablePrice { get; set; }
        public decimal CaseReturnableQuantity { get; set; }
        public decimal MaxCaseReturnableQuantity { get; set; }

        public long Balance { get; set; }

        public decimal MaxQuantity
        {
            get { return MaxEachQuantity; }
        }
    }
}