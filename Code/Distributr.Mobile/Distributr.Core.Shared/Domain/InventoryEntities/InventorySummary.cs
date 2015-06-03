using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.InventoryEntities
{
    /// <summary>
    /// Way to break down and report
    /// on inventory levels especially consolidated products
    /// </summary>
    public class InventorySummary
    {
        public InventorySummary(List<Inventory> inventory)
        {
            Items = new List<InventorySummaryItem>();
            _inventory = inventory;
            LoadItems();
        }

        List<Inventory> _inventory;

        void LoadItems()
        {
            foreach (var inv in _inventory)
            {
                DomainProductType pt = GetProductType(inv.Product);
                Guid cc = inv.Warehouse.Id;
                Guid productId = inv.Product.Id;
                if (pt == DomainProductType.Consolidated)
                    AddConsolidatedProduct(inv);
                else
                    AddBasicProduct(inv);

            }
        }

        void AddBasicProduct(Inventory inv)
        {
            InventorySummaryItem isi = new InventorySummaryItem
            {
                ProductId = inv.Product.Id,
                Description = inv.Product.Description,
                Product = inv.Product,
                Warehouse = inv.Warehouse,
                Qty = inv.Balance,
                IsConsolidatedProductChild = false,
                ProductType = GetProductType(inv.Product)

            };
            Items.Add(isi);
        }

        void AddConsolidatedProduct(Inventory inv)
        {
            ConsolidatedProduct cp = inv.Product as ConsolidatedProduct;
            InventorySummaryItem isi = new InventorySummaryItem
            {
                ProductId = cp.Id,
                Description = cp.Description,
                ProductType = DomainProductType.Consolidated,
                Product = inv.Product,
                Warehouse = inv.Warehouse,
                Qty = inv.Balance,
                IsConsolidatedProductChild = false,
                HomeConsolidatedProductId = cp.Id,
                Level = 0,
                HomeConsolidatedProduct = cp,
                ParentConsolidatedProduct = null,
                QuantityPerConsolidatedProduct = 1,
                TotalProductsForHomeConsolidatedProduct = inv.Balance
            };
            Items.Add(isi);
            var fps = cp.GetFlattenedProducts();
            foreach (var fp in fps)
            {
                var isic = new InventorySummaryItem
                {
                    ProductId = fp.Product.Id,
                    Description = fp.Product.Description,
                    ProductType = GetProductType(fp.Product),
                    Warehouse = inv.Warehouse,
                    IsConsolidatedProductChild = true,
                    HomeConsolidatedProductId = cp.Id,
                    Level = fp.Level,
                    HomeConsolidatedProduct = cp,
                    ParentConsolidatedProduct = fp.DirectParent,
                    QuantityPerConsolidatedProduct = fp.QuantityPerConsolidatedProduct,
                    TotalProductsForHomeConsolidatedProduct = fp.TotalProductsForHomeConsolidatedProduct,
                    Qty = inv.Balance * fp.TotalProductsForHomeConsolidatedProduct
                };
                Items.Add(isic);
            }
        }

        DomainProductType GetProductType(Product product)
        {
            if (product is SaleProduct)
                return DomainProductType.Sale;
            if (product is ReturnableProduct)
                return DomainProductType.Returnable;
            return DomainProductType.Consolidated;
        }

        internal List<InventorySummaryItem> Items { get; set; }
        internal class InventorySummaryItem
        {
            internal Guid ProductId { get; set; }
            internal string Description { get; set; }
            internal DomainProductType ProductType { get; set; }
            internal Product Product { get; set; }
            internal Warehouse Warehouse { get; set; }
            internal decimal Qty { get; set; }
            internal bool IsConsolidatedProductChild { get; set; }

            //consolidated product info
            internal Guid HomeConsolidatedProductId { get; set; }
            internal int Level { get; set; }
            internal ConsolidatedProduct HomeConsolidatedProduct { get; set; }
            internal ConsolidatedProduct ParentConsolidatedProduct { get; set; }
            internal decimal QuantityPerConsolidatedProduct { get; set; }
            internal decimal TotalProductsForHomeConsolidatedProduct { get; set; }
        }

        public List<SimpleInventoryItem> SimpleInventoryList { get { return BuildSimpleInventoryList(); } }
        List<SimpleInventoryItem> BuildSimpleInventoryList()
        {
            List<SimpleInventoryItem> sii = Items
                .Where(n => !n.IsConsolidatedProductChild)
                .Select(n => new SimpleInventoryItem
            {
                ProductId = n.ProductId,
                Description = n.Description,
                ProductType = n.ProductType,
                Product = n.Product,
                Warehouse = n.Warehouse,
                Qty = n.Qty
            })
            .ToList();

            foreach (var item in sii)
            {
                if (item.ProductType == DomainProductType.Consolidated)
                {
                    var cpChildItems = Items.Where(n => n.HomeConsolidatedProductId == item.ProductId && n.Warehouse.Id == item.Warehouse.Id && n.IsConsolidatedProductChild);
                    item.ChildProducts = cpChildItems.Select(n => new SimpleInventoryItem.SimpleInventoryItemChild
                    {
                        Product = n.Product,
                        Description = n.Description,
                        ProductType = n.ProductType,
                        ProductId = n.ProductId,
                        Qty = n.Qty,
                        Warehouse = n.Warehouse,
                        Level = n.Level,
                        ParentConsolidatedProduct = n.ParentConsolidatedProduct,
                        QuantityPerConsolidatedProduct = n.QuantityPerConsolidatedProduct,
                        TotalProductsForHomeConsolidatedProduct = n.TotalProductsForHomeConsolidatedProduct,
                        TotalQty = n.Qty
                    }).ToList();
                }
            }
            return sii;
        }

        public List<FlattenedInventoryItem> FlattenedInventoryList { get { return BuildFlattenedInventoryList(); } }

        private List<FlattenedInventoryItem> BuildFlattenedInventoryList()
        {
            List<FlattenedInventoryItem> inventoryItems = new List<FlattenedInventoryItem>();
            Guid[] productIds = Items.Select(n => n.ProductId).Distinct().ToArray();
            foreach (Guid productId in productIds)
            {
                List<InventorySummaryItem> items = Items.Where(n => n.ProductId == productId).ToList();
                FlattenedInventoryItem fii = new FlattenedInventoryItem
                {
                    ProductId = productId,
                    Description = items[0].Description,
                    Product = items[0].Product,
                    ProductType = items[0].ProductType,
                    Quantity = items.Sum(n => n.Qty),
                    ChildProducts = new List<FlattenedInventoryItem.FlattenedInventoryItemChild>()
                };
                foreach (var item in items)
                {
                    var fic = new FlattenedInventoryItem.FlattenedInventoryItemChild
                    {
                        Warehouse = item.Warehouse,
                        HomeConsolidatedProduct = item.HomeConsolidatedProduct,
                        HomeConsolidatedProductId = item.HomeConsolidatedProductId,
                        IsConsolidatedProductChild = item.IsConsolidatedProductChild,
                        Level = item.Level,
                        ParentConsolidatedProduct = item.ParentConsolidatedProduct,
                        Quantity = item.Qty,
                        QuantityPerConsolidatedProduct = item.QuantityPerConsolidatedProduct,
                        TotalProductsForHomeConsolidatedProduct = item.TotalProductsForHomeConsolidatedProduct
                    };
                    fii.ChildProducts.Add(fic);
                }
                inventoryItems.Add(fii);

            }
            return inventoryItems;
        }

    }

    #region simple inventory items

    public class SimpleInventoryItemBase
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public DomainProductType ProductType { get; set; }
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
        public decimal Qty { get; set; }
    }


    public class SimpleInventoryItem : SimpleInventoryItemBase
    {
        public List<SimpleInventoryItemChild> ChildProducts { get; set; }
        public class SimpleInventoryItemChild : SimpleInventoryItemBase
        {
            public int Level { get; set; }
            public ConsolidatedProduct ParentConsolidatedProduct { get; set; }
            public decimal QuantityPerConsolidatedProduct { get; set; }
            public decimal TotalProductsForHomeConsolidatedProduct { get; set; }
            public decimal TotalQty { get; set; }
        }
    }
    #endregion

    #region Flattened Inventory Items

    public class FlattenedInventoryItem
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public DomainProductType ProductType { get; set; }
        public Product Product { get; set; }
        public decimal Quantity { get; set; }
        public List<FlattenedInventoryItemChild> ChildProducts { get; set; }
        public class FlattenedInventoryItemChild
        {
            public bool IsConsolidatedProductChild { get; set; }
            public Warehouse Warehouse { get; set; }
            public Guid HomeConsolidatedProductId { get; set; }
            public int Level { get; set; }
            public ConsolidatedProduct HomeConsolidatedProduct { get; set; }
            public ConsolidatedProduct ParentConsolidatedProduct { get; set; }
            public decimal QuantityPerConsolidatedProduct { get; set; }
            public decimal TotalProductsForHomeConsolidatedProduct { get; set; }
            public decimal Quantity { get; set; }
        }
    }

    #endregion





}
