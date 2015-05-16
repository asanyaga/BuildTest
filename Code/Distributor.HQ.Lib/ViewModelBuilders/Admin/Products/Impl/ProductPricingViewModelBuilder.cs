using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class ProductPricingViewModelBuilder:IProductPricingViewModelBuilder
    {
        IProductPricingRepository _productPricingRepository;
        IProductPricingFactory _productPricingFactory;
        IProductRepository _productRepository;
        IProductPricingTierRepository _productPricingTierRepository;
       
        public ProductPricingViewModelBuilder
            (
            IProductPricingRepository productPricingRepository,
            IProductPricingFactory productPricingFactory,
            IProductRepository productRepository,
            IProductPricingTierRepository productPricingTierRepository
            )
        {
            _productPricingRepository = productPricingRepository;
            _productPricingFactory = productPricingFactory;
            _productPricingTierRepository = productPricingTierRepository;
            _productRepository = productRepository;
        }

        public IList<ProductPricingViewModel> GetAll(bool inactive = false)
        {
            var productPricing=_productPricingRepository.GetAll(inactive)
                .Select(s => Map(s)).OrderBy(o => o.ProductName).ThenBy(t => t.TierName).ToList();
            return productPricing;// ;
        }
        private  ProductPricingViewModel Map(ProductPricing productPricing)
        {
          
            ProductPricingViewModel productPricingVm = new ProductPricingViewModel();
            {
               productPricingVm.CurrentEffectiveDate = productPricing.CurrentEffectiveDate;
                productPricingVm.CurrentExFactory = productPricing.CurrentExFactory;
               productPricingVm. CurrentSellingPrice = productPricing.CurrentSellingPrice;
               productPricingVm. Id = productPricing.Id;
               if (productPricing.ProductRef.ProductId != null)
               {
                   productPricingVm.ProductId =  productPricing.ProductRef.ProductId;
                   
               }
               productPricingVm.ProductName = _productRepository.GetById(productPricing.ProductRef.ProductId).Description;
               if (_productRepository.GetById(productPricing.ProductRef.ProductId).Description != null)
               {
                   productPricingVm.ProductName = _productRepository.GetById(productPricing.ProductRef.ProductId).Description;
               }
               productPricingVm. TierId = productPricing.Tier.Id;
               productPricingVm. TierName = productPricing.Tier.Name;
               productPricingVm.Active = productPricing._Status == EntityStatus.Active ? true : false;
            };
            return productPricingVm;
        }
        public ProductPricingViewModel Get(Guid id)
        {
            return Map( _productPricingRepository.GetById(id));
        }

        public void Save(ProductPricingViewModel productPricingViewModel)
        {
            ProductPricing pp=_productPricingRepository.GetById(productPricingViewModel.Id) ;
            ValidationResultInfo vri = productPricingViewModel.BasicValidation();
            if (pp == null)
            {
                var allPricing = _productPricingRepository.GetAll(true)
                    .Where(n => n.ProductRef.ProductId == productPricingViewModel.ProductId && n.Tier.Id == productPricingViewModel.TierId)
                    .Select(n => Map(n)).ToList();
                if (allPricing.Count > 0)
                {
                    throw new DomainValidationException(vri, "Pricing exists for product:");
                }
                else
                {
                    pp = _productPricingFactory.CreateProductPricing
                        (
                          productPricingViewModel.ProductId,
                          productPricingViewModel.TierId,
                          productPricingViewModel.CurrentExFactory,
                          productPricingViewModel.CurrentSellingPrice,
                          productPricingViewModel.CurrentEffectiveDate
                        );
                    _productPricingRepository.Save(pp);
                   
                    //GetProductPricings(productPricingViewModel.ProductId,productPricingViewModel.CurrentExFactory);
                }
            }
            else
            {
                _productPricingRepository.AddProductPricing
                    (
                     productPricingViewModel.Id,
                     productPricingViewModel.CurrentExFactory,
                     productPricingViewModel.CurrentSellingPrice,
                     productPricingViewModel.CurrentEffectiveDate
                    );
            }
        }
        private void GetProductPricings(Guid productId, decimal exFactory)
        {
            List<ProductPricing> qry = _productPricingRepository.GetAll().Where(n => n.ProductRef.ProductId == productId).ToList();
            foreach (ProductPricing pp in qry)
            {
                ProductPricing pPricing = _productPricingRepository.GetById(pp.Id);
               // pp.CurrentExFactory = exFactory;
                pPricing.ProductPricingItems.Add(new ProductPricing.ProductPricingItem(pp.Id) { ExFactoryRate = exFactory, EffectiveDate=pp.CurrentEffectiveDate,SellingPrice=pp.CurrentSellingPrice }
                    );
                
                _productPricingRepository.Save(pPricing);
            }
            //return qry;
        }
        public void SetInactive(Guid id)
        {
            ProductPricing pp = _productPricingRepository.GetById(id);
            _productPricingRepository.SetInactive(pp);
        }
        public void SetAsDeleted(Guid id)
        {
            ProductPricing productPricing = _productPricingRepository.GetById(id);
            _productPricingRepository.SetAsDeleted(productPricing);
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll()
                .Select(s => new { s.Id, s.Description })
                .OrderBy(s=>s.Description)
                .ToDictionary(d=>d.Id,d=>d.Description);
        }

        public Dictionary<Guid, string> TierList()
        {
            return _productPricingTierRepository.GetAll()
               .Select(s => new { s.Id, s.Name })
               
               .OrderBy(s=>s.Name)
               .ToDictionary(d => d.Id, d => d.Name);
        }




        //public IList<ProductPricingViewModel> Search(string searchParam, bool inactive = false)
        //{
            
        //    return _productPricingRepository.GetAll(inactive).ToList().Where(n=>(n.Tier.Name.ToLower().StartsWith(searchParam.ToLower()))).Select(s => Map(s)).ToList();//.Where(n =>n. (n.ProductName.ToLower().StartsWith(searchParam.ToLower())) || (n.TierName.ToLower().StartsWith(searchParam.ToLower())) || (n.CurrentExFactory.ToString().StartsWith(searchParam.ToLower())) || (n.CurrentSellingPrice.ToString().StartsWith(searchParam)) || (n.CurrentEffectiveDate.ToString("dd-MMM-yyyy").StartsWith(searchParam))).Select(s => Map(s)).ToList();
        //}


        public ProductPricingViewModel GetPricingSkipTake(bool inactive = false)
        {
            ProductPricingViewModel productPricingVM = new ProductPricingViewModel
            {
                Items = _productPricingRepository.GetAll(inactive)
                .Select(n => new ProductPricingViewModel.ProductPricingViewModelItems
            {
                TierId = n.Tier.Id,
                ProductId = n.ProductRef.ProductId == null ? Guid.Empty: n.ProductRef.ProductId,
                ProductName = n.ProductRef.ProductId == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description,
                Active = n._Status == EntityStatus.Active ? true : false,
                Id=n.Id,
                 CurrentEffectiveDate=n.CurrentEffectiveDate,
                  CurrentExFactory=n.CurrentExFactory,
                   CurrentSellingPrice=n.CurrentSellingPrice,
                    ErrorText="",
                     TierName=n.Tier.Name
            }).ToList()
            };
            return productPricingVM;
        }


        public ProductPricingViewModel SearchPricing(string searchParam,bool inactive = false)
        {
            var foundProductId = _productRepository.GetAll().Where(n => n.Description.ToLower().StartsWith(searchParam.ToLower())).Select(n => n.Id).ToArray();
            ProductPricingViewModel productPricingVM = new ProductPricingViewModel
            {
                Items = _productPricingRepository.GetAll(inactive).ToList().Where(n => (n.Tier.Name.ToLower().StartsWith(searchParam.ToLower())) || (foundProductId.Contains(n.ProductRef.ProductId)))
                .Select(n => new ProductPricingViewModel.ProductPricingViewModelItems
                {
                    TierId = n.Tier.Id,
                    ProductId = n.ProductRef.ProductId == null ? Guid.Empty : n.ProductRef.ProductId,
                    ProductName = n.ProductRef.ProductId == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description,
                    Active = n._Status == EntityStatus.Active ? true : false,
                    Id = n.Id,
                    CurrentEffectiveDate = n.CurrentEffectiveDate,
                    CurrentExFactory = n.CurrentExFactory,
                    CurrentSellingPrice = n.CurrentSellingPrice,
                    ErrorText = "",
                    TierName = n.Tier.Name
                }).ToList()
            };
            return productPricingVM;
        }

        public QueryResult<ProductPricingViewModel> Query(QueryStandard q)
        {
            var queryResult = _productPricingRepository.Query(q);

            var result = new QueryResult<ProductPricingViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<ProductPricingViewModel> Search(string searchParam, bool inactive = false)
        {
            var foundProductId = _productRepository.GetAll().Where(n => n.Description.ToLower().StartsWith(searchParam.ToLower())).Select(n => n.Id).ToArray();
            var items = _productPricingRepository.GetAll(inactive).ToList().Where(n => (n.Tier.Name.ToLower().StartsWith(searchParam.ToLower())) || (foundProductId.Contains(n.ProductRef.ProductId)));
            return items.Select(n => new ProductPricingViewModel
                {
                    TierId = n.Tier.Id,
                    ProductId = n.ProductRef.ProductId == null ? Guid.Empty : n.ProductRef.ProductId,
                    ProductName = n.ProductRef.ProductId == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description == null ? "" : _productRepository.GetById(n.ProductRef.ProductId).Description,
                    Active = n._Status == EntityStatus.Active ? true : false,
                    Id = n.Id,
                    CurrentEffectiveDate = n.CurrentEffectiveDate,
                    CurrentExFactory = n.CurrentExFactory,
                    CurrentSellingPrice = n.CurrentSellingPrice,
                    ErrorText = "",
                    TierName = n.Tier.Name
                }).ToList();            
        }


        public void SetActive(Guid id)
        {
            ProductPricing productPricing = _productPricingRepository.GetById(id);
            _productPricingRepository.SetActive(productPricing);
        }
    }
}
