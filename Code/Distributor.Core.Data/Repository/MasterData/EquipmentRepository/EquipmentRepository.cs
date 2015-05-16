using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.EquipmentRepository
{
    internal class EquipmentRepository : RepositoryMasterBase<Equipment>, IEquipmentRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IContainerTypeRepository _containerTypeRepository;
        private IHubRepository _hubRepository;

        public EquipmentRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IContainerTypeRepository containerTypeRepository, IHubRepository hubRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _containerTypeRepository = containerTypeRepository;
            _hubRepository = hubRepository;
        }

        protected override string _cacheKey
        {
            get { return "Equipment-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "EquipmentList"; }
        }

        public Guid Save(Equipment entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Equipment.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("Equipment.validation.error"));
            }
            DateTime dt = DateTime.Now;

            tblEquipment equip = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (equip == null)
            {
                equip = new tblEquipment();
                equip.Id = entity.Id;
                equip.IM_Status = (int)EntityStatus.Active;
                equip.IM_DateCreated = dt;
                equip.Id = entity.Id;
                
                _ctx.tblEquipment.AddObject(equip);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (equip.IM_Status != (int)entityStatus)
                equip.IM_Status = (int)entity._Status;
            equip.Code = entity.Code;
            equip.IM_DateLastUpdated = dt;
            equip.Name = entity.Name;
            equip.Make = entity.Make;
            equip.EquipmentType = (int)entity.EquipmentType;
            equip.Model = entity.Model;
            equip.CostCentreId = entity.CostCentre.Id;
            equip.Description = entity.Description;
            equip.EquipmentNumber = entity.EquipmentNumber;
            if(entity is SourcingContainer)
            {
                SourcingContainer container = entity as SourcingContainer ;
                equip.ContainerTypeId = container.ContainerType.Id;
                
            }
            if(entity is Printer)
            {
                Printer printer = (Printer) entity;
                entity = printer;
            }
            if(entity is WeighScale)
            {
                WeighScale weighScale = (WeighScale) entity;
                entity = weighScale;
            }
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            return equip.Id; 
        }

        public void SetInactive(Equipment entity)
        {
            tblEquipment equip = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Inactive;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public void SetActive(Equipment entity)
        {
            tblEquipment equip = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Active;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public void SetAsDeleted(Equipment entity)
        {
            tblEquipment equip = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Deleted;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public Equipment GetById(Guid id, bool includeDeactivated = false)
        {
            Equipment entity = (Equipment)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblEquipment
                    .Where(n => n.EquipmentType != (int)EquipmentType.Vehicle)
                    .FirstOrDefault(n => n.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Equipment> GetAll(bool includeDeactivated = false)
        {
            IList<Equipment> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Equipment>(ids.Count);
                foreach (Guid id in ids)
                {
                    Equipment entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblEquipment
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                    .Where(n => n.EquipmentType != (int)EquipmentType.Vehicle)
                    .ToList()
                    .Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Equipment p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            entities.ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<Equipment> Query(QueryEquipment q)
        {
            IQueryable<tblEquipment> equipmentQuery;
            if (q.ShowInactive)
                equipmentQuery = _ctx.tblEquipment.Where(s => (s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive) && s.EquipmentType==q.EquipmentType).AsQueryable();
            else
                equipmentQuery = _ctx.tblEquipment.Where(s => s.IM_Status == (int)EntityStatus.Active && s.EquipmentType == q.EquipmentType).AsQueryable();


            var queryResult = new QueryResult<Equipment>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                equipmentQuery = equipmentQuery
                    .Where(
                        s =>
                        s.Make.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()) || s.Name.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = equipmentQuery.Count();
            equipmentQuery = equipmentQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                equipmentQuery = equipmentQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = equipmentQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Equipment>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        private Equipment Map(tblEquipment equipment)
        {
            Equipment equip = null; 

            if(equipment.EquipmentType == (int)EquipmentType.Printer)
            {
                Printer printer = new Printer(equipment.Id);
                equip = printer;
            } 
            if (equipment.EquipmentType == (int)EquipmentType.WeighingScale)
            {
                WeighScale scale = new WeighScale(equipment.Id);
                equip = scale;
            } 
            if (equipment.EquipmentType == (int)EquipmentType.Container)
            {
                SourcingContainer container = new SourcingContainer(equipment.Id);
                if (equipment.ContainerTypeId != null)
                    container.ContainerType = _containerTypeRepository.GetById(equipment.ContainerTypeId.Value);
                equip = container;
            }

            equip.Code = equipment.Code;
            equip.EquipmentNumber = equipment.EquipmentNumber;
            equip.Description = equipment.Description;
            equip.EquipmentType = (EquipmentType) equipment.EquipmentType;
            equip.Make = equipment.Make;
            equip.Model = equipment.Model;
            equip.Name = equipment.Name;
            equip.CostCentre = (Hub) _hubRepository.GetById(equipment.CostCentreId);

            equip._SetDateCreated(equipment.IM_DateCreated);
            equip._SetDateLastUpdated(equipment.IM_DateLastUpdated);
            equip._SetStatus((EntityStatus)equipment.IM_Status);
            return equip;
        }

        //todo: this is a quick fix to be refactored to use correct repository
        CommodityGrade MapGrade(tblCommodityGrade tblGrade)
        {
            CommodityGrade grade = new CommodityGrade(tblGrade.Id)
                                       {
                                           Name = tblGrade.Name,
                                           Code = tblGrade.Code,
                                           Commodity = new Commodity(tblGrade.tblCommodity.Id)
                                                           {
                                                               Name = tblGrade.tblCommodity.Name,
                                                               Code = tblGrade.tblCommodity.Name,
                                                           },
                                           Description = tblGrade.Description,
                                       };
            return grade;
        }

        public ValidationResultInfo Validate(Equipment itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (itemToValidate.EquipmentType==EquipmentType.Container)
            {
                SourcingContainer container = itemToValidate as SourcingContainer;
                if (itemToValidate.CostCentre != null)
                {
                    bool hasDuplicateCode = GetAll(true).OfType<SourcingContainer>()
                        .Where(s => s.Id != itemToValidate.Id)
                        .Any(p => p.Code == itemToValidate.Code);
                    if (hasDuplicateCode)
                        vri.Results.Add(new ValidationResult("Duplicate Code found"));
                }
            }
            if (itemToValidate.EquipmentType == EquipmentType.WeighingScale)
            {
                if (itemToValidate.CostCentre != null)
                {
                    bool hasDuplicateCode = GetAll(true).OfType<WeighScale>()
                        .Where(s => s.Id != itemToValidate.Id)
                        .Any(p => p.Code == itemToValidate.Code);
                    if (hasDuplicateCode)
                        vri.Results.Add(new ValidationResult("Duplicate Code found"));
                }
            }
            return vri;
        }
    }
}
