﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class VatClassImportService : MasterDataImportServiceBase, IVatClassImportService
    {
        private readonly IVATClassRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public VatClassImportService(IVATClassRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
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
                        _repository.Save(res.Entity as VATClass, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(VATClassDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblVATClass.FirstOrDefault(p =>p.Name.ToLower() == dto.Name.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;

                var res = _repository.Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.Name),
                    Entity = entity
                };
                return vResult;

            });


        }

        private async Task<IEnumerable<VATClassDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<VATClassDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row => new VATClassDTO()
                {
                    Name = SetFieldValue(row, 1),
                    VatClass = SetFieldValue(row,2),

                    VatClassItems = new List<VatClassItemDTO>()
                                        {
                                            new VatClassItemDTO()
                                                {
                                                    Rate =GetDecimal(SetFieldValue(row, 3)),
                                                    EffectiveDate =GetDatetime(SetFieldValue(row, 4)), 
                                                }
                                        }

                }));
                return items;
            });

        }
    }
}