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
    public class PricingTierImporterService:BaseImporterService,IPricingTierImporterService
    {
        private IProductPricingTierRepository _productPricingTierRepository;
        private readonly CokeDataContext _context;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PricingTierImporterService(IProductPricingTierRepository productPricingTierRepository, CokeDataContext context)
        {
            _productPricingTierRepository = productPricingTierRepository;
            _context = context;
        }

        public ImportResponse Save(List<PricingTierImport> imports)
        {
            List<ProductPricingTier> productPricingTiers = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productPricingTiers.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<ProductPricingTier> changedProductPricingTiers = HasChanged(productPricingTiers);

            foreach (var changedProductPricingTier in changedProductPricingTiers)
            {
                _productPricingTierRepository.Save(changedProductPricingTier);
            }
            return new ImportResponse() { Status = true, Info = changedProductPricingTiers.Count + " Product Pricing Tier Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var pricingTierId = _context.tblPricingTier.Where(p => p.Code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var pricingTier = _productPricingTierRepository.GetById(pricingTierId);
                    if (pricingTier != null)
                    {
                        _productPricingTierRepository.SetAsDeleted(pricingTier);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Pricing Tier Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Pricing Tier Deleted Succesfully", Status = true };
        }


        private List<ProductPricingTier> HasChanged(List<ProductPricingTier> productPricingTiers)
        {
            var changedProductPricingTiers = new List<ProductPricingTier>();
            foreach (var productPricingTier in productPricingTiers)
            {
                var entity = _productPricingTierRepository.GetById(productPricingTier.Id);
                if (entity == null)
                {
                    changedProductPricingTiers.Add(productPricingTier);
                    continue;
                }
                bool hasChanged = entity.Description.ToLower() != productPricingTier.Description.ToLower() ||
                                  entity.Code.ToLower() != productPricingTier.Code.ToLower() ||
                                  entity.Name != productPricingTier.Name;

                if (hasChanged)
                {
                    changedProductPricingTiers.Add(productPricingTier);
                }
            }
            return changedProductPricingTiers;
        }

        protected ValidationResultInfo Validate(ProductPricingTier productPricingTier)
        {
            return _productPricingTierRepository.Validate(productPricingTier);
        }

        protected ProductPricingTier Map(PricingTierImport pricingTierImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblPricingTier, p => p.Code == pricingTierImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            

            var productPricingTier = new ProductPricingTier(id);
            productPricingTier.Description = pricingTierImport.Description;
            productPricingTier.Name = pricingTierImport.Name;
            productPricingTier.Code = pricingTierImport.Code;

            return productPricingTier;


        }
    }
}
