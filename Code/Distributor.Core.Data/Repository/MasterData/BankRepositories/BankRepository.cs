using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.BankRepositories
{
   internal  class BankRepository:RepositoryMasterBase<Bank>, IBankRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public BankRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
       }

        public Guid Save(Bank entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                _log.Debug("Bank not valid");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.bank.validation.error"));
            }
            tblBank bank = _ctx.tblBank.FirstOrDefault(n => n.Id == entity.Id);
            if (bank == null)
            {
                bank = new tblBank();
                bank.Id = entity.Id;
                bank.IM_DateCreated = dt;
                bank.IM_Status = (int)EntityStatus.Active;//true;
                bank.Id = entity.Id;
                _ctx.tblBank.AddObject(bank );
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (bank.IM_Status != (int)entityStatus)
                bank.IM_Status = (int)entity._Status;
            bank.Name = entity.Name;
            bank.Code = entity.Code;
            bank.Description = entity.Description;
            bank.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblBank.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, bank.Id));
            return bank.Id ;
        }

        public void SetInactive(Bank entity)
        {
            ValidationResultInfo vri = Validate(entity);
            _log.Debug("Deactivating Bank");
            var hasDependency = false;
            hasDependency = _ctx.tblBankBranch.Where(u => u.IM_Status == (int)EntityStatus.Active).Any(u => u.BankId == entity.Id);
            if (hasDependency)
            {
                throw new DomainValidationException(vri, "Cannot Deactivate Bank\r\n Dependency Found");
            }
            tblBank bank = _ctx.tblBank.FirstOrDefault(n=>n.Id==entity.Id );
            if (bank != null)
            {
                bank.IM_Status = (int)EntityStatus.Inactive;//false;
                bank.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblBank.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, bank.Id));
            }
        }

       public void SetActive(Bank entity)
       {
           tblBank bank = _ctx.tblBank.FirstOrDefault(n => n.Id == entity.Id);
           if (bank != null)
           {

               bank.IM_Status = (int)EntityStatus.Active;
               bank.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblBank.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, bank.Id));
           }
       }

       public void SetAsDeleted(Bank entity)
       {
           var vri = Validate(entity);
           tblBank bank = _ctx.tblBank.FirstOrDefault(n => n.Id == entity.Id);
           bool hasDependency = _ctx.tblBankBranch
               .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
               .Any(n => n.BankId == entity.Id);
           if (hasDependency)
               throw new DomainValidationException(vri, "Cannot Delete - Has a bank dependent on it.");
           if (bank != null)
           {
               bank.IM_Status = (int)EntityStatus.Deleted;
               bank.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblBank.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, bank.Id));
           }
       }

       public Bank GetById(Guid Id, bool includeDeactivated = false)
        {
            Bank entity = (Bank)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblBank.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Bank itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.bank.validation.dupname")));
            return vri;
        }


       protected override string _cacheKey
       {
           get { return "Bank-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "BankList"; }
       }

       public override IEnumerable<Bank> GetAll(bool includeDeactivated = false)
        {
            IList<Bank> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Bank>(ids.Count);
                foreach (Guid id in ids)
                {
                    Bank entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblBank.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Bank p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public Bank GetByCode(string code)
       {
           var tbl = _ctx.tblBank.FirstOrDefault(s => s.Code.ToLower() == code.ToLower());
           if (tbl != null)
           {
               return Map(tbl);
           }
           return null;
       }

       public QueryResult<Bank> Query(QueryStandard q)
       {
           IQueryable<tblBank> bankQuery;
           if (q.ShowInactive)
               bankQuery = _ctx.tblBank.Where(s => s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive).AsQueryable();
           else
               bankQuery = _ctx.tblBank.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();


           var queryResult = new QueryResult<Bank>();
           if (!string.IsNullOrWhiteSpace(q.Name))
           {
               bankQuery = bankQuery
                   .Where(
                       s =>
                       s.Code.ToLower().Contains(q.Name.ToLower()) || s.Name.ToLower().Contains(q.Name.ToLower()));
           }

           queryResult.Count = bankQuery.Count();
           bankQuery = bankQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);

           if (q.Skip.HasValue && q.Take.HasValue)
               bankQuery = bankQuery.Skip(q.Skip.Value).Take(q.Take.Value);
           var result = bankQuery.ToList();
           queryResult.Data = result.Select(Map).OfType<Bank>().ToList();
           q.ShowInactive = false;
           return queryResult;
       }

       public Bank Map(tblBank bank)
        {
            Bank ban = new Bank(bank.Id)
            {
                Name=bank.Name,
                Code=bank.Code,
                Description=bank.Description,
            };
            ban._SetDateCreated(bank.IM_DateCreated );
            ban._SetDateLastUpdated(bank.IM_DateLastUpdated );
            ban._SetStatus((EntityStatus)bank.IM_Status );
            return ban;
        }
    }
}
