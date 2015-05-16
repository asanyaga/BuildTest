using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders.Impl
{
    public class CoolerViewModelBuilder : ICoolerViewModelBuilder
    {
        IAssetRepository _coolerRepository;
        IAssetTypeRepository _coolerTypeRepository;
        public CoolerViewModelBuilder(IAssetRepository coolerRepository, IAssetTypeRepository coolerTypeRepository)
        {
            _coolerRepository = coolerRepository;
            _coolerTypeRepository = coolerTypeRepository;
        }
        CoolerViewModel Map(Asset cooler)
        {
            ////CoolerViewModel coolerViewModel = new CoolerViewModel();
            ////coolerViewModel.Id = cooler.Id;
            ////coolerViewModel.Code = cooler.Code;
            ////coolerViewModel.AssetNo = cooler.AssetNo;
            ////coolerViewModel.SerialNo = cooler.SerialNo;
            ////if (cooler.CoolerTypeId != null)
            ////{
            ////    coolerViewModel.CoolerTypeId = _coolerTypeRepository.GetById(cooler.CoolerTypeId).Id;
            ////}
            ////if (cooler.CoolerType != null)
            ////{
            ////    coolerViewModel.CoolerType = _coolerTypeRepository.GetById(cooler.CoolerTypeId).Name;
            ////}
            ////coolerViewModel.IsActive = cooler._Status;
            ////return coolerViewModel;
            return new CoolerViewModel
                       {
                           Id = cooler.Id,
                           Code = cooler.Code,
                           Name = cooler.Name,
                           AssetNo = cooler.AssetNo,
                           Capacity = cooler.Capacity,
                           SerialNo = cooler.SerialNo,
                           CoolerTypeId = _coolerTypeRepository.GetById(cooler.AssetType.Id).Id,
                           CoolerType = _coolerTypeRepository.GetById(cooler.AssetType.Id).Name,
                           IsActive = cooler._Status == EntityStatus.Active ? true : false
                       };
        }
        public IList<CoolerViewModel> GetAll(bool inactive = false)
        {
            var cooler = _coolerRepository.GetAll(inactive);
            return cooler.Select(n => Map(n)).ToList();
        }

        public List<CoolerViewModel> Search(string srchParam, bool inactive = false)
        {
            var items =
                _coolerRepository.GetAll().Where(
                    n =>
                    (n.AssetNo.ToLower().StartsWith(srchParam.ToLower())) ||
                    (n.Code.ToLower().StartsWith(srchParam.ToLower())) ||
                    (n.Name.ToLower().StartsWith(srchParam.ToLower())) ||
                    (n.Capacity.ToLower().StartsWith(srchParam.ToLower())) ||
                    (n.SerialNo.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public CoolerViewModel Get(Guid Id)
        {
            Asset  cooler = _coolerRepository.GetById(Id);
            if (cooler == null) return null;
            return Map(cooler);
        }

        public void Save(CoolerViewModel coolerViewModel)
        {
            Asset cooler = new Asset(coolerViewModel.Id)
            {
                AssetType = _coolerTypeRepository.GetById(coolerViewModel.CoolerTypeId ),
                Code = coolerViewModel.Code,
                Name = coolerViewModel.Name,
                Capacity = coolerViewModel.Capacity,
                AssetNo = coolerViewModel.AssetNo,
                SerialNo = coolerViewModel.SerialNo 
            };
            _coolerRepository.Save(cooler);
        }

        public void SetInactive(Guid id)
        {
            Asset cooler = _coolerRepository.GetById(id);
            _coolerRepository.SetInactive(cooler);
        }


        public Dictionary<Guid, string> CoolerType()
        {
            return _coolerTypeRepository.GetAll()
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }
    }
}
