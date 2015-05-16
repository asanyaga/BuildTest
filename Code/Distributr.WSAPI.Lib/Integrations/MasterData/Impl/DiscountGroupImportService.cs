using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class DiscountGroupImportService : MasterDataImportServiceBase, IDiscountGroupImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
        private readonly IDiscountGroupRepository _repository;

        public DiscountGroupImportService(IDTOToEntityMapping mappingService, CokeDataContext ctx, IDiscountGroupRepository discountGroupRepository)
        {
            _mappingService = mappingService;
            _ctx = ctx;
            _repository = discountGroupRepository;
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
                var dtos = await ConstructDtOs(imports);
                int index = 1;
                foreach (var dto in dtos)
                {
                    var res = await MapAndValidate(dto, index);

                    if (res.IsValid && HasChanged(res.Entity as DiscountGroup))
                    {
                        _repository.Save(res.Entity as DiscountGroup, true);
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

        private bool HasChanged(DiscountGroup item)
        {
            using (var ctx=new CokeDataContext(Con))
            {
                var group = ctx.tblDiscountGroup.FirstOrDefault(p => p.id == item.Id);
                if (group == null) return true;
                
                if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(group.Name) && item.Name.ToLower() != group.Name.ToLower())
                    return true;

                if (!string.IsNullOrEmpty(item.Code) && !string.IsNullOrEmpty(group.Code) && item.Code.ToLower() != group.Code.ToLower())
                   return true;
                return (group.Code != item.Code || group.Name != item.Name);
            }
        }
        private async Task<ImportValidationResultInfo> MapAndValidate(DiscountGroupDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblDiscountGroup.FirstOrDefault(p => p.Code.ToLower() == dto.Code.ToLower() || p.Name.ToLower() == dto.Name.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;

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
        private async Task<IEnumerable<DiscountGroupDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<DiscountGroupDTO>();
            return await Task.Run(() =>
                                      {
                                          items.AddRange(entities.Select(n => n.Fields)
                                                             .Select(row => new DiscountGroupDTO()
                                                                                {
                                                                                    Code = SetFieldValue(row, 1),
                                                                                    Name = SetFieldValue(row, 2),
                                                                                    
                                                                                }));

                return items;
            });

        }
    }
}
