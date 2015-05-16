using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.WPF.Lib.UI.Hierarchy
{
    public class ConsolidatedProductHierarchyHelper
    {
        public static IEnumerable<HierarchyNode<ProductViewer>> GetConsolidatedProductForTreeview(ConsolidatedProduct consolidatedProduct, decimal qty)
        {
            var fp = consolidatedProduct.GetFlattenedProducts();
            ProductViewer pv = new ProductViewer
            {
                ProductId = consolidatedProduct.Id,
                ParentProductId = Guid.Empty,
                Description = consolidatedProduct.Description,
                TotalItems = 1 * qty,
                ProdType = "C",
                Level = 0,
                QtyPerConsolidatedProduct = 1
            };
            List<ProductViewer> pvs = fp.Select(n => new ProductViewer
            {
                ProductId = n.ProductId,
                ParentProductId = n.DirectParentId,
                Description = n.Description,
                Level = n.Level,
                ProdType = n.ProductDetailType.ToString(),
                TotalItems = n.TotalProductsForHomeConsolidatedProduct * qty,
                QtyPerConsolidatedProduct = n.QuantityPerConsolidatedProduct
            })
            .ToList();

            pvs.Add(pv);

            return pvs.AsHierarchy(n => n.ProductId, n => n.ParentProductId);
            
        }
        public static IEnumerable<ProductViewer> GetConsolidatedProductFLat(ConsolidatedProduct consolidatedProduct, int qty)
        {
            var fp = consolidatedProduct.GetFlattenedProducts();
            ProductViewer pv = new ProductViewer
            {
                ProductId = consolidatedProduct.Id,
                ParentProductId = Guid.Empty,
                Description = consolidatedProduct.Description,
                TotalItems = 1 * qty,
                ProdType = "C",
                Level = 0,
                QtyPerConsolidatedProduct = 1
            };
            List<ProductViewer> pvs = fp.Select(n => new ProductViewer
            {
                ProductId = n.ProductId,
                ParentProductId = n.DirectParentId,
                Description = n.Description,
                Level = n.Level,
                ProdType = n.ProductDetailType.ToString(),
                TotalItems = n.TotalProductsForHomeConsolidatedProduct * qty,
                QtyPerConsolidatedProduct = n.QuantityPerConsolidatedProduct
            })
            .ToList();

           // pvs.Add(pv);

            return pvs;

        }
    }

    public class ProductViewer
    {
        public Guid ProductId { get; set; }
        public Guid ParentProductId { get; set; }
        public string Description { get; set; }
        public decimal TotalItems { get; set; }
        public string strTotalItems
        {
            get { return string.Format("({0})", TotalItems); }
        }
        public string ProdType { get; set; }
        public string strProdType
        {
            get { return string.Format("-{0}-", ProdType.Substring(0,1)); }
        }
        public int Level { get; set; }
        public int QtyPerConsolidatedProduct { get; set; }
    }
}
