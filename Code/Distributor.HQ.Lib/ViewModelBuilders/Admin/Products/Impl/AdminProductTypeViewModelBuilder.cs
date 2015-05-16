using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class AdminProductTypeViewModelBuilder :  IProductTypeViewModelBuilder
    {
        IProductTypeRepository _productTypeRepository;


         public AdminProductTypeViewModelBuilder(IProductTypeRepository productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }


         public IList<AdminProductTypeViewModel> GetAll(bool inactive = false)
        {
            var types = _productTypeRepository.GetAll(inactive);
            return types
                .Select(n => new AdminProductTypeViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description=n.Description,
                    isActive = n._Status == EntityStatus.Active ? true : false
                    
                })
                .ToList();
        }

         public AdminProductTypeViewModel Get(Guid id)
        {
            ProductType type = _productTypeRepository.GetById(id);
            if (type == null) return null;
             
            return Map(type);


        }

        AdminProductTypeViewModel Map(ProductType productType)
        {
            return new AdminProductTypeViewModel
            {
                Id = productType.Id,
                Name = productType.Name,
                Description=productType.Description,
                isActive = productType._Status == EntityStatus.Active ? true : false,
                Code = productType.Code,
            };
        }

        public void Save(AdminProductTypeViewModel adminProductTypeViewModel)
        {
            ProductType pb = new ProductType(adminProductTypeViewModel.Id)
            {
                Name = adminProductTypeViewModel.Name,
                Description=adminProductTypeViewModel.Description,
                Code=adminProductTypeViewModel.Code,
            };
            _productTypeRepository.Save(pb);
        }

        public void SetInactive(Guid id)
        {
            ProductType pt = _productTypeRepository.GetById(id);
            _productTypeRepository.SetInactive(pt);

        }

        public void SetActive (Guid id)
        {
            ProductType pt = _productTypeRepository.GetById(id);
            _productTypeRepository.SetActive(pt);
        }

        public void SetAsDeleted(Guid id)
        {
            ProductType pt = _productTypeRepository.GetById(id);
            _productTypeRepository.SetAsDeleted(pt);
        }

        public QueryResult<AdminProductTypeViewModel> Query(QueryStandard query)
        {
            var queryResults = _productTypeRepository.Query(query);
            var result = new QueryResult<AdminProductTypeViewModel>();
            result.Data = queryResults.Data.Select(Map).ToList();
            result.Count = queryResults.Count;

            return result;
        }

    }
}
