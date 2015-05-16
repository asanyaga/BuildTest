using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Util;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders.Impl
{
    public class TargetPeriodViewModelBuilder : ITargetPeriodViewModelBuilder
    {
        ITargetPeriodRepository _targetPeriodRepository;
        ITargetRepository _targetRepository;
        private EntityUsage _entityUsage;

        public TargetPeriodViewModelBuilder(ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository)
        {
            _targetPeriodRepository = targetPeriodRepository;
            _targetRepository = targetRepository;
            _entityUsage = new EntityUsage(_targetRepository);
        }

        TargetPeriodViewModel Map(TargetPeriod targetPeriod)
        {
            return new TargetPeriodViewModel { Id = targetPeriod.Id, Name = targetPeriod.Name, StartDate = targetPeriod.StartDate, EndDate = targetPeriod.EndDate, IsActive = targetPeriod._Status == EntityStatus.Active ? true : false };
        }

        public IList<TargetPeriodViewModel> GetAll(bool inactive = false)
        {
            var targetPeriod = _targetPeriodRepository.GetAll(inactive);
            return targetPeriod.Select(n => Map(n)).ToList();
        }

        public TargetPeriodViewModel Get(Guid Id)
        {
            TargetPeriod targetPeriod = _targetPeriodRepository.GetById(Id);
            if (targetPeriod == null) return null;
            return Map(targetPeriod);
        }

        public void Save(TargetPeriodViewModel targetPeriodViewModel)
        {
            TargetPeriod targetPeriod = new TargetPeriod(targetPeriodViewModel.Id)
            {
                Name = targetPeriodViewModel.Name,
                StartDate = targetPeriodViewModel.StartDate,
                EndDate = targetPeriodViewModel.EndDate
            };

            _targetPeriodRepository.Save(targetPeriod);
        }

        public void SetInactive(Guid id)
        {
            TargetPeriod targetPeriod = _targetPeriodRepository.GetById(id);
            if (_entityUsage.TargetPeriodUsed(targetPeriod))
            {
                throw new Exception("Cannot deactivate. Target period used by one or more targets.");
            }
            _targetPeriodRepository.SetInactive(targetPeriod);
        }

        public void Activate(Guid id)
        {
            TargetPeriod targetPeriod = _targetPeriodRepository.GetById(id);
            _targetPeriodRepository.SetActive(targetPeriod);
        }

        public void Delete(Guid id)
        {
            TargetPeriod targetPeriod = _targetPeriodRepository.GetById(id);
            if (_entityUsage.TargetPeriodUsed(targetPeriod))
            {
                throw new Exception("Cannot deactivate. Target period used by one or more targets.");
            }
            _targetPeriodRepository.SetAsDeleted(targetPeriod);
        }

        public QueryResult<TargetPeriodViewModel> Query(QueryStandard query)
        {
            var queryResult = _targetPeriodRepository.Query(query);

            var result = new QueryResult<TargetPeriodViewModel>();

            result.Data = queryResult.Data.OfType<TargetPeriod>().Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }


        public List<TargetPeriodViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _targetPeriodRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.StartDate.ToString().StartsWith(srchParam)) || (n.EndDate.ToString().StartsWith(srchParam)));
            return items
                .Select(n => new TargetPeriodViewModel
                {
                    Name = n.Name,
                    StartDate = n.StartDate,
                    EndDate = n.EndDate,
                    IsActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }
    }
}
