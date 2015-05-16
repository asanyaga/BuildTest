using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CommodityRepositories
{
    internal class CommodityRepository : RepositoryMasterBase<Commodity>, ICommodityRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        ICommodityTypeRepository _commoditytypeRepository;

        public CommodityRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ICommodityTypeRepository commodityTypeRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _commoditytypeRepository = commodityTypeRepository;
        }

        protected override string _cacheKey
        {
            get { return "Commodity-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CommodityList"; }
        }

        public Guid Save(Commodity entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Commodity not valid");
                throw new DomainValidationException(vri, "");

            }
            DateTime dt = DateTime.Now;

            tblCommodity commodity = _ctx.tblCommodity.FirstOrDefault(n => n.Id == entity.Id);
            if (commodity == null)
            {
                commodity = new tblCommodity();
                commodity.Id = entity.Id;
                commodity.IM_Status = (int)EntityStatus.Active;
                commodity.IM_DateCreated = dt;
                commodity.Id = entity.Id;
                _ctx.tblCommodity.AddObject(commodity);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (commodity.IM_Status != (int)entityStatus)
                commodity.IM_Status = (int)entity._Status;
            foreach (CommodityGrade cG in entity.CommodityGrades)
            {

                tblCommodityGrade commodityGrade = commodity.tblCommodityGrade.FirstOrDefault(s => s.Id == cG.Id);

                if (commodityGrade == null)
                {
                    commodityGrade = new tblCommodityGrade();
                    commodityGrade.Id = (cG.Id != Guid.Empty) ? cG.Id : Guid.NewGuid();
                    commodityGrade.IM_Status = (int)EntityStatus.Active;
                    commodity.tblCommodityGrade.Add(commodityGrade);

                }
                else
                {
                    commodityGrade.Id = cG.Id;
                    commodityGrade.IM_Status = (int)cG._Status;
                }
                commodityGrade.Name = cG.Name;
                commodityGrade.Code = cG.Code;
                commodityGrade.Description = cG.Description;
                commodityGrade.UsageTypeId = cG.UsageTypeId;
                commodityGrade.IM_DateCreated = dt;
                commodityGrade.IM_DateLastUpdated = dt;


            }

            commodity.CommodityTypeId = entity.CommodityType.Id;
            commodity.Description = entity.Description;
            commodity.Code = entity.Code;
            commodity.Name = entity.Name;
            commodity.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCommodity.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, commodity.Id));
            return commodity.Id;
        }

        public void SetInactive(Commodity entity)
        {
            tblCommodity commodity = _ctx.tblCommodity.FirstOrDefault(n => n.Id == entity.Id);
            if (commodity != null)
            {
                commodity.IM_Status = (int)EntityStatus.Inactive;
                commodity.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodity.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodity.Id));
            }
        }

        public void SetActive(Commodity entity)
        {
            tblCommodity commodity = _ctx.tblCommodity.FirstOrDefault(n => n.Id == entity.Id);
            if (commodity != null)
            {
                commodity.IM_Status = (int)EntityStatus.Active;
                commodity.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodity.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodity.Id));
            }
        }

        public void SetAsDeleted(Commodity entity)
        {
            tblCommodity commodity = _ctx.tblCommodity.FirstOrDefault(n => n.Id == entity.Id);
            if (commodity != null)
            {
                commodity.IM_Status = (int)EntityStatus.Deleted;
                commodity.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodity.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodity.Id));
            }
        }

        public Commodity GetById(Guid Id, bool includeDeactivated = false)
        {
            Commodity entity = (Commodity)_cacheProvider.Get(string.Format(_cacheKey, Id));

            if (entity == null)
            {
                var tbl = _ctx.tblCommodity.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Commodity> GetAll(bool includeDeactivated = false)
        {
            IList<Commodity> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Commodity>(ids.Count);
                foreach (Guid id in ids)
                {
                    Commodity entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCommodity.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Commodity p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public ValidationResultInfo Validate(Commodity itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Commodity Name found"));

            if (GetAll(true).Any(n => n.Id != itemToValidate.Id && n.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Commodity Code found"));

            return vri;
        }

        public Commodity Map(tblCommodity entity)
        {
            Commodity commodity = new Commodity(entity.Id)
            {
                CommodityType = _commoditytypeRepository.GetById(entity.tblCommodityType.Id),
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
            };

            commodity.CommodityGrades = entity.tblCommodityGrade.Select(s =>
            {
                var pi = new CommodityGrade(s.Id)
                {
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    UsageTypeId = s.UsageTypeId,
                    Commodity = new Commodity(s.CommodityId)
                };

                pi._SetStatus((EntityStatus)s.IM_Status);
                pi._SetDateCreated(s.IM_DateCreated);
                pi._SetDateLastUpdated(s.IM_DateLastUpdated);

                return pi;
            }).ToList();
            commodity._SetDateCreated(entity.IM_DateCreated);
            commodity._SetDateLastUpdated(entity.IM_DateLastUpdated);
            commodity._SetStatus((EntityStatus)entity.IM_Status);

            return commodity;
        }

        public void AddCommodityGrade(Guid commodityId, Guid gradeId, string commodityGradeName, int usageTypeId, string commodityGradeCode, string commodityGradeDescription)
        {
            Commodity commodity = GetById(commodityId);
            var newGrade = new CommodityGrade(gradeId)
                {

                    Name = commodityGradeName,
                    UsageTypeId = usageTypeId,
                    Code = commodityGradeCode,
                    Description = commodityGradeDescription,
                    Commodity = commodity
                };

            ValidationResultInfo vri = ValidateGrade(commodity, newGrade);

            if (!vri.IsValid)
            {
                _log.Debug("Commodity not valid");
                throw new DomainValidationException(vri, "");

            }

            if (commodity != null)
            {
                if (gradeId == Guid.Empty)
                    gradeId = Guid.NewGuid();
                CommodityGrade commodityGrade = null;

                commodityGrade = commodity.CommodityGrades.FirstOrDefault(n => n.Id == gradeId) ??
                                 new CommodityGrade(gradeId);

                commodityGrade.Commodity = commodity;
                commodityGrade.Description = commodityGradeDescription;
                commodityGrade.Code =commodityGradeCode;
                commodityGrade.UsageTypeId = usageTypeId;
                commodityGrade.Name = commodityGradeName;

                commodity.CommodityGrades.Add(commodityGrade);

                Save(commodity);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Commodity not set");
            }
        }

        public ValidationResultInfo ValidateGrade(Commodity commodity, CommodityGrade itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (commodity.CommodityGrades.Any(n =>  n._Status != EntityStatus.Deleted &&  n.Name.ToLower() == itemToValidate.Name.ToLower().Trim() && n.Id != itemToValidate.Id))
                vri.Results.Add(new ValidationResult(
                                    string.Format("Duplicate grade name {0} found for commodity {1}",
                                                  itemToValidate.Name, commodity.Name)));
             if (commodity.CommodityGrades.FirstOrDefault(n => n.Code != null) != null)
             {
                 if (commodity.CommodityGrades.Any(n => n._Status != EntityStatus.Deleted && n.Code.ToLower() == itemToValidate.Code.ToLower().Trim() && n.Id != itemToValidate.Id))
                     vri.Results.Add(new ValidationResult(
                                         string.Format("Duplicate grade code {0} found for commodity {1}",
                                                       itemToValidate.Code, commodity.Name)));
             }

            return vri;
        }

        public IEnumerable<CommodityGrade> GetAllGradeByCommodityId(Guid commodityId, bool includeDeactivated = false)
        {
            IList<CommodityGrade> entities = null;
            Commodity commodity = GetById(commodityId);

            if (commodity != null)
            {
                entities = commodity.CommodityGrades.Where(n => n._Status != EntityStatus.Deleted).Select(s => new CommodityGrade(s.Id)
                {
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    UsageTypeId = s.UsageTypeId,
                    _Status = s._Status,
                    _DateCreated = s._DateCreated,
                    _DateLastUpdated = s._DateLastUpdated

                }).ToList();
                
                if (!includeDeactivated)
                    entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            }

           
            return entities;

        }

        public Commodity CreateCommodityItems(string commodityName, string commodityCode, string commodityDescription,
            Guid commodityType, string commodityGradeName, int usageTypeId, string commodityGradeCode, string commodityGradeDescription)
        {
            Guid commodityId = Guid.NewGuid();
            Commodity commodity = new Commodity(commodityId)
            {
                CommodityType = new CommodityType(commodityType),
                Name = commodityName,
                Code = commodityCode,
                Description = commodityDescription,

            };
            commodity.CommodityGrades.Add(new CommodityGrade(Guid.NewGuid())
            {
                Name = commodityGradeName,
                UsageTypeId = usageTypeId,
                Code = commodityGradeCode,
                Description = commodityGradeDescription,
                Commodity = commodity

            });
            return commodity;

        }

        public void SetGradeInactive(Guid commodityId, Guid gradeId)
        {
            var commodity = GetById(commodityId);
            CommodityGrade grades = null;
            if (commodity != null)
            {
                grades = commodity.CommodityGrades.FirstOrDefault(s => s.Id == gradeId);
            }
            if (grades != null)
            {
                grades._SetStatus(EntityStatus.Inactive);
                commodity.CommodityGrades.Add(grades);
                Save(commodity);
            }

        }

        public void SetGradeActive(Guid commodityId, Guid gradeId)
        {
            var commodity = GetById(commodityId);
            CommodityGrade grades = null;
            if (commodity != null)
            {
                grades = commodity.CommodityGrades.FirstOrDefault(s => s.Id == gradeId);
            }
            if (grades != null)
            {
                grades._SetStatus(EntityStatus.Active);
                commodity.CommodityGrades.Add(grades);
                Save(commodity);
            }

        }

        public void SetGradeAsDeleted(Guid commodityId, Guid gradeId)
        {
            var commodity = GetById(commodityId);
            CommodityGrade grades = null;
            if (commodity != null)
            {
                grades = commodity.CommodityGrades.FirstOrDefault(s => s.Id == gradeId);
            }
            if (grades != null)
            {
                grades._SetStatus(EntityStatus.Deleted);
                commodity.CommodityGrades.Add(grades);
                Save(commodity);
            }

        }

        public CommodityGrade GetGradeByGradeId(Guid gradeId)
        {
            return GetAll().SelectMany(s => s.CommodityGrades).FirstOrDefault(s => s.Id == gradeId);
        }

        public QueryResult<Commodity> Query(QueryStandard q)
        {
            IQueryable<tblCommodity> commodityQuery;
            if (q.ShowInactive)
                commodityQuery = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive).AsQueryable();
            else
                commodityQuery = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();


            var queryResult = new QueryResult<Commodity>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                commodityQuery = commodityQuery
                    .Where(
                        s =>
                        s.Code.ToLower().Contains(q.Name.ToLower()) || s.Name.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = commodityQuery.Count();
            commodityQuery = commodityQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                commodityQuery = commodityQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = commodityQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Commodity>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }
    }
}
