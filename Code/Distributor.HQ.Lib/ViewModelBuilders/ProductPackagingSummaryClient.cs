using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders
{
   public class ProductPackagingSummaryClient : IProductPackagingSummaryClient
    {
        private IProductRepository _productRepository;

        public ProductPackagingSummaryClient(IProductRepository productRepository)
        {
            _productRepository = productRepository;

        }
        private List<ProductPackagingSummary> _temp = new List<ProductPackagingSummary>();
        private void SetKey(string key)
        {
            _temp = MemoryCache.Default.Get(key) as List<ProductPackagingSummary>;
            if (_temp == null)
            {
                _temp = new List<ProductPackagingSummary>();
                MemoryCache.Default.Add(key, _temp, DateTimeOffset.Now.AddHours(12));
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
            HttpContext.Current.Session["ProductPackagingSummary"] = _temp;
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

        public void AddProduct(string key, Guid productId, decimal quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix = true)
        {
            SetKey(key);
            Product product = _productRepository.GetById(productId);
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
            SetData(key,_temp);
        }

       private void SetData(string key,List<ProductPackagingSummary> temp)
       {
           MemoryCache.Default.Add(key, _temp, DateTimeOffset.Now.AddHours(12));
       }

       public List<PackagingSummary> GetProductSummary(string key)
        {
             SetKey(key);
            List<PackagingSummary> _proccesTemp = ProcessSummary(_temp);
            return _proccesTemp;
        }

        public void ClearBuffer(string key)
        {
            SetKey(key);
            _temp.Clear();
            SetData(key, _temp);
        }

        public void RemoveProduct(string key, Guid productId)
        {
            SetKey(key);
            ProductPackagingSummary delep = _temp.FirstOrDefault(p => p.Product.Id == productId);
            _temp.Remove(delep);
            SetData(key, _temp);
        }

        public List<PackagingSummary> GetProductSummaryByProduct(Guid productId, decimal quantity, bool isOneToOne = false)
        {
            List<ProductPackagingSummary> priceSummary = new List<ProductPackagingSummary>();
            List<PackagingSummary> _proccesTemp = new List<PackagingSummary>();
            Product product = _productRepository.GetById(productId);

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
            Product product = _productRepository.GetById(productId);
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
    }
}
