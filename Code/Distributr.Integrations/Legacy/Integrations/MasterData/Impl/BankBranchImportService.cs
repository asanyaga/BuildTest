using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class BankBranchImportService : MasterDataImportServiceBase, IBankBranchImportService
    {
        private readonly IBankBranchRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
       

        public BankBranchImportService(IBankBranchRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
            validationResultInfos = new List<ImportValidationResultInfo>();
        }

        public async Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            return await Task.Run(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos = await ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    vResults.ValidationResults.Add(res);
                    index++;
                }
                if (validationResultInfos.Any())
                    vResults.ValidationResults.AddRange(validationResultInfos);
                return vResults;
            });
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
                        _repository.Save(res.Entity as BankBranch, true);
                    }
                    vResults.ValidationResults.Add(res);
                    if (validationResultInfos.Any())
                        vResults.ValidationResults.AddRange(validationResultInfos);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(BankBranchDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblBankBranch.FirstOrDefault(p => p.Code.ToLower() == dto.Code.ToLower() || p.Name.ToLower() == dto.Name.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;

                var res = _repository.Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.Name ?? entity.Code),
                    Entity = entity
                };
                return vResult;

            });


        }

        private async Task<IEnumerable<BankBranchDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<BankBranchDTO>();
            return await Task.Run(() =>
                                      {
                                          items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                          {
                                                                              var bankname = SetFieldValue(row, 4);
                                                                              tblBank bank = null;
                                                                              if (!string.IsNullOrEmpty(bankname))
                                                                                  bank =
                                                                                      _ctx.tblBank.FirstOrDefault(
                                                                                          p =>
                                                                                          p.Name.ToLower() ==
                                                                                          bankname.ToLower() || p.Code != null && p.Code.ToLower() == bankname.ToLower());
                                                                              if (bank == null)
                                                                              {
                                                                                  var res = new List<ValidationResult>
                                     {
                                         new ValidationResult(string.Format("Bank with Name={0} not found",
                                                                            SetFieldValue(row, 4)))
                                     };
                                                                                  validationResultInfos.Add(new ImportValidationResultInfo()
                                                                                                                {
                                                                                                                    Results = res
                                                                                                                });
                                                                                  return null;
                                                                              }
                                                                              return new BankBranchDTO()
                                                                              {
                                                                                  Code = SetFieldValue(row, 1),
                                                                                  Name = SetFieldValue(row, 2),
                                                                                  Description = SetFieldValue(row, 3),
                                                                                  BankMasterId = bank.Id
                                                                              };
                                                                          }));
                                          return items;
                                      });
        }
    }
}
