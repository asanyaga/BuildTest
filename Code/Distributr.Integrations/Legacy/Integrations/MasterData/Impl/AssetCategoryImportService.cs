using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class AssetCategoryImportService :MasterDataImportServiceBase,IAssetCategoryImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IAssetCategoryRepository _repository;
        private readonly CokeDataContext _ctx;

        public AssetCategoryImportService(IDTOToEntityMapping mappingService, IAssetCategoryRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
            _ctx = ctx;
        }

        

        private async Task<ImportValidationResultInfo> MapAndValidate(AssetCategoryDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblAssetCategory.FirstOrDefault(p => p.Name.ToLower() == dto.Name.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;

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
                        _repository.Save(res.Entity as AssetCategory, true);
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

        private async Task<IEnumerable<AssetCategoryDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<AssetCategoryDTO>();
            return await Task.Run(() =>
                                      {
                                          items.AddRange(entities.Select(n => n.Fields)
                                                             .Select(row => new AssetCategoryDTO()
                                                                                {
                                                                                    Name = SetFieldValue(row, 1),
                                                                                    Description =
                                                                                        SetFieldValue(row, 2)

                                                                                }));

                return items;
            });

        }
    }
}
