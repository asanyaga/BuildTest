using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders.Impl
{
    public class ContainerTypeViewModelBuilder : IContainerTypeViewModelBuilder
    {
        private IContainerTypeRepository _containerTypeRepository;
        private ICommodityRepository _commodityRepository;
        private IMasterDataUsage _masterDataUsage;

        public ContainerTypeViewModelBuilder(IContainerTypeRepository containerTypeRepository, ICommodityRepository commodityRepository, IMasterDataUsage masterDataUsage)
        {
            _containerTypeRepository = containerTypeRepository;
            _commodityRepository = commodityRepository;
            _masterDataUsage = masterDataUsage;

        }

        #region Implementation of ICommodityTypeViewModelBuilder

        public IList<ContainerTypeViewModel> GetAll(bool inactive = false)
        {
           return  _containerTypeRepository.GetAll(inactive).Select(Map).ToList();
        }

         ContainerTypeViewModel Map(ContainerType containerType)
        {
            var vm = new ContainerTypeViewModel();
            vm.Id = containerType.Id;
            vm.Code = containerType.Code;
            vm.Name = containerType.Name;
            vm.Make = containerType.Make;
            vm.Model = containerType.Model;
            vm.Description = containerType.Description;
            vm.LoadCariage = containerType.LoadCarriage;
            vm.TareWeight = containerType.TareWeight;
            vm.Width = containerType.Width;
            vm.Height = containerType.Height;
            vm.BubbleSpace = containerType.BubbleSpace;
            vm.Volume = containerType.Volume;
            vm.FreezerTemp = containerType.FreezerTemp;
            vm.ContainerUserType = (int)containerType.ContainerUseType;
            vm.IsActive = (int)containerType._Status;

            if (containerType.CommodityGrade != null)
            {
                vm.SelectedCommodityGradeId = containerType.CommodityGrade.Id;
                vm.SelectedCommodityGradeName = containerType.CommodityGrade.Name;
                vm.SelectedCommodityId = containerType.CommodityGrade.Commodity.Id;
                vm.SelectedCommodityName = containerType.CommodityGrade.Commodity.Name;
            }
            
            return vm;
        }

        public ContainerTypeViewModel Get(Guid id)
        {
            ContainerType containerType = _containerTypeRepository.GetById(id);
            if (containerType == null) return null;
            return Map(containerType);
        }

        public ContainerTypeViewModel Setup()
        {
            throw new NotImplementedException();
        }

        public void Save(ContainerTypeViewModel vm)
        {
            ContainerType containerType = new ContainerType(vm.Id);
            containerType.Name = vm.Name;
            containerType.Code = vm.Code;
            containerType.Make = vm.Make;
            containerType.Model = vm.Model;
            containerType.Description = vm.Description;
            containerType.LoadCarriage = vm.LoadCariage;
            containerType.TareWeight = vm.TareWeight;
            containerType.Length = vm.Length;
            containerType.Width = vm.Width;
            containerType.Height = vm.Height;
            containerType.BubbleSpace = vm.BubbleSpace;
            containerType.Volume = vm.Volume;
            containerType.FreezerTemp = vm.FreezerTemp;
            containerType._Status = EntityStatus.Active;
            containerType.ContainerUseType = (ContainerUseType)vm.ContainerUserType;
            containerType.CommodityGrade = _commodityRepository.GetGradeByGradeId(vm.SelectedCommodityGradeId);


            _containerTypeRepository.Save(containerType);
        }

        public void SetInactive(Guid Id)
        {
            ContainerType containerType = _containerTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckContainerTypeIsUsed(containerType, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate container type. Centre type is assigned to container(s). Remove assignment first to continue");
            }
            _containerTypeRepository.SetInactive(containerType);
        }

        public void SetActive(Guid Id)
        {
            ContainerType containerType = _containerTypeRepository.GetById(Id);
            _containerTypeRepository.SetActive(containerType);
        }

        public void SetAsDeleted(Guid Id)
        {
            ContainerType containerType = _containerTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckContainerTypeIsUsed(containerType, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete container type. Centre type is assigned to container(s). Remove assignment first to continue");
            }
            _containerTypeRepository.SetAsDeleted(containerType);
        }

        public Dictionary<Guid, string> Commodities()
        {
            Dictionary<Guid, string> commodities = new Dictionary<Guid, string>();
            commodities.Add(key: Guid.Empty, value: "--Select Commodity--");
            _commodityRepository.GetAll().OrderBy(n => n.Name)
                                  .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name).ToList().ForEach(d => commodities.Add(d.Key, d.Value));
            return commodities;
        }

        public Dictionary<int, string> ContainerUserTypes()
        {
            var dict = Enum.GetValues(typeof(ContainerUseType))
              .Cast<ContainerUseType>()
              .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public SelectList GradeByCommodity(Guid id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = Guid.Empty.ToString(), Text = "--Select Grade--" });
            var commodity = _commodityRepository.GetById(id);
            if (commodity != null)
            {
                commodity.CommodityGrades.OrderBy(n => n.Name)
                     .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList().ForEach(d => list.Add((SelectListItem)d));
            }
         
            return new SelectList(list, "Value", "Text");
        }

        public List<SelectListItem> CommodityGrade(Guid commodityId, Guid gradeId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = Guid.Empty.ToString(), Text = "--Select Grade--" });
            var commodity = _commodityRepository.GetById(commodityId);
            if (commodity != null)
            {
                return commodity.CommodityGrades.OrderBy(n => n.Name)
                   .Where(r => r.Id == gradeId )
                   .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
            }
            return list;
        }

        public QueryResult<ContainerTypeViewModel> Query(QueryStandard query)
        {
            var queryResult = _containerTypeRepository.Query(query);

            var result = new QueryResult<ContainerTypeViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();

            result.Count = queryResult.Count;

            return result;
        }

    #endregion
    }
}
