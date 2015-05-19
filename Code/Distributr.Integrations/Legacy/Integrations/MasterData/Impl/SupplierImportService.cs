using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class SupplierImportService : MasterDataImportServiceBase, ISupplierImportService
    {
        private readonly ISupplierRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public SupplierImportService(ISupplierRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
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
                        _repository.Save(res.Entity as Supplier, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(SupplierDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblSupplier.FirstOrDefault(p => p.Code.ToLower() == dto.Code.ToLower() || p.Name.ToLower() == dto.Name.ToLower());

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

        private async Task<IEnumerable<SupplierDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<SupplierDTO>();
            return await Task.Run(() =>
            {
                
                items.AddRange(entities.Select(n => n.Fields).Select(row => new SupplierDTO()
                {
                    Code = SetFieldValue(row, 1),
                    Name = SetFieldValue(row, 2),
                    Description = string.IsNullOrEmpty(SetFieldValue(row, 3)) ? SetFieldValue(row, 2) : SetFieldValue(row, 3)

                }));
                return items;
            });

        }
    }
}
