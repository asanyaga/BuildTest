using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using System.Linq;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.WorkFlow.Orders;
using StructureMap;

namespace Distributr.WPF.Lib.Impl.Services.Utility
{
    /*
     * Code written by Juvenalis Gitau
     * This service is generate a summary of product returnable after one adds product 
     * AddProduct(int productId, int quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix=true) function is used to add the main 
     * product
     *  GetSummaryProduct() generate the returnable product that exist or relate to that product
     *  
     */
    //REFACTOR - needs to be refactored - too tightly coupled to viewmodels
    public class ProductPackagingSummaryService : IProductPackagingSummaryService
    {
        private IProductRepository _productService;
        private IInventoryRepository _inventoryService;
        public ProductPackagingSummaryService(IProductRepository productService, IInventoryRepository inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService;
        }

        private List<ProductPackagingSummary> _temp = new List<ProductPackagingSummary>();
        List<ReturnableProduct> productReturnables = new List<ReturnableProduct>();//cn
        private List<PackagingSummary> mixedPackReturnables = new List<PackagingSummary>();//cn

        public void AddProduct(Guid productId, decimal quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix = true)
        {
            Product product = _productService.GetById(productId);
            if (product is SaleProduct)
            {
                InsertProductSummary(_temp, product, quantity, isOneToOne, IsEdit, isMix);
            }
            else if (product is ReturnableProduct)
            {
                ReturnableProduct returnableProduct = product as ReturnableProduct;
                InsertProductSummary(_temp, product, quantity, isOneToOne, IsEdit, isMix);
            }
            else if (product is ConsolidatedProduct)
            {
                ConsolidatedProduct consolidatedProduct = product as ConsolidatedProduct;
                InsertProductSummary(_temp, product, quantity, isOneToOne, IsEdit, isMix);
            }
        }

        private void InsertProductSummary(List<ProductPackagingSummary> temp, Product product, decimal quantity, bool isOneToOne, bool isEdit, bool isMix)
        {

            ProductPackagingSummary summary = temp.FirstOrDefault(p => p.Product.Id == product.Id);
            if (summary == null)
            {
                summary = new ProductPackagingSummary()
                              {
                                  Product = product,
                                  Quantity = quantity,
                                  IsOneToOne = isOneToOne,
                                  IsMix = isMix,

                              };
                temp.Add(summary);
            }
            else
            {
                if (isEdit)
                    summary.Quantity = quantity;
                else
                    summary.Quantity = summary.Quantity + quantity;
            }
        }

        public List<PackagingSummary> GetProductSummary()
        {
            List<PackagingSummary> _proccesTemp = ProcessSummary(_temp);
            return _proccesTemp;
        }
        public List<PackagingSummary> GetProductFinalSummary()
        {
            List<PackagingSummary> _proccesTemp = ProcessSummary(_temp);
            var mixPackGroups = _proccesTemp
                .Where(n=>n.Product is ReturnableProduct)
                .Where(n => ((ReturnableProduct)n.Product).ReturnAbleProduct != null)
                .GroupBy(n => ((ReturnableProduct)n.Product).ReturnAbleProduct.Id);

            foreach (var group in mixPackGroups)
            {
                decimal qty = group.Sum(n => n.Quantity);
                var returnablecontontiner = _productService.GetById(group.Key) as ReturnableProduct;
                if (returnablecontontiner != null)
                {
                   // ReturnableProduct rp =returnableProduct.ReturnAbleProduct;
                    int expected = (int) (qty /returnablecontontiner.Capacity);
                    decimal current = 0;
                    var firstOrDefault   = _proccesTemp.FirstOrDefault(s => s.Product.Id == returnablecontontiner.Id);
                    if (firstOrDefault != null)
                    {
                         current = firstOrDefault.Quantity;
                        
                    }
                    if (expected>current)
                    {
                        if (firstOrDefault != null)
                        {
                            firstOrDefault.Quantity = expected;
                        }
                        else
                        {

                            _proccesTemp.Add(new PackagingSummary
                                                 {
                                                     Product = returnablecontontiner,
                                                     Quantity = expected,
                                                 });
                        }
                    }
                }
            }
            return _proccesTemp;
        }
        private List<PackagingSummary> ProcessSummary(List<ProductPackagingSummary> temp)
        {
            List<PackagingSummary> _proccesTemp = new List<PackagingSummary>();
            foreach (ProductPackagingSummary ps in temp)
            {
                if (ps.Product is SaleProduct)
                {
                    SaleProduct s = ps.Product as SaleProduct;
                    if (s.ReturnableProduct != null)
                    {
                        ReturnableProduct r = s.ReturnableProduct;
                        if (r.ReturnAbleProduct != null)
                        {
                            ReturnableProduct c = r.ReturnAbleProduct;
                            decimal createsToAdd = GetCratesToAdd(_proccesTemp, ps.Quantity, r, c, ps.IsMix, ps.Product.Id);
                            if (createsToAdd > 0 && ps.IsOneToOne == false)
                            {
                                InsertSummary(_proccesTemp, c, createsToAdd, false, true, ps.IsMix, ps.Product.Id);
                            }
                        }
                        InsertSummary(_proccesTemp, r, ps.Quantity, false, true, ps.IsMix, ps.Product.Id);
                    }

                    InsertSummary(_proccesTemp, s, ps.Quantity, true, false, ps.IsMix, ps.Product.Id);
                }
                else if (ps.Product is ReturnableProduct)
                {
                    ReturnableProduct r = ps.Product as ReturnableProduct;
                    if (r.ReturnAbleProduct != null)
                    {
                        ReturnableProduct c = r.ReturnAbleProduct;
                        decimal createsToAdd = GetCratesToAdd(_proccesTemp, ps.Quantity, r, c, ps.IsMix, ps.Product.Id);
                        if (createsToAdd > 0 && ps.IsOneToOne == false)
                        {
                            InsertSummary(_proccesTemp, c, createsToAdd, false, true, ps.IsMix, ps.Product.Id);
                        }
                    }
                    InsertSummary(_proccesTemp, r, ps.Quantity, true, false, ps.IsMix, ps.Product.Id);
                }
                else if (ps.Product is ConsolidatedProduct)
                {
                    ConsolidatedProduct cp = ps.Product as ConsolidatedProduct;
                    InsertSummary(_proccesTemp, cp, ps.Quantity, true, false, ps.IsMix, ps.Product.Id);
                }

            }
            return _proccesTemp;
        }

        public void ClearBuffer()
        {
            _temp.Clear();
        }

        private decimal GetCratesToAdd(List<PackagingSummary> proccesTemp, decimal quantity, ReturnableProduct r, ReturnableProduct c, bool isMix, Guid parentProductId)
        {
            List<PackagingSummary> bottlesInaCrate;
            List<PackagingSummary> Crate;
            if (isMix)
            {
                bottlesInaCrate = proccesTemp.Where(y => y.Product.Id == r.Id && y.IsAuto == true).ToList();
                Crate = proccesTemp.Where(y => y.Product.Id == c.Id && y.IsAuto == true).ToList();
            }
            else
            {
                bottlesInaCrate = proccesTemp.Where(y => y.Product.Id == r.Id && y.IsAuto == true && y.ParentProductId == parentProductId).ToList();
                Crate = proccesTemp.Where(y => y.Product.Id == c.Id && y.IsAuto == true && y.ParentProductId == parentProductId).ToList();
            }


            decimal bottlesInaCrateQuantity = bottlesInaCrate != null ? bottlesInaCrate.Sum(o => o.Quantity) : 0;
            decimal CrateQuantity = Crate != null ? Crate.Sum(n => n.Quantity) : 0;
            decimal TotalBottles = bottlesInaCrateQuantity + quantity;
            decimal TotalCrate = TotalBottles / c.Capacity;
            decimal q = TotalCrate - CrateQuantity;
            return (int)q;
        }

        private void InsertSummary(List<PackagingSummary> proccesTemp, Product product, decimal quantity, bool isEditable, bool isAuto, bool isMix, Guid parentProductId)
        {
            PackagingSummary summary = null;
            if (isMix)
                summary = proccesTemp.FirstOrDefault(p => p.Product.Id == product.Id && p.IsAuto == isAuto);
            else
                summary = proccesTemp.FirstOrDefault(p => p.Product.Id == product.Id && p.IsAuto == isAuto && p.ParentProductId == parentProductId);
            if (summary == null)
            {
                summary = new PackagingSummary()
                {
                    Product = product,
                    Quantity = quantity,
                    IsEditable = isEditable,
                    IsAuto = isAuto,
                };
                if (!isMix)
                    summary.ParentProductId = parentProductId;
                proccesTemp.Add(summary);
            }
            else
            {
                summary.Quantity += quantity;
            }

        }

        public void RemoveProduct(Guid productId)
        {
            ProductPackagingSummary delep = _temp.FirstOrDefault(p => p.Product.Id == productId);
            _temp.Remove(delep);
        }

        public List<PackagingSummary> GetProductSummaryByProduct(Guid productId, decimal quantity, bool isOneToOne = false)
        {
            List<ProductPackagingSummary> priceSummary = new List<ProductPackagingSummary>();
            List<PackagingSummary> _proccesTemp = new List<PackagingSummary>();
            Product product = _productService.GetById(productId);

            if (product is SaleProduct)
            {
                InsertProductSummary(priceSummary, product, quantity, isOneToOne, true, true);
            }
            else if (product is ReturnableProduct)
            {
                InsertProductSummary(priceSummary, product, quantity, isOneToOne, true, true);
            }
            else if (product is ConsolidatedProduct)
            {
                InsertProductSummary(priceSummary, product, quantity, isOneToOne, true, true);
            }
            else
            {
                return null;
            }
            _proccesTemp = ProcessSummary(priceSummary);
            return _proccesTemp;

        }

        public decimal GetProductQuantityInBulk(Guid productId)
        {
            int quantity = 1;
            Product product = _productService.GetById(productId);
            if (product is SaleProduct)
            {
                SaleProduct s = product as SaleProduct;
                if (s.ReturnableProduct != null)
                {
                    ReturnableProduct r = s.ReturnableProduct;
                    if (r.ReturnAbleProduct != null)
                    {
                        ReturnableProduct c = r.ReturnAbleProduct;
                        if (c != null)
                            return c.Capacity;
                    }

                }
            }
            else if (product is ReturnableProduct)
            {
                ReturnableProduct r = product as ReturnableProduct;
                if (r.ReturnAbleProduct != null)
                {
                    ReturnableProduct c = r.ReturnAbleProduct;
                    if (c != null)
                        return c.Capacity;
                }
            }
            else if (product is ConsolidatedProduct)
            {
                ConsolidatedProduct cp = product as ConsolidatedProduct;

            }
            return quantity;

        }

        private ReturnableProduct returnable = null;

        public ReturnableProduct GetProductBulkReturnable(Product product)
        {
            if (product is SaleProduct)
            {
                SaleProduct s = product as SaleProduct;
                if (s.ReturnableProduct != null)
                {
                    ReturnableProduct r = s.ReturnableProduct;
                    if (r.ReturnAbleProduct != null)
                    {
                        GetProductBulkReturnable(r);
                    }
                    else
                    {
                        returnable = r;
                    }
                    //if (r.ReturnAbleProduct != null)
                    //{
                    //    ReturnableProduct c = r.ReturnAbleProduct;
                    //    if (c != null)
                    //        return c;
                    //}

                }


            }
            else if (product is ReturnableProduct)
            {
                ReturnableProduct r = product as ReturnableProduct;
                ReturnableProduct c = r.ReturnAbleProduct;
                if (c.ReturnAbleProduct != null)
                {
                    GetProductBulkReturnable(c);
                }
                else
                {
                    returnable = c;
                }
                //if (r.ReturnAbleProduct != null)
                //{
                //    ReturnableProduct c = r.ReturnAbleProduct;
                //    if (c != null)
                //        return c;
                //}

            }
            return returnable;
        }

        public bool IsProductInStock(Guid costCentreId, Guid productId, decimal prodQtytyAddedToLineItems, decimal prodQtyToAdd, out decimal invBalance)
        {
            bool retVal = false;
            decimal invBal = 0m;

            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                _inventoryService = c.GetInstance<IInventoryRepository>();

                var prodInv = _inventoryService.GetByProductIdAndWarehouseId(productId, costCentreId);
                if (prodInv != null)
                {
                    invBal = prodInv.Balance;

                    if ((invBal - prodQtytyAddedToLineItems) > 0)
                        retVal = true;
                }

                invBalance = invBal;
            }
            return retVal;
        }

        public List<ReturnableProduct> GetProductReturnables(Product product, decimal qty)
        {
            ReturnableProduct foundReturnable = null;

            if (product.GetType() == typeof(SaleProduct))
            {
                ReturnableProduct returnable = ((SaleProduct)product).ReturnableProduct;
                if (returnable != null)
                {
                    foundReturnable = returnable;
                }
            }
            else if (product.GetType() == typeof(ReturnableProduct))
            {
                ReturnableProduct returnable = ((ReturnableProduct)product).ReturnAbleProduct;
                if (returnable != null)
                {
                    foundReturnable = returnable;
                }
            }

            if (foundReturnable != null)
            {
                if (foundReturnable.Capacity == 1)
                {
                    productReturnables.Add(foundReturnable);
                    GetProductReturnables(foundReturnable, qty);
                }
                else if (foundReturnable.Capacity > 1)
                {
                    if (qty / foundReturnable.Capacity > 1)
                    {
                        productReturnables.Add(foundReturnable);
                        GetProductReturnables(foundReturnable, (qty / foundReturnable.Capacity));
                    }
                }
            }

            return productReturnables;
        }

        public List<PackagingSummary> GetMixedPackContainers(List<PackagingSummary> returnableProducts)
        {
            //1. check for returnable that share container
            //2. if their sum qty can fit a container, return container.
            var mixPackGroups =
              returnableProducts.Where(n => ((ReturnableProduct)n.Product).ReturnAbleProduct != null)
                  .GroupBy(n => ((ReturnableProduct)n.Product).ReturnAbleProduct.Id);


            foreach (var group in mixPackGroups)
            {
                decimal qty = group.Sum(n => n.Quantity);
                ReturnableProduct rp = (group.FirstOrDefault().Product as ReturnableProduct).ReturnAbleProduct;

                var div = qty / rp.Capacity;
                if (qty / rp.Capacity >= 1)
                {
                    mixedPackReturnables.Add(new PackagingSummary
                                                 {
                                                     Product = rp,
                                                     Quantity = (int)(qty / rp.Capacity)
                                                 });
                }
            }

            return mixedPackReturnables;
        }


        public void ClearProductReturnables()
        {
            productReturnables.Clear();
        }

        public void ClearMixedPackReturnables()
        {
            mixedPackReturnables.Clear();
        }


        public List<OrderLineItemBase> OrderLineItems(List<OrderLineItemBase> lineItems)
        {
            List<OrderLineItemBase> temp = new List<OrderLineItemBase>();
            //cn: consolidated
            lineItems.Where(n => n.ProductType == "ConsolidatedProduct").OrderBy(n => n.OrderLineItemType).ThenBy(n => n.Product).ToList().
                ForEach(temp.Add);
            //cn: sale product
            lineItems.Where(n => n.ProductType == "SaleProduct").OrderBy(n => n.OrderLineItemType).ThenBy(n => n.Product).ToList().
                ForEach(temp.Add);
            //cn: returnables
            lineItems.Where(n => n.ProductType == "ReturnableProduct" && n.TotalPrice >= 0).OrderByDescending(n => n.Qty).ThenBy(n => n.Product).ToList().
                ForEach(temp.Add);
            //cn: received returnables
            lineItems.Where(n => n.ProductType == "ReturnableProduct" && n.TotalPrice < 0).OrderByDescending(n => n.Qty).ThenBy(n => n.Product).ToList().
                ForEach(temp.Add);
            if (lineItems.Count != temp.Count)
                throw new Exception("lineItems.Count != temp.Count");//scream impossible!!!

            //re-number sequence id
            int seqNo = 1;
            foreach (var item in temp)
            {
                item.SequenceNo = seqNo;
                seqNo++;
            }

            return temp;
        }
    }
}
