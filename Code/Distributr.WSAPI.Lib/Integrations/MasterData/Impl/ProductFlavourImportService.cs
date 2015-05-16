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
    public class ProductFlavourImportService : MasterDataImportServiceBase, IProductFlavourImportService
    {
        private readonly IProductFlavourRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;

        public ProductFlavourImportService(IProductFlavourRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
        {
            _repository = repository;
            _mappingService = mappingService;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
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
                        _repository.Save(res.Entity as ProductFlavour, true);
                    }
                    vResults.ValidationResults.Add(res);
                    index++;
                }
                if(validationResultInfos.Any())
                vResults.ValidationResults.AddRange(validationResultInfos);
                return vResults;
            });
        }

        private async Task<ImportValidationResultInfo> MapAndValidate(ProductFlavourDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblProductFlavour.FirstOrDefault(p =>p.name.ToLower() == dto.Name.ToLower());

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

        private async Task<IEnumerable<ProductFlavourDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductFlavourDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                         {
                                                                             var bnameCode =
                                                                                 SetFieldValue(row, 4);
                                                                             tblProductBrand brand = null;
                                                                             if(!string.IsNullOrEmpty(bnameCode))
                                                                            brand =
                                                                                 _ctx.tblProductBrand.FirstOrDefault(
                                                                                     p =>
                                                                                     p.name.ToLower() == bnameCode.ToLower() ||
                                                                                     p.code !=null &&
                                                                                     p.code.ToLower() == bnameCode.ToLower());
                                                                             if (brand == null)
                                                                             {
                                                                                 var res = new List<ValidationResult>
                                                                                               {
                                                                                                   new ValidationResult(
                                                                                                       string.Format(
                                                                                                           "product Brand with Name={0} not found",
                                                                                                           SetFieldValue(row, 4)))
                                                                                               };
                                                                                 validationResultInfos.Add(new ImportValidationResultInfo()
                                                                                 {
                                                                                     Results = res
                                                                                 });
                                                                                 return null;
                                                                             }
                                                                             return new ProductFlavourDTO()
                                                                             {
                                                                                 Code = SetFieldValue(row, 1),
                                                                                 Name = SetFieldValue(row, 2),
                                                                                 ProductBrandMasterId = brand.id,
                                                                                 Description = SetFieldValue(row, 3)

                                                                             };
                                                                         }));
                return items;
            });

        }
    }
}

