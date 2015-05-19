using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class ProductDiscountGroupImporterService:BaseImporterService,IProductDiscountGroupImporterService
    {
        private readonly CokeDataContext _context;
        private readonly IProductDiscountGroupRepository _productDiscountGroupRepository;
        private readonly IDiscountGroupRepository _discountGroupRepository;
        private readonly IProductRepository _productRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<Guid,string>_productList= new Dictionary<Guid, string>();
      
        public ProductDiscountGroupImporterService(IProductDiscountGroupRepository productDiscountGroupRepository, CokeDataContext context, IDiscountGroupRepository discountGroupRepository, IProductRepository productRepository)
        {
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _context = context;
            _discountGroupRepository = discountGroupRepository;
            _productRepository = productRepository;
        }

        public ImportResponse Save(List<ProductDiscountGroupItemImport> imports)
        {
            _productList =_context.tblProduct.Where(s => s.IM_Status == (int) EntityStatus.Active).ToDictionary(p => p.id, c => c.ProductCode.ToLower());
          var mappingValidationList = new List<string>(); 
            List<ProductGroupDiscount> productGroupDiscounts = imports.Select(s=>Map(s,mappingValidationList)).ToList();
            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }

            List<ValidationResultInfo> validationResults = productGroupDiscounts.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<ProductGroupDiscount> changedProductPricingTiers = HasChanged(productGroupDiscounts);

            foreach (var entity in changedProductPricingTiers)
            {
                DateTime dt = DateTime.Now;
                tblProductDiscountGroup tblPDG = _context.tblProductDiscountGroup.FirstOrDefault(n => n.id==entity.Id);
                if (tblPDG == null)
                {
                    tblPDG = new tblProductDiscountGroup
                    {
                        IM_Status = (int)EntityStatus.Active,
                        IM_DateCreated = dt,
                        id = entity.Id
                    };
                    _context.tblProductDiscountGroup.AddObject(tblPDG);
                }
                var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
                if (tblPDG.IM_Status != (int)entityStatus)
                    tblPDG.IM_Status = (int)entity._Status;
                tblPDG.DiscountRate = entity.DiscountRate;
                tblPDG.EndDate = entity.EndDate;
                tblPDG.EffectiveDate = entity.EffectiveDate;
                tblPDG.Quantity = entity.Quantity;
                tblPDG.ProductRef = entity.Product.ProductId;
                tblPDG.IM_DateLastUpdated = dt;
                tblPDG.DiscountGroup = entity.GroupDiscount.Id;
                //_productDiscountGroupRepository.Save(changedProductPricingTier);
            }
            _context.SaveChanges();

            return new ImportResponse() { Status = true, Info = changedProductPricingTiers.Count + " Product Group Discount Successfully Imported" };
        }


        protected ValidationResultInfo Validate(ProductGroupDiscount productGroupDiscount)
        {
            return _productDiscountGroupRepository.Validate(productGroupDiscount);
        }
        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                var split = deletedCode.Split('|');
                if(split.Length<2)
                    continue;
                string discountGroupCode = split[0].ToString();
                string productCode = split[1].ToString();
                try
                {
                    var productDiscountGroupId = _context.tblProductDiscountGroupItem.Where(p => p.tblProductDiscountGroup.tblDiscountGroup.Code == discountGroupCode && p.tblProduct.ProductCode == productCode).Select(p => p.id).FirstOrDefault();

                    var discountGroup = _productDiscountGroupRepository.GetById(productDiscountGroupId);
                    if (discountGroup != null)
                    {
                        _productDiscountGroupRepository.SetAsDeleted(discountGroup);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Product Group Discount Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Product Group Discount Deleted Succesfully", Status = true };
        }


        private List<ProductGroupDiscount> HasChanged(List<ProductGroupDiscount> productGroupDiscounts)
        {
            var changedProductGroupDiscounts = new List<ProductGroupDiscount>();
            foreach (var productGroupDiscount in productGroupDiscounts)
            {
                var entity = _productDiscountGroupRepository.GetById(productGroupDiscount.Id);
                if (entity == null)
                {
                    changedProductGroupDiscounts.Add(productGroupDiscount);
                    continue;
                }
             

                var previousRate = entity.DiscountRate;
                var currentRate = productGroupDiscount.DiscountRate;
                bool hasChanged=previousRate!=currentRate;
                 
                if (hasChanged)
                {
                    changedProductGroupDiscounts.Add(productGroupDiscount);
                }
            }
            return changedProductGroupDiscounts;
        }

        protected ProductGroupDiscount Map(ProductDiscountGroupItemImport productDiscountGroupItemImport, List<string> mappingvalidationList)
        {
            var discountGroup = _discountGroupRepository.GetByCode(productDiscountGroupItemImport.ProductDiscountGroupCode);
            if (discountGroup==null)
            {
                mappingvalidationList.Add(string.Format((string) "Invalid Discount Group Code {0}", (object) productDiscountGroupItemImport.ProductDiscountGroupCode));
                return null;
            }
            if (!_productList.ContainsValue(productDiscountGroupItemImport.ProductCode.ToLower()))
            {
                mappingvalidationList.Add(string.Format((string) "Invalid Product Code {0}", (object) productDiscountGroupItemImport.ProductCode));
                return null;
            }
            var productId =_productList.FirstOrDefault(k => k.Value.ToLower() == productDiscountGroupItemImport.ProductCode.ToLower()).Key;


            tblProductDiscountGroup exists = Queryable.FirstOrDefault(_context.tblProductDiscountGroup, n => n.DiscountGroup == discountGroup.Id && n.Quantity == productDiscountGroupItemImport.Quantity && n.ProductRef == productId);


            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var productGroupDiscount = new ProductGroupDiscount(id);

            productGroupDiscount.GroupDiscount = discountGroup; //Check this out
            productGroupDiscount.DiscountRate = productDiscountGroupItemImport.Rate;
            productGroupDiscount.EffectiveDate = productDiscountGroupItemImport.EffectiveDate;
            productGroupDiscount.EndDate = productDiscountGroupItemImport.EndDate;
            productGroupDiscount.Quantity = productDiscountGroupItemImport.Quantity;
            productGroupDiscount.Product = new ProductRef() { ProductId = productId };
            productGroupDiscount.IsByQuantity = productDiscountGroupItemImport.Quantity > 0;
            return productGroupDiscount;


        }
    }
}
