using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class ProductBrandImporterService:BaseImporterService,IProductBrandImporterService
    {
         private IProductBrandRepository _productBrandRepository;
        private ISupplierRepository _supplierRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductBrandImporterService(IProductBrandRepository productBrandRepository, CokeDataContext context, ISupplierRepository supplierRepository)
        {
            _productBrandRepository = productBrandRepository;
            _context = context;
            _supplierRepository = supplierRepository;
        }


        public ImportResponse Save(List<ProductBrandImport> imports)
        {
            List<ProductBrand> productBrands = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productBrands.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
               
            }
            List<ProductBrand> changedProductBrands = HasChanged(productBrands);

            foreach (var changedProductBrand in changedProductBrands)
            {
                _productBrandRepository.Save(changedProductBrand);
            }
            return new ImportResponse() {Status = true, Info = changedProductBrands.Count+" Product Brands Successfully Imported"};
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var productBrandId = _context.tblProductBrand.Where(p => p.code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var productBrand = _productBrandRepository.GetById(productBrandId);
                    if (productBrand != null)
                    {
                        _productBrandRepository.SetAsDeleted(productBrand);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Product Brand Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Product Brand Deleted Succesfully", Status = true };
        }



        private List<ProductBrand> HasChanged(List<ProductBrand> productBrands)
        {
            var changedProductBrands = new List<ProductBrand>();
            foreach (var productBrand in productBrands)
            {
                var entity = _productBrandRepository.GetById(productBrand.Id);
                if(entity==null)
                {
                    changedProductBrands.Add(productBrand);
                    continue;
                }
                var nameHasChanged = entity.Name.ToLower() != productBrand.Name.ToLower();
                var codeHasChanged = entity.Code.ToLower() != productBrand.Code.ToLower();
                var supplierHasChanged = entity.Supplier.Id != productBrand.Supplier.Id;
                bool hasChanged =nameHasChanged  || codeHasChanged||supplierHasChanged;
                
                if(hasChanged)
                {
                    changedProductBrands.Add(productBrand);
                }
            }
            return changedProductBrands;
        }

        protected ValidationResultInfo Validate(ProductBrand productBrand)
        {
            return _productBrandRepository.Validate(productBrand);
        }

        protected ProductBrand Map(ProductBrandImport productBrandImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblProductBrand, p => p.code == productBrandImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var supplier = _supplierRepository.GetByCode(productBrandImport.SupplierCode.ToLower());

            var productBrand = new ProductBrand(id);
            productBrand.Name = productBrandImport.Name;
            productBrand.Code = productBrandImport.Code;
            productBrand.Description = productBrandImport.Description;
            productBrand.Supplier = supplier;
            

            return productBrand;

        }
        
    }
}
