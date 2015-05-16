using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class DistrictViewModelBuilder:IDistrictViewModelBuilder
    {
        ICountryRepository _countryRepository;
        IProvincesRepository _provinceRepository;
        IDistrictRepository _districtRepository;
        
        public DistrictViewModelBuilder( ICountryRepository countryRepository,
        IProvincesRepository provinceRepository,
        IDistrictRepository districtRepository)
        {
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
        }
        public Dictionary<Guid, string> GetCountry()
        {
            return _countryRepository.GetAll()
                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
        }

        public Dictionary<Guid, string> GetProvince()
        {
            return _provinceRepository.GetAll()
                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
        }

        public List<DistrictViewModel> GetAll(bool inactive = false)
        {
            //var items = _districtRepository.GetAll(inactive);
            //return items
            //    .Select(n => new DistrictViewModel
            //    {
            //         DistrictName=n.DistrictName,
                     
            //         ProvinceId=_provinceRepository.GetById(n.Province.Id).Id,
            //         ProvinceName=_provinceRepository.GetById(n.Province.Id).Name,
            //         Id=n.Id,
            //         isActive=n._Status
            //    }
            //    ).ToList();
            //var result = from p in _districtRepository.GetAll(inactive)
            //             let prv = GetProvinces(p.Province.Id)
            //             select new DistrictViewModel
            //             {
            //                 DistrictName = p.DistrictName,
            //                 Id = p.Id,
            //                 isActive = p._Status,
            //                 LoadProvince = new Lazy<List<ProvinceViewModel>>(() => { return GetProvinces(p.Province.Id); }),
            //                 ProvinceName = p.Province.Name,
            //                 ProvinceId = p.Province.Id
            //             };
            //return result.ToList();
            return _districtRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }
        private List<ProvinceViewModel> GetProvinces(Guid provinceId)
        {
            var qry = _provinceRepository.GetAll().Where(n => n.Id == provinceId)
                .Select(n => new ProvinceViewModel { 
                 Id=n.Id,
                  Description=n.Description,
                   Name=n.Name,
                 IsActive = n._Status == EntityStatus.Active ? true : false
                }
                )
                .ToList();
            return qry;
        }
        public DistrictViewModel GetById(Guid id)
        {
            District dist = _districtRepository.GetById(id);
            if (dist ==null) return null;
              
           return Map(dist);
           
        }
        DistrictViewModel Map(District dist)
        {
            return new DistrictViewModel 
            {
                DistrictName = dist.DistrictName,
                
                ProvinceId = _provinceRepository.GetById(dist.Province.Id).Id,
                ProvinceName = _provinceRepository.GetById(dist.Province.Id).Name,
                Id = dist.Id,
                isActive = dist._Status == EntityStatus.Active ? true : false
            };
        }
        public void Save(DistrictViewModel dvm)
        {
            District dist = new District(dvm.Id)
            {
             
             Province=_provinceRepository.GetById(dvm.ProvinceId),
             DistrictName=dvm.DistrictName
            };
            _districtRepository.Save(dist);
        }

        public void SetInactive(Guid id)
        {
            District dist = _districtRepository.GetById(id);
            _districtRepository.SetInactive(dist);
        }

        public void SetActive(Guid id)
        {
            District dist = _districtRepository.GetById(id);
            _districtRepository.SetActive(dist);
        }


        public List<DistrictViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _districtRepository.GetAll(inactive).Where(n => (n.DistrictName.ToLower().StartsWith(srchParam.ToLower())) || (n.Province.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items
                .Select(n => new DistrictViewModel
                {
                    DistrictName = n.DistrictName,
                    
                    ProvinceId = _provinceRepository.GetById(n.Province.Id).Id,
                    ProvinceName = _provinceRepository.GetById(n.Province.Id).Name,
                    Id = n.Id,
                    isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }





        public IList<DistrictViewModel> GetByProvince(Guid provinceId, bool inactive = false)
        {
            var items = _districtRepository.GetAll(inactive).Where(n =>n.Province.Id==provinceId);
            return items
                .Select(n => new DistrictViewModel
                {
                    DistrictName = n.DistrictName,
                   
                    ProvinceId = _provinceRepository.GetById(n.Province.Id).Id,
                    ProvinceName = _provinceRepository.GetById(n.Province.Id).Name,
                    Id = n.Id,
                    isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }


        public void SetAsDeleted(Guid id)
        {
            District dist = _districtRepository.GetById(id);
            _districtRepository.SetAsDeleted(dist);
        }

        public QueryResult<DistrictViewModel> Query(QueryStandard q)
        {
            var queryResult = _districtRepository.Query(q);
            var result = new QueryResult<DistrictViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }
    }
}
