using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.SuppliersRepositories
{
    internal class SupplierRepository : RepositoryMasterBase<Supplier>,ISupplierRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public SupplierRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }


        public Guid Save(Supplier entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Updating Supplier");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.supplier.validation.error"));
            }
            tblSupplier tblSupp = _ctx.tblSupplier.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (tblSupp == null)
            {
                tblSupp = new tblSupplier();
                tblSupp.IM_Status = (int)EntityStatus.Active;// true;
                tblSupp.IM_DateCreated = dt;
                tblSupp.id = entity.Id;
                _ctx.tblSupplier.AddObject(tblSupp);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblSupp.IM_Status != (int)entityStatus)
                tblSupp.IM_Status = (int)entity._Status;    
            tblSupp.IM_DateLastUpdated = dt;
            tblSupp.Name = entity.Name;
            tblSupp.Code = entity.Code;
            tblSupp.Description = entity.Description;
            _ctx.SaveChanges();
            _log.InfoFormat("Invalidating Cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblSupp.id));
            return tblSupp.id;


        }

        public void SetInactive(Supplier entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasSupplierDependencies = _ctx.tblProductBrand
                                                  .Where(s => s.IM_Status == (int)EntityStatus.Active)
                                                  .Any(p => p.SupplierId == entity.Id);
            if (hasSupplierDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                _log.InfoFormat("Setting Supplier Inactive");
                tblSupplier tblSupp = _ctx.tblSupplier.FirstOrDefault(n => n.id == entity.Id);
                if (tblSupp != null)
                {
                    tblSupp.IM_Status = (int)EntityStatus.Inactive;// false;
                    tblSupp.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, tblSupp.id));
                }
            }
        }

        public void SetActive(Supplier entity)
        {
			_log.InfoFormat("Setting Supplier As Active");
        	tblSupplier supplier = _ctx.tblSupplier.FirstOrDefault(n => n.id == entity.Id);
			if(supplier != null)
			{
				supplier.IM_Status = (int) EntityStatus.Active;
				supplier.IM_DateLastUpdated = DateTime.Now;
				_ctx.SaveChanges();
				_cacheProvider.Put(_cacheListKey, _ctx.tblSupplier.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(n=>n.id).ToList());
				_cacheProvider.Remove(string.Format(_cacheKey, supplier.id));
			}
        }

        public void SetAsDeleted(Supplier entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasSupplierDependencies = _ctx.tblProductBrand
                                                  .Where(s => s.IM_Status == (int)EntityStatus.Active)
                                                  .Any(p => p.SupplierId == entity.Id);
            if (hasSupplierDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
            }
            else
            {
                _log.InfoFormat("Setting Supplier Delete");
                tblSupplier tblSupp = _ctx.tblSupplier.FirstOrDefault(n => n.id == entity.Id);
                if (tblSupp != null)
                {
                    tblSupp.IM_Status = (int)EntityStatus.Deleted;// false;
                    tblSupp.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, tblSupp.id));
                }
            }
        }

        public Supplier GetById(Guid Id, bool includeDeactivated = false)
        {
            Supplier entity = (Supplier)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSupplier.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }


        protected override string _cacheKey
        {
            get { return "Supplier-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SupplierList"; }
        }

        public override IEnumerable<Supplier> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Getting all suppliers");
            IList<Supplier> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Supplier>(ids.Count);
                foreach (Guid id in ids)
                {
                    Supplier entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Supplier p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public Supplier GetByCode(string code, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).FirstOrDefault(p => p.Code != null && p.Code.ToLower() == code.ToLower());
        }

       
        public QueryResult<Supplier> Query(QueryStandard query)
        {
            IQueryable<tblSupplier> supplierQuery;
            if (query.ShowInactive)
                supplierQuery = _ctx.tblSupplier.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                supplierQuery = _ctx.tblSupplier.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Supplier>();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                supplierQuery = supplierQuery
                    .Where(s => s.Name.ToLower().Contains(query.Name.ToLower()) || s.Code.ToLower().Contains(query.Name.ToLower()));
            }

            if (query.SupplierId.HasValue)
                supplierQuery = supplierQuery.Where(j => j.id == query.SupplierId.Value);

            queryResult.Count = supplierQuery.Count();
            supplierQuery = supplierQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (query.Skip.HasValue && query.Take.HasValue)
                supplierQuery = supplierQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            var result = supplierQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Supplier>().ToList();
            query.ShowInactive = false;
            return queryResult;
        }

        Supplier Map(tblSupplier tblSupp)
        {
            Supplier supplier = new Supplier(tblSupp.id) 
            {
              Description=tblSupp.Description,
               Code=tblSupp.Code, 
                Name=tblSupp.Name
            };
            supplier._SetDateCreated(tblSupp.IM_DateCreated);
            supplier._SetDateLastUpdated(tblSupp.IM_DateLastUpdated);
            supplier._SetStatus((EntityStatus)tblSupp.IM_Status);
            return supplier;
        }
        public ValidationResultInfo Validate(Supplier itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status != EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (string.IsNullOrEmpty(itemToValidate.Name) ||string.IsNullOrEmpty(itemToValidate.Code))
            {
                return vri;
            }
                
            
            bool hasDuplicateName =_ctx.tblSupplier
                .Where(s => s.id != itemToValidate.Id)
                .Where(p => p.Name.ToLower() == itemToValidate.Name.ToLower())
                .Any();
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.supplier.validation.dupsupplier")));
            return vri;
        }
        
    }
}
