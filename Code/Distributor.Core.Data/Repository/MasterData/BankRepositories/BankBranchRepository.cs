using System;
using System.Collections.Generic;
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
   internal  class BankBranchRepository:RepositoryMasterBase<BankBranch>,IBankBranchRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IBankRepository _bankRepository;
        public BankBranchRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IBankRepository bankRepository)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
           _bankRepository = bankRepository;
       }

       

       public Guid Save(BankBranch entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                _log.Info("Bank Branch Not Valid!");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.bbranch.validation.error"));
            
            }
            tblBankBranch bankBranch = _ctx.tblBankBranch.FirstOrDefault(n=>n.Id==entity.Id );
            if (bankBranch == null)
            {
                bankBranch = new tblBankBranch();
                bankBranch.Id = entity.Id;
                bankBranch.IM_Status = (int)EntityStatus.Active;// ;
                bankBranch.IM_DateCreated = dt;
                bankBranch.Id = entity.Id;
                _ctx.tblBankBranch.AddObject(bankBranch);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (bankBranch.IM_Status != (int)entityStatus)
                bankBranch.IM_Status = (int)entity._Status;
            bankBranch.Name = entity.Name;
            bankBranch.Code = entity.Code;
            bankBranch.Description = entity.Description;
            bankBranch.BankId = entity.Bank.Id;
            bankBranch.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblBankBranch.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, bankBranch.Id));
            return bankBranch.Id;
        }

        public void SetInactive(BankBranch entity)
        {
            tblBankBranch bankBranch = _ctx.tblBankBranch.FirstOrDefault(n=>n.Id==entity.Id );
            if (bankBranch != null)
            {
               // bankBranch = new tblBankBranch();
                bankBranch.IM_Status = (int)EntityStatus.Inactive;//false;
                bankBranch.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblBankBranch.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, bankBranch.Id));
            }
        }

       public void SetActive(BankBranch entity)
       {
           ValidationResultInfo vri = Validate(entity);
           _log.Debug("Activating BankBranch");
           var hasUserDependency = false;
           hasUserDependency = _ctx.tblBank.Where(u => u.IM_Status == (int)EntityStatus.Active).Any(u => u.Id == entity.Bank.Id);

           if (!hasUserDependency)
           {
               throw new DomainValidationException(vri, "Cannot activate\r\nInactive Bank");
           }
           else
           {
               tblBankBranch tblBankBranch = _ctx.tblBankBranch.FirstOrDefault(n => n.Id == entity.Id);
               if (tblBankBranch != null)
               {
                   tblBankBranch.IM_Status = (int)EntityStatus.Active;
                   tblBankBranch.IM_DateLastUpdated = DateTime.Now;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey, _ctx.tblBankBranch.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.Id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, tblBankBranch.Id));
               }
           }
		   
       }

       public void SetAsDeleted(BankBranch entity)
       {
           tblBankBranch bankBranch = _ctx.tblBankBranch.FirstOrDefault(n => n.Id == entity.Id);
           if (bankBranch != null)
           {
               // bankBranch = new tblBankBranch();
               bankBranch.IM_Status = (int)EntityStatus.Deleted;//false;
               bankBranch.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblBankBranch.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, bankBranch.Id));
           }
       }

       public BankBranch GetById(Guid Id, bool includeDeactivated = false)
        {
            BankBranch entity = (BankBranch)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblBankBranch.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(BankBranch itemToValidate)
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
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.bbranch.validation.dupname")));
            return vri;
        }


       protected override string _cacheKey
       {
           get { return "BankBranch-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "BankBranchList"; }
       }

       public override IEnumerable<BankBranch> GetAll(bool includeDeactivated = false)
        {
            IList<BankBranch> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<BankBranch>(ids.Count);
                foreach (Guid id in ids)
                {
                    BankBranch entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblBankBranch.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (BankBranch p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public List<BankBranch> GetByBankMasterId(Guid BankmasterId)
       {
           List<tblBankBranch> data = _ctx.tblBankBranch.Where(n => n.BankId == BankmasterId).ToList();
           return data.Select(Map).ToList();
       }

       public BankBranch GetByCode(string code)
       {
           var tbl = _ctx.tblBankBranch.FirstOrDefault(s => s.Code.ToLower() == code.ToLower());
           if (tbl != null)
           {
               return Map(tbl);
           }
           return null;
       }

       public QueryResult<BankBranch> Query(QueryStandard q)
       {
           IQueryable<tblBankBranch> bankBranchQuery;
           if (q.ShowInactive)
               bankBranchQuery = _ctx.tblBankBranch.Where(s => s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive).AsQueryable();
           else
               bankBranchQuery = _ctx.tblBankBranch.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();


           var queryResult = new QueryResult<BankBranch>();
           if (!string.IsNullOrWhiteSpace(q.Name))
           {
               bankBranchQuery = bankBranchQuery
                   .Where(
                       s => 
                           s.tblBank.Name.ToLower().Contains(q.Name.ToLower()) ||
                       s.Code.ToLower().Contains(q.Name.ToLower()) || s.Name.ToLower().Contains(q.Name.ToLower()));
           }

           queryResult.Count = bankBranchQuery.Count();
           bankBranchQuery = bankBranchQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
           if (q.Skip.HasValue && q.Take.HasValue)
               bankBranchQuery = bankBranchQuery.Skip(q.Skip.Value).Take(q.Take.Value);
           var result = bankBranchQuery.ToList();
           queryResult.Data = result.Select(Map).ToList();
           q.ShowInactive = false;
           return queryResult;
       }

       public BankBranch Map(tblBankBranch bankBranch)
        {
            BankBranch branch = new BankBranch(bankBranch.Id)
            {
                Name = bankBranch.Name,
                Code = bankBranch.Code,
                Description = bankBranch.Description,
                Bank = _bankRepository.GetById(bankBranch.tblBank.Id ) 
            };
            branch._SetDateCreated(bankBranch.IM_DateCreated);
            branch._SetDateLastUpdated(bankBranch.IM_DateLastUpdated );
            branch._SetStatus((EntityStatus)bankBranch.IM_Status );

            return branch;
        }
    }
}
