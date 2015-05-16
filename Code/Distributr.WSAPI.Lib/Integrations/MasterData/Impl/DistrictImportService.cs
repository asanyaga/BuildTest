using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData
{
    public class DistrictImportService : MasterDataImportServiceBase, IDistrictImportService
    {
        private readonly IDistrictRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;

        public DistrictImportService(IDistrictRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx, List<ImportValidationResultInfo> validationResultInfos)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
            this.validationResultInfos = validationResultInfos;
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
                return vResults;
            });
        }

        public async Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return await Task.Run(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos = await ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    if (res.IsValid)
                    {
                        _repository.Save(res.Entity as District, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(DistrictDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblDistrict.FirstOrDefault(p => p.District.ToLower() == dto.DistrictName.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;

                var res = _repository.Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.DistrictName),
                    Entity = entity
                };
                return vResult;

            });


        }

        private async Task<IEnumerable<DistrictDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<DistrictDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                {
                    var provincename = SetFieldValue(row, 2);
                    var name = SetFieldValue(row, 1);
                    if (string.IsNullOrEmpty(name))
                    {
                        var res = new List<ValidationResult>{new ValidationResult("Name or code is either null or empty")};
                        validationResultInfos.Add(new ImportValidationResultInfo()
                        {
                            Results=res
                        });
                        return null;

                    }
                    tblProvince province = null;
                    if (!string.IsNullOrEmpty(provincename))
                        province =
                            _ctx.tblProvince.FirstOrDefault(
                                p =>
                                p.Name.ToLower() ==
                                provincename.ToLower());
                    if (province == null)
                    {
                        var res = new List<ValidationResult>
                                     {
                                         new ValidationResult(string.Format("Province with Name={0} not found",
                                                                            provincename))
                                     };
                        validationResultInfos.Add(new ImportValidationResultInfo()
                        {
                            Results = res
                        });
                        return null;
                    }
                    return new DistrictDTO()
                    {
                        DistrictName = name,
                        ProvinceMasterId = province.Id
                    };
                }));

                return items;
            });

        }
    }
}
