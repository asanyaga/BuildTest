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
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
    public class ServiceProviderRepository : RepositoryMasterBase<ServiceProvider>, IServiceProviderRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        private IBankRepository _bankRepository;
        private IBankBranchRepository _bankBranchRepository;

        public ServiceProviderRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
        }

        

        public Guid Save(ServiceProvider entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("ServiceProvider.validation.error"));
                throw new DomainValidationException(vri, "" );
            }
            DateTime dt = DateTime.Now;

            tblServiceProvider provider = _ctx.tblServiceProvider.FirstOrDefault(n => n.id == entity.Id);
            if (provider == null)
            {
                provider = new tblServiceProvider();
                provider.id = entity.Id;
                provider.IM_Status = (int)EntityStatus.Active;
                provider.IM_DateCreated = dt;
                _ctx.tblServiceProvider.AddObject(provider);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (provider.IM_Status != (int)entityStatus)
                provider.IM_Status = (int)entity._Status;
            provider.Code = entity.Code;
            provider.Name = entity.Name;
            provider.Mobile = entity.MobileNumber;
            provider.AccountName = entity.AccountName;
            provider.AccountNumber = entity.AccountNumber;
            provider.Description = entity.Description;
            provider.Gender = (int)entity.Gender;
            provider.IDNo = entity.IdNo;
            provider.PIN = entity.PinNo;
            if (entity.BankBranch != null)
                provider.BankBranchId = entity.BankBranch.Id;
            if (entity.Bank != null)
                provider.BankId = entity.Bank.Id;
            provider.IM_DateLastUpdated = dt;
           
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, provider.id));
            return provider.id; 
        }

        public void SetInactive(ServiceProvider entity)
        {
            var serviceProvider = _ctx.tblServiceProvider.FirstOrDefault(n => n.id == entity.Id);
            if (serviceProvider != null)
            {
                serviceProvider.IM_Status = (int)EntityStatus.Inactive;
                serviceProvider.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, serviceProvider.id));
            }
        }

        public void SetActive(ServiceProvider entity)
        {
            var serviceProvider = _ctx.tblServiceProvider.FirstOrDefault(n => n.id == entity.Id);
            if (serviceProvider != null)
            {
                serviceProvider.IM_Status = (int)EntityStatus.Active;
                serviceProvider.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, serviceProvider.id));
            }
        }

        public void SetAsDeleted(ServiceProvider entity)
        {
            var serviceProvider = _ctx.tblServiceProvider.FirstOrDefault(n => n.id == entity.Id);
            if (serviceProvider != null)
            {
                serviceProvider.IM_Status = (int)EntityStatus.Deleted;
                serviceProvider.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, serviceProvider.id));
            }
        }
      
        public ServiceProvider GetById(Guid id, bool includeDeactivated = false)
        {
            ServiceProvider entity = (ServiceProvider)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblServiceProvider.FirstOrDefault(s => s.id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<ServiceProvider> GetAll(bool includeDeactivated = false)
        {
            IList<ServiceProvider> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ServiceProvider>(ids.Count);
                foreach (Guid id in ids)
                {
                    ServiceProvider entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblServiceProvider.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ServiceProvider p in entities)
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

       

        public QueryResult<ServiceProvider> Query(QueryBase query)
        {
            var q = query as QueryServiceProvider;
            IQueryable<tblServiceProvider> serviceProviderQuery;
            if (q.ShowInactive)
                serviceProviderQuery = _ctx.tblServiceProvider.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                serviceProviderQuery = _ctx.tblServiceProvider.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ServiceProvider>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                serviceProviderQuery = serviceProviderQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = serviceProviderQuery.Count();
            serviceProviderQuery = serviceProviderQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                serviceProviderQuery = serviceProviderQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = serviceProviderQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ServiceProvider>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(ServiceProvider itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblServiceProvider.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (query.Any(s => s.id != itemToValidate.Id && s.IDNo == itemToValidate.IdNo))
                vri.Results.Add(new ValidationResult("Duplicate ID Number found"));

            if(query.Any(s => s.id != itemToValidate.Id &&  !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (query.Any(s => s.id != itemToValidate.Id &&  s.Name == itemToValidate.Name))
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            
            return vri;
        }

        public ServiceProvider Map(tblServiceProvider p)
        {
            var bank =p.BankId.HasValue? _bankRepository.GetById(p.BankId.Value):null;
            var branch =p.BankBranchId.HasValue?_bankBranchRepository.GetById(p.BankBranchId.Value):null;

            var provider = new ServiceProvider(p.id)
            {
                Code = (p.Code ?? ""),
               Description = p.Description??"",
              Name = p.Name,
                Gender = p.Gender.HasValue?(Gender)(p.Gender.Value):Gender.Unknown,
                IdNo = p.IDNo,
               PinNo = (p.PIN??""),
               Bank = bank,
               BankBranch = branch,
               MobileNumber = p.Mobile,
               AccountName = p.AccountName,
               AccountNumber = p.AccountNumber
            };
            provider._SetDateCreated(p.IM_DateCreated);
            provider._SetDateLastUpdated(p.IM_DateLastUpdated);
            provider._SetStatus((EntityStatus)p.IM_Status);
            return provider;
        }

        protected override string _cacheKey
        {
            get { return "ServiceProvider-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ServiceProviderList"; }
        }
    }
}
