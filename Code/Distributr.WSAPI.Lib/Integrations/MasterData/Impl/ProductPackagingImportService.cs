using System;
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
    public class ProductPackagingImportService : MasterDataImportServiceBase, IProductPackagingImportService
    {
        private readonly IProductPackagingRepository _repository;
        private readonly IDTOToEntityMapping _mappingService;
        private readonly CokeDataContext _ctx;

        public ProductPackagingImportService(IProductPackagingRepository repository, IDTOToEntityMapping mappingService, CokeDataContext ctx)
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
                        _repository.Save(res.Entity as ProductPackaging, true);
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

        private async Task<ImportValidationResultInfo> MapAndValidate(ProductPackagingDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblProductPackaging.FirstOrDefault(p => p.Name.ToLower() == dto.Name.ToLower()||
                    p.code !=null&& p.code.ToLower()==dto.Code.ToLower());

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

        private async Task<IEnumerable<ProductPackagingDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductPackagingDTO>();
            return await Task.Run(() =>
            {
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                {
                    var returnableCode =SetFieldValue(row, 4);
                    tblProduct returnableProduct=null;
                    if(!string.IsNullOrEmpty(returnableCode))
                    returnableProduct=
                        _ctx.tblProduct.FirstOrDefault(
                            p => p.ProductCode.ToLower() == returnableCode.ToLower());
                    string code = SetFieldValue(row, 1);
                    string name = SetFieldValue(row, 2);
                    string desc = SetFieldValue(row, 3);
                    return new ProductPackagingDTO()
                    {
                        Code = SetFieldValue(row,1),
                        Name = (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc)) ? desc : name??code,
                        Description = (string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(name)) ? name : desc??code,
                        ReturnableProductMasterId = returnableProduct !=null?returnableProduct.id:Guid.Empty

                    };
                }));
                return items;
            });

        }
    }
}
