using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class VATClassRepository : RepositoryMasterBase<VATClass>, IVATClassRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public VATClassRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;

        }

        public Guid Save(VATClass entity, bool? isSync = null)
        {
            _log.Debug("Saving VATClass");
            //validation
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("vat.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("vat.validation.error"));
            }
           
            tblVATClass vat = null;
            DateTime date = DateTime.Now;
            bool exists = _ctx.tblVATClass.Any(n => n.id == entity.Id);
            if (!exists)
            {
                vat = new tblVATClass
                {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    id = entity.Id
                };
                foreach (var vatItem in entity.VATClassItems)
                {
                    vat.tblVATClassItem.Add(new tblVATClassItem()
                                                {
                                                    Rate = vatItem.Rate,
                                                    EffectiveDate = vatItem.EffectiveDate,
                                                    IM_Status = 
                                                        (vatItem._Status == EntityStatus.New  || vatItem._Status == EntityStatus.Active) 
                                                            ? (int)EntityStatus.Active : (int)vatItem._Status,
                                                    IM_DateCreated = DateTime.Now,
                                                    IM_DateLastUpdated = DateTime.Now,
                                                    id = vatItem.Id != Guid.Empty ? vatItem.Id : Guid.NewGuid()
                                                });
                }

                _ctx.tblVATClass.AddObject(vat);
            }
            else
            {
                vat = _ctx.tblVATClass.First(n => n.id == entity.Id);
               // vat.tblVATClassItem.First(n => n.id = vatclass.VATClassItems.)
                foreach (var vatItem in entity.VATClassItems)
                {
                    var vatClassItem = vat.tblVATClassItem.FirstOrDefault(v => v.id == vatItem.Id);
                    if (vatClassItem != null)
                    {
                        vatClassItem.Rate = vatItem.Rate;
                        vatClassItem.EffectiveDate = vatItem.EffectiveDate;
                        vatClassItem.IM_Status =
                            (vatItem._Status == EntityStatus.New || vatItem._Status == EntityStatus.Active)
                                ? (int) EntityStatus.Active
                                : (int) vatItem._Status;
                        vatClassItem.IM_DateLastUpdated = vatItem._DateLastUpdated;
                    }
                    else
                    {
                        vat.tblVATClassItem.Add(new tblVATClassItem()
                        {
                            Rate = vatItem.Rate,
                            EffectiveDate = vatItem.EffectiveDate,
                            IM_Status =
                                (vatItem._Status == EntityStatus.New || vatItem._Status == EntityStatus.Active)
                                    ? (int)EntityStatus.Active : (int)vatItem._Status,
                            IM_DateCreated = DateTime.Now,
                            IM_DateLastUpdated = DateTime.Now,
                            id = vatItem.Id != Guid.Empty ? vatItem.Id : Guid.NewGuid()
                        });
                    }
                }
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (vat.IM_Status != (int)entityStatus)
                vat.IM_Status = (int)entity._Status;
            vat.Name = entity.Name;
            vat.Class = entity.VatClass;
            vat.IM_DateLastUpdated = date;


            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblVATClass.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, vat.id));
            _log.DebugFormat("VATClass Successfully Saved with id: {0}", vat.id);

            return vat.id;

        }

        public void SetInactive(VATClass entity)
        {
            _log.DebugFormat("Setting VATClass {0} inactive", entity.Id);

            ValidationResultInfo vri = Validate(entity);
            bool hasOutletDependencies = _ctx.tblCostCentre.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(d => d.VATClass_Id == entity.Id);
            bool hasProductDependencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(d => d.VatClassId == entity.Id);
            //bool dependenciesPresent = false;
           
            if (hasOutletDependencies||hasProductDependencies)
            {
                
                throw new  DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblVATClass vat = _ctx.tblVATClass.First(n => n.id == entity.Id);
                if (vat == null || vat.IM_Status==(int)EntityStatus.Inactive)
                {
                    return;
                }
                vat.IM_Status = (int)EntityStatus.Inactive;//false;
                vat.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblVATClass.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, vat.id));
                _log.DebugFormat("VATClass Item {0} has been deactivated", entity.Id);

            }

        }

        public void SetActive(VATClass entity)
        {
            tblVATClass tblVatClass = _ctx.tblVATClass.FirstOrDefault(n => n.id == entity.Id);
            if (tblVatClass != null)
            {
                tblVatClass.IM_Status = (int) EntityStatus.Active;
                tblVatClass.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblVATClass.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(n=>n.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblVatClass.id));
            }
        }

        public void SetAsDeleted(VATClass entity)
        {
            _log.DebugFormat("Setting VATClass {0} delete", entity.Id);

            ValidationResultInfo vri = Validate(entity);
            bool hasOutletDependencies = _ctx.tblCostCentre.Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                .Any(d => d.VATClass_Id == entity.Id);
            bool hasProductDependencies = _ctx.tblProduct.Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                .Any(d => d.VatClassId == entity.Id);
            if (hasOutletDependencies || hasProductDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                tblVATClass vat = _ctx.tblVATClass.FirstOrDefault(n => n.id == entity.Id);
                if (vat != null)
                {
                    vat.IM_Status = (int)EntityStatus.Deleted;
                    vat.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblVATClass.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, vat.id));

                }
            }
        }

        public VATClass GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting VATClass by ID {0}", Id);
            VATClass entity = (VATClass)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblVATClass.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        private VATClass Map(tblVATClass vatclass)
        {
            VATClass vat = new VATClass(vatclass.id)
            {
                Name = vatclass.Name,
                VatClass = vatclass.Class,

            };

            vat._SetDateCreated(vatclass.IM_DateCreated);
            vat._SetDateLastUpdated(vatclass.IM_DateLastUpdated);
            vat._SetStatus((EntityStatus)vatclass.IM_Status);

            var items = vatclass.tblVATClassItem;
            vat.VATClassItems = items
                                .Select(n => MapItem(n))
                                .ToList();

            return vat;
        }

        public VATClass.VATClassItem MapItem(tblVATClassItem vci)
        {
            var item = new VATClass.VATClassItem(vci.id, vci.IM_DateCreated, vci.IM_DateLastUpdated, (EntityStatus)vci.IM_Status)
                                           {
                                               Rate = vci.Rate,
                                               EffectiveDate = vci.EffectiveDate,
                                           };
            item._SetDateCreated(vci.IM_DateCreated);
            item._SetDateLastUpdated(vci.IM_DateLastUpdated);
            item._SetStatus((EntityStatus)vci.IM_Status);
            return item;
        }


        protected override string _cacheKey
        {
            get { return "VATClass-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "VATClassList"; }
        }

        public override IEnumerable<VATClass> GetAll(bool includeDeactivated = false)
        {
            IList<VATClass> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<VATClass>(ids.Count);
                foreach (Guid id in ids)
                {
                    VATClass entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblVATClass.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (VATClass p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }


        public void AddNewVatClassLineItem(VATClass vatclass, decimal rate, DateTime effectiveDate)
        {
            _log.Debug("Entering New VatClass");
            tblVATClass vc = _ctx.tblVATClass.First(n => n.id == vatclass.Id);
            tblVATClassItem vci = new tblVATClassItem
            {
                id= Guid.NewGuid(),
                IM_DateCreated = DateTime.Now,
                IM_Status =(int)EntityStatus.Active,// true,
                IM_DateLastUpdated = DateTime.Now,
                Rate = rate,
                EffectiveDate = effectiveDate
            };
            vc.tblVATClassItem.Add(vci);
            vc.IM_DateLastUpdated = DateTime.Now;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblVATClass.Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, vc.id));
            
            _log.DebugFormat("Successfully Saved {0}", vatclass.Id);
        }

      
        public QueryResult<VATClass> Query(QueryBase query)
        {
            var q = query as QueryStandard;
            IQueryable<tblVATClass> vatClassQuery;
            if (q.ShowInactive)
                vatClassQuery = _ctx.tblVATClass.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                vatClassQuery = _ctx.tblVATClass.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<VATClass>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                vatClassQuery = vatClassQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Class.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = vatClassQuery.Count();
            vatClassQuery = vatClassQuery.OrderBy(s => s.Name).ThenBy(s => s.Class);
            if (q.Skip.HasValue && q.Take.HasValue)
                vatClassQuery = vatClassQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = vatClassQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<VATClass>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

       /* QueryResult<VATClass> IVATClassRepository.QueryLineItems(QueryBase query)
        {
            throw new NotImplementedException();
        }*/

        public QueryResult QueryLineItems(QueryBase query)
        {
            var q = query as QueryVatClassLineItems;
            IQueryable<tblVATClassItem> VATClassLineItemsQuery;
            if (q.ShowInactive)
                VATClassLineItemsQuery = _ctx.tblVATClassItem.Where(p => p.IM_Status != (int)EntityStatus.Deleted && p.VATClassID == q.VatClassId).AsQueryable();
            else
                VATClassLineItemsQuery = _ctx.tblVATClassItem.Where(s => s.IM_Status == (int)EntityStatus.Active && s.VATClassID == q.VatClassId).AsQueryable();

            var queryResult = new QueryResult();
            

            queryResult.Count = VATClassLineItemsQuery.Count();
            VATClassLineItemsQuery = VATClassLineItemsQuery.OrderBy(s => s.EffectiveDate).ThenBy(s => s.Rate);
            if (q.Skip.HasValue && q.Take.HasValue)
                VATClassLineItemsQuery = VATClassLineItemsQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = VATClassLineItemsQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<MasterEntity>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        private VATClass.VATClassItem Map(tblVATClassItem tbl)
        {
           var vatItem = new VATClass.VATClassItem(tbl.id)
            {
                Rate = tbl.Rate,
                EffectiveDate=tbl.EffectiveDate

            };

            vatItem._SetDateCreated(tbl.IM_DateCreated);
            vatItem._SetDateLastUpdated(tbl.IM_DateLastUpdated);
            vatItem._SetStatus((EntityStatus)tbl.IM_Status);


            return vatItem;
        }


        public ValidationResultInfo Validate(VATClass itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
           
            var all = GetAll(true);
            bool hasDuplicateName= all
                .Where(n=>n.Id!=itemToValidate.Id)
                .Where(n=>n.Name==itemToValidate.Name)
                .Any();
            bool hasDuplicateVatClass = GetAll()
                .Where(n => n.Id != itemToValidate.Id)
                .Where(n => n.VatClass == itemToValidate.VatClass)
                .Any();
            if (hasDuplicateName)
            {
                vri.Results.Add(new ValidationResult("Duplicate Name Found"));
            }
            if (hasDuplicateVatClass)
            {
                vri.Results.Add(new ValidationResult("Duplicate VATClass Found"));
            }
            // bool hasDuplicate = _ctx.tblRegion.Where(n => n.id != itemToValidate.Id).Any(n => n.Name == itemToValidate.Name);
           //// bool hasDuplicate = false; //AJM Duplicate check does not make sense??
           ////// var vcitems = _ctx.tblVATClassItem.Where(n => n.n != itemToValidate.Id).ToList();
           //// var vcitems = _ctx.tblVATClass.Where(n => n.Name != itemToValidate.Name).ToList();
           //// if (vcitems.Count() > 0)
           //// {
           ////     hasDuplicate = vcitems.Any(n=>n.Name==itemToValidate.Name);//.Max(e => e. <= DateTime.Now);
           //// }

           //// if (hasDuplicate)
           //// {
           ////     vri.Results.Add(new ValidationResult("Duplicate Name found"));
           //// }
            return vri;
        }


        
    }
}
