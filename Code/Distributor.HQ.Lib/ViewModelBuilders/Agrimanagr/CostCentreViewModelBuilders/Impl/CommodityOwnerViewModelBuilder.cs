using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class CommodityOwnerViewModelBuilder: ICommodityOwnerViewModelBuilder
    {
        private ICommodityOwnerRepository _commodityOwnerRepository;
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private ICommoditySupplierRepository _commoditySupplierRepository;
        //private IMaritalStatusRepository _maritalStatusRepository;
        private IMasterDataUsage _masterDataUsage;
        #region Implementation of ICommodityOwnerViewModelBuilder

        public CommodityOwnerViewModelBuilder(ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommoditySupplierRepository commoditySupplierRepository, IMasterDataUsage masterDataUsage, ICommodityOwnerRepository commodityOwnerRepository)
        {
            
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _masterDataUsage = masterDataUsage;
            _commodityOwnerRepository = commodityOwnerRepository;
        }

        public IList<CommodityOwnerViewModel> GetAll(bool inactive = false)
        {
            var hubs = _commodityOwnerRepository.GetAll(true).Select(n => Map((CommodityOwner)n)).ToList();
            return hubs;
        }
        CommodityOwnerViewModel Map(CommodityOwner commodityOwner)
        {
            CommodityOwnerViewModel commodityOwnerVM = new CommodityOwnerViewModel();
            commodityOwnerVM.Id = commodityOwner.Id;
            commodityOwnerVM.Code = commodityOwner.Code;
            commodityOwnerVM.Surname = commodityOwner.Surname;
            commodityOwnerVM.FirstName = commodityOwner.FirstName;
            commodityOwnerVM.LastName = commodityOwner.LastName;
            commodityOwnerVM.IdNo = commodityOwner.IdNo;
            commodityOwnerVM.PinNo = commodityOwner.PinNo;
            commodityOwnerVM.Gender = (int)commodityOwner.Gender;
            commodityOwnerVM.PhysicalAddress = commodityOwner.PhysicalAddress;
            commodityOwnerVM.PostalAddress = commodityOwner.PostalAddress;
            commodityOwnerVM.Email = commodityOwner.Email;
            commodityOwnerVM.PhoneNumber = commodityOwner.PhoneNumber;
            commodityOwnerVM.BusinessNumber = commodityOwner.BusinessNumber;
            commodityOwnerVM.FaxNumber = commodityOwner.FaxNumber;
            commodityOwnerVM.OfficeNumber = commodityOwner.OfficeNumber;
            commodityOwnerVM.Description = commodityOwner.Description;
            commodityOwnerVM.DateOfBirth = DateTime.Parse(commodityOwner.DateOfBirth.ToString());
            commodityOwnerVM.MaritalStatus = (int) commodityOwner.MaritalStatus;//commodityOwner.MaritalStatus.Id;
            commodityOwnerVM.CommodityOwnerType = commodityOwner.CommodityOwnerType.Id;
            commodityOwnerVM.CommoditySupplier = commodityOwner.CommoditySupplier.Id;
            commodityOwnerVM.IsActive = (int)commodityOwner._Status;
            return commodityOwnerVM;
        }

        public List<CommodityOwnerViewModel> SearchCommodityOwners(string srchParam, bool inactive = false)
        {
            var items =
                _commodityOwnerRepository.GetAll(inactive).OfType<CommodityOwner>().Where(
                    n =>
                    (n.FullName.ToLower().Contains(srchParam.ToLower())) ||
                    n.Code.ToLower().Contains(srchParam.ToLower()));
            return items.Select(n => Map(n)).ToList();
        }

        public CommodityOwnerViewModel Get(Guid id)
        {
            CommodityOwner commodityOwner = (CommodityOwner)_commodityOwnerRepository.GetById(id);
            if (commodityOwner == null) return null;
            return Map(commodityOwner);
        }

        public void Save(CommodityOwnerViewModel commodityOwnerVM)
        {
            //var age = DateTime.Now.Year - commodityOwnerVM.DateOfBirth.Year;
            //if(age < 18 || age > 120)
            //{
            //    throw new DomainValidationException(new ValidationResultInfo(), "Farmer must be over 17 years old.");
            //}
            CommodityOwner commodityOwner = new CommodityOwner(commodityOwnerVM.Id);
            commodityOwner.Code = commodityOwnerVM.Code;
            commodityOwner.Surname = commodityOwnerVM.Surname;
            commodityOwner.FirstName = commodityOwnerVM.FirstName;
            commodityOwner.LastName = commodityOwnerVM.LastName;
            commodityOwner.IdNo = commodityOwnerVM.IdNo;
            commodityOwner.PinNo = commodityOwnerVM.PinNo;
            commodityOwner.DateOfBirth = commodityOwnerVM.DateOfBirth;
            commodityOwner.MaritalStatus = (MaritalStatas) commodityOwnerVM.MaritalStatus;//_maritalStatusRepository.GetById(commodityOwnerVM.MaritalStatus);
            commodityOwner.Gender = (Gender)commodityOwnerVM.Gender;
            commodityOwner.PhysicalAddress = commodityOwnerVM.PhysicalAddress;
            commodityOwner.PostalAddress = commodityOwnerVM.PostalAddress;
            commodityOwner.Email = commodityOwnerVM.Email;
            commodityOwner.PhoneNumber = commodityOwnerVM.PhoneNumber;
            commodityOwner.BusinessNumber = commodityOwnerVM.BusinessNumber;
            commodityOwner.FaxNumber = commodityOwnerVM.FaxNumber;
            commodityOwner.OfficeNumber = commodityOwnerVM.OfficeNumber;
            commodityOwner.Description = commodityOwnerVM.Description;
            commodityOwner.CommodityOwnerType = _commodityOwnerTypeRepository.GetById(commodityOwnerVM.CommodityOwnerType);
            commodityOwner._Status = EntityStatus.Active;
            commodityOwner.CommoditySupplier = (CommoditySupplier) _commoditySupplierRepository.GetById(commodityOwnerVM.CommoditySupplier);
            _commodityOwnerRepository.Save(commodityOwner);
        }

        public void SetInactive(Guid Id)
        {
            CommodityOwner commodityOwner = (CommodityOwner)_commodityOwnerRepository.GetById(Id);
            if (_masterDataUsage.CommodityOwnerHasPurchases(commodityOwner.CommoditySupplier))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity owner. Commodity owner is used in transactions. Remove dependencies first to continue");
            }
            _commodityOwnerRepository.SetInactive(commodityOwner);
        }

        public void SetActive(Guid Id)
        {
            CommodityOwner commodityOwner = (CommodityOwner)_commodityOwnerRepository.GetById(Id);
            _commodityOwnerRepository.SetActive(commodityOwner);
        }

        public void SetAsDeleted(Guid Id)
        {
            CommodityOwner commodityOwner = _commodityOwnerRepository.GetById(Id);
            if (_masterDataUsage.CommodityOwnerHasPurchases(commodityOwner.CommoditySupplier))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete commodity owner. Commodity owner is used in transactions. Remove dependencies first to continue");
            }
            _commodityOwnerRepository.SetAsDeleted(commodityOwner);
        }

        public Dictionary<int, string> Gender()
        {
            var dict = Enum.GetValues(typeof(Gender))
               .Cast<Gender>()
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public Dictionary<int, string> MaritalStatus()
        {
            //return _maritalStatusRepository.GetAll().OrderBy(n => n.MStatus).Select(r => new { r.Id, r.MStatus }).ToList().ToDictionary(d => d.Id, d => d.MStatus);
            var dict = Enum.GetValues(typeof(MaritalStatas))
               .Cast<MaritalStatas>()
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public Dictionary<Guid, string> CommodityOwnerType()
        {
            return _commodityOwnerTypeRepository.GetAll().OrderBy(n => n.Name).Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> CommoditySupplier()
        {
            return _commoditySupplierRepository.GetAll().OrderBy(n => n.Name).Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public CommodityOwnerViewModel GetByQuery(QueryCommodityOwner query)
        {
            var commodityOwner = _commodityOwnerRepository.Query(query).Data.OfType<CommodityOwner>().FirstOrDefault();
            if (commodityOwner == null) return null;
            return Map(commodityOwner);
        }

        public QueryResult<CommodityOwnerViewModel> Query(QueryCommodityOwner q)
        {
            var queryResult = _commodityOwnerRepository.Query(q);

            var results = new QueryResult<CommodityOwnerViewModel>();

            results.Data = queryResult.Data.Select(Map).ToList();
            results.Count = queryResult.Count;

            return results;
        }

        #endregion
    }
}
