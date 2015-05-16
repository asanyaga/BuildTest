using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class ProductTypeImporterService:BaseImporterService,IProductTypeImporterService
    {
        private IProductTypeRepository _productTypeRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductTypeImporterService(CokeDataContext context, IProductTypeRepository productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
            _context = context;
           
        }


        public ImportResponse Save(List<ProductTypeImport> imports)
        {
            List<ProductType> productTypes = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productTypes.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
               
            }
            List<ProductType> changedProductTypes = HasChanged(productTypes);

            foreach (var changedProductType in changedProductTypes)
            {
                _productTypeRepository.Save(changedProductType);
            }
            return new ImportResponse() {Status = true, Info = changedProductTypes.Count+" Product Type Successfully Imported"};
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var productTypeId = _context.tblProductType.Where(p => p.code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var productType = _productTypeRepository.GetById(productTypeId);
                    if (productType != null)
                    {
                        _productTypeRepository.SetAsDeleted(productType);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("ProductType Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "ProductType Deleted Succesfully", Status = true };
        }


        private List<ProductType> HasChanged(List<ProductType> productTypes)
        {
            var changedProductTypes = new List<ProductType>();
            foreach (var productType in productTypes)
            {
                var entity = _productTypeRepository.GetById(productType.Id);
                if(entity==null)
                {
                    changedProductTypes.Add(productType);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != productType.Name.ToLower() ||
                                  entity.Code.ToLower() != productType.Code.ToLower();
                
                if(hasChanged)
                {
                    changedProductTypes.Add(productType);
                }
            }
            return changedProductTypes;
        }

        protected ValidationResultInfo Validate(ProductType productType)
        {
            return _productTypeRepository.Validate(productType);
        }

        protected ProductType Map(ProductTypeImport productTypeImport)
        {
            var exists = _context.tblProductFlavour.FirstOrDefault(p => p.code == productTypeImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

           
            var productType = new ProductType(id);
            productType.Name = productTypeImport.Name;
            productType.Code = productTypeImport.Code;
            productType.Description = productTypeImport.Description;
            

            return productType;

        }
    }
}
