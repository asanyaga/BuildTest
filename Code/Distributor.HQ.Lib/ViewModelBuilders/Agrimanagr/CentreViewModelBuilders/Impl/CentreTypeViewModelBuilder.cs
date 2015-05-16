using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders.Impl
{
    public class CentreTypeViewModelBuilder : ICentreTypeViewModelBuilder
    {
        private ICentreTypeRepository _centreTypeRepository;
        private IMasterDataUsage _masterDataUsage;

        public CentreTypeViewModelBuilder(ICentreTypeRepository centreTypeRepository, IMasterDataUsage masterDataUsage)
        {
            _centreTypeRepository = centreTypeRepository;
            _masterDataUsage = masterDataUsage;
        }
        CentreTypeViewModel Map(CentreType centreType)
        {
            return new CentreTypeViewModel { 
                Id = centreType.Id,
                Code = centreType.Code,
                Name = centreType.Name, 
                Description = centreType.Description, 
                IsActive = centreType._Status == EntityStatus.Active ? true : false };
        }
        
        public IList<CentreTypeViewModel> GetAll(bool inactive = false)
        {
            var centreType = _centreTypeRepository.GetAll(inactive);
            return centreType.Select(n => Map(n)).ToList();
        }

        public CentreTypeViewModel Get(Guid Id)
        {
            CentreType centreType = _centreTypeRepository.GetById(Id);
            if (centreType == null) return null;
            return Map(centreType);
        }

        public void Save(CentreTypeViewModel assetTypeViewModel)
        {
            CentreType centreType = new CentreType(assetTypeViewModel.Id)
            {
                Name = assetTypeViewModel.Name,
                Description = assetTypeViewModel.Description,
                Code = assetTypeViewModel.Code
            };
            _centreTypeRepository.Save(centreType);
        }

        public void SetInactive(Guid Id)
        {
            CentreType centreType = _centreTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckAgriCentreTypeIsUsed(centreType, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate centre type. Centre type is assigned to centre(s). Remove assignment first to continue");
            }
            _centreTypeRepository.SetInactive(centreType);
        }

        public void SetActive(Guid Id)
        {
            CentreType centreType = _centreTypeRepository.GetById(Id);
            _centreTypeRepository.SetActive(centreType);
        }

        public void SetDeleted(Guid Id)
        {
            CentreType centreType = _centreTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckAgriCentreTypeIsUsed(centreType, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete centre type. Centre type is assigned to centre(s). Remove assignment first to continue");
            }
            _centreTypeRepository.SetAsDeleted(centreType);
        }

        public QueryResult<CentreTypeViewModel> Query(QueryStandard q)
        {
            var queryResult = _centreTypeRepository.Query(q);

            var result = new QueryResult<CentreTypeViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;
            
            return result;
        }
    }
}
