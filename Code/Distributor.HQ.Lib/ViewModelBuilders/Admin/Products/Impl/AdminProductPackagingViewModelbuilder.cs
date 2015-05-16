using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class AdminProductPackagingViewModelBuilder : IAdminProductPackagingViewModelbuilder
    {
        IProductPackagingRepository _productPackagingRepository;
        public AdminProductPackagingViewModelBuilder(IProductPackagingRepository productPackaginRepository)
        {
            _productPackagingRepository = productPackaginRepository;
        }
        public IList<AdminProductPackagingViewModel> GetAll(bool inactive = false)
        {
            var packaging = _productPackagingRepository.GetAll(inactive);
            return packaging
                .Select(n => new AdminProductPackagingViewModel
                {
                    Id = n.Id,                    
                    Name = n.Name,
                    Description = n.Description,
                    isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }
        public AdminProductPackagingViewModel Get(Guid id)
        {
            ProductPackaging packaging = _productPackagingRepository.GetById(id);
            if (packaging == null) return null;
               
            return Map(packaging);

        }
        AdminProductPackagingViewModel Map(ProductPackaging productPackaging)
        {
            return new AdminProductPackagingViewModel
            {
                Id = productPackaging.Id,
                Name = productPackaging.Name,
                Description = productPackaging.Description,
                isActive = productPackaging._Status == EntityStatus.Active ? true : false,
                Code = productPackaging.Code,
            };
        }
        public void Save(AdminProductPackagingViewModel adminProductPackagingViewModel)
        {
            ProductPackaging pPackaging = new ProductPackaging(adminProductPackagingViewModel.Id)
            {
                Name = adminProductPackagingViewModel.Name,
                Description = adminProductPackagingViewModel.Description,
                Code = adminProductPackagingViewModel.Code
               
            };
            _productPackagingRepository.Save(pPackaging);
        }


        public void SetInactive(Guid id)
        {
            ProductPackaging pPKG = _productPackagingRepository.GetById(id);
            _productPackagingRepository.SetInactive(pPKG);
        }

        public void SetActive(Guid id)
        {
            ProductPackaging pp = _productPackagingRepository.GetById(id);
            _productPackagingRepository.SetActive(pp);
        }

        public void SetAsDeleted(Guid id)
        {
            ProductPackaging pp = _productPackagingRepository.GetById(id);
            _productPackagingRepository.SetAsDeleted(pp);
        }


        public IList<AdminProductPackagingViewModel> Search(string searchParam, bool inactive = false)
        {
            var items = _productPackagingRepository.GetAll().ToList().Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower())) || (n.Description.ToLower().StartsWith(searchParam.ToLower())));
            return items
                .Select(n => new AdminProductPackagingViewModel 
                {
                 Description=n.Description,
                 Name=n.Name,
                 Id=n.Id,
                 isActive = n._Status == EntityStatus.Active ? true : false,
                 Code = n.Code,
                }).ToList();
        }


        public AdminProductPackagingTypeViewModel GetProductPackagingSkipTake(bool inactive = false)
        {
            throw new NotImplementedException();
        }

        public QueryResult<AdminProductPackagingViewModel> Query(QueryStandard query)
        {
            var queryResult = _productPackagingRepository.Query( query);
            var result = new QueryResult<AdminProductPackagingViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;


        }
    }
}
