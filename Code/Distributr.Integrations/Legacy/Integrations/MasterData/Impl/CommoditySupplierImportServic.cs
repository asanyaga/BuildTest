using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class CommoditySupplierImportService : MasterDataImportServiceBase, ICommoditySupplierImportService
    {
        private readonly ICommoditySupplierRepository _repository;
        private readonly IBankRepository _bankRepository;
        private readonly IBankBranchRepository _bankBranchRepository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public CommoditySupplierImportService(ICommoditySupplierRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository)
        {
            _repository = repository;
            _mappingService = mappingService;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _ctx = ctx;
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
                                                var dtos = ConstructDtOs(imports);
                                                int index = 1;
                                                foreach (var dto in dtos)
                                                {
                                                    var res = await MapAndValidate(dto, index);
                                                    if (res.IsValid)
                                                    {
                                                        res.Entity._SetStatus(EntityStatus.Active);
                                                        _repository.Save(res.Entity as CommoditySupplier, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(CommoditySupplierDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist =
                    _ctx.tblCostCentre.FirstOrDefault(
                        p =>p.CostCentreType==(int)CostCentreType.CommoditySupplier &&
                        p.Name.ToLower() == dto.Name.ToLower() ||
                        p.Cost_Centre_Code != null && p.Cost_Centre_Code.Equals(dto.CostCentreCode, StringComparison.CurrentCultureIgnoreCase));

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;

                var res = _repository.Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.Name ?? entity.CostCentreCode),
                    Entity = entity
                };
                return vResult;

            });

        }
        private IEnumerable<CommoditySupplierDTO> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CommoditySupplierDTO>();
          
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                     {
                                                                         var name = SetFieldValue(row, 1);
                                                                         var code = SetFieldValue(row, 2);
                                                                         var typ = SetFieldValue(row, 3);
                                                                         var hubcodeOrName = GetParentCostCentreId(SetFieldValue(row, 4));
                                                                         var jDate = GetDatetime(SetFieldValue(row, 5));
                                                                         var acName = SetFieldValue(row, 6);
                                                                         var acNo = SetFieldValue(row, 7);
                                                                         var pin = SetFieldValue(row, 8);
                                                                         var bnk = GetBank(SetFieldValue(row, 9));
                                                                         var branch =
                                                                             GetBankBranch(SetFieldValue(row, 10),SetFieldValue(row,9));
                                                                         
                                                                         
                                                                         if (string.IsNullOrEmpty(name))
                                                                         {
                                                                             var res = new List<ValidationResult>
                                                                                           {
                                                                                               new ValidationResult(
                                                                                                   string.Format(
                                                                                                       "Commodity supplier name cannot be empty"))
                                                                                           };
                                                                             validationResultInfos.Add(new ImportValidationResultInfo
                                                                                                           ()
                                                                             {
                                                                                 Results = res
                                                                             });
                                                                             return null;
                                                                         }
                                                                         
                                                                         if (string.IsNullOrEmpty(code))
                                                                         {
                                                                             var res = new List<ValidationResult>
                                                                                           {
                                                                                               new ValidationResult(
                                                                                                   string.Format(
                                                                                                       "Commodity supplier code cannot be empty"))
                                                                                           };
                                                                             validationResultInfos.Add(new ImportValidationResultInfo
                                                                                                           ()
                                                                             {
                                                                                 Results = res
                                                                             });
                                                                             return null;
                                                                         }

                                                                        
                                                                         if (hubcodeOrName==Guid.Empty)
                                                                         {
                                                                             var res = new List<ValidationResult>
                                                                                           {
                                                                                               new ValidationResult(
                                                                                                   string.Format(
                                                                                                       "Commodity Supplier Hub cannot be found in the DB"))
                                                                                           };
                                                                             validationResultInfos.Add(new ImportValidationResultInfo
                                                                                                           ()
                                                                             {
                                                                                 Results = res
                                                                             });
                                                                             return null;
                                                                         }
                                                                         CommoditySupplierType type = 0;
                                                                         try
                                                                         {

                                                                             Enum.TryParse(typ, true, out type);
                                                                         }
                                                                         catch
                                                                         {

                                                                         }
                                                                        var sup= new CommoditySupplierDTO()
                                                                                    {
                                                                                        Name = name,
                                                                                        CostCentreCode = code,
                                                                                        CommoditySupplierTypeId =(int)type,
                                                                                        JoinDate = jDate,
                                                                                        AccountName = acName,
                                                                                        AccountNo = acNo,
                                                                                        PinNo = pin,
                                                                                        ParentCostCentreId = hubcodeOrName
                                                                                    };
                                                                         if(bnk !=null)
                                                                             sup.BankId = bnk.Id;
                                                                         if (branch != null)
                                                                             sup.BankBranchId = branch.Id;

                                                                         return sup;
                                                                     }));
           
            return items.Where(p => p != null);

        }

       

        private tblBank GetBank(string name)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblBank supplier = null;
                if (!string.IsNullOrEmpty(name))
                {
                    supplier = ctx
                        .tblBank.FirstOrDefault(
                            p => p.Name.ToLower() == name.ToLower() ||
                                 p.Code != null &&
                                 p.Code.ToLower() == name.ToLower());
                }
                if(supplier==null)
                { 
                    CreateBank(name);
                    supplier = ctx
                       .tblBank.FirstOrDefault(
                           p => p.Name.ToLower() == name.ToLower() ||
                                p.Code != null &&
                                p.Code.ToLower() == name.ToLower());
                }
                
               
                return supplier;
            }
        }
        private tblBankBranch GetBankBranch(string name,string bankName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblBankBranch supplier = null;
                if (!string.IsNullOrEmpty(name))
                {
                    supplier = ctx
                        .tblBankBranch.FirstOrDefault(
                            p => p.Name.ToLower() == name.ToLower() ||
                                 p.Code != null &&
                                 p.Code.ToLower() == name.ToLower());


                }
                if (supplier == null)
                {
                    CreateBankBranch(name, bankName);
                   
                    supplier = ctx
                        .tblBankBranch.FirstOrDefault(
                            p => p.Name.ToLower() == name.ToLower() ||
                                 p.Code != null &&
                                 p.Code.ToLower() == name.ToLower());
                }

                
                return supplier;
            }
        }

        private void CreateBankBranch(string name,string bankName)
        {
           var bankBranch = new BankBranch(Guid.NewGuid());
                    bankBranch.Code = name;
                    bankBranch.Name = name;
                    bankBranch.Description = name;
                    bankBranch.Bank = _bankRepository.GetByCode(bankName);
                    _bankBranchRepository.Save(bankBranch);
        }

        private void CreateBank(string name)
        {
            var bank = new Bank(Guid.NewGuid());
            bank.Code = name;
            bank.Name = name;
            bank.Description = name;
            _bankRepository.Save(bank);
        }

        private Guid GetParentCostCentreId(string costcentreName)
        {
            var Id = Guid.Empty;
            using(var c=new CokeDataContext(Con))
            {
               if(!string.IsNullOrEmpty(costcentreName))
               {
                   var table = c.tblCostCentre.FirstOrDefault(p => p.Name.ToLower() == costcentreName.ToLower() && p.CostCentreType==(int)CostCentreType.Hub);
                   if(table!=null)
                   {
                       Id = table.Id;
                       return Id;
                   }
               }

            }
            
            return Id;
        }
    }
}
