using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;

namespace Distributr.Integrations.Imports.Impl
{
    public class ProductDiscountImporterService :BaseImporterService, IProductDiscountImporterService
    {
        private readonly CokeDataContext _context;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private Dictionary<Guid, string> _productList = new Dictionary<Guid, string>();
        private IProductPricingTierRepository _productPricingTierRepository;
        public ProductDiscountImporterService(CokeDataContext context, IProductDiscountRepository productDiscountRepository, IProductPricingTierRepository productPricingTierRepository)
        {
            _context = context;
            _productDiscountRepository = productDiscountRepository;
            _productPricingTierRepository = productPricingTierRepository;
        }

        public ImportResponse Save(List<ProductDiscountImport> imports)
        {
            _productList = _context.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).ToDictionary(p => p.id, c => c.ProductCode.ToLower());
            var mappingValidationList = new List<string>();
            List<ProductDiscount> productDiscounts = imports.Select(s => Map(s, mappingValidationList)).ToList();
            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }
            List<ValidationResultInfo> validationResults = productDiscounts.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<ProductDiscount> changedProductPricingTiers = HasChanged(productDiscounts);
            foreach (var entity in changedProductPricingTiers)
            {
                _productDiscountRepository.Save(entity);
            }
            return new ImportResponse() { Status = true, Info = changedProductPricingTiers.Count + " Product  Discount Successfully Imported" };
     
        }
        private List<ProductDiscount> HasChanged(List<ProductDiscount> productGroupDiscounts)
        {
            var changedProductGroupDiscounts = new List<ProductDiscount>();
            foreach (var productGroupDiscount in productGroupDiscounts)
            {
                var entity = _productDiscountRepository.GetById(productGroupDiscount.Id);
                if (entity == null)
                {
                    changedProductGroupDiscounts.Add(productGroupDiscount);
                    continue;
                }

                var item = productGroupDiscount.DiscountItems.FirstOrDefault();
                if (item != null)
                {
                    var previousRate = entity.CurrentDiscountRate(item.Quantity);
                    var currentRate = productGroupDiscount.CurrentDiscountRate(item.Quantity);
                    bool hasChanged = previousRate != currentRate;

                    if (hasChanged)
                    {
                        changedProductGroupDiscounts.Add(productGroupDiscount);
                    }
                }
            }
            return changedProductGroupDiscounts;
        }
        protected ValidationResultInfo Validate(ProductDiscount productGroupDiscount)
        {
            return _productDiscountRepository.Validate(productGroupDiscount);
        }
        protected ProductDiscount Map(ProductDiscountImport productDiscountImport,List<string> mappingvalidationList)
        {
            if (!_productList.ContainsValue(productDiscountImport.ProductCode.ToLower()))
            {
                mappingvalidationList.Add(string.Format((string) "Invalid Product Code {0}", (object) productDiscountImport.ProductCode));
                return null;
            }
            var tier = _productPricingTierRepository.GetByCode(productDiscountImport.TierCode);
            if (tier== null)
            {
                mappingvalidationList.Add(string.Format((string) "Invalid Tier Code {0}", (object) productDiscountImport.TierCode));
                return null;
            }
            var productId = _productList.FirstOrDefault(k => k.Value.ToLower() == productDiscountImport.ProductCode.ToLower()).Key;
            tblDiscounts exists = _context.tblDiscounts.FirstOrDefault(n => n.ProductRef == productId && n.TierId == tier.Id);


            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var productDiscount = new ProductDiscount(id);
            productDiscount.ProductRef = new ProductRef {ProductId = productId};
            productDiscount.Tier = tier;
            productDiscount.DiscountItems.Add(new ProductDiscount.ProductDiscountItem(Guid.NewGuid())
            {
                DiscountRate = productDiscountImport.Rate,
                EffectiveDate = productDiscountImport.EffectiveDate,
                EndDate = productDiscountImport.EndDate,
            });

            return productDiscount;
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            throw new NotImplementedException();
        }
    }
}
