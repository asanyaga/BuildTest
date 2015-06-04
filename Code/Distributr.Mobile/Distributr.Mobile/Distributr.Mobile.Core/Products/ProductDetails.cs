using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.Products
{
    public enum UnitType {Each, Case};

    public class ProductDetails
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


        public ProductDetails()
        {
            
        }

        public ProductDetails(ProductLineItem lineItem)
        {
            Description = lineItem.Description;
            Balance = lineItem.AvailableQauntity;
            AvailableQuantity = lineItem.AvailableQauntity;
            SaleProduct = lineItem.Product;
            Quantity = lineItem.Quantity;
            LineItem = lineItem;
            SellReturnables = lineItem.SellReturnables;
            Price = lineItem.Price;
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
        public decimal Balance { get; set; }
        public SaleProduct SaleProduct {get; set; }

        public decimal AvailableQuantity { get; set; }

        public decimal AvailableEachQuantity
        {
            get { return AvailableQuantity % SaleProduct.ContainerCapacity; }
        }

        public decimal AvailableCaseQuantity
        {
            get { return Math.Floor(AvailableQuantity / SaleProduct.ContainerCapacity); }
        }

        public decimal Quantity { get; set; }

        public decimal EachQuantity
        {
            get { return Quantity%SaleProduct.ContainerCapacity; }
        }

        public decimal CaseQuantity
        {
            get { return Math.Floor(Quantity/SaleProduct.ContainerCapacity); }
        }

        public decimal Price { get; set; }

        public decimal CasePrice
        {
            get { return SaleProduct.ContainerCapacity * Price; }
        }

        public bool SellReturnables { get; set; }
        public ProductLineItem LineItem { get; set; }

        public bool IsValid
        {
            get { return true; }
//            get { return Quantity < AvailableQuantity; }
        }

        public bool HasQuantityChanged
        {
            get { return Quantity != LineItem.Quantity; }
        }

        public bool HasReturnablesChanged
        {
            get { return SellReturnables != LineItem.SellReturnables; }
        }

        public bool IsNew
        {
            get { return LineItem == null; }
        }

        public bool IsRemoved()
        {
            return Quantity == 0;
        }
    }
}