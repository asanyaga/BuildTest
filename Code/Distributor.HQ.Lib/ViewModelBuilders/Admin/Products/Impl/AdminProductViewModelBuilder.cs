using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Impl
{
    public class AdminProductViewModelBuilder : IAdminProductViewModelBuilder
    {
        IProductBrandRepository _productBrandRepository;

        public AdminProductViewModelBuilder(IProductBrandRepository productBrandRepository)
        {
            _productBrandRepository = productBrandRepository;
        }

        public IList<AdminProductBrandViewModel> GetAll()
        {
            var brands = _productBrandRepository.GetAll();
            return brands
                .Select(n => new AdminProductBrandViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description = n.Description,
                    Code = n.Code
                })
                .ToList();
        }

        public AdminProductBrandViewModel Get(Guid id)
        {
            ProductBrand brand = _productBrandRepository.GetById(id);
            if (brand != null) return null;
                
            return Map(brand);
        }

        AdminProductBrandViewModel Map(ProductBrand productBrand)
        {
            return new AdminProductBrandViewModel
                           {
                               Id = productBrand.Id,
                               Name = productBrand.Name,
                               Description = productBrand.Description,
                               Code = productBrand.Code
                           };
        }


        public void Save(AdminProductBrandViewModel adminProductBrandViewModel)
        {
            ProductBrand pb = new ProductBrand (adminProductBrandViewModel.Id)
            {
                Name = adminProductBrandViewModel.Name,
                Description = adminProductBrandViewModel.Description,
                Code = adminProductBrandViewModel.Code
            };
            _productBrandRepository.Save(pb);
        }

       
    }
}
