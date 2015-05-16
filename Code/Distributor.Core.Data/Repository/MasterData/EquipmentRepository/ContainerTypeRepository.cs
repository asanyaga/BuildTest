using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.EquipmentRepository
{
    internal class ContainerTypeRepository : RepositoryMasterBase<ContainerType> ,IContainerTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        ICommodityRepository _commodityRepository;

        public ContainerTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ICommodityRepository commodityRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _commodityRepository = commodityRepository;
        }

        protected override string _cacheKey
        {
            get { return "ContainerType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ContainerTypeList"; }
        }

        public Guid Save(ContainerType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("ContainerType.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("ContainerType.validation.error"));
            }
            DateTime dt = DateTime.Now;
            tblContainerType equip = _ctx.tblContainerType.FirstOrDefault(n => n.Id == entity.Id);
            if (equip == null)
            {
                equip = new tblContainerType();
                equip.Id = entity.Id;
                equip.IM_Status = (int)EntityStatus.Active;
                equip.IM_DateCreated = dt;
                equip.Id = entity.Id;

                _ctx.tblContainerType.AddObject(equip);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (equip.IM_Status != (int)entityStatus)
                equip.IM_Status = (int)entity._Status;
          
            equip.IM_DateLastUpdated = dt;
            equip.Name = entity.Name;
            equip.Make = entity.Make;
            equip.Model = entity.Model;
            equip.Description = entity.Description;
            equip.BubbleSpace = entity.BubbleSpace;
            if (entity.CommodityGrade != null)
            {
                equip.CommodityGradeId = entity.CommodityGrade.Id;
            }
            equip.ContainerUseId =(int)entity.ContainerUseType;
            equip.Width = entity.Width;
            equip.Lenght = entity.Length;
            equip.Height = entity.Height;
            equip.Volume = entity.Volume;
            equip.FreezerTemp = entity.FreezerTemp;
            equip.TareWeight = entity.TareWeight;
            equip.LoadCariage = entity.LoadCarriage;
            equip.Code = entity.Code;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblContainerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            return equip.Id; 
            
        }

        public void SetInactive(ContainerType entity)
        {
            tblContainerType equip = _ctx.tblContainerType.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Inactive;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContainerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public void SetActive(ContainerType entity)
        {
            tblContainerType equip = _ctx.tblContainerType.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Active;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContainerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public void SetAsDeleted(ContainerType entity)
        {
            tblContainerType equip = _ctx.tblContainerType.FirstOrDefault(n => n.Id == entity.Id);
            if (equip != null)
            {
                equip.IM_Status = (int)EntityStatus.Deleted;
                equip.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContainerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, equip.Id));
            }
        }

        public ContainerType GetById(Guid Id, bool includeDeactivated = false)
        {
            ContainerType entity = (ContainerType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblContainerType.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }
        private ContainerType Map(tblContainerType equipment)
        {
            ContainerType equip = new ContainerType(equipment.Id);
            equip.BubbleSpace = equipment.BubbleSpace.HasValue ? equipment.BubbleSpace.Value : 0;
            equip.FreezerTemp = equipment.FreezerTemp.HasValue ? equipment.FreezerTemp.Value : 0;
            equip.Height = equipment.Height.HasValue ? equipment.Height.Value : 0;
            equip.Length = equipment.Lenght.HasValue ? equipment.Lenght.Value : 0;
            equip.LoadCarriage = equipment.LoadCariage.HasValue ? equipment.LoadCariage.Value : 0;
            equip.TareWeight = equipment.TareWeight.HasValue ? equipment.TareWeight.Value : 0;
            equip.Volume = equipment.Volume.HasValue ? equipment.Volume.Value : 0;
            equip.Width = equipment.Width.HasValue ? equipment.Width.Value : 0;
            if (equipment.tblCommodityGrade != null)
            {
                equip.CommodityGrade = _commodityRepository.GetGradeByGradeId(equipment.tblCommodityGrade.Id);
            }
            equip.Description = equipment.Description;
            if (equipment.ContainerUseId != null)
                equip.ContainerUseType = (ContainerUseType)equipment.ContainerUseId;
            equip.Make = equipment.Make;
            equip.Model = equipment.Model;
            equip.Name = equipment.Name;
            equip.Code = equipment.Code;
            equip._SetDateCreated(equipment.IM_DateCreated);
            equip._SetDateLastUpdated(equipment.IM_DateLastUpdated);
            equip._SetStatus((EntityStatus)equipment.IM_Status);
            return equip;
        }

        public override IEnumerable<ContainerType> GetAll(bool includeDeactivated = false)
        {
            IList<ContainerType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ContainerType>(ids.Count);
                foreach (Guid id in ids)
                {
                    ContainerType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblContainerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ContainerType p in entities)
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

        public QueryResult<ContainerType> Query(QueryStandard q)
        {
            
            IQueryable<tblContainerType> containerTypeQuery;
            if (q.ShowInactive)
                containerTypeQuery = _ctx.tblContainerType.Where(s => s.IM_Status==(int)EntityStatus.Active || s.IM_Status==(int)EntityStatus.Inactive).AsQueryable();
            else
                containerTypeQuery = _ctx.tblContainerType.Where(s =>s.IM_Status == (int)EntityStatus.Active).AsQueryable();


            var queryResult = new QueryResult<ContainerType>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                containerTypeQuery = containerTypeQuery
                    .Where(
                        s =>
                        s.Make.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()) || s.Name.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = containerTypeQuery.Count();
            containerTypeQuery = containerTypeQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                containerTypeQuery = containerTypeQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = containerTypeQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ContainerType>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(ContainerType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
           
            return vri;
        }
    }
}
