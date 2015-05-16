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
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;


namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders.Impl
{
    public class SourcingContainerViewModelBuilder:ISourcingContainerViewModelBuilder
    {
        private IEquipmentRepository _equipmentRepository;
        private IHubRepository _hubRepository;
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;

        public SourcingContainerViewModelBuilder(IEquipmentRepository equipmentRepository, IHubRepository hubRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository)
        {
            _equipmentRepository = equipmentRepository;
            _hubRepository = hubRepository;
            _commodityRepository = commodityRepository;
            _containerTypeRepository = containerTypeRepository;
        }

        #region Implementation of IEquipmentViewModelBuilder

        public IList<SourcingContainerViewModel> GetAll(bool inactive = false)
        {
            var containers = _equipmentRepository.GetAll(inactive).Where(n=>n.EquipmentType == EquipmentType.Container)
              .Select(n => Map((SourcingContainer)n)).ToList();
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
            vm.EquipmentType = (int) EquipmentType.Container;
            ContainerType containerType = _containerTypeRepository.GetById(equipment.ContainerType.Id);
            vm.CostCentreName = equipment.CostCentre.Name;

            vm.ContainerType = containerType.Id;
            vm.ContainerTypeName = containerType.Name;
            vm.Description = equipment.Description;
            vm.CostCentre = equipment.CostCentre.Id;
            vm.IsActive = (int) equipment._Status;
            //vm.LoadCariage = equipment.LoadCariage;
            //vm.TareWeight = equipment.TareWeight;
            //vm.Length = equipment.Lenght;
            //vm.Width = equipment.Width;
            //vm.Height = equipment.Height;
            //vm.BubbleSpace = equipment.BubbleSpace;
            //vm.Volume = equipment.Volume;
            //vm.FreezerTemp = equipment.FreezerTemp;
            Guid grade = Guid.Empty;
            Guid commodity = Guid.Empty;
            string gradeName = "";
            //if(equipment.CommodityGrade==null)
            //{
            //}
            //else if (equipment.CommodityGrade != null)
            //{
            //    grade = equipment.CommodityGrade.Id;
            //    commodity = equipment.CommodityGrade.Commodity.Id;
            //    gradeName = equipment.CommodityGrade.Name;
            //}
           /* vm.CommodityGrade = grade;
            vm.IsActive = (int)equipment._Status;
            vm.CostCentreName = equipment.CostCentre.Name;
            vm.CommodityGradeName = gradeName;
            vm.CommodityId = commodity;*/


            return vm;
        }

        public List<SourcingContainerViewModel> SearchContainers(string srchParam, bool inactive = false)
        {
            var items = _equipmentRepository.GetAll(inactive).OfType<SourcingContainer>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
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
            sourcingContainer.EquipmentType = EquipmentType.Container;
           // sourcingContainer.ContainerType = (ContainerUserType)vm.ContainerType;
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
           /* Commodity commodity = (Commodity) _commodityRepository.GetById((Guid) vm.CommodityId);
            if (vm.CommodityGrade==null)
            {

            }*/
           /* else if (vm.CommodityGrade != null)
            {
               // sourcingContainer.CommodityGrade = commodity.CommodityGrades.Where(n => n.Id == vm.CommodityGrade).FirstOrDefault();
            }*/
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
              /* .Where(n => (int)n == (int)EquipmentType.Container)*/
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }
        public Dictionary<Guid, string> ContainerTypes()
        {
            return _containerTypeRepository.GetAll().OrderBy(n => n.Name).Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public QueryResult<SourcingContainerViewModel> Query(QueryEquipment query)
        {
            var queryResults = _equipmentRepository.Query(query);

            var results = new QueryResult<SourcingContainerViewModel>();
            results.Data = queryResults.Data.OfType<SourcingContainer>().Select(Map).ToList();
            results.Count = queryResults.Count;

            return results;
        }


        public IList<SourcingContainerViewModel> QueryList(List<SourcingContainer> list)
        {
            var sourcingContainer = list.Select(Map).ToList();
            return sourcingContainer;
        }

        public Dictionary<Guid, string> CostCentres()
        {
            return _hubRepository.GetAll().OrderBy(n => n.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> Commodities()
        {
            Dictionary<Guid, string> commodities=new Dictionary<Guid, string>();
            commodities.Add(key:Guid.Empty,value:"--Select Commodity--");
            _commodityRepository.GetAll().OrderBy(n => n.Name)
                                  .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name).ToList().ForEach(d => commodities.Add(d.Key,d.Value));
            return commodities;/*_commodityRepository.GetAll().OrderBy(n => n.Name)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);*/
        }
        public List<SelectListItem> /*Dictionary<string, string>*/ CommodityGrade(Guid commodityId, Guid commodityGrade)
        {
            List<SelectListItem> list =new List<SelectListItem>();
            list.Add(new SelectListItem { Value = Guid.Empty.ToString(), Text = "--Select Grade--" });
            var commodity = _commodityRepository.GetById(commodityId);
            if (commodity != null)
            {
                 return commodity.CommodityGrades.OrderBy(n => n.Name)
                    .Where(r=>r.Id == commodityGrade)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
                //return list;
            }
            return list;
            
        }

        public List<SelectListItem> GradeByCommodity(Guid id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = Guid.Empty.ToString(), Text = "--Select Grade--" });
            var commodity = _commodityRepository.GetById(id);
            if (commodity != null)
            {
               commodity.CommodityGrades.OrderBy(n => n.Name)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList().ForEach(d => list.Add((SelectListItem)d));
            }
            return list;
        }

        /* public Dictionary<Guid, string> Grades()
        {
            
        }*/

        #endregion
    }
}
