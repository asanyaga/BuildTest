using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class ProductGroupDiscountViewModelBuilder:IProductGroupDiscountViewModelBuilder
    {
       IProductDiscountGroupRepository _productDiscountGroupRepository;
       IProductDiscountGroupFactory _productDiscountGroupFactory;
       IProductRepository _productRepository;
       IDiscountGroupRepository _discountGroupRepository;

       public ProductGroupDiscountViewModelBuilder(IProductDiscountGroupRepository productDiscountGroupRepository,
       IProductRepository productRepository,
       IDiscountGroupRepository discountGroupRepository,
           IProductDiscountGroupFactory productDiscountGroupFactory)
       {
           _discountGroupRepository = discountGroupRepository;
           _productDiscountGroupRepository = productDiscountGroupRepository;
           _productRepository = productRepository;
           _productDiscountGroupFactory=productDiscountGroupFactory;
       }

       public ProductDiscountGroupViewModel Get(Guid id)
       {
           ProductGroupDiscount pgd = _productDiscountGroupRepository.GetById(id);
           if (pgd == null) return null;
           return Map(pgd);
       }

       public void Save(ProductDiscountGroupViewModel productGroupDiscount)
       {
           ProductGroupDiscount pgd = _productDiscountGroupRepository.GetByGroupbyProductByQuantity(
               productGroupDiscount.DiscountGroup, productGroupDiscount.Product,productGroupDiscount.Quantity);
           if (pgd == null)
           {
               pgd = _productDiscountGroupFactory.CreateProductGroupDiscount(
                   _discountGroupRepository.GetById(productGroupDiscount.DiscountGroup),
                   new ProductRef
                       {
                           ProductId = productGroupDiscount.Product
                       },
                   productGroupDiscount.discountRate,
                   productGroupDiscount.EffectiveDate,
                   productGroupDiscount.EndDate, productGroupDiscount.IsByQuantity,productGroupDiscount.Quantity);
           }
           else
           {
               pgd.EffectiveDate = productGroupDiscount.EffectiveDate;
               pgd.DiscountRate = productGroupDiscount.discountRate;
               pgd.Product = new ProductRef {ProductId = productGroupDiscount.Product};
               pgd.EndDate = productGroupDiscount.EndDate;
               pgd.Quantity = productGroupDiscount.Quantity;
               pgd.IsByQuantity = productGroupDiscount.IsByQuantity;

        
           }
           _productDiscountGroupRepository.Save(pgd);
       }

       public void SetInactive(Guid id)
        {
            ProductGroupDiscount pgd=_productDiscountGroupRepository.GetById(id);
            _productDiscountGroupRepository.SetInactive(pgd);
        }

        public List<ProductDiscountGroupViewModel> GetAll(bool inactive = false)
        {
            return _productDiscountGroupRepository.GetAll(inactive)
                
                .ToList().Select(n=>Map(n)).ToList();
        }

        public List<ProductDiscountGroupViewModel> Search(string srcParam, bool inactive = false)
        {
            return null;
                
        }

        public Dictionary<Guid, string> DiscountGroupList()
        {
            return _discountGroupRepository.GetAll().ToList().Select(n => new {n.Id,n.Name }).ToDictionary(n=>n.Id,n=>n.Name);
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> ProductListWithoutReturnables()
        {
            return _productRepository.GetAll().ToList().Where(n => (n.GetType() == typeof(SaleProduct)) || (n.GetType() == typeof(ConsolidatedProduct))).Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

        public void AddProductGroupDiscount(Guid discountGroupId, Guid Product, decimal discountRate, DateTime effectiveDate, DateTime endDate)
        {
            //_productDiscountGroupRepository.AddProductGroupDiscount(discountGroupId, new ProductRef { ProductId = Product }, discountRate, effectiveDate, endDate);
        }
        ProductDiscountGroupViewModel Map(ProductGroupDiscount pgd)
        {
            var pd = _productDiscountGroupRepository.GetById(pgd.Id);

            var product = _productRepository.GetById(pd.Product.ProductId);
            if (product == null) return new ProductDiscountGroupViewModel();
            return new ProductDiscountGroupViewModel
                       {
                           EffectiveDate = pd.EffectiveDate,
                           EndDate = pd.EndDate,
                           discountRate = pd.DiscountRate,
                           DiscountGroup = pgd.GroupDiscount.Id,
                           DiscountGroupName = pd.GroupDiscount != null ? pd.GroupDiscount.Name : "",
                           Id = pgd.Id,
                           isActive = pgd._Status == EntityStatus.Active ? true : false,
                           Product = pd.Product.ProductId,
                           ProductName = product.Description,
                           Quantity = pd.Quantity,
                       };
        }

        public void SetLineItemsInactive(Guid id)
        {
            ProductGroupDiscount pgd = _productDiscountGroupRepository.GetById(id);
            _productDiscountGroupRepository.SetLineItemsInactive(pgd);
        }

        public List<ProductDiscountGroupViewModel> GetByDiscountGroup(Guid discountGroup, bool inactive = false)
        {
            return _productDiscountGroupRepository.GetByDiscountGroup(discountGroup,inactive)
                
                .ToList().Select(n => Map(n)).ToList();
        }

        public ProductDiscountGroupViewModel GetByDiscountGroup(Guid id)
        {
            throw new NotImplementedException();
        }

        public void ThrowIfExists(ProductDiscountGroupViewModel pgdvm)
        {
            //ProductGroupDiscount pgd = _productDiscountGroupRepository.GetByDiscountGroup(pgdvm.DiscountGroup, 
            //    pgdvm.Product);
            //if (pgd == null || !pgd.GroupDiscountItems.Any()) return;
            //ValidationResultInfo vri = pgd.BasicValidation();
            //vri.Results.Add(new ValidationResult("Discount already set for the discount group and product"));
            //throw new DomainValidationException(vri, "Failed to validate product group discount");
        }

       public QueryResult<ProductDiscountGroupViewModel> Query(QueryStandard q)
       {
           return null;
       }
    }
}
