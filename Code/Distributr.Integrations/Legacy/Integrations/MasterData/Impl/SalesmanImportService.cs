using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Security;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class SalesmanImportService : MasterDataImportServiceBase, ISalesmanImportService
    {
        private readonly ICostCentreRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;
        private Dictionary<string, string> mobileNumbers; 

        public SalesmanImportService(ICostCentreRepository repository, IUserRepository userRepository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mappingService = mappingService;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
            mobileNumbers = new Dictionary<string, string>(); 
        }

        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

        public async Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return await  Task.Run<MasterDataImportResponse>(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos = await ConstructSalesmanDtOs(imports);
                int index = 1;
                var savedSalesmen = new List<DistributorSalesman>();
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    if (res.IsValid)
                    {
                        var entity = res.Entity as DistributorSalesman;
                        if(HasSalesmanChanged(entity))
                        _repository.Save(entity, true);
                        savedSalesmen.Add(entity);
                    }
                    vResults.ValidationResults.Add(res);
                    index++;
                }
                var users = ConstructUsers(savedSalesmen);
                if(users.Any())
                {
                    var validatedUsers = ValidateUsers(users).ToList();
                    var valid = validatedUsers.Where(p => p.IsValid).Select(p => p.Entity).ToList();
                    if (valid.Any())
                    {
                        valid.ForEach(n => _userRepository.Save((User)n, true));
                    }
                    vResults.ValidationResults.AddRange(validatedUsers.Where(p=>!p.IsValid).ToList());

                }
                if (validationResultInfos.Any())
                {
                    vResults.ValidationResults.AddRange(validationResultInfos);
                    validationResultInfos.Clear();
                }
                return vResults;
            });
        }

        private async Task<ImportValidationResultInfo> MapAndValidate(DistributorSalesmanDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblCostCentre.FirstOrDefault(p => p.Cost_Centre_Code.ToLower() == dto.CostCentreCode.ToLower() && p.CostCentreType==(int)CostCentreType.DistributorSalesman);

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;

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

        private User[] ConstructUsers(IEnumerable<DistributorSalesman> salesmen)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {

                var newSalesmenUsers = new List<User>();

                foreach (var importentity in salesmen)
                {
                    var domainEntity = context.GetInstance<IUserRepository>()
                        .GetAll(true).FirstOrDefault(
                            p => p.Code != null &&
                                 p.Code.Trim() == importentity.CostCentreCode.Trim() || p.Username.ToLower() == importentity.Name.ToLower());
                    bool isNew = false;
                    if (domainEntity == null)
                    {
                        domainEntity = new User(Guid.NewGuid());
                        isNew = true;
                    }
                    string defaultMobileNo = string.Empty;
                    if(mobileNumbers.Any())
                    {
                        try
                        {
                            mobileNumbers.TryGetValue(domainEntity.Code, out defaultMobileNo);
                        }
                        catch
                        {
                            
                        }
                        
                    }
                    
                     if (string.IsNullOrEmpty(defaultMobileNo))
                         defaultMobileNo = "0700000000";
                    domainEntity.CostCentre = importentity.Id;
                    domainEntity.Username = importentity.Name.Trim();
                    domainEntity.UserType = UserType.DistributorSalesman;
                    domainEntity.Mobile = defaultMobileNo;
                    domainEntity.Password = EncryptorMD5.GetMd5Hash("12345678");
                    domainEntity.Mobile = defaultMobileNo;
                    domainEntity._Status = EntityStatus.Active;
                    domainEntity.FirstName = importentity.Name.Trim();
                    domainEntity.LastName = importentity.Name.Trim();
                    domainEntity.Code = importentity.CostCentreCode.Trim();

                    if (isNew || HasSalesmanUserChanged(domainEntity))
                        newSalesmenUsers.Add(domainEntity);

                }
                return newSalesmenUsers.ToArray();
            }

        }
        private bool HasSalesmanUserChanged(User user)
        {
             using (var ctx = new CokeDataContext(Con))
             {
                 var item = ctx.tblUsers.FirstOrDefault(p => p.Id == user.Id);

                 return item == null ||
                        (user.Code != item.Code ||
                         user.Username.Trim().ToLower() != item.UserName.Trim().ToLower());
             }
        }
        private ImportValidationResultInfo[] ValidateUsers(IEnumerable<User> salesmen)
        {
            return (from salesman in salesmen
                    let res =
                        ObjectFactory.Container.GetNestedContainer().GetInstance<IUserRepository>().Validate(
                            salesman)
                    select new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Entity = salesman
                    }).ToArray();
        }

        private bool HasSalesmanChanged(DistributorSalesman import)
        {
            using (var context =new CokeDataContext(Con))
            {
                var salesman =
                    context.tblCostCentre.FirstOrDefault(
                        p =>
                        p.Cost_Centre_Code == import.CostCentreCode && p.CostCentreType == (int) import.CostCentreType);

                return salesman == null ||
                       salesman.Cost_Centre_Code.Trim().ToLower() == import.CostCentreCode.Trim().ToLower()
                       && (salesman.Name.Trim().ToLower() !=import.Name.Trim().ToLower());
            }


        }
       
        private async Task<IEnumerable<DistributorSalesmanDTO>> ConstructSalesmanDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<DistributorSalesmanDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                {
                    var salesmanCode = SetFieldValue(row, 1);
                    var name = SetFieldValue(row, 2);
                    var distributrCode = SetFieldValue(row, 3);
                    var mobileNumber = SetFieldValue(row, 4);
                    if (string.IsNullOrEmpty(salesmanCode) || string.IsNullOrEmpty(name))
                   {
                        var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Salesman name or code is null"))
                                      };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                   }
                      if(string.IsNullOrEmpty(distributrCode))
                    {
                        var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Distributr Code is required for salesman {0}",salesmanCode))
                                      };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                    }

                   
                    
                var distributr = GetDistributr(distributrCode);
                if (distributr == null)
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format(
                                              "Distributor with code={0} not found",
                                              distributrCode))
                                  };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                                                  {
                                                      Results = res
                                                  });
                    return null;
                }
                if (!string.IsNullOrEmpty(mobileNumber) && !mobileNumbers.ContainsValue(mobileNumber))
                    mobileNumbers.Add(name, mobileNumber);
                    return new DistributorSalesmanDTO()
                               {
                                   CostCentreCode = salesmanCode,
                                   Name = name,
                                   ParentCostCentreId = distributr.Id,
                                   CostCentreTypeId = (int) CostCentreType.DistributorSalesman,
                               };


                }));
                return items.Where(p=>p !=null);
            });

        }
       
        
    }
}
