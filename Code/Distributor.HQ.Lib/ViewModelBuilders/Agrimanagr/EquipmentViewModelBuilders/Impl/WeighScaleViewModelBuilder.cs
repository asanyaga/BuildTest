using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders.Impl
{
    public class WeighScaleViewModelBuilder: IWeighScaleViewModelBuilder
    {
        #region Implementation of IWeighScaleViewModelBuilder
        private IHubRepository _hubRepository;
        private IEquipmentRepository _equipmentRepository;

        public WeighScaleViewModelBuilder(IHubRepository hubRepository, IEquipmentRepository equipmentRepository)
        {
            _hubRepository = hubRepository;
            _equipmentRepository = equipmentRepository;
        }

        public IList<WeighScaleViewModel> GetAll(bool inactive = false)
        {
            var scales = _equipmentRepository.GetAll(inactive).Where(n => n.EquipmentType == EquipmentType.WeighingScale).Select(n => Map((WeighScale)n)).ToList();
            return scales;
        }
        WeighScaleViewModel Map(WeighScale equipment)
        {
            WeighScaleViewModel vm = new WeighScaleViewModel();
            vm.Id = equipment.Id;
            vm.Code = equipment.Code;
            vm.EquipmentNumber = equipment.EquipmentNumber;
            vm.Name = equipment.Name;
            vm.Make = equipment.Make;
            vm.Model = equipment.Model;
            vm.EquipmentType = (int)equipment.EquipmentType;
            vm.Description = equipment.Description;
            vm.CostCentre = equipment.CostCentre.Id;
            vm.IsActive = (int)equipment._Status;
            vm.CostCentreName = equipment.CostCentre.Name;
            return vm;
        }

        public List<WeighScaleViewModel> SearchWeighScales(string srchParam, bool inactive = false)
        {
            var items = _equipmentRepository.GetAll(inactive).OfType<WeighScale>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public WeighScaleViewModel Get(Guid id)
        {
            WeighScale weighScale = (WeighScale)_equipmentRepository.GetById(id);
            if (weighScale == null) return null;
            return Map(weighScale);
        }

        public void Save(WeighScaleViewModel vm)
        {
            WeighScale weighScale = new WeighScale(vm.Id);
            weighScale.Name = vm.Name;
            weighScale.Code = vm.Code;
            weighScale.EquipmentNumber = vm.EquipmentNumber;
            weighScale.Make = vm.Make;
            weighScale.Model = vm.Model;
            weighScale.EquipmentType = (EquipmentType)vm.EquipmentType;
            weighScale.Description = vm.Description;
            weighScale.CostCentre = (Hub)_hubRepository.GetById(vm.CostCentre);
            weighScale._Status = EntityStatus.Active;
            _equipmentRepository.Save(weighScale);
        }

        public void SetInactive(Guid id)
        {
            Equipment equipment = _equipmentRepository.GetById(id);
            _equipmentRepository.SetInactive(equipment);
        }

        public void SetActive(Guid id)
        {
            Equipment equipment = _equipmentRepository.GetById(id);
            _equipmentRepository.SetActive(equipment);
        }

        public void SetAsDeleted(Guid id)
        {
            Equipment equipment = _equipmentRepository.GetById(id);
            _equipmentRepository.SetAsDeleted(equipment);
        }

        public Dictionary<int, string> EquipmentTypes()
        {
            var dict = Enum.GetValues(typeof(EquipmentType))
               .Cast<EquipmentType>()
               .Where(n => (int)n == (int)EquipmentType.WeighingScale)
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public Dictionary<Guid, string> CostCentres()
        {
            return _hubRepository.GetAll().OrderBy(n => n.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }


        public QueryResult<WeighScaleViewModel> Query(QueryEquipment query)
        {
            var queryResults = _equipmentRepository.Query(query);

            var results = new QueryResult<WeighScaleViewModel>();
            results.Data = queryResults.Data.OfType<WeighScale>().Select(Map).ToList();
            results.Count = queryResults.Count;

            return results;
        }

        public IList<WeighScaleViewModel> QueryList(List<WeighScale> list)
        {
            return list.Select(Map).ToList();
        }

        #endregion
    }
}
