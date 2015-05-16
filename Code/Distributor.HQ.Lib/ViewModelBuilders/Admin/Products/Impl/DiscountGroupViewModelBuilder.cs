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
   public class DiscountGroupViewModelBuilder:IDiscountGroupViewModelBuilder
    {
       IDiscountGroupRepository _discountGroupRepository;
       public DiscountGroupViewModelBuilder(IDiscountGroupRepository discountGroupRepository)
       {
           _discountGroupRepository = discountGroupRepository;
       }
       public DiscountGroupViewModel Get(Guid id)
       {
           DiscountGroup dg = _discountGroupRepository.GetById(id);
           if (dg == null)
               return null;
            return Map(dg);
        }

        public void Save(DiscountGroupViewModel customerDiscount)
        {
            DiscountGroup dg = new DiscountGroup(customerDiscount.id)
            {
                 Name=customerDiscount.Name,
                  Code=customerDiscount.Code
            };
            _discountGroupRepository.Save(dg);
        }

        public void SetInactive(Guid id)
        {
            DiscountGroup dg = _discountGroupRepository.GetById(id);
            _discountGroupRepository.SetInactive(dg);
        }

        public List<DiscountGroupViewModel> GetAll(bool inactive = false)
        {
            return _discountGroupRepository.GetAll(inactive).ToList().Select(n=>Map(n)).ToList();
        }

        public List<DiscountGroupViewModel> Search(string srcParam, bool inactive = false)
        {
            return _discountGroupRepository.GetAll(inactive).ToList().Where(n=>n.Code.ToLower().StartsWith(srcParam.ToLower())||(n.Name.ToLower().StartsWith(srcParam.ToLower()))).Select(n => Map(n)).ToList();
        }

       public QueryResult<DiscountGroupViewModel> Query(QueryStandard q)
       {
           var queryResult = _discountGroupRepository.QueryResult(q);

           var result = new QueryResult<DiscountGroupViewModel>();

           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();

           return result;
       }

       DiscountGroupViewModel Map(DiscountGroup dg)
        { 
        return new DiscountGroupViewModel
            {
                 Code=dg.Code,
                  Name=dg.Name,
                   id=dg.Id,
                 isActive = dg._Status == EntityStatus.Active ? true : false
            };
        }
    }
}
