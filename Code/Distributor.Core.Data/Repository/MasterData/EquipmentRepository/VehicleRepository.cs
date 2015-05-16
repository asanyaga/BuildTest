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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.EquipmentRepository
{
    internal class VehicleRepository : RepositoryMasterBase<Vehicle>, IVehicleRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IHubRepository _hubRepository;

        public VehicleRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IHubRepository hubRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _hubRepository = hubRepository;
        }

        public Guid Save(Vehicle entity, bool? isSync = null)
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

            tblEquipment vehicle = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (vehicle == null)
            {
                vehicle = new tblEquipment();
                vehicle.Id = entity.Id;
                vehicle.IM_Status = (int)EntityStatus.Active;
                vehicle.IM_DateCreated = dt;
                vehicle.Id = entity.Id;

                _ctx.tblEquipment.AddObject(vehicle);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (vehicle.IM_Status != (int)entityStatus)
                vehicle.IM_Status = (int)entity._Status;
            vehicle.Code = entity.Code;
            vehicle.IM_DateLastUpdated = dt;
            vehicle.Name = entity.Name;
            vehicle.Make = entity.Make;
            vehicle.EquipmentType = (int)EquipmentType.Vehicle;
            vehicle.Model = entity.Model;
            vehicle.CostCentreId = entity.CostCentre.Id;
            vehicle.Description = entity.Description;
            vehicle.EquipmentNumber = entity.EquipmentNumber;
            
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, vehicle.Id));
            return vehicle.Id; 
        }

        public void SetInactive(Vehicle entity)
        {
            tblEquipment vehicle = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (vehicle != null)
            {
                vehicle.IM_Status = (int)EntityStatus.Inactive;
                vehicle.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, vehicle.Id));
            }
        }

        public void SetActive(Vehicle entity)
        {
            tblEquipment vehicle = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (vehicle != null)
            {
                vehicle.IM_Status = (int)EntityStatus.Active;
                vehicle.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, vehicle.Id));
            }
        }

        public void SetAsDeleted(Vehicle entity)
        {
            tblEquipment vehicle = _ctx.tblEquipment.FirstOrDefault(n => n.Id == entity.Id);
            if (vehicle != null)
            {
                vehicle.IM_Status = (int)EntityStatus.Deleted;
                vehicle.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, vehicle.Id));
            }
        }

        public Vehicle GetById(Guid id, bool includeDeactivated = false)
        {
            var entity = (Vehicle)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblEquipment.FirstOrDefault(s => s.Id == id && s.EquipmentType==(int)EquipmentType.Vehicle);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Vehicle> GetAll(bool includeDeactivated = false)
        {
            IList<Vehicle> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Vehicle>(ids.Count);
                foreach (Guid id in ids)
                {
                    Vehicle entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblEquipment.Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.EquipmentType == (int)EquipmentType.Vehicle).ToList().Select(s => Map(s)).ToList();
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

        public QueryResult<Vehicle> Query(QueryEquipment q)
        {
            IQueryable<tblEquipment> equipmentQuery;
            if (q.ShowInactive)
                equipmentQuery = _ctx.tblEquipment.Where(s => s.IM_Status != (int)EntityStatus.Deleted  && s.EquipmentType == (int)EquipmentType.Vehicle).AsQueryable();
            else
                equipmentQuery = _ctx.tblEquipment.Where(s => s.IM_Status == (int)EntityStatus.Active && s.EquipmentType == q.EquipmentType).AsQueryable();


            var queryResult = new QueryResult<Vehicle>();
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
            queryResult.Data = result.Select(Map).OfType<Vehicle>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(Vehicle itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Invalid Vehicle ID"));
            if (itemToValidate.EquipmentNumber ==string.Empty)
                vri.Results.Add(new ValidationResult("Vehicle Registration number is required"));
            if (itemToValidate.EquipmentType != EquipmentType.Vehicle)
                vri.Results.Add(new ValidationResult("Type incorrect"));
            if (GetAll(true).OrderBy(p => p.EquipmentNumber).Any(p => p.EquipmentNumber == itemToValidate.EquipmentNumber && p.Id != itemToValidate.Id))
                vri.Results.Add(new ValidationResult("A vehicle with Similar Registration Number exist in"));
            if (GetAll(true).OrderBy(p => p.Code).Any(p => p.Code == itemToValidate.Code && p.CostCentre.Id == itemToValidate.CostCentre.Id && p.Id != itemToValidate.Id))
                vri.Results.Add(new ValidationResult("A vehicle with Similar code exist in"+itemToValidate.CostCentre.Name+"costcentre"));
            if (GetAll(true).OrderBy(p => p.Name).Any(p => p.Name == itemToValidate.Name && p.CostCentre.Id == itemToValidate.CostCentre.Id && p.Id != itemToValidate.Id))
                vri.Results.Add(new ValidationResult("A vehicle with Similar Name exist in" + itemToValidate.CostCentre.Name + "costcentre"));
            
           
            return vri;
        }
       
        private Vehicle Map(tblEquipment vehicle)
        {
            var equip = new Vehicle(vehicle.Id);

            equip.Code = vehicle.Code;
            equip.EquipmentNumber = vehicle.EquipmentNumber;
            equip.Description = vehicle.Description;
            equip.EquipmentType = (EquipmentType)vehicle.EquipmentType;
            equip.Make = vehicle.Make;
            equip.Model = vehicle.Model;
            equip.Name = vehicle.Name;
            equip.CostCentre = (Hub)_hubRepository.GetById(vehicle.CostCentreId);

            equip._SetDateCreated(vehicle.IM_DateCreated);
            equip._SetDateLastUpdated(vehicle.IM_DateLastUpdated);
            equip._SetStatus((EntityStatus)vehicle.IM_Status);
            return equip;
        }
        protected override string _cacheKey
        {
            get { return "Vehicle-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "VehicleList"; }
        }

    }
}
