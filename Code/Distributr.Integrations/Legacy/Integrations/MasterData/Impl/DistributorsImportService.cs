using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Security;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class DistributorsImportService :MasterDataImportServiceBase, IDistributorsImportService
    {
        private readonly ICostCentreRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;
        public DistributorsImportService()
        {
           
        }

        public DistributorsImportService(ICostCentreRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
            validationResultInfos = new List<ImportValidationResultInfo>();
        }

        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

        public async Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return await Task.Run<MasterDataImportResponse>(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos = await ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    if (res.IsValid)
                    {
                        var dist = res.Entity as Distributor;
                       var id= _repository.Save(dist, true);
                        AddUser(dist.Name.Replace("-", "") + "User", id, UserType.WarehouseManager, Guid.Empty);
                       AddDistributorPendingDispatchWarehouse(id, dist.Name);
                    }
                    vResults.ValidationResults.Add(res);
                    index++;
                }
                if (validationResultInfos.Any())
                {
                    vResults.ValidationResults.AddRange(validationResultInfos);
                    validationResultInfos.Clear();
                }
                return vResults;
            });
        }

        private async Task<ImportValidationResultInfo> MapAndValidate(DistributorDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblCostCentre.FirstOrDefault(p => p.Cost_Centre_Code == dto.CostCentreCode.ToLower()||
                    p.Name.ToLower()==dto.Name.ToLower() && p.CostCentreType==(int)CostCentreType.Distributor);
                var producer = ObjectFactory.GetInstance<IProducerRepository>().GetProducer();
                if (exist == null)
                {
                    var groupId = AddUserGroup("admin");
                   
                    entity.Id = Guid.NewGuid();
                    entity.ASM = AddUser("ASM", producer.Id, UserType.ASM, groupId);
                    entity.SalesRep = AddUser("SalesRep", producer.Id, UserType.SalesRep, groupId);
                    entity.Surveyor = AddUser("Surveyor", producer.Id, UserType.Surveyor, groupId);
                }
                else
                {
                    var groupId = AddUserGroup("admin");
                    var  test= ObjectFactory.GetInstance<ICostCentreRepository>().GetById(exist.Id) as Distributor;
                    entity.ASM = AddUser("ASM", producer.Id, UserType.ASM, groupId);
                    entity.SalesRep = AddUser("SalesRep", producer.Id, UserType.SalesRep, groupId);
                    entity.Surveyor = AddUser("Surveyor", producer.Id, UserType.Surveyor, groupId);
                    entity.Id = test.Id;

                }
                entity.ParentCostCentre = new CostCentreRef() {Id = producer.Id};
                var res = _repository.Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =string.Format("Row-{0} name or code=>{1}", index,
                                      entity.Name ?? entity.CostCentreCode),
                    Entity = entity
                };
                return vResult;

            });


        }
        private async Task<IEnumerable<DistributorDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<DistributorDTO>();
            var vris = new List<ImportValidationResultInfo>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                {
                    var code = SetFieldValue(row, 1);
                    var name = SetFieldValue(row, 2);
                    var regionName = SetFieldValue(row, 3);
                    var pin = SetFieldValue(row, 4);
                    var vat = SetFieldValue(row, 5);
                    var patbill = SetFieldValue(row, 6);
                    var marchantNo = SetFieldValue(row, 7);
                    var tier = SetFieldValue(row, 8);
                    var lat = SetFieldValue(row, 9);
                    var longt = SetFieldValue(row, 10);
                     
                    if (string.IsNullOrEmpty(name)|| string.IsNullOrEmpty(code))
                    {
                        var res = new List<ValidationResult> { new ValidationResult("Name or code is either null or empty") };
                        vris.Add(new ImportValidationResultInfo()
                        {
                            Results = res
                        });
                        return null;

                    }
                    tblRegion region = null;
                    if (!string.IsNullOrEmpty(regionName))
                        region =ObjectFactory.GetInstance<CokeDataContext>()
                            .tblRegion.FirstOrDefault(p =>
                                p.Name.ToLower() ==regionName.ToLower());
                    if (region == null)
                    {
                        var res = new List<ValidationResult>
                                     {
                                         new ValidationResult(string.Format("region with Name={0} not found",
                                                                            regionName))
                                     };
                        vris.Add(new ImportValidationResultInfo()
                        {
                            Results = res
                        });
                        return null;
                    }
                    tblPricingTier tierval=null;
                    if (!string.IsNullOrEmpty(tier))
                        tierval = GetPricingTier(tier);

                    
                     
                    
                    return new DistributorDTO()
                    {
                        RegionMasterId = region.id,
                        Name = name,
                        CostCentreCode = code,
                        AccountNo = " ",
                        PIN = pin,
                        PaybillNumber = patbill,
                        VatRegistrationNo = vat,
                        ProductPricingTierMasterId = tierval==null?default(Guid):tierval.id,
                        ASMUserMasterId =default(Guid),
                        SurveyorUserMasterId = default(Guid),
                        SalesRepUserMasterId = default(Guid),
                        CostCentreTypeId = (int)CostCentreType.Distributor,
                        StatusId = (int)EntityStatus.Active,
                        MerchantNumber = marchantNo,
                        Owner = name,
                        Latitude = lat,
                        Longitude = longt
                    };
                }));
                validationResultInfos.AddRange(vris);
                return items.Where(n=>n !=null);
            });

        }
        
     Guid AddUserGroup(string name)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblUserGroup group=null;
                group = ctx.tblUserGroup.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
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
                    ctx.tblUserGroup.AddObject(group);
                    ctx.SaveChanges();
                    AddUserGroupRoles(group.Id);
                }
                return group.Id;
            }

        }

        User AddUser(string username, Guid costCenter, UserType usertype, Guid groupId)
        {
            var userrepo = ObjectFactory.GetInstance<IUserRepository>();
            var tbluser= ObjectFactory.GetInstance<CokeDataContext>().tblUsers.FirstOrDefault(
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
        void AddDistributorPendingDispatchWarehouse(Guid distributorId, string name)
        {
            CostCentre d = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(distributorId);
            CostCentre dpdw = ObjectFactory.GetInstance<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorPendingDispatchWarehouse, d);
            dpdw.Name = name;
          ObjectFactory.GetInstance<ICostCentreRepository>().Save(dpdw);
        }
        void AddUserGroupRoles(Guid groupid)
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
