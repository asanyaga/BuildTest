using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class ProductPackagingTypeImportService : MasterDataImportServiceBase, IProductPackagingTypeImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IProductPackagingTypeRepository _repository;
        private readonly CokeDataContext _ctx;

        public ProductPackagingTypeImportService(IDTOToEntityMapping mappingService, IProductPackagingTypeRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
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
                        _repository.Save(res.Entity as ProductPackagingType, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(ProductPackagingTypeDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblProductPackagingType.FirstOrDefault(p => p.name.ToLower() == dto.Name.ToLower() || p.code.ToLower() == dto.Name.ToLower());

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

        private async Task<IEnumerable<ProductPackagingTypeDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductPackagingTypeDTO>();
            return await Task.Run(() =>
            {
                
                                          items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                          {
                                                                              var name = SetFieldValue(row, 2);
                                                                             
                                                                              if (string.IsNullOrEmpty(name))
                                                                              {
                                                                                  var res = new List<ValidationResult>
                                                                                                {
                                                                                                    new ValidationResult
                                                                                                        (string.Format(
                                                                                                            "Packagingtype name is null or empty"))
                                                                                                };
                                                                                  validationResultInfos.Add(new ImportValidationResultInfo()
                                                                                                                {
                                                                                                                    Results = res
                                                                                                                });
                                                                                  return null;
                                                                              }
                                                                              var desc = SetFieldValue(row, 3);
                                                                              return new ProductPackagingTypeDTO()
                                                                              {
                                                                                  Code = SetFieldValue(row, 1),
                                                                                  Name = name,
                                                                                  Description =string.IsNullOrEmpty(desc)?name:desc,
                                                                                 
                                                                              };
                                                                          }));
                                          return items;
                                      });
               

        }
    }
}
