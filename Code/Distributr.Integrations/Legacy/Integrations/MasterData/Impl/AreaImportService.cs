using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class AreaImportService : MasterDataImportServiceBase, IAreaImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IAreaRepository _repository;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;

        public AreaImportService(IDTOToEntityMapping mappingService, IAreaRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
        }

        

        private async Task<ImportValidationResultInfo> MapAndValidate(AreaDTO dto, int index)
        {
            return await Task.Run(() =>
                                      {
                                          if (dto == null) return null;
                                          var entity = _mappingService.Map(dto);
                                          var exist =
                                              _ctx.tblArea.FirstOrDefault(p => p.Name.ToLower() == dto.Name.ToLower());

                                          entity.Id = exist == null ? Guid.NewGuid() : exist.id;

                                          var res = _repository.Validate(entity);
                                          var vResult = new ImportValidationResultInfo()
                                                            {
                                                                Results = res.Results,
                                                                Description =
                                                                    string.Format("Row-{0} name or code=>{1}", index,
                                                                                  entity.Name ?? entity.Description),
                                                                Entity = entity
                                                            };
                                          return vResult;

                                      });
        }
        private async Task<IEnumerable<AreaDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<AreaDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                         {
                                                                             var name = SetFieldValue(row, 1);
                                                                             var desc = SetFieldValue(row, 2);
                                                                             var reg = SetFieldValue(row, 3);
                                                                             var region = _ctx.tblRegion.FirstOrDefault(p => p.Name == reg);

                                                                             if (string.IsNullOrEmpty(name))
                                                                             {
                                                                                 var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(string.Format("Area  Name is null or not found"))};
                                                                                 validationResultInfos.Add(new ImportValidationResultInfo()
                                                                                 {
                                                                                     Results = res
                                                                                 });
                                                                                 return null;

                                                                             }
                    if (region == null)
                    {
                        var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(string.Format("Area in region with Name={0} not found",
                                                                             SetFieldValue(row, 2)))
                                      };
                        validationResultInfos.Add(new ImportValidationResultInfo()
                        {
                            Results = res
                        });
                        return null;
                        }
                    return new AreaDTO()
                               {
                                   Name = name,
                                   RegionMasterId = region.id,
                                   Description =desc

                               };
                }));

                return items;
            });

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
                        _repository.Save(res.Entity as Area, true);
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
    }
}
