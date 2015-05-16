using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility.MasterData;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class ProductBrandViewModelBuilder : IProductBrandViewModelBuilder
    {
        IProductBrandRepository _productBrandRepository;
        ISupplierRepository _supplierRepository;

        public ProductBrandViewModelBuilder(IProductBrandRepository productBrandRepository, ISupplierRepository supplierRepository)
        {
            _productBrandRepository = productBrandRepository;
            _supplierRepository = supplierRepository;
        }

        public IList<ProductBrandViewModel> GetAll(bool inactive = false)
        {
            return _productBrandRepository.GetAll(inactive).Select(s => Map(s)).ToList();
        }

        public ProductBrandViewModel Get(Guid id)
        {
            ProductBrand brand = _productBrandRepository.GetById(id);
            if (brand == null) return null;
              
            return Map(brand);
        }

        public void Save(ProductBrandViewModel productBrandViewModel)
        {
            ProductBrand pb = new ProductBrand(productBrandViewModel.Id)
            {
                Name = productBrandViewModel.Name,
                Description = productBrandViewModel.Description,
                Code = productBrandViewModel.Code,
                 Supplier=_supplierRepository.GetById(productBrandViewModel.SupplierId)
            };
            _productBrandRepository.Save(pb);
        }

        private ProductBrandViewModel Map(ProductBrand productBrand) 
        {
            return new ProductBrandViewModel {
                Id = productBrand.Id,
                Name = productBrand.Name,
                Description = productBrand.Description,
                Code = productBrand.Code,
                isActive = productBrand._Status == EntityStatus.Active ? true : false,
                SupplierId = productBrand.Supplier.Id,
                SupplierName = productBrand.Supplier.Name
            };
            
        }





        public void SetInactive(Guid id)
        {
            ProductBrand pb = _productBrandRepository.GetById(id);
            _productBrandRepository.SetInactive(pb);
           
        }
        
        public void SetActive (Guid id)
        {
            ProductBrand pb = _productBrandRepository.GetById(id);
            _productBrandRepository.SetActive(pb);
        }

        public void SetAsDeleted(Guid id)
        {
            ProductBrand pb = _productBrandRepository.GetById(id);
            _productBrandRepository.SetAsDeleted(pb);
        }

        public QueryResult<ProductBrandViewModel> Query(QueryStandard query)
        {
            var queryResult = _productBrandRepository.Query(query);
            var result = new QueryResult<ProductBrandViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }


        public IList<ProductBrandViewModel> Search(string brandName, bool inactive = false)
        {
           
            var items = _productBrandRepository.GetAll(inactive).Where(n => (n.Code.ToLower().StartsWith(brandName.ToLower())) || (n.Description.ToLower().StartsWith(brandName.ToLower())) || (n.Name.ToLower().StartsWith(brandName.ToLower()))||(n.Supplier.Name.ToLower().StartsWith(brandName.ToLower())));
            return items.Select(n => Map(n)).ToList();           
        }


        public Dictionary<Guid, string> GetSuppliers()
        {
            return _supplierRepository.GetAll()
                .Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
        }

       //public ProductBrandViewModel GetBrandsSkipTake(bool inactive = false)
       // {
       //     ProductBrandViewModel productBrandVM = new ProductBrandViewModel 
       //     {
       //     Items=_productBrandRepository.GetAll(inactive)
       //     .Select(n=>MapSkipTake(n)).ToList()
       //     };
       //     return productBrandVM;
       // }

        //ProductBrandViewModel.ProductBrandViewModelItem MapSkipTake(ProductBrand productBrand)
        //{
        //    return new ProductBrandViewModel.ProductBrandViewModelItem{
        //                   Code = productBrand.Code,
        //                   Description = productBrand.Description,
        //                   Name = productBrand.Name,
        //                   SupplierId = productBrand.Supplier == null ? Guid.Empty : productBrand.Supplier.Id,
        //                   SupplierName = productBrand.Supplier == null ? "" : productBrand.Supplier.Name,
        //                   ErrorText = "",
        //                   Id = productBrand.Id,
        //                   isActive = productBrand._Status,
        //               };
        //}

    }
}
