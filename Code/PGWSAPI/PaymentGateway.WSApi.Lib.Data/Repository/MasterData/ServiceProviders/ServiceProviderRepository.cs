using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Data.Util.Caching;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Repository;
using PaymentGateway.WSApi.Lib.Repository.MasterData;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Util;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WSApi.Lib.Data.Repository.MasterData.ServiceProviders
{
    public class ServiceProviderRepository : RepositoryBase, IServiceProviderRepository
    {
        private ICacheProvider _cacheProvider;
        private PGDataContext _ctx;

        public ServiceProviderRepository(ICacheProvider cacheProvider, PGDataContext ctx)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }

        protected override string _cacheKey
        {
            get { return "ServiceProvider-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ServiceProviderList"; }
        }

        public ValidationResultInfo Validate(ServiceProvider objToValidate)
        {
            ValidationResultInfo vri = objToValidate.BasicValidation();
            //bool hasDuplicateAppId = GetAll().Where(v => v.Id != objToValidate.Id)
            //    .Any(p => p.SdpAppId == objToValidate.SdpAppId);
            //if (hasDuplicateAppId)
            //    vri.Results.Add(new ValidationResult("Duplicate Applicatin ID Found"));
            bool hasDuplicateServiceAppId = GetAll().Where(v => v.Id != objToValidate.Id)
               .Any(p => p.Sid == objToValidate.Sid);
            if (hasDuplicateServiceAppId)
                vri.Results.Add(new ValidationResult("Duplicate Service Provider ID Found"));
            return vri;
        }

        public int Save(ServiceProvider entity)
        {
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Product Details provided not valid");
            }
            DateTime date = DateTime.Now;
            tblServiceProvider tbl = _ctx.tblServiceProvider.FirstOrDefault(s => s.Id == entity.Id);
            if (tbl == null)
            {
                tbl = new tblServiceProvider ();
                tbl.IM_DateCreated = date;
                tbl.IM_IsActive = true;
                _ctx.tblServiceProvider.Add(tbl);
            }
            tbl.IM_DateUpdated = date;
            tbl.AllowOverPayment= entity.AllowOverPayment;
            tbl.AllowPartialPayment = entity.AllowPartialPayment;
            tbl.Currecy = entity.Currency;
            tbl.SDP_APP_ID = entity.SdpAppId;
            tbl.SDP_Password = entity.SdpPassword;
            tbl.SPCode = entity.Code;
            tbl.SPId = entity.Sid;
            tbl.SPName = entity.Name;
            tbl.SubscriberId = entity.SubscriberId;
            tbl.SmsShortCode = entity.SmsShortCode;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(x => x.IM_IsActive).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tbl.Id));
            return tbl.Id;
        }

        public ServiceProvider GetById(int id)
        {
            ServiceProvider entity = (ServiceProvider)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblServiceProvider.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        private ServiceProvider Map(tblServiceProvider tbl)
        {
            return new ServiceProvider
            {
                Id = tbl.Id,
                IsActive = tbl.IM_IsActive,
                DateCreated = tbl.IM_DateCreated,
                DateUpdated = tbl.IM_DateUpdated,
                SubscriberId=tbl.SubscriberId,
                Sid=tbl.SPId,
                SdpPassword=tbl.SDP_Password,
                SdpAppId=tbl.SDP_APP_ID,
                Name=tbl.SPName,
                Currency=tbl.Currecy,
                Code=tbl.SPCode,
                AllowPartialPayment=tbl.AllowPartialPayment,
                AllowOverPayment=tbl.AllowOverPayment,
                SmsShortCode = tbl.SmsShortCode,

            };
        }

        public List<ServiceProvider> GetAll()
        {
            IList<ServiceProvider> entities = null;
            IList<int> ids = (IList<int>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ServiceProvider>(ids.Count);
                foreach (int id in ids)
                {
                    ServiceProvider entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblServiceProvider.Where(x => x.IM_IsActive).ToList().Select(s => Map(s)).ToList();
                if (entities!= null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ServiceProvider p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            return entities.ToList();
        }

        public void Delete(int id)
        {
            var tbl = _ctx.tblServiceProvider.FirstOrDefault(s => s.Id == id);
            if (tbl != null)
            {
                tbl.IM_IsActive = false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblServiceProvider.Where(x => x.IM_IsActive).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tbl.Id));
            }
        }

        public bool RegisterFarmer(FarmerSummary farmer)
        {
            tblFarmers tfarmer = _ctx.tblFarmers.FirstOrDefault(n => n.Id == farmer.Id);
            if (tfarmer == null)
            {
                tfarmer = new tblFarmers()
                              {
                                  IM_Status = true,
                                  Id = farmer.Id,
                                  IM_DateCreated = DateTime.Now
                              };
                _ctx.tblFarmers.Add(tfarmer);
            }
            tfarmer.FullName = farmer.FullName.Trim();
            tfarmer.FactoryCode = farmer.FactoryCode.Trim();
            tfarmer.Code = farmer.Code.Trim();
            tfarmer.FactoryId = farmer.FactoryId;
            tfarmer.FactoryUrl = farmer.FactoryWSUrl.Trim();
            tfarmer.IM_DateLastUpdated = DateTime.Now;

            _ctx.SaveChanges();
            return true;
        }

        public FarmerSummary GetFarmer(string farmerCode, string farmerFactoryCode)
        {
            var farmer = _ctx.tblFarmers.FirstOrDefault(n => n.Code.ToLower() == farmerCode.ToLower().Trim() &&
                n.FactoryCode.ToLower() == farmerFactoryCode.ToLower().Trim());
            if (farmer == null) return null;
            return Map(farmer);
        }

        public List<FarmerSummary> GetRegisteredFarmers(string searchText)
        {
            var farmers =
                _ctx.tblFarmers
                //.Where(n => n.FactoryCode.ToLower().Contains(searchText.ToLower()) ||
                //                           n.FullName.ToLower().Contains(searchText.ToLower()) ||
                //                           n.FactoryCode.ToLower().Contains(searchText.ToLower()) ||
                //                           n.Code.ToLower().Contains(searchText.ToLower())
                //    )
                    .OrderBy(n => n.FullName);

            return farmers.Select(Map).ToList();
        }

        public ServiceProvider GetByServiceProviderId(string id)
        {
            var tbl = _ctx.tblServiceProvider.ToList().FirstOrDefault(s => s.SPId.ToString().ToUpper() == id.ToString().ToUpper());
            if (tbl != null)
            {
                return  Map(tbl);
                
            }
            return null;
        }

        public QueryResult<ServiceProvider> Query(QueryStandard q)
        {
            IQueryable<tblServiceProvider> sProviderQuery;
            if (q.ShowInactive)
                sProviderQuery = _ctx.tblServiceProvider.Where(l => l.IM_IsActive == false || l.IM_IsActive == true).AsQueryable();
            else
                sProviderQuery = _ctx.tblServiceProvider.Where(p => p.IM_IsActive == true).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Name))
                sProviderQuery = sProviderQuery.Where(k => k.SPName.ToLower().Contains(q.Name.ToLower()));

            var queryResult = new QueryResult<ServiceProvider>();
            queryResult.Count = sProviderQuery.Count();

            sProviderQuery = sProviderQuery.OrderBy(l => l.SPName).ThenBy(o => o.SPCode); 
            
            if (q.Skip.HasValue && q.Take.HasValue)
                sProviderQuery = sProviderQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            
            var result = sProviderQuery.ToList();

            queryResult.Data = result.Select(Map).ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        FarmerSummary Map(tblFarmers farmer)
        {
            return new FarmerSummary
                       {
                           Id = farmer.Id,
                           FactoryWSUrl = farmer.FactoryUrl,
                           Code = farmer.Code,
                           DateCreated = farmer.IM_DateCreated,
                           DateUpdated = farmer.IM_DateLastUpdated,
                           FactoryCode = farmer.FactoryCode,
                           FactoryId = farmer.FactoryId,
                           FullName = farmer.FullName,
                           IsActive = farmer.IM_Status
                       };
        }
    }
}
