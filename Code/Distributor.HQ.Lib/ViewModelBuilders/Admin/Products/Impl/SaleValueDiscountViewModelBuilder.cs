using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class SaleValueDiscountViewModelBuilder:ISaleValueDiscountViewModelBuilder
    {
       IProductPricingTierRepository _productPricingTierRepository;
       ISaleValueDiscountRepository _saleValueDiscountRepository;
       ISaleValueDiscountFactory _saleValueDiscountFactory;
       IProductRepository _productRepository;
       public SaleValueDiscountViewModelBuilder(IProductPricingTierRepository productPricingTierRepository, ISaleValueDiscountRepository saleValueDiscountRepository, ISaleValueDiscountFactory saleValueDiscountFactory)
       {
           _productPricingTierRepository = productPricingTierRepository;
           _saleValueDiscountRepository = saleValueDiscountRepository;
           _saleValueDiscountFactory = saleValueDiscountFactory;
       }
        public IList<SaleValueDiscountViewModel> GetAll(bool inactive = false)
        {
            return _saleValueDiscountRepository.GetAll(inactive)
                .Where(s => s.DiscountItems.Any())
                .ToList().Select(n=>Map(n)).ToList();
        }

        public IList<SaleValueDiscountViewModel> Search(string searchParam, bool inactive = false)
        {
             var foundTierID = _productPricingTierRepository.GetAll()
                 .Where(n => n.Name.ToLower().Contains(searchParam.ToLower())).Select(n => n.Id).ToArray();
             return _saleValueDiscountRepository.GetAll(inactive).ToList().Where(s => s.DiscountItems.Any())
                .Select(n => Map(n)).ToList().Where(s => (s.SaleValue.ToString().StartsWith(searchParam)) || (s.EffectiveDate.ToString("dd-MMM-yyyy").StartsWith(searchParam)) || (s.Rate.ToString().StartsWith(searchParam))|| (foundTierID.Contains(s.TierId))).ToList();
        }

        public SaleValueDiscountViewModel Get(Guid id)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetById(id);
            if (svd == null) return null;
            return Map(svd);
        }

        public void Save(SaleValueDiscountViewModel saleValueDiscountViewModel)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetByAmount(saleValueDiscountViewModel.SaleValue, saleValueDiscountViewModel.TierId);
            if (svd == null)
            {
                svd =  _saleValueDiscountFactory.CreateSaleValueDiscount(
                    saleValueDiscountViewModel.TierId, 
                    saleValueDiscountViewModel.Rate, 
                    saleValueDiscountViewModel.SaleValue, 
                    saleValueDiscountViewModel.EffectiveDate, 
                    saleValueDiscountViewModel.EndDate);
            }
            else
            {
                svd.DiscountItems.Add(new SaleValueDiscount.SaleValueDiscountItem(Guid.NewGuid())
                {
                    EffectiveDate = saleValueDiscountViewModel.EffectiveDate,
                    DiscountValue = saleValueDiscountViewModel.Rate,
                    DiscountThreshold = saleValueDiscountViewModel.SaleValue,
                    EndDate = saleValueDiscountViewModel.EndDate,
                });
            }
            _saleValueDiscountRepository.Save(svd);
        }

        public void AddSaleValueDiscountItem(Guid saleValueDiscountId, decimal discountRate, decimal saleValue, DateTime effectiveDate, DateTime endDate)
        {
            if (effectiveDate < DateTime.Now.Date)
                throw new Exception("Effective date cant be  in the past");
            SaleValueDiscount svd = _saleValueDiscountRepository.GetById(saleValueDiscountId);
            _saleValueDiscountRepository.AddSaleValueDiscount(saleValueDiscountId, effectiveDate, discountRate, saleValue, endDate);
        }

        public void SetInactive(Guid id)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetById(id);
            _saleValueDiscountRepository.SetInactive(svd);
        }
        public void SetAsDeleted(Guid id)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetById(id);
            _saleValueDiscountRepository.SetAsDeleted(svd);
        }
        public void SetActive(Guid id)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetById(id);
            _saleValueDiscountRepository.SetActive(svd);
        }
        
        public Dictionary<Guid, string> TierList()
        {
            return _productPricingTierRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n=>n.Id,n=>n.Name);
        }
        SaleValueDiscountViewModel Map(SaleValueDiscount svd)
        {
            var discountItem = svd.DiscountItems.FirstOrDefault();
            return new SaleValueDiscountViewModel
                       {
                           isActive = svd._Status == EntityStatus.Active ? true : false,
                           Id = svd.Id,
                           SaleValue = discountItem != null ? discountItem.DiscountThreshold : 0,
                           Rate = discountItem != null ? discountItem.DiscountValue : 0,
                           EffectiveDate = discountItem != null ? discountItem.EffectiveDate : DateTime.Parse("01-jan-1900"),
                           EndDate = discountItem != null ? discountItem.EndDate : DateTime.Parse("01-jan-1900"),
                           TierId = svd.Tier.Id,
                           TierName = svd.Tier.Name
                       };
        }

        public void DeacivateLineItem(Guid id)
        {
            _saleValueDiscountRepository.DeactivateLineItem(id);
        }

        public void ThrowIfExists(SaleValueDiscountViewModel svdvm)
        {
            SaleValueDiscount svd = _saleValueDiscountRepository.GetByAmount(svdvm.SaleValue, svdvm.TierId);
            if (svd == null || !svd.DiscountItems.Any()) return;
            ValidationResultInfo vri = svd.BasicValidation();
            vri.Results.Add(new ValidationResult("Discount already set for the sale value and tier, consider editing"));
            throw new DomainValidationException(vri, "Failed to validate sale value discount");
        }

       public QueryResult<SaleValueDiscountViewModel> Query(QueryStandard q)
       {
           var queryResult = _saleValueDiscountRepository.Query(q);

           var result = new QueryResult<SaleValueDiscountViewModel>();

           result.Data = queryResult.Data.OfType<SaleValueDiscount>().Select(Map).ToList();
           result.Count = queryResult.Count;

           return result;
       }
    }
}
