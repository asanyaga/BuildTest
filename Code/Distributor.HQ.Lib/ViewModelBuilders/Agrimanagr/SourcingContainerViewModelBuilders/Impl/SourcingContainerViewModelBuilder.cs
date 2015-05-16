using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SourcingContainerViewModelBuilders.Impl
{
    public class SourcingContainerViewModelBuilder:ISourcingContainerViewModelBuilder
    {
        private IEquipmentRepository _equipmentRepository;
        private IHubRepository _hubRepository;
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;

        public SourcingContainerViewModelBuilder(IContainerTypeRepository containerTypeRepository, ICommodityRepository commodityRepository, IHubRepository hubRepository, IEquipmentRepository equipmentRepository)
        {
            _containerTypeRepository = containerTypeRepository;
            _commodityRepository = commodityRepository;
            _hubRepository = hubRepository;
            _equipmentRepository = equipmentRepository;
        }

        #region Implementation of IEquipmentViewModelBuilder

        public IList<SourcingContainerViewModel> GetAll(bool inactive = false)
        {
            var containers = _equipmentRepository.GetAll(inactive).Where(n=>n.EquipmentType == EquipmentType.Container).Select(n => Map((SourcingContainer)n)).ToList();
            //var hubs = _hubRepository.GetAll(inactive).Select(s => Map((Hub) s)).Where(s=>s.CostCentreType == CostCentreType.Hub).ToList();
            return containers;
        }
        SourcingContainerViewModel Map(SourcingContainer equipment)
        {
            SourcingContainerViewModel vm = new SourcingContainerViewModel();
            vm.Id = equipment.Id;
            vm.Code = equipment.Code;
            vm.EquipmentNumber = equipment.EquipmentNumber;
            vm.Name = equipment.Name;
            vm.Make = equipment.Make;
            vm.Model = equipment.Model;
            vm.EquipmentType = (int)equipment.EquipmentType;
            vm.Description = equipment.Description;
            vm.CostCentre = equipment.CostCentre.Id;
            vm.ContainerType = equipment.ContainerType.Id;
            //vm.LoadCariage = equipment.LoadCariage;
            //vm.TareWeight = equipment.TareWeight;
            //vm.Length = equipment.Lenght;
            //vm.Width = equipment.Width;
            //vm.Height = equipment.Height;
            //vm.BubbleSpace = equipment.BubbleSpace;
            //vm.Volume = equipment.Volume;
            //vm.FreezerTemp = equipment.FreezerTemp;
            //vm.CommodityGrade = equipment.CommodityGrade.Id;
            vm.IsActive = (int)equipment._Status;

            return vm;
        }

        public List<SourcingContainerViewModel> SearchContainers(string srchParam, bool inactive = false)
        {
            var items = _hubRepository.GetAll(inactive).OfType<SourcingContainer>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public SourcingContainerViewModel Get(Guid id)
        {
            SourcingContainer sourcingContainer = (SourcingContainer)_equipmentRepository.GetById(id);
            if (sourcingContainer == null) return null;
            return Map(sourcingContainer);
        }

        public void Save(SourcingContainerViewModel vm)
        {
            SourcingContainer sourcingContainer = new SourcingContainer(vm.Id);
            sourcingContainer.Name = vm.Name;
            sourcingContainer.Code = vm.Code;
            sourcingContainer.EquipmentNumber = vm.EquipmentNumber;
            sourcingContainer.Make = vm.Make;
            sourcingContainer.Model = vm.Model;
            sourcingContainer.EquipmentType = (EquipmentType)vm.EquipmentType;
            sourcingContainer.Description = vm.Description;
            sourcingContainer.CostCentre = (Hub)_hubRepository.GetById(vm.CostCentre);// _producerRepository.GetById();
            sourcingContainer.ContainerType = _containerTypeRepository.GetById(vm.ContainerType);
            //sourcingContainer.LoadCariage = vm.LoadCariage;
            //sourcingContainer.TareWeight = vm.TareWeight;
            //sourcingContainer.Lenght = vm.Length;
            //sourcingContainer.Width = vm.Width;
            //sourcingContainer.Height = vm.Height;
            //sourcingContainer.BubbleSpace = vm.BubbleSpace;
            //sourcingContainer.Volume = vm.Volume;
            //sourcingContainer.FreezerTemp = vm.FreezerTemp;
            /*Commodity commodity = (Commodity) _commodityRepository.GetById((Guid) vm.CommodityId);*/
           // sourcingContainer.CommodityGrade = commodity.CommodityGrades.Where(n=>n.Id == vm.CommodityGrade).FirstOrDefault();
            sourcingContainer._Status =EntityStatus.Active;
            _equipmentRepository.Save(sourcingContainer);
        }

        public void SetInactive(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetInactive(equipment);
        }

        public void SetActive(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetActive(equipment);
        }

        public void SetAsDeleted(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetAsDeleted(equipment);
        }

        public Dictionary<int, string> EquipmentTypes()
        {
            var dict = Enum.GetValues(typeof(EquipmentType))
               .Cast<EquipmentType>()
               .Where(n => (int)n == (int)EquipmentType.Container)
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public Dictionary<Guid, string> CostCentres()
        {
            return _hubRepository.GetAll().OrderBy(n => n.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> Commodities()
        {
            return _commodityRepository.GetAll().OrderBy(n => n.Name)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public List<SelectListItem> GradeByCommodity(Guid id)
        {
            var commodity = _commodityRepository.GetById(id);
            if (commodity != null)
            {
                return commodity.CommodityGrades.OrderBy(n => n.Name)
                    .Select(r => new SelectListItem {Value = r.Id.ToString(),Text = r.Name }).ToList();
            }
            return new List<SelectListItem>();
        }

        /* public Dictionary<Guid, string> Grades()
        {
            
        }*/

        #endregion
    }
}
