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
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class SalesmanSupplierRepository : RepositoryMasterBase<SalesmanSupplier>, ISalesmanSupplierRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private ISupplierRepository _supplierRepository;
        private IDistributorSalesmanRepository _distributorSalesmanRepository;


        public SalesmanSupplierRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ISupplierRepository supplierRepository, IDistributorSalesmanRepository distributorSalesmanRepository)
        {

            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _supplierRepository = supplierRepository;
            _distributorSalesmanRepository = distributorSalesmanRepository;



        }

        protected override string _cacheKey
        {
            get { return "SalesmanSupplier-{0}"; }
        }


        protected override string _cacheListKey
        {
            get { return "SalesmanSupplierList"; }
        }


        public override IEnumerable<Domain.Master.CostCentreEntities.SalesmanSupplier> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all SalesmanSuppliers");
            IList<SalesmanSupplier> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<SalesmanSupplier>(ids.Count);
                foreach (Guid id in ids)
                {
                    SalesmanSupplier entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSalesmanSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (SalesmanSupplier p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public SalesmanSupplier Map(tblSalesmanSupplier salesmanSupplier)
        {
            SalesmanSupplier salesmansupplier = new SalesmanSupplier(salesmanSupplier.id)
            {
                //_Status = region.Active.Value,
                Supplier = _supplierRepository.GetById(salesmanSupplier.SupplierId, true),
                DistributorSalesmanRef = new CostCentreRef { Id = salesmanSupplier.CostCentreId },

            };
            salesmansupplier._SetDateCreated(salesmanSupplier.IM_DateCreated);
            salesmansupplier._SetDateLastUpdated(salesmanSupplier.IM_DateLastUpdated);
            salesmansupplier._SetStatus((EntityStatus)salesmanSupplier.IM_Status);

            return salesmansupplier;
        }


        public void Delete(Guid id)
        {
            tblSalesmanSupplier delObj = _ctx.tblSalesmanSupplier.FirstOrDefault(p => p.id == id);
            if (delObj != null)
                _ctx.tblSalesmanSupplier.DeleteObject(delObj);
            _ctx.SaveChanges();
        }

        public List<SalesmanSupplier> GetBySalesman(Guid salemanId)
        {
            return _ctx.tblSalesmanSupplier.Where(p => p.CostCentreId == salemanId && p.IM_Status == (int)EntityStatus.Active).Select(Map).ToList();

        }

        public SalesmanSupplier GetBySalesmanAndSupplier(Guid supplierId, Guid distributorSalesmanRefId)
        {
            var tblsalesmanSupplier =
                _ctx.tblSalesmanSupplier.FirstOrDefault(
                    p =>
                        p.CostCentreId == distributorSalesmanRefId && p.SupplierId == supplierId &&
                        p.IM_Status == (int)EntityStatus.Active);
            if (tblsalesmanSupplier != null)
                return Map(tblsalesmanSupplier);
            return null;

        }



        public Guid Save(SalesmanSupplier entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating SalesmanSupplier");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid SalesmanSupplier");
                throw new DomainValidationException(vri, "Failed to save invalid SalesmanSupplier");
            }
            tblSalesmanSupplier salesmansupplier = _ctx.tblSalesmanSupplier.FirstOrDefault(n => n.id == entity.Id);
            DateTime date = DateTime.Now;
            if (salesmansupplier == null)
            {
                salesmansupplier = new tblSalesmanSupplier
                {
                    IM_DateCreated = date,
                    IM_Status = (int)EntityStatus.Active,// true,
                    id = entity.Id
                };
                _ctx.tblSalesmanSupplier.AddObject(salesmansupplier);
                salesmansupplier.IM_DateLastUpdated = date;
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (salesmansupplier.IM_Status != (int)entityStatus)
                salesmansupplier.IM_Status = (int)entity._Status;
            salesmansupplier.SupplierId = entity.Supplier.Id;
            salesmansupplier.CostCentreId = entity.DistributorSalesmanRef.Id;
            _log.Debug("Saving SalesmanSupplier");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblSalesmanSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, salesmansupplier.id));
            _log.DebugFormat("Successfully saved item id:{0}", salesmansupplier.id);
            return salesmansupplier.id;






        }
        public void SetActive(Domain.Master.CostCentreEntities.SalesmanSupplier entity)
        {

            _log.Debug("Activating SalesmanSupplier Id " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                _log.Debug("Failed to activate invalid salesman Supplier");
                throw new DomainValidationException(vri, "Failed to save invalid SalesmanSupplier");
            }

            tblSalesmanSupplier tblSr = _ctx.tblSalesmanSupplier.FirstOrDefault(n => n.id == entity.Id);
            if (tblSr != null)
            {
                tblSr.IM_Status = (int)EntityStatus.Active;
                tblSr.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalesmanSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblSr.id));
            }

        }

        public void SetInactive(Domain.Master.CostCentreEntities.SalesmanSupplier entity)
        {

            _log.Debug("Inactivating SalesmanSupplier");

            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblSalesmanSupplier sales = _ctx.tblSalesmanSupplier.First(n => n.id == entity.Id);
                if (sales == null || sales.IM_Status == (int)EntityStatus.Inactive)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Inactive;// false;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalesmanSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, sales.id));
            }
        }



        public void SetAsDeleted(Domain.Master.CostCentreEntities.SalesmanSupplier entity)
        {

            _log.Debug("Deleted SalesmanSupplier Id: " + entity.Id);

            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblSalesmanSupplier sales = _ctx.tblSalesmanSupplier.First(n => n.id == entity.Id);
                if (sales == null || sales.IM_Status == (int)EntityStatus.Deleted)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Deleted;// false;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                var val = _ctx.tblSalesmanSupplier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList();
                _cacheProvider.Put(_cacheListKey, val);
                _cacheProvider.Remove(string.Format(_cacheKey, sales.id));
            }
        }

        public Domain.Master.CostCentreEntities.SalesmanSupplier GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting SalesmanSupplier by ID: {0}", Id);
            SalesmanSupplier entity = (SalesmanSupplier)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSalesmanSupplier.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Domain.Master.CostCentreEntities.SalesmanSupplier itemToValidate)
        {
            bool hasDuplicateSalesmanSupplier = false;
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (itemToValidate.Supplier != null && itemToValidate.DistributorSalesmanRef != null)
            {
                hasDuplicateSalesmanSupplier = _ctx.tblSalesmanSupplier
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.id != itemToValidate.Id)
                    .Any(n => n.SupplierId == itemToValidate.Supplier.Id && n.id == itemToValidate.DistributorSalesmanRef.Id);
            }
            if (hasDuplicateSalesmanSupplier)
            {
                vri.Results.Add(new ValidationResult("Duplicate Distributor saleman Supplier found"));
            }
            return vri;
        }



        public new IEnumerable<Domain.Master.CostCentreEntities.SalesmanSupplier> GetItemUpdated(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        Domain.Master.Util.IPagenatedList<Domain.Master.CostCentreEntities.SalesmanSupplier> Core.Repository.IRepositoryMaster<Domain.Master.CostCentreEntities.SalesmanSupplier>.GetAll(int currentPage, int itemPerPage, string searchText, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

    }
}
