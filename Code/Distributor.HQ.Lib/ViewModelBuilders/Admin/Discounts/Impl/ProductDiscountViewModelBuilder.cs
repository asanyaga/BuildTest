using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.Discounts;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Discounts.Impl
{
    public class ProductDiscountViewModelBuilder : IProductDiscountViewModelBuilder
    {
        IProductDiscountRepository _productDiscountRepository;
        IProductPricingTierRepository _productPricingTierRepository;
        IProductRepository _productRepository;
        IProductDiscountFactory _productDiscountFactory;

        public ProductDiscountViewModelBuilder(IProductDiscountRepository productDiscountRepository, IProductPricingTierRepository productPricingTierRepository, IProductRepository productRepository, IProductDiscountFactory productDiscountFactory)
        {
            _productDiscountRepository = productDiscountRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _productRepository = productRepository;
            _productDiscountFactory = productDiscountFactory;
        }

        public IList<ProductDiscountViewModel> GetAll(bool inactive=false)
        {
            
            var list = _productDiscountRepository.GetAll(inactive)
                .Where(p => p.DiscountItems.Any())
                .Select(Map).ToList();

            return list;
        }

        public IList<ProductDiscountViewModel.ProductDiscountItemViewModel> GetProductDiscountItem(Guid productDiscountId)
        {
            List<ProductDiscountViewModel.ProductDiscountItemViewModel> itemList =
                new List<ProductDiscountViewModel.ProductDiscountItemViewModel>();
            var productDiscount= _productDiscountRepository.GetById(productDiscountId);
            if(productDiscount!=null)
            {
                itemList = productDiscount.DiscountItems.OrderByDescending(n => n.EffectiveDate).Select(s => MapItem(s, productDiscountId)).ToList();
            }
            return itemList;
        }

        private ProductDiscountViewModel.ProductDiscountItemViewModel MapItem(ProductDiscount.ProductDiscountItem productDiscountItem,Guid productDiscountId)
        {
            return new ProductDiscountViewModel.ProductDiscountItemViewModel
                       {
                           DiscountRate=productDiscountItem.DiscountRate,
                           EffectiveDate=productDiscountItem.EffectiveDate,
                           EndDate = productDiscountItem.EndDate,
                           LineItemId = productDiscountItem.LineItemId,
                           IsActive = productDiscountItem._Status == EntityStatus.Active ? true : false,
                           ProductDiscountId=productDiscountId,
                           IsByQuantity = productDiscountItem.IsByQuantity,
                           Quantity = productDiscountItem.Quantity,
                       };
        }

        ProductDiscountViewModel Map(ProductDiscount discount)
        {
            var tier = _productPricingTierRepository.GetById(discount.Tier.Id);
            var product = _productRepository.GetById(discount.ProductRef.ProductId);
            var discountItem = discount.DiscountItems.FirstOrDefault();
            return new ProductDiscountViewModel
            {
                Id = discount.Id,
                DiscountRate = discountItem != null ? discountItem.DiscountRate : 0,
                EffectiveDate = discountItem != null ? discountItem.EffectiveDate : DateTime.Parse("01-jan-1900"),
                EndDate = discountItem != null ? discountItem.EndDate : DateTime.Parse("01-jan-1900"),
                TierId = discount.Tier.Id,
                IsActive = discount._Status == EntityStatus.Active ? true : false,
                ProductId = discount.ProductRef.ProductId,
                TierName = tier != null ? tier.Name : "",
                ProductName = product != null ? product.Description : "",
            };
        }

        public IList<ProductDiscountViewModel> Search(string searchParam, bool inactive)
        {
            return GetAll().Where(s => (s.DiscountRate.ToString().Contains(searchParam))
                     || s.EffectiveDate.ToString("dd-MMM-yyyy").Contains(searchParam)
                     || s.TierName.ToString().ToLower().Contains(searchParam.ToLower())
                     || s.ProductName.ToString().ToLower().Contains(searchParam.ToLower())
                     ).ToList();
        }

        public ProductDiscountViewModel Get(Guid id)
        {
            ProductDiscount pd = _productDiscountRepository.GetById(id);
            if (pd == null) return null;
            return Map(pd);
        }

        public void Save(ProductDiscountViewModel productDiscountViewModel)
        {
            ProductDiscount pd = _productDiscountRepository.GetProductDiscount(productDiscountViewModel.ProductId, productDiscountViewModel.TierId);
            if (pd == null)
            {
                pd = _productDiscountFactory.CreateProductDiscount(
                            productDiscountViewModel.ProductId,
                            productDiscountViewModel.TierId,
                            productDiscountViewModel.DiscountRate,
                            productDiscountViewModel.EffectiveDate,
                            productDiscountViewModel.EndDate,productDiscountViewModel.IsByQuantity,productDiscountViewModel.Quantity);
                
               
            } 
            else
            {
                pd.DiscountItems.Add(new ProductDiscount.ProductDiscountItem(Guid.NewGuid())
                {
                    EffectiveDate = productDiscountViewModel.EffectiveDate,
                    DiscountRate = productDiscountViewModel.DiscountRate,
                    EndDate = productDiscountViewModel.EndDate,
                    IsByQuantity = productDiscountViewModel.IsByQuantity,
                    Quantity = productDiscountViewModel.Quantity,
                });
            }
            _productDiscountRepository.Save(pd);
        }

        public void AddDiscountItem(Guid productDiscountId, decimal rate, DateTime effectiveDate, DateTime endDate, bool isByQuantity, decimal quantity)
        {
            _productDiscountRepository.AddDiscount(productDiscountId, effectiveDate, rate, endDate,isByQuantity,quantity);
        }

        public void SetInactive(Guid id)
        {
             ProductDiscount pd = _productDiscountRepository.GetById(id);
             if (pd != null)
                 _productDiscountRepository.SetInactive(pd);
        }

        public void SetDeleted(Guid id)
        {
            ProductDiscount pd = _productDiscountRepository.GetById(id);
            if (pd != null)
                _productDiscountRepository.SetAsDeleted(pd);
        }

        public void DeacivateLineItem(Guid id)
        {
            _productDiscountRepository.DeactivateLineItem(id);
        }

        public string GetProductName(Guid productId)
        {
            var productdiscount = _productDiscountRepository.GetById(productId);
            Guid productid= productdiscount != null ? productdiscount.ProductRef.ProductId : Guid.Empty;
            if(productid!=Guid.Empty)
            {
                var product = _productRepository.GetById(productid);
                return product != null ? product.Description : "None";
            }
            return "";
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().OfType<SaleProduct>().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> TierList()
        {
            return _productPricingTierRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n => n.Id, n => n.Name);
        }

        public void ThrowIfExists(ProductDiscountViewModel pdvm)
        {
            ProductDiscount pd = _productDiscountRepository.GetProductDiscount(pdvm.ProductId, pdvm.TierId);
            if (pd == null || !pd.DiscountItems.Any()) return;
            ValidationResultInfo vri = pd.BasicValidation();
            vri.Results.Add(new ValidationResult("Discount already set for the product and tier"));
            throw new DomainValidationException(vri, "Failed to validate product discount");
        }

        public QueryResult<ProductDiscountViewModel> Query(QueryStandard q)
        {
            var queryResult = _productDiscountRepository.Query(q);

            var result = new QueryResult<ProductDiscountViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }
    }
}
