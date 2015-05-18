using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class CommodityOwnerTypeImportService : MasterDataImportServiceBase, ICommodityOwnerTypeImportService
    {
        private readonly ICommodityOwnerTypeRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public CommodityOwnerTypeImportService(ICommodityOwnerTypeRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
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
            return await Task.Run<MasterDataImportResponse>(async () =>
            {
                var vResults = new MasterDataImportResponse();
                var dtos =ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);
                    if (res.IsValid)
                    {
                        res.Entity._SetStatus(EntityStatus.Active);
                        _repository.Save(res.Entity as CommodityOwnerType, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(CommodityOwnerTypeDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist =
                    _ctx.tblCommodityOwnerType.FirstOrDefault(
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
        private IEnumerable<CommodityOwnerTypeDTO> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CommodityOwnerTypeDTO>();
            items.AddRange(entities.Select(n => n.Fields)
                                   .Select(row => new CommodityOwnerTypeDTO()
                                   {
                                       Name = SetFieldValue(row, 1),
                                       Code = SetFieldValue(row, 2),
                                       Description = SetFieldValue(row, 3),

                                   }));
            return items.Where(p => p != null);

        }
    }
}
