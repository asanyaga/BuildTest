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
    public class ProductFlavourImporterService:BaseImporterService,IProductFlavourImporterService
    {
        private IProductBrandRepository _productBrandRepository;
        private IProductFlavourRepository _productFlavourRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductFlavourImporterService(IProductBrandRepository productBrandRepository, CokeDataContext context, IProductFlavourRepository productFlavourRepository)
        {
            _productBrandRepository = productBrandRepository;
            _context = context;
            _productFlavourRepository = productFlavourRepository;
        }


        public ImportResponse Save(List<ProductFlavourImport> imports)
        {
            List<ProductFlavour> productFlavours = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = productFlavours.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
               
            }
            List<ProductFlavour> changedProductFlavours = HasChanged(productFlavours);

            foreach (var changedProductFlavour in changedProductFlavours)
            {
                _productFlavourRepository.Save(changedProductFlavour);
            }
            return new ImportResponse() {Status = true, Info = changedProductFlavours.Count+" Product Flavours Successfully Imported"};
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var productFlavourId = _context.tblProductFlavour.Where(p => p.code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var productFlavour = _productFlavourRepository.GetById(productFlavourId);
                    if (productFlavour != null)
                    {
                        _productFlavourRepository.SetAsDeleted(productFlavour);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("ProductFlavour Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "ProductFlavour Deleted Succesfully", Status = true };
        }


        private List<ProductFlavour> HasChanged(List<ProductFlavour> productFlavours)
        {
            var changedProductFlavours = new List<ProductFlavour>();
            foreach (var productFlavour in productFlavours)
            {
                var entity = _productFlavourRepository.GetById(productFlavour.Id);
                if(entity==null)
                {
                    changedProductFlavours.Add(productFlavour);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != productFlavour.Name.ToLower() || entity.Code.ToLower() != productFlavour.Code.ToLower()||entity.ProductBrand!=productFlavour.ProductBrand;
                
                if(hasChanged)
                {
                    changedProductFlavours.Add(productFlavour);
                }
            }
            return changedProductFlavours;
        }

        protected ValidationResultInfo Validate(ProductFlavour productFlavour)
        {
            return _productFlavourRepository.Validate(productFlavour);
        }

        protected ProductFlavour Map(ProductFlavourImport productFlavourImport)
        {
            var exists = _context.tblProductFlavour.FirstOrDefault(p => p.code == productFlavourImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var productBrandId =_context.tblProductBrand.Where(p => p.code == productFlavourImport.ProductBrandCode).Select(p => p.id).FirstOrDefault();
            var productBrand = _productBrandRepository.GetById(productBrandId);

            var productFlavour = new ProductFlavour(id);
            productFlavour.Name = productFlavourImport.Name;
            productFlavour.Code = productFlavourImport.Code;
            productFlavour.Description = productFlavourImport.Description;
            productFlavour.ProductBrand = productBrand;
            

            return productFlavour;

        }
    }
}
