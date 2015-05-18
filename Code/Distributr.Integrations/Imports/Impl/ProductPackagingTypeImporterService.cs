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
    public class ProductPackagingTypeImporterService : BaseImporterService, IProductPackagingTypeImporterService
    {
        private readonly CokeDataContext _context;
        private readonly IProductPackagingTypeRepository _productPackagingTypeRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductPackagingTypeImporterService(IProductPackagingTypeRepository productPackagingTypeRepository, CokeDataContext context)
        {
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _context = context;
        }

        public ImportResponse Save(List<ProductPackagingTypeImport> imports)
        {
            List<ProductPackagingType> productPackagingTypes = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productPackagingTypes.Select(Validate).ToList();

            var invalidProductPackagingTypes = validationResults.Where(p => !p.IsValid).ToList();
            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<ProductPackagingType> changedProductPackagingTypes = HasChanged(productPackagingTypes);

            foreach (var changedProductPackagingType in changedProductPackagingTypes)
            {
                _productPackagingTypeRepository.Save(changedProductPackagingType);
            }
            return new ImportResponse() { Status = true, Info = changedProductPackagingTypes.Count + " Product Packaging Type Successfully Imported" };
        }


        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var productPackagingTypeId = _context.tblProductPackagingType.Where(p => p.code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var productPackagingType = _productPackagingTypeRepository.GetById(productPackagingTypeId);
                    if (productPackagingType != null)
                    {
                        _productPackagingTypeRepository.SetAsDeleted(productPackagingType);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Product Packaging Type Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Product Packaging Type Deleted Succesfully", Status = true };
        }


        private List<ProductPackagingType> HasChanged(List<ProductPackagingType> productPackagingTypes)
        {
            var changedProductPackagingTypes = new List<ProductPackagingType>();
            foreach (var productPackagingType in productPackagingTypes)
            {
                var entity = _productPackagingTypeRepository.GetById(productPackagingType.Id);
                if (entity == null)
                {
                    changedProductPackagingTypes.Add(productPackagingType);
                    continue;
                }
                bool hasChanged = false;
                if (entity.Name.Trim().ToLower() != productPackagingType.Name.Trim().ToLower() || entity.Code.Trim().ToLower() != productPackagingType.Code.Trim().ToLower())
                {
                    hasChanged = true;
                }


                if (hasChanged)
                {
                    changedProductPackagingTypes.Add(productPackagingType);
                }
            }
            return changedProductPackagingTypes;
        }

        private ValidationResultInfo Validate(ProductPackagingType productPackagingType)
        {
            return _productPackagingTypeRepository.Validate(productPackagingType);
        }

        private ProductPackagingType Map(ProductPackagingTypeImport productPackagingTypeImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblProductPackagingType, p => p.code == productPackagingTypeImport.Code);


            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var productPackagingType = new ProductPackagingType(id);

            productPackagingType.Name = productPackagingTypeImport.Name;
            productPackagingType.Code = productPackagingTypeImport.Code;
            productPackagingType.Description = productPackagingTypeImport.Description;
            

            return productPackagingType;
        }
    }
}
