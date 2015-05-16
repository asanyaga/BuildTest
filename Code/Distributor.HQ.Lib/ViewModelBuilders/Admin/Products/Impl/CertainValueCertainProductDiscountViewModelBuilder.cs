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
using Distributr.Core.Factory.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class CertainValueCertainProductDiscountViewModelBuilder:ICertainValueCertainProductDiscountViewModelBuilder
    {
       IProductRepository _productRepository;
       ICertainValueCertainProductDiscountRepository _certaiValueCertainProductRepository;
       ICertainValueCertainProductDiscountFactory _certainValueCertainProductFactory;
       public CertainValueCertainProductDiscountViewModelBuilder(IProductRepository productRepository,
       ICertainValueCertainProductDiscountRepository certaiValueCertainProductRepository,
       ICertainValueCertainProductDiscountFactory certainValueCertainProductFactory)
       {
           _certainValueCertainProductFactory = certainValueCertainProductFactory;
           _certaiValueCertainProductRepository = certaiValueCertainProductRepository;
           _productRepository = productRepository;
       }
       public CertainValueCertainProductDiscountViewModel Get(Guid id)
       {
           var cvcp = _certaiValueCertainProductRepository.GetById(id);
           if (cvcp == null) return null;
           return Map(cvcp);
       }

        public void Save(CertainValueCertainProductDiscountViewModel focDiscount)
        {
            CertainValueCertainProductDiscount cvcpd = _certaiValueCertainProductRepository.GetByInitialValue(focDiscount.InitialValue);
            if (cvcpd == null)
            {
                cvcpd = _certainValueCertainProductFactory.CreateCertainValueCertainProductDiscount(
                    new ProductRef { ProductId = focDiscount.Product },
                    focDiscount.Quantity,
                    focDiscount.InitialValue,
                    focDiscount.EffectiveDate,
                    focDiscount.EndDate);
            }
            else
            {
                cvcpd.CertainValueCertainProductDiscountItems.Add(
                    new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(Guid.NewGuid())
                        {
                            Product = new ProductRef {ProductId = focDiscount.Product},
                            CertainValue = focDiscount.InitialValue,
                            Quantity = focDiscount.Quantity,
                            EffectiveDate = focDiscount.EffectiveDate,
                            EndDate = focDiscount.EndDate,
                        });
            }
            _certaiValueCertainProductRepository.Save(cvcpd);
        }

        public void SetInactive(Guid id)
        {
            CertainValueCertainProductDiscount foc = _certaiValueCertainProductRepository.GetById(id);
            _certaiValueCertainProductRepository.SetInactive(foc);
        }
        public void SetActive(Guid id)
        {
            CertainValueCertainProductDiscount foc = _certaiValueCertainProductRepository.GetById(id);
            _certaiValueCertainProductRepository.SetActive(foc);
        }
        public void SetDeleted(Guid id) 
        {
            CertainValueCertainProductDiscount foc = _certaiValueCertainProductRepository.GetById(id);
            _certaiValueCertainProductRepository.SetAsDeleted(foc);
        }

        public List<CertainValueCertainProductDiscountViewModel> GetAll(bool inactive = false)
        {
            return _certaiValueCertainProductRepository
                .GetAll(inactive).Where(c => c.CertainValueCertainProductDiscountItems.Any())
                .ToList().Select(n => Map(n)).ToList();
        }

        public List<CertainValueCertainProductDiscountViewModel> Search(string srcParam, bool inactive = false)
        {
            var foundProductIds = _productRepository.GetAll().Where(n => n.Description.ToLower().Contains(srcParam.ToLower())).Select(n => n.Id).ToArray();

            return _certaiValueCertainProductRepository.GetAll(inactive).Where(c => c.CertainValueCertainProductDiscountItems.Any()).ToList()
                .Where(n =>
                           {
                               var certainValueCertainProductDiscountItem =
                                   n.CertainValueCertainProductDiscountItems.FirstOrDefault();
                               return certainValueCertainProductDiscountItem != null &&
                                      (foundProductIds.Contains(certainValueCertainProductDiscountItem.Product.ProductId));
                           }).Select(n => Map(n)).ToList();

        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().OfType<SaleProduct>().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);

        }

        
        CertainValueCertainProductDiscountViewModel Map(CertainValueCertainProductDiscount foc)
        {
            var item = foc.CertainValueCertainProductDiscountItems.FirstOrDefault();
            return new CertainValueCertainProductDiscountViewModel
            {
                EffectiveDate = item != null ?  item.EffectiveDate : DateTime.Parse("01-jan-1900"),
                EndDate = item != null ? item.EndDate : DateTime.Parse("01-jan-1900"),
                InitialValue  = item != null ? item.CertainValue : 0,
                Quantity      = item != null ? item.Quantity : 0,
                Product       = (item != null && item.Product != null) ? item.Product.ProductId : Guid.Empty,
                ProductName   = (item != null && item.Product != null) ? _productRepository.GetById(item.Product.ProductId).Description : "",
                id            = foc.Id,
                isActive = foc._Status == EntityStatus.Active ? true : false
            };
        }

        public void AddFreeOfChargeDiscount(Guid cvcpId, int ProductQuantity, Guid Product, decimal Value, DateTime effectiveDate, DateTime endDate)
        {
            _certaiValueCertainProductRepository.AddCertainValueCertainProductDiscount(cvcpId, ProductQuantity, new ProductRef { ProductId=Product}, Value, effectiveDate, endDate);
        }

        public void DeacivateLineItem(Guid id)
        {
            _certaiValueCertainProductRepository.DeactivateLineItem(id);
        }

        public void ThrowIfExists(CertainValueCertainProductDiscountViewModel cvcpdvm)
        {
            CertainValueCertainProductDiscount cvcpd = _certaiValueCertainProductRepository.GetByInitialValue(cvcpdvm.InitialValue);
            if (cvcpd == null || !cvcpd.CertainValueCertainProductDiscountItems.Any()) return;
            ValidationResultInfo vri = cvcpd.BasicValidation();
            vri.Results.Add(new ValidationResult("Discount already set for the initial value"));
            throw new DomainValidationException(vri, "Failed to validate certain value certain product discount");
        }

       public QueryResult<CertainValueCertainProductDiscountViewModel> Query(QueryStandard query)
       {
           var queryResult = _certaiValueCertainProductRepository.Query(query);
           var result = new QueryResult<CertainValueCertainProductDiscountViewModel>();
           result.Count  = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;
       }
    }
}
