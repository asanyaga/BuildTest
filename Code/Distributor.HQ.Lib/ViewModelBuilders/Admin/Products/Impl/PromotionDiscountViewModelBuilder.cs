using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class PromotionDiscountViewModelBuilder : IPromotionDiscountViewModelBuilder
    {
       IProductRepository _productRepository;
       IPromotionDiscountRepository _promotionDiscountRepository;
       IPromotionDiscountFactory _promotionDiscountFactory;
       public PromotionDiscountViewModelBuilder(IProductRepository productRepository, IPromotionDiscountRepository promotionDiscountRepository, IPromotionDiscountFactory promotionDiscountFactory)
       {
           _productRepository = productRepository;
           _promotionDiscountRepository = promotionDiscountRepository;
           _promotionDiscountFactory = promotionDiscountFactory;
       }
       public PromotionDiscountViewModel Get(Guid id)
        {
            PromotionDiscount foc = _promotionDiscountRepository.GetById(id);
            if (foc == null) return null;
            return Map(foc);
        }

        public void Save(PromotionDiscountViewModel focDiscount)
        {
            PromotionDiscount foc = _promotionDiscountRepository.GetByProductAndQuantity(focDiscount.Product, focDiscount.ParentProductQuantity);
            if (foc == null)
            {
                foc = _promotionDiscountFactory.CreateFreeOfChargeDiscount(
                    new ProductRef { ProductId = focDiscount.Product },
                    focDiscount.FreeProduct,
                    focDiscount.ParentProductQuantity,
                    focDiscount.FreeOfChargeProductQuantity == null
                        ? 0 : focDiscount.FreeOfChargeProductQuantity.Value,
                    focDiscount.EffectiveDate, focDiscount.DiscountRate,
                    focDiscount.EndDate);
            }
            else
            {
                foc.PromotionDiscountItems.Add(new PromotionDiscount.PromotionDiscountItem(Guid.NewGuid())
                    {
                        EffectiveDate =  focDiscount.EffectiveDate,
                        FreeOfChargeProduct = new ProductRef { ProductId = focDiscount.FreeProduct.Value },
                        FreeOfChargeQuantity = focDiscount.FreeOfChargeProductQuantity == null 
                            ? 0 : focDiscount.FreeOfChargeProductQuantity.Value,
                        ParentProductQuantity = focDiscount.ParentProductQuantity,
                        DiscountRate = focDiscount.DiscountRate,
                        EndDate = focDiscount.EndDate,
                    });
            }
            _promotionDiscountRepository.Save(foc);
        }

        public void SetDeleted(Guid id)
        {
            PromotionDiscount pd = _promotionDiscountRepository.GetById(id);
            if (pd != null)
                _promotionDiscountRepository.SetAsDeleted(pd);
        }

        public List<PromotionDiscountViewModel> GetAll(bool inactive = false)
        {
            var list = _promotionDiscountRepository.GetAll(inactive)
                .Where(p => p.PromotionDiscountItems.Any())
                .Select(Map).ToList();

            return list;
        }

        public List<PromotionDiscountViewModel> Search(string srcParam, bool inactive = false)
        {
            var foundProductIds = _productRepository.GetAll().Where(n => n.Description.ToLower().Contains(srcParam.ToLower())).Select(n => n.Id).ToArray();

            return _promotionDiscountRepository.GetAll(inactive).Where(p => p.PromotionDiscountItems.Any()).Select(n => Map(n)).ToList(); ;
            //.ToList().Where(n => (foundProductIds.Contains(n.ProductRef.ProductId)) || (foundProductIds.Contains(n.PromotionDiscountItems.FirstOrDefault().FreeOfChargeProduct.ProductId)) || (n.CurrentEffectiveDate.ToString("dd-MMM-yyyy").StartsWith(srcParam))).Select(n => Map(n)).ToList();
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().OfType<SaleProduct>().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);

        }

        public void AddFreeOfChargeDiscount(Guid focId, int parentProductQuantity, Guid freeOfChargeProduct, int freeOfChargeQuantity, DateTime effectiveDate, decimal DiscountRate, DateTime endDate)
        {
            if (effectiveDate.Date < DateTime.Now.Date)
                throw new Exception("Effective date cant be  in the past");
            _promotionDiscountRepository.AddFreeOfChargeDiscount(focId, parentProductQuantity, freeOfChargeProduct , freeOfChargeQuantity, effectiveDate,DiscountRate, endDate);
        }

        PromotionDiscountViewModel Map(PromotionDiscount foc)
        {
            PromotionDiscountViewModel pdvm = new PromotionDiscountViewModel();
            var discountItem = foc.PromotionDiscountItems.FirstOrDefault();
            pdvm.EffectiveDate = discountItem != null ? discountItem.EffectiveDate : DateTime.Parse("01-jan-1900");
            pdvm.EndDate = discountItem != null ? discountItem.EndDate : DateTime.Parse("01-jan-1900");
            pdvm.FreeOfChargeProductQuantity = discountItem != null ? discountItem.FreeOfChargeQuantity : 0;
            pdvm.FreeProduct = (discountItem != null && discountItem.FreeOfChargeProduct != null)
                ? discountItem.FreeOfChargeProduct.ProductId : Guid.Empty;
            pdvm.Product = foc.ProductRef.ProductId;
            pdvm.ParentProductQuantity = discountItem != null ? discountItem.ParentProductQuantity : 0;
            var currentFreeOfChargeProduct = discountItem != null && discountItem.FreeOfChargeProduct != null
                ? discountItem.FreeOfChargeProduct : null;
            if (currentFreeOfChargeProduct != null)
            {
                if (discountItem.FreeOfChargeProduct.ProductId != Guid.Empty)
                {
                    pdvm.FreeProductName = discountItem.FreeOfChargeProduct == null
                        ? "None" 
                        : _productRepository.GetById(discountItem.FreeOfChargeProduct.ProductId).Description;
                }
            }
            pdvm.ProductName = _productRepository.GetById(foc.ProductRef.ProductId).Description;
            pdvm.Id = foc.Id;
            pdvm.isActive = foc._Status == EntityStatus.Active ? true : false;
            pdvm.DiscountRate = discountItem != null ? discountItem.DiscountRate : 0;
            return pdvm;
        }

       public void DeacivateLineItem(Guid id)
       {
           _promotionDiscountRepository.DeactivateLineItem(id);
       }

       public void ThrowIfExists(PromotionDiscountViewModel vm)
       {
           PromotionDiscount pd = _promotionDiscountRepository.GetByProductAndQuantity(vm.Product, vm.ParentProductQuantity);
           if (pd == null || !pd.PromotionDiscountItems.Any()) return;
           ValidationResultInfo vri = pd.BasicValidation();
           vri.Results.Add(new ValidationResult("Discount already set for the sale product and quantity"));
           throw new DomainValidationException(vri, "Failed to validate promotion discount");
       }

       public QueryResult<PromotionDiscountViewModel> Query(QueryStandard q)
       {
           var queryResult = _promotionDiscountRepository.Query(q);

           var result = new QueryResult<PromotionDiscountViewModel>();

           result.Count = queryResult.Count;
           result.Data = queryResult.Data.OfType<PromotionDiscount>().Select(Map).ToList();

           return result;
       }
    }
}
