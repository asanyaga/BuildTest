using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class CommodityTypeImportService : MasterDataImportServiceBase, ICommodityTypeImportService
    {
        private readonly ICommodityTypeRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public CommodityTypeImportService(ICommodityTypeRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
        }

        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

        public async Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return await Task.Run(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos = ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    if (res.IsValid)
                    {
                        _repository.Save(res.Entity as CommodityType, true);
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


        private async Task<ImportValidationResultInfo> MapAndValidate(CommodityTypeDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist =
                    _ctx.tblCommodityType.FirstOrDefault(
                        p =>
                        p.Name.ToLower() == dto.Name.ToLower() ||
                        p.Code != null && p.Code.Equals(dto.Code, StringComparison.CurrentCultureIgnoreCase));

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
        private IEnumerable<CommodityTypeDTO> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CommodityTypeDTO>();
            items.AddRange(entities.Select(n => n.Fields)
                                   .Select(row => new CommodityTypeDTO()
                                   {
                                       Name = SetFieldValue(row, 1),
                                       Code = SetFieldValue(row, 2),
                                       Description = SetFieldValue(row, 3)

                                   }));
            return items.Where(p => p != null);

        }
    }
}
