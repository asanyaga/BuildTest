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
    public class ProductPackagingImporterService : BaseImporterService, IProductPackagingImporterService
    {
        private readonly CokeDataContext _context;
        private readonly IProductPackagingRepository _productPackagingRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductPackagingImporterService(IProductPackagingRepository productPackagingRepository, CokeDataContext context)
        {
            _productPackagingRepository = productPackagingRepository;
            _context = context;
        }

        public ImportResponse Save(List<ProductPackagingImport> imports)
        {
            List<ProductPackaging> productPackagings = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productPackagings.Select(Validate).ToList();

            var invalidProductPackagings = validationResults.Where(p => !p.IsValid).ToList();
            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<ProductPackaging> changedProductPackagings = HasChanged(productPackagings);

            foreach (var changedProductPackaging in changedProductPackagings)
            {
                _productPackagingRepository.Save(changedProductPackaging);
            }
            return new ImportResponse() { Status = true, Info = changedProductPackagings.Count + " Product Packaging Successfully Imported" };
        }


        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var productPackagingId = _context.tblProductPackaging.Where(p => p.code == deletedCode).Select(p => p.Id).FirstOrDefault();

                    var productPackaging = _productPackagingRepository.GetById(productPackagingId);
                    if (productPackaging != null)
                    {
                        _productPackagingRepository.SetAsDeleted(productPackaging);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Product Packaging Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Product Packaging Deleted Succesfully", Status = true };
        }


        private List<ProductPackaging> HasChanged(List<ProductPackaging> productPackagings)
        {
            var changedProductPackagings = new List<ProductPackaging>();
            foreach (var productPackaging in productPackagings)
            {
                var entity = _productPackagingRepository.GetById(productPackaging.Id);
                if (entity == null)
                {
                    changedProductPackagings.Add(productPackaging);
                    continue;
                }
                bool hasChanged = false;
                if (entity.Name.Trim().ToLower() != productPackaging.Name.Trim().ToLower() || entity.Code.Trim().ToLower() != productPackaging.Code.Trim().ToLower())
                {
                    hasChanged = true;
                }


                if (hasChanged)
                {
                    changedProductPackagings.Add(productPackaging);
                }
            }
            return changedProductPackagings;
        }

        private ValidationResultInfo Validate(ProductPackaging productPackaging)
        {
            return _productPackagingRepository.Validate(productPackaging);
        }

        private ProductPackaging Map(ProductPackagingImport productPackagingImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblProductPackaging, p => p.code == productPackagingImport.Code);


            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var productPackaging = new ProductPackaging(id);

            productPackaging.Name = productPackagingImport.Name;
            productPackaging.Code = productPackagingImport.Code;
            productPackaging.Description = productPackagingImport.Description;


            return productPackaging;
        }
    }
}