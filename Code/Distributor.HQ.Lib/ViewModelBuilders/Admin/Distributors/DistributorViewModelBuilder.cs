using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.Distributors;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.HQ.Lib.Helper;
using System.Web.Mvc;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors
{
    public class DistributorViewModelBuilder : IDistributorViewModelBuilder
    {
        IDistributorRepository _distrepository;
        IRegionRepository _regionrepository;
        IUserRepository _userRepository;
        private ICostCentreFactory _costCentreFactory;
        private IProducerRepository _producerRepository;
        private ICostCentreRepository _costCentreRepository;
        private IProductPricingTierRepository _pricingTierRepository;
        private IMasterDataUsage _masterDataUsage;

        public DistributorViewModelBuilder(IDistributorRepository distrepository, IRegionRepository regionrepository, IUserRepository userRepository, ICostCentreFactory costCentreFactory, IProducerRepository producerRepository, ICostCentreRepository costCentreRepository,IProductPricingTierRepository pricingTierRepository, IMasterDataUsage masterDataUsage)
        {
            _distrepository = distrepository;
            _regionrepository = regionrepository;
            _userRepository = userRepository;
            _costCentreFactory = costCentreFactory;
            _producerRepository = producerRepository;
            _costCentreRepository = costCentreRepository;
            _pricingTierRepository=pricingTierRepository;
            _masterDataUsage = masterDataUsage;
        }

        public IList<DistributorViewModel> GetAll(bool inactive = false)
        {
            var types = _distrepository.GetAll(inactive).OfType<Distributor>();
            return types
                .Select(n => Map(n))
                .ToList();
        }

        public DistributorViewModel Get(Guid Id)
        {
            Distributor distributor = (Distributor)_distrepository.GetById(Id);
            if (distributor == null) return null;
               
            return Map(distributor);

        }

        public void Save(DistributorViewModel distviewmodel)
        {
            bool isNew = false;
            CostCentre distributorCC = _costCentreRepository.GetById(distviewmodel.Id);
            if (distributorCC==null) //then new
            {
                
                isNew = true;
                Producer parentCC = _producerRepository.GetProducer();
                distributorCC = _costCentreFactory.CreateCostCentre(distviewmodel.Id, CostCentreType.Distributor, parentCC);
            }
          

            Distributor distributor = distributorCC as Distributor;
            distributor.CostCentreCode = distviewmodel.CostCentreCode;
            distributor.Name = distviewmodel.Name;
            distributor.Owner = distviewmodel.Owner;
            distributor.Region = _regionrepository.GetById(distviewmodel.RegionId);
            distributor.ASM = distviewmodel.ASM != Guid.Empty  ? _userRepository.GetById(distviewmodel.ASM) : null;
            distributor.SalesRep = distviewmodel.SalesRep != Guid.Empty
                                       ? _userRepository.GetById(distviewmodel.SalesRep)
                                       : null;
            distributor.Surveyor = distviewmodel.Surveyor != Guid.Empty
                                       ? _userRepository.GetById(distviewmodel.Surveyor)
                                       : null;
            distributor.AccountNo = distviewmodel.AccountNo;
            distributor.VatRegistrationNo = distviewmodel.VatRegistrationNo;
            distributor.PIN = distviewmodel.PIN;
            distributor.ProductPricingTier = _pricingTierRepository.GetById(distviewmodel.PricingTierId);
            distributor.PaybillNumber = distviewmodel.PayBillNumber;
            distributor.MerchantNumber = distviewmodel.MerchantNumber;

            Guid distributorId = _distrepository.Save(distributor);

            if (isNew)
            {
                CostCentre savedDistributor = _distrepository.GetById(distributorId);
                
                //also setup a distributor pending dispatchwarehouse
                CostCentre _ccdpdw =
                    _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorPendingDispatchWarehouse,
                                                        savedDistributor);
                DistributorPendingDispatchWarehouse ccdpdw = _ccdpdw as DistributorPendingDispatchWarehouse;
                ccdpdw.Name = savedDistributor.Name + " - Pending Dispatch Warehouse ";
                _costCentreRepository.Save(ccdpdw);
            }

        }

        DistributorViewModel Map(Distributor distributor)
        {
            DistributorViewModel distributorViewModel = new DistributorViewModel();
            distributorViewModel.Id = distributor.Id;
            distributorViewModel.Name = distributor.Name;
            distributorViewModel.Owner = distributor.Owner;
            if (distributor.Region != null)
            {
                distributorViewModel.RegionId = distributor.Region.Id;
                distributorViewModel.RegionName = distributor.Region.Name;
            }


            if (distributor.SalesRep != null)
            {
                distributorViewModel.SalesRepName =_userRepository.GetById(distributor.SalesRep.Id).Username;
            }
            if (distributor.Surveyor != null)
            {
                distributorViewModel.SurveyorName =_userRepository.GetById(distributor.Surveyor.Id).Username;
            }
            if (distributor.ASM != null)
            {
                distributorViewModel.ASMName =_userRepository.GetById( distributor.ASM.Id).Username;
            }

            distributorViewModel.RegionId = distributor.Region == null ? Guid.Empty: distributor.Region.Id;
            distributorViewModel.PricingTierId = distributor.ProductPricingTier == null ? Guid.Empty : distributor.ProductPricingTier.Id;
            distributorViewModel.Region =distributor.Region==null ? "": _regionrepository.GetById(distributor.Region.Id).Name;
            distributorViewModel.AccountNo = distributor.AccountNo;
            distributorViewModel.VatRegistrationNo = distributor.VatRegistrationNo;
            distributorViewModel.PIN = distributor.PIN;
            distributorViewModel.CostCentreCode = distributor.CostCentreCode;
            distributorViewModel.DateCreated = distributor._DateCreated;
            distributorViewModel.PayBillNumber = distributor.PaybillNumber;
            distributorViewModel.MerchantNumber = distributor.MerchantNumber;
            distributorViewModel.ASM = distributor.ASM == null ? Guid.Empty : distributor.ASM.Id;
            distributorViewModel.SalesRep = distributor.SalesRep == null ? Guid.Empty : distributor.SalesRep.Id;
            distributorViewModel.Surveyor = distributor.Surveyor == null ? Guid.Empty : distributor.Surveyor.Id;
            if (distributor.Id == Guid.Empty)
                distributorViewModel.IsActive = true;
            else
                distributorViewModel.IsActive = distributor._Status == EntityStatus.Active ? true : false;

            distributorViewModel.CanEditRegion = _masterDataUsage.CanEditHubOrDistributrRegion(distributor);

            return distributorViewModel;


        }

        public void SetInactive(Guid Id)
        {

            var childCcs = _costCentreRepository.GetAll(true).Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == Id).ToList();
            if (childCcs.Where(n => n.CostCentreType != CostCentreType.DistributorPendingDispatchWarehouse).ToList().Count > 0)
            {
                throw new Exception("Distributor has child costcentres which must be deactivated first to continue");
            }
            CostCentre distributor = _distrepository.GetById(Id);
            _distrepository.SetInactive(distributor);
        }

        public void Activate(Guid id)
        {
            CostCentre distributor = _distrepository.GetById(id);
            _distrepository.SetActive(distributor);
        }

        public void Delete(Guid id)
        {
            var childCcs = _costCentreRepository.GetAll(true).Where(n => n.ParentCostCentre != null && n.ParentCostCentre.Id == id).ToList();
            if (childCcs.Where(n => n.CostCentreType != CostCentreType.DistributorPendingDispatchWarehouse).ToList().Count > 0)
            {
                throw new Exception("Distributor has child costcentres which must be deactivated first to continue");
            }
            CostCentre distributor = _distrepository.GetById(id);
            _distrepository.SetAsDeleted(distributor);
        }

        public Dictionary<Guid, string> Region()
        {
            return _regionrepository.GetAll()
                .Select(s => new { s.Id, s.Name }
                ).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> ASM()
        {
            Dictionary<Guid, string> ASMDictionary = new Dictionary<Guid, string>();
            //ASMDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> ASMList = _userRepository.GetAll().Where(n => n.UserType == UserType.ASM);
            foreach (User asm in ASMList)
            {
                ASMDictionary.Add(asm.Id, asm.Username);
            }
            return ASMDictionary;
        }

        public Dictionary<Guid, string> SalesRep()
        {
            Dictionary<Guid, string> SalesRepDictionary = new Dictionary<Guid, string>();
            //SalesRepDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> SalesRepList = _userRepository.GetAll().Where(n => n.UserType == UserType.SalesRep);
            foreach (User SalesRep in SalesRepList)
            {
                SalesRepDictionary.Add(SalesRep.Id, SalesRep.Username);
            }
            return SalesRepDictionary;
        }

        public Dictionary<Guid, string> Surveyor()
        {
            Dictionary<Guid, string> SurveyorDictionary = new Dictionary<Guid, string>();
            //SurveyorDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> SurveyorList = _userRepository.GetAll().Where(n => n.UserType == UserType.Surveyor);
            foreach (User Surveyor in SurveyorList)
            {
                SurveyorDictionary.Add(Surveyor.Id, Surveyor.Username);
            }
            return SurveyorDictionary;
        }

        public List<DistributorViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _distrepository.GetAll(inactive).OfType<Distributor>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items
                .Select(n => Map(n)).ToList();
        }


        public Dictionary<Guid, string> PricingTier()

        {
            return _pricingTierRepository .GetAll()
                .Select(s => new { s.Id, s.Name }
                ).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public QueryResult<DistributorViewModel> Query(QueryStandard query)
        {
            var queryResult = _distrepository.Query(query);
            var result = new QueryResult<DistributorViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }
    }
}
