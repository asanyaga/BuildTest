using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.ReOrderLevelViewModel;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ReOrderLevelViewModelBuilders.Impl
{
   public class ReOrderLevelViewModelBuilder:IReOrderLevelViewModelBuilder
    {
       ICostCentreRepository _costCentreRepository;
       IProductRepository _productRepository;
       IReOrderLevelRepository _reOrderLevelRepository;
       public ReOrderLevelViewModelBuilder(
           ICostCentreRepository costCentreRepository,
       IProductRepository productRepository,
       IReOrderLevelRepository reOrderLevelRepository
           )
       {
           _costCentreRepository = costCentreRepository;
           _productRepository = productRepository;
          _reOrderLevelRepository = reOrderLevelRepository;
       }
       public List<ReOrderLevelViewModel> GetAll(bool inactive = false)
        {
            return _reOrderLevelRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        public List<ReOrderLevelViewModel> Search(string srcParam, bool inactive = false)
        {

            return _reOrderLevelRepository.GetAll(inactive).ToList().Where(n => (n.ProductId.Description.ToLower().StartsWith(srcParam.ToLower())) || (n.DistributorId.Name.ToLower().StartsWith(srcParam.ToLower())) || (n.ProductReOrderLevel.ToString().StartsWith(srcParam))).Select(n => Map(n)).ToList();
           
        }

        public ReOrderLevelViewModel GetById(Guid id)
        {
            ReOrderLevel rvm = _reOrderLevelRepository.GetById(id);
            if (rvm == null) return null;
             
            return Map(rvm);
        }

        public void Save(ReOrderLevelViewModel rolvm)
        {
            ReOrderLevel rol = new ReOrderLevel(rolvm.Id) 
            {
             ProductReOrderLevel=rolvm.ProductReOrderLevel,
             DistributorId=_costCentreRepository.GetById(rolvm.DistributorId),
             ProductId=_productRepository.GetById(rolvm.ProductId)
            };
            _reOrderLevelRepository.Save(rol);
        }

        public void SetInactive(Guid id)
        {
            ReOrderLevel rvm = _reOrderLevelRepository.GetById(id);
            _reOrderLevelRepository.SetInactive(rvm);
        }

       public void Activate(Guid id)
       {
           ReOrderLevel rl = _reOrderLevelRepository.GetById(id);
           _reOrderLevelRepository.SetActive(rl);
       }

       public void Delete(Guid id)
       {
           ReOrderLevel rl = _reOrderLevelRepository.GetById(id);
           _reOrderLevelRepository.SetAsDeleted(rl);
       }

       public Dictionary<Guid, string> GetDistributor()
        {
            return _costCentreRepository.GetAll().OfType<Distributor>().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> GetProducts()
        {
            return _productRepository.GetAll().Where(n => !(n is ReturnableProduct)).Select(n => new { n.Id, n.Description }).ToList().ToDictionary(n => n.Id, n => n.Description);
        }

       public QueryResult<ReOrderLevelViewModel> Query(QueryStandard query)
       {
           var queryResult = _reOrderLevelRepository.Query(query);
           var result = new QueryResult<ReOrderLevelViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;



       }

       protected ReOrderLevelViewModel Map(ReOrderLevel rlevel)
        {
            return new ReOrderLevelViewModel
                       {
                           DistributorId       = rlevel.DistributorId.Id,
                           DistributorName     = rlevel.DistributorId.Name,
                           ProductId           = rlevel.ProductId.Id,
                           ProductName         = rlevel.ProductId.Description,
                           ProductReOrderLevel = rlevel.ProductReOrderLevel,
                           Id                  = rlevel.Id,
                           isActive = rlevel._Status == EntityStatus.Active ? true : false
                       };
        }
    }
}
