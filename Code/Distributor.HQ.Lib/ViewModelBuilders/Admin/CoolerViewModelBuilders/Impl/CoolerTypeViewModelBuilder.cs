using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders.Impl
{
   public class CoolerTypeViewModelBuilder:ICoolerTypeViewModelBuilder 
    {
       IAssetTypeRepository _coolerTypeRepository;
       public CoolerTypeViewModelBuilder(IAssetTypeRepository coolerTypeRepository)
       {
           _coolerTypeRepository = coolerTypeRepository;
       }
       CoolerTypeViewModel Map(AssetType coolerType)
       {
           return new CoolerTypeViewModel { Id = coolerType.Id, Name = coolerType.Name, Code = coolerType.Description, IsActive = coolerType._Status == EntityStatus.Active ? true : false };
       }
        public IList<CoolerTypeViewModel> GetAll(bool inactive = false)
        {
            var coolerType = _coolerTypeRepository.GetAll(inactive);
            return coolerType.Select(n => Map(n)).ToList();
        }

        public List<CoolerTypeViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _coolerTypeRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower()) || (n.Description.ToLower().StartsWith(srchParam.ToLower()))));
            return items.Select(n => Map(n)).ToList();
        }

        public CoolerTypeViewModel Get(Guid Id)
        {
            AssetType coolerType = _coolerTypeRepository.GetById(Id);
            if (coolerType == null) return null;
            return Map(coolerType);
        }

        public void Save(CoolerTypeViewModel coolerTypeViewModel)
        {
            AssetType coolerType = new AssetType(coolerTypeViewModel.Id)
            {
                Name=coolerTypeViewModel.Name,
                Description=coolerTypeViewModel.Code 
            };
            _coolerTypeRepository.Save(coolerType);
        }

        public void SetInactive(Guid id)
        {
            AssetType coolerType = _coolerTypeRepository.GetById(id);
            _coolerTypeRepository.SetInactive(coolerType);
        }
    }
}
