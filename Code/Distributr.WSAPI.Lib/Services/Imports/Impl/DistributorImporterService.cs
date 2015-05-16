using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using StructureMap;
using log4net;
using RabbitMQ.Client.Impl;
using MethodBase = System.Reflection.MethodBase;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class DistributorImporterService : BaseImporterService, IDistributorImporterService
    {
        private IDistributorRepository _distributorRepository;
        private IRegionRepository _regionRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private IUserRepository _userRepository;
        private ICostCentreRepository _costCentreRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public DistributorImporterService(IDistributorRepository distributorRepository, CokeDataContext context,
                                          IRegionRepository regionRepository,
                                          IProductPricingTierRepository productPricingTierRepository,
                                          IUserRepository userRepository, ICostCentreRepository costCentreRepository)
        {
            _distributorRepository = distributorRepository;
            _context = context;
            _regionRepository = regionRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _userRepository = userRepository;
            _costCentreRepository = costCentreRepository;
        }

        public ImportResponse Save(List<DistributorImport> imports)
        {
            try
            {
                var mappingValidationList = new List<string>();
                List<Distributor> distributors = imports.Select(s=>Map(s,mappingValidationList)).ToList();
                if (mappingValidationList.Any())
                {
                    return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };
                }
                List<ValidationResultInfo> validationResults = distributors.Select(Validate).ToList();

                if (validationResults.Any(p => !p.IsValid))
                {
                    return new ImportResponse() {Status = false, Info = ValidationResultsInfo(validationResults)};

                }
                List<Distributor> changedDistributors = HasChanged(distributors);

                var groupId = AddUserGroup("admin");
                foreach (var changedDistributor in changedDistributors)
                {
                    var id = _distributorRepository.Save(changedDistributor);
                    if(changedDistributor.Name.Contains(" "))
                    {
                        AddUser(changedDistributor.Name.Replace(" ", "") + "User", id, UserType.WarehouseManager, groupId);
                    }
                    else
                    {
                        AddUser(changedDistributor.Name.Replace("-", "") + "User", id, UserType.WarehouseManager, groupId);
                    }

                 
                    AddDistributorPendingDispatchWarehouse(id, changedDistributor.Name);
                }
                return new ImportResponse()
                           {Status = true, Info = changedDistributors.Count + " Distributors Successfully Imported"};
            }
            catch (Exception ex)
            {

                _log.Error("Saving Distributors Error" + ex.ToString());
                return null;
            }

        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var distributrId =
                        _context.tblCostCentre.Where(
                            p => p.Name == deletedCode && p.CostCentreType == (int) CostCentreType.Distributor).Select(
                                p => p.Id).FirstOrDefault();

                    var distributr = _costCentreRepository.GetById(distributrId);
                    if (distributr != null)
                    {
                        _costCentreRepository.SetAsDeleted(distributr);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Distributr Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() {Info = "Distributr Deleted Succesfully", Status = true};
        }

        private List<Distributor> HasChanged(List<Distributor> distributors)
        {
            var changedDistributors = new List<Distributor>();
            foreach (var distributor in distributors)
            {
                var entity = _costCentreRepository.GetById(distributor.Id) as Distributor;
                if (entity == null)
                {
                    changedDistributors.Add(distributor);
                    continue;
                }
                bool hasChanged = false;
                if (entity.Name.ToLower() != distributor.Name.ToLower() ||
                    entity.CostCentreCode.ToLower() != distributor.CostCentreCode.ToLower())
                {
                    hasChanged = true;
                }

                var currentRegion = distributor.Region;
                var currentSalesRep = distributor.SalesRep;
                var currentSurveyor = distributor.Surveyor;
                var currentASM = distributor.ASM;

                var previousRegion = entity.Region;
                var previousSalesRep = entity.SalesRep;
                var previousSurveyor = entity.Surveyor;
                var previousASM = entity.ASM;


                //if (currentASM != previousASM || currentRegion != previousRegion || currentSalesRep != previousSalesRep ||
                //    currentSurveyor != previousSurveyor)
                //{
                //    hasChanged = true;
                //}

                if (hasChanged)
                {
                    changedDistributors.Add(distributor);
                }
            }
            return changedDistributors;
        }

        protected ValidationResultInfo Validate(Distributor distributor)
        {
            return _distributorRepository.Validate(distributor);
        }

        protected Distributor Map(DistributorImport distributorImport, List<string> mappingvalidationList)
        {
            var exists = _context.tblCostCentre.FirstOrDefault(p => p.Cost_Centre_Code == distributorImport.Code);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();


            var regionId = _context.tblRegion.Where(p => p.Name == distributorImport.RegionName).Select(p => p.id).FirstOrDefault();
            var region = _regionRepository.GetById(regionId);
            if(region==null){mappingvalidationList.Add(string.Format("Invalid Region Code {0}",distributorImport.RegionName));}
           
            var productPricingTier = _productPricingTierRepository.GetByCode(distributorImport.ProductPricingTierCode);

            var producer = ObjectFactory.GetInstance<IProducerRepository>().GetProducer();
            var groupId = AddUserGroup("admin");

            User ASM = AddUser("ASM", producer.Id, UserType.ASM, groupId);
            User salesRep = AddUser("SalesRep", producer.Id, UserType.SalesRep, groupId);
            User surveyor = AddUser("Surveyor", producer.Id, UserType.Surveyor, groupId);

            

            if (exists != null)
            {
                AddUserGroup("Admin");

                ASM = exists.Distributor_ASM_Id != null
                          ? _userRepository.GetById(exists.Distributor_ASM_Id.Value)
                          : ASM;//AddUser("ASM", producer.Id, UserType.ASM, groupId);
                salesRep = exists.SalesRep_Id != null
                               ? _userRepository.GetById(exists.SalesRep_Id.Value)
                               : salesRep;//AddUser("SalesRep", producer.Id, UserType.SalesRep, groupId);
                surveyor = exists.Surveyor_Id != null
                               ? _userRepository.GetById(exists.Surveyor_Id.Value)
                               : surveyor;//AddUser("Surveyor", producer.Id, UserType.Surveyor, groupId);

            }
            var distributor = new Distributor(id);
            distributor.Name = distributorImport.Name;
            distributor.CostCentreCode = distributorImport.Code;
            distributor.PIN = distributorImport.PIN;
            distributor.VatRegistrationNo = distributorImport.VatRegistrationNo;
            distributor.AccountNo = distributorImport.AccountNo;
            distributor.PaybillNumber = distributorImport.PaybillNumber;
            distributor.MerchantNumber = distributorImport.MerchantNumber;
            distributor.Owner = distributorImport.Name;
            distributor.CostCentreType = CostCentreType.Distributor;
            distributor.AccountNo = "";

            distributor.Region = region;
            distributor.ProductPricingTier = productPricingTier;
            distributor.ASM = ASM;
            distributor.SalesRep = salesRep;
            distributor.Surveyor = surveyor;

            distributor.ParentCostCentre = new CostCentreRef() {Id = producer.Id};
            //distributor.Latitude = distributorImport.Latitude;
            //distributor.Longitude = distributorImport.Longitude;

            return distributor;

        }

        private User AddUser(string username, Guid costCenter, UserType usertype, Guid groupId)
        {
            var userrepo = ObjectFactory.GetInstance<IUserRepository>();
            var tbluser = ObjectFactory.GetInstance<CokeDataContext>().tblUsers.FirstOrDefault(
                p => p.UserName.ToLower() == username.ToLower() && p.UserType == (int) usertype);
            if (tbluser != null)
                return userrepo.GetById(tbluser.Id);

            var usr = new User(Guid.NewGuid())
                          {
                              Username = username,
                              Password = EncryptorMD5.GetMd5Hash("12345678"),
                              Mobile = "07000000",
                              PIN = "",
                              UserType = usertype,
                              CostCentre = costCenter,
                              Group = ObjectFactory.GetInstance<IUserGroupRepository>().GetById(groupId),


                          };
            usr._SetStatus(EntityStatus.Active);
            userrepo.Save(usr);
            return userrepo.GetById(usr.Id);
        }

        private void AddDistributorPendingDispatchWarehouse(Guid distributorId, string name)
        {
            CostCentre d = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(distributorId);
            CostCentre dpdw = ObjectFactory.GetInstance<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(),
                                                                                               CostCentreType.
                                                                                                   DistributorPendingDispatchWarehouse,
                                                                                               d);
            dpdw.Name = name;
            ObjectFactory.GetInstance<ICostCentreRepository>().Save(dpdw);
        }

        private Guid AddUserGroup(string name)
        {

            tblUserGroup group = null;
            group = _context.tblUserGroup.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (group == null)
            {
                group = new tblUserGroup
                            {
                                Id = Guid.NewGuid(),
                                Description = name,
                                Name = name,
                                IM_DateCreated = DateTime.Now,
                                IM_DateLastUpdated = DateTime.Now,
                                IM_Status = (int) EntityStatus.Active
                            };
                _context.tblUserGroup.AddObject(group);
                _context.SaveChanges();
                AddUserGroupRoles(group.Id);
            }
            return group.Id;

        }

        private void AddUserGroupRoles(Guid groupid)
        {
            UserGroup usergroup = ObjectFactory.GetInstance<IUserGroupRepository>().GetById(groupid);
            foreach (var val in RolesHelper.GetRoles())
            {
                Guid id = Guid.NewGuid();
                bool canAcess = true;

                UserGroupRoles r = new UserGroupRoles(id)
                                       {
                                           UserGroup = usergroup,
                                           UserRole = val.Id,
                                           CanAccess = canAcess
                                       };
                ObjectFactory.GetInstance<IUserGroupRolesRepository>()
                    .Save(r);
            }
        }
    }
}
