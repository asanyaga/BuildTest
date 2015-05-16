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
    public class AdminProductPackagingTypeViewModelBuilder : IAdminProductPackagingTypeViewModelBuilder
    {
        IProductPackagingTypeRepository _productPackagingTypeRepository;
        public AdminProductPackagingTypeViewModelBuilder(IProductPackagingTypeRepository productPackagingTypeRepository)
        {
            _productPackagingTypeRepository = productPackagingTypeRepository;
        }
       public IList<AdminProductPackagingTypeViewModel> GetAll(bool inactive = false)
        {
            var packagingType = _productPackagingTypeRepository.GetAll(inactive);
            return packagingType
                .Select(n => new AdminProductPackagingTypeViewModel { 
                Id=n.Id,
                Name=n.Name,
                Description = n.Description,
                isActive = n._Status == EntityStatus.Active ? true : false
                }).ToList();
        }

       public AdminProductPackagingTypeViewModel Get(Guid id)
        {
            ProductPackagingType packagingType = _productPackagingTypeRepository.GetById(id);
            if (packagingType == null) return null;
               
            return Map(packagingType);
        }
        AdminProductPackagingTypeViewModel Map(ProductPackagingType packagingType)
        {
            return new AdminProductPackagingTypeViewModel
            {
                Id=packagingType.Id,
                Name=packagingType.Name,
                Description=packagingType.Description,
                isActive = packagingType._Status == EntityStatus.Active ? true : false,
                Code = packagingType.Code
            };
        }
        public void Save(AdminProductPackagingTypeViewModel adminProductPackagingTypeViewModel)
        {
            ProductPackagingType packType = new ProductPackagingType(adminProductPackagingTypeViewModel.Id)
            {
                Name = adminProductPackagingTypeViewModel.Name,
                Description = adminProductPackagingTypeViewModel.Description,
                Code = adminProductPackagingTypeViewModel.Code,
            };
            _productPackagingTypeRepository.Save(packType);
        }

        public void SetInActive(Guid id)
        {
            ProductPackagingType packType = _productPackagingTypeRepository.GetById(id);
            _productPackagingTypeRepository.SetInactive(packType);
        }

        public void SetActive(Guid id)
        {
            ProductPackagingType ppt = _productPackagingTypeRepository.GetById(id);
            _productPackagingTypeRepository.SetActive(ppt);
        }

        public void SetAsDeleted(Guid id)
        {
            ProductPackagingType ppt = _productPackagingTypeRepository.GetById(id);
            _productPackagingTypeRepository.SetAsDeleted(ppt);
        }

        public QueryResult<AdminProductPackagingTypeViewModel> Query(QueryStandard query)
        {
            var queryResult = _productPackagingTypeRepository.Query(query);
            var result = new QueryResult<AdminProductPackagingTypeViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }


        public IList<AdminProductPackagingTypeViewModel> Search(string searchParam,bool inactive = false)
        {
            var items = _productPackagingTypeRepository.GetAll().ToList().Where(n => (n.Name!=null &&n.Name.ToLower().Contains(searchParam.ToLower())) || (n.Description!=null && n.Description.ToLower().Contains(searchParam.ToLower()))).ToList();
            return items
                .Select(n=>new AdminProductPackagingTypeViewModel 
                {
                     Description=n.Description,
                     Name=n.Name,
                     isActive = n._Status == EntityStatus.Active ? true : false,
                      Id=n.Id,
                      Code = n.Code
                      
                }
                ).ToList();
        }
    }
}
