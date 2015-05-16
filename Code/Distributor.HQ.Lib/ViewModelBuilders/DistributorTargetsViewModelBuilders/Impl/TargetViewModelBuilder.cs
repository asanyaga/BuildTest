using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders.Impl
{
    public class TargetViewModelBuilder:ITargetViewModelBuilder
    {
        ITargetPeriodRepository _targetPeriodRepository;
        ICostCentreRepository _costCentreRepository;
        IProductRepository _productRepository;
        ITargetRepository _targeteRepository;
        public TargetViewModelBuilder(
        ITargetPeriodRepository targetPeriodRepository,
        ICostCentreRepository costCentreRepository,
        IProductRepository productRepository,
            ITargetRepository targetRepository)
        {
            _productRepository = productRepository;
            _costCentreRepository = costCentreRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _targeteRepository = targetRepository;
        }
        public Dictionary<Guid, string> GetProduct()
        {
            return _productRepository.GetAll()
                .Select(n => new { n.Id, n.Description }).ToList().ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> GetDistributor()
        {
            return _costCentreRepository.GetAll().OfType<Distributr.Core.Domain.Master.CostCentreEntities.Distributor>()
                .Select(n => new { n.Id,n.Name}).ToList().ToDictionary(n=>n.Id,n=>n.Name);
        }

        public Dictionary<Guid, string> GetPeriod()
        {
            return _targetPeriodRepository.GetAll()
                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
        }

        public IList<TargetViewModel> GetAll(bool inactive = false)
        {

            return _targeteRepository.GetAll(inactive).Select(n => Map(n))
                .Where(n => n.CostCentreType == CostCentreType.Distributor).ToList();
        }

        public QueryResult<TargetViewModel> Query(QueryStandard query)
        {
            var queryResult = _targeteRepository.Query(query);

            var result = new QueryResult<TargetViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<TargetViewModel> Search(string srchParam, bool inactive = false)
        {

            return _targeteRepository.GetAll(inactive)
                      .Where(n => (n.CostCentre.Name.ToLower()
                                    .StartsWith(srchParam.ToLower())) ||
                                       (n.TargetPeriod.Name.ToLower().StartsWith(srchParam.ToLower())) ||
                                       (n.TargetValue.ToString().StartsWith(srchParam)))
                                    .Select(n => Map(n))
                                    .Where(n => n.CostCentreType == CostCentreType.Distributor)
                                    .ToList();
        }

        public TargetViewModel GetById(Guid id)
        {
            Target tvm = _targeteRepository.GetById(id);
            if (tvm == null) return null;
                

            return Map(tvm);
        }
        protected TargetViewModel Map(Target target)
        {
            TargetViewModel targetViewModel = new TargetViewModel();
            if (target.CostCentre != null)
            {
                targetViewModel.CostCentreId = _costCentreRepository.GetById(target.CostCentre.Id).Id;
            }
            targetViewModel.Id = target.Id;
            if (target._Status == EntityStatus.Active)
                targetViewModel.isActive =true;
            targetViewModel.Period = _targetPeriodRepository.GetById(target.TargetPeriod.Id).Id;
            targetViewModel.PeriodName = _targetPeriodRepository.GetById(target.TargetPeriod.Id).Name;
            targetViewModel.TargetValue = target.TargetValue;
            targetViewModel.IsQuantityTarget = target.IsQuantityTarget;
            targetViewModel.CostCentreName = _costCentreRepository.GetById(target.CostCentre.Id).Name;
            targetViewModel.CostCentreType = _costCentreRepository.GetById(target.CostCentre.Id).CostCentreType;
            return targetViewModel;
            }
        public void Save(TargetViewModel trgViewModel)
        {
            Target trg = new Target(trgViewModel.Id,DateTime.Now,DateTime.Now,EntityStatus.Active) 
            {
             TargetPeriod=_targetPeriodRepository.GetById(trgViewModel.Period),
              CostCentre=_costCentreRepository.GetById(trgViewModel.CostCentreId),
              TargetValue=trgViewModel.TargetValue,
              IsQuantityTarget=trgViewModel.IsQuantityTarget
            };
           
            _targeteRepository.Save(trg );
        }


        public void SetInactive(Guid id)
        {
            Target targetPeriod = _targeteRepository.GetById(id);
            _targeteRepository.SetInactive(targetPeriod);
        }

        public void Activate(Guid id)
        {
            Target targetPeriod = _targeteRepository.GetById(id);
            _targeteRepository.SetActive(targetPeriod);
        }

        public void Delete(Guid id)
        {
            Target targetPeriod = _targeteRepository.GetById(id);
            _targeteRepository.SetAsDeleted(targetPeriod);
        }

    }
}
