using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{

    public class PricingImporterService : BaseImporterService, IPricingImporterService
    {
        private IProductPricingRepository _productPricingRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PricingImporterService(IProductPricingRepository productPricingRepository, CokeDataContext context, IProductPricingTierRepository productPricingTierRepository)
        {
            _productPricingRepository = productPricingRepository;
            _context = context;
            _productPricingTierRepository = productPricingTierRepository;
        }

        public ImportResponse Save(List<PricingImport> imports)
        {
            try
            {
                var mappingValidationList = new List<string>();
                List<ProductPricing> productPricings = imports.Select(s=>Map(s,mappingValidationList)).ToList();
                if (mappingValidationList.Any())
                {
                    return new ImportResponse() { Status = false, Info =String.Join(",", mappingValidationList) };

                }

                List<ValidationResultInfo> validationResults = productPricings.Select(Validate).ToList();
                var invalidResults = validationResults.Where(p => !p.IsValid).ToList();
                if (validationResults.Any(p => !p.IsValid))
                {
                    return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

                }
                List<ProductPricing> changedProductPricings = HasChanged(productPricings);

                foreach (var changedProductPricing in changedProductPricings)
                {
                    _productPricingRepository.Save(changedProductPricing);
                }
                return new ImportResponse() { Status = true, Info = changedProductPricings.Count + " Product Pricing Successfully Imported" };
            }
            catch (Exception ex)
            {

                _log.Error("Saving Pricing Error" + ex.ToString());
                return new ImportResponse() { Status = false, Info = ex.ToString() };
            }
            
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {

                try
                {
                    var split = deletedCode.Split('|');
                    if (split.Length < 2)
                        continue;
                    string productCode = split[0];
                    string pricingTierCode = split[1];
                    var pricingId =
                        _context.tblPricing.Where(
                            p => p.tblProduct.ProductCode == productCode && p.tblPricingTier.Code == pricingTierCode)
                            .Select(p => p.id)
                            .FirstOrDefault();

                    var pricing = _productPricingRepository.GetById(pricingId);
                    if (pricing != null)
                    {
                        _productPricingRepository.SetAsDeleted(pricing);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Pricing Delete Error" + ex.ToString());
                }
            }
            return new ImportResponse() { Info = "Pricing Deleted Succesfully", Status = true };
        }

        private List<ProductPricing> HasChanged(List<ProductPricing> productPricings)
        {
            var changedProductPricings = new List<ProductPricing>();
            foreach (var productPricing in productPricings)
            {
                var entity = _productPricingRepository.GetById(productPricing.Id);
                if (entity == null)
                {
                    changedProductPricings.Add(productPricing);
                    continue;
                }

                var currentPrice = Math.Round(productPricing.CurrentSellingPrice, 2);
                var currentfactory = Math.Round(productPricing.CurrentExFactory, 2);

                var pPrice = Math.Round(entity.CurrentSellingPrice, 2);
                var pExF = Math.Round(entity.CurrentExFactory, 2);

                bool hasChanged=false;
                if ((currentPrice != pPrice) || (currentfactory != pExF))
                    hasChanged = true;
           
                   

                if (hasChanged)
                {
                    changedProductPricings.Add(productPricing);
                }
            }
            return changedProductPricings;
        }

        protected ValidationResultInfo Validate(ProductPricing country)
        {
            return _productPricingRepository.Validate(country);
        }

        protected ProductPricing Map(PricingImport pricingImport, List<string> mappingvalidationList )
        {
            
            tblPricing exists = null;
            var product = Queryable.FirstOrDefault(_context.tblProduct, p => p.ProductCode == pricingImport.ProductCode);
            if (product == null) { mappingvalidationList.Add(string.Format((string) "Invalid Product Code {0}", (object) pricingImport.ProductCode)); }
            var pricingTier = _productPricingTierRepository.GetByCode(pricingImport.ProductPricingTierCode); //_context.tblPricingTier.FirstOrDefault(p => p.Code == pricingImport.ProductPricingTierCode);

            if (pricingTier == null) { mappingvalidationList.Add(string.Format((string) "Invalid PricingTier Code {0}", (object) pricingImport.ProductPricingTierCode)); }

            if(product!=null && pricingTier!=null)
            {
                exists = _context.tblPricing.FirstOrDefault(p => p.ProductRef == product.id && p.Tier == pricingTier.Id);
            }
            
            
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var productPricing = new ProductPricing(id);

            productPricing.ProductRef =product!=null? new ProductRef { ProductId = product.id }:null;
            productPricing.Tier = pricingTier;
           

            var productPricingItems = new List<ProductPricing.ProductPricingItem>();

            var productPricingItem = new ProductPricing.ProductPricingItem(new Guid())
                                         {
                                             EffectiveDate = pricingImport.EffectiveDate,
                                             ExFactoryRate = pricingImport.ExFactoryRate,
                                             SellingPrice = pricingImport.SellingPrice
                                         };
            productPricingItems.Add(productPricingItem);
            productPricing.ProductPricingItems = productPricingItems;

            return productPricing;
        }
    }
}
