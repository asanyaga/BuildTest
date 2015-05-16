using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class CommodityProducerRepository: RepositoryMasterBase<CommodityProducer>,ICommodityProducerRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private ICommoditySupplierRepository _commoditySupplierRepository;
        private IRegionRepository _regionRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepository;
        private ICentreRepository _centreRepository;

        public CommodityProducerRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ICommoditySupplierRepository commoditySupplierRepository,IRegionRepository regionRepository, IMasterDataAllocationRepository masterDataAllocationRepository, ICentreRepository centreRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _commoditySupplierRepository = commoditySupplierRepository;
            _regionRepository = regionRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _centreRepository = centreRepository;

            _log.Debug("Region Repository Constructor Bootstrap");
        }

        #region Overrides of RepositoryMasterBase<CommodityProducer>

        protected override string _cacheKey
        {
            get { return "CommodityProducer-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CommodityProducerList"; }
        }

        public Guid Save(CommodityProducer entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
                       
            if (!vri.IsValid)
            {
                _log.Debug("CostCentre not valid");
                throw new DomainValidationException(vri, ""/*"CostCentre Entity Not valid"*/);
            }
            DateTime dt = DateTime.Now;

            tblCommodityProducer commodityProducer = _ctx.tblCommodityProducer.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityProducer == null)
            {
                commodityProducer = new tblCommodityProducer();
                commodityProducer.IM_Status = (int)EntityStatus.Active; //true;
                commodityProducer.IM_DateCreated = dt;
                commodityProducer.Id = entity.Id;
                _ctx.tblCommodityProducer.AddObject(commodityProducer);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (commodityProducer.IM_Status != (int)entityStatus)
                commodityProducer.IM_Status = (int)entity._Status;
            commodityProducer.Code = entity.Code;
            commodityProducer.Name = entity.Name;
            commodityProducer.Acrage = ((CommodityProducer)entity).Acrage;
            commodityProducer.RegNo = ((CommodityProducer)entity).RegNo;
            commodityProducer.PhysicalAddress = ((CommodityProducer)entity).PhysicalAddress;
            commodityProducer.Description = ((CommodityProducer)entity).Description;
            commodityProducer.CostCentreId = ((CommodityProducer)entity).CommoditySupplier.Id;
            commodityProducer.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityProducer.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, commodityProducer.Id));
            return commodityProducer.Id;
        }

        public void SetInactive(CommodityProducer entity)
        {
            /*ValidationResultInfo vri = Validate(entity);
            bool hasCommodityTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);

            if (hasCommodityTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {*/
            tblCommodityProducer commodityProducer = _ctx.tblCommodityProducer.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityProducer != null)
                {
                    commodityProducer.IM_Status = (int)EntityStatus.Inactive;
                    commodityProducer.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityProducer.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityProducer.Id));

                }
           /* }*/
        }

        public void SetActive(CommodityProducer entity)
        {
            ValidationResultInfo vri = Validate(entity);
            /*bool hasCommodityTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);
            if (hasCommodityTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }*/
            tblCommodityProducer commodityProducer = _ctx.tblCommodityProducer.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityProducer != null)
            {
                commodityProducer.IM_Status = (int)EntityStatus.Active;
                commodityProducer.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityProducer.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodityProducer.Id));

            }
        }

        public void SetAsDeleted(CommodityProducer entity)
        {
            /*ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);

            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {*/
            tblCommodityProducer commodityProducer = _ctx.tblCommodityProducer.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityProducer != null)
                {
                    commodityProducer.IM_Status = (int)EntityStatus.Deleted;
                    commodityProducer.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityProducer.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityProducer.Id));

                }
            /*}*/
        }

        public CommodityProducer GetById(Guid Id, bool includeDeactivated = false)
        {
            CommodityProducer entity = (CommodityProducer)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCommodityProducer.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public CommodityProducer Map(tblCommodityProducer commodityProducer)
        {
            CommodityProducer commProducer = new CommodityProducer(commodityProducer.Id)
            {
                Code = commodityProducer.Code,
                Name = commodityProducer.Name,
                Acrage = commodityProducer.Acrage,
                RegNo = commodityProducer.RegNo,
                PhysicalAddress= commodityProducer.PhysicalAddress,
                Description = commodityProducer.Description,
                CommoditySupplier = (CommoditySupplier) _commoditySupplierRepository.GetById(commodityProducer.CostCentreId)
            };

            commProducer = MapCentres(commProducer);

            commProducer._SetDateCreated(commodityProducer.IM_DateCreated);
            commProducer._SetDateLastUpdated(commodityProducer.IM_DateLastUpdated);
            commProducer._SetStatus((EntityStatus)commodityProducer.IM_Status);
            return commProducer;
        }

        private CommodityProducer MapCentres(CommodityProducer farm)
        {
            List<MasterDataAllocation> centreAllocations =
                _masterDataAllocationRepository.GetByAllocationType(MasterDataAllocationType.CommodityProducerCentreAllocation,
                                                                    farm.Id, Guid.Empty);
            farm.CommodityProducerCentres = centreAllocations
                .Select(n => _centreRepository.GetById(n.EntityBId)).ToList();

            return farm;
        }

        public override IEnumerable<CommodityProducer> GetAll(bool includeDeactivated = false)
        {
            IList<CommodityProducer> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CommodityProducer>(ids.Count);
                foreach (Guid id in ids)
                {
                    CommodityProducer entity = (CommodityProducer) GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCommodityProducer.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => (CommodityProducer)Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CommodityProducer p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public List<CommodityProducer> GetBySupplier(Guid supplierId)
        {
            return GetAll().Where(s => s.CommoditySupplier.Id == supplierId).ToList();
        }

        public QueryResult<CommodityProducer> Query(QueryBase query)
        {
            var q = query as QueryCommodityProducer;
            IQueryable<tblCommodityProducer> commodityProducer;
            if(q.ShowInactive)
                commodityProducer = _ctx.tblCommodityProducer.Where(s => s.CostCentreId == q.SupplierId).AsQueryable();
            else
                commodityProducer = _ctx.tblCommodityProducer.Where(s => s.CostCentreId == q.SupplierId && s.IM_Status==(int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<CommodityProducer>();
            if (!string.IsNullOrWhiteSpace(q.Name) && commodityProducer.Count()!=0)
            {
                commodityProducer = commodityProducer
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = commodityProducer.Count();
            commodityProducer = commodityProducer.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                commodityProducer = commodityProducer.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = commodityProducer.ToList();
            queryResult.Data = result.Select(Map).OfType<CommodityProducer>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        #endregion

        #region Implementation of IValidation<CostCentre>

        public ValidationResultInfo Validate(CommodityProducer itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            var query = _ctx.tblCommodityProducer.Where(p => p.IM_Status != (int) EntityStatus.Deleted);
            if(query.Any(p => p.Code == itemToValidate.Code && p.Id != itemToValidate.Id))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (!_masterDataAllocationRepository.GetByAllocationType(MasterDataAllocationType.CommodityProducerCentreAllocation, itemToValidate.Id).Any())
            {
                if (itemToValidate.CommodityProducerCentres==null||!itemToValidate.CommodityProducerCentres.Any())
                    vri.Results.Add(new ValidationResult("Assign at Least one Commodity Producer Centre"));
            }
            return vri;

        }

        #endregion
    }
}
