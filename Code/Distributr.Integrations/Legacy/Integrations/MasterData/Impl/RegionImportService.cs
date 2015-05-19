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
    public class RegionImportService : MasterDataImportServiceBase, IRegionImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IRegionRepository _repository;
        private readonly CokeDataContext _ctx;

        private List<ImportValidationResultInfo> validationResultInfos;

        public RegionImportService(IDTOToEntityMapping mappingService, IRegionRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
        }

        

        private async Task<ImportValidationResultInfo> MapAndValidate(RegionDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblRegion.FirstOrDefault(p => p.Name.ToLower() == dto.Name.ToLower());

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
                        _repository.Save(res.Entity as Region, true);
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
        /// <summary>
        /// Region Field array positions
        /// name 1,country =2, description =3
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private async Task<IEnumerable<RegionDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<RegionDTO>();
            return await Task.Run(() =>
            {
                var vris = new List<ImportValidationResultInfo>();
                items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                         {
                                                                             var countryname = SetFieldValue(row, 3);
                                                                             tblCountry country = null;
                                                                             if (!string.IsNullOrEmpty(countryname))
                                                                                 country =
                                                                                     _ctx.tblCountry.FirstOrDefault(
                                                                                         p =>
                                                                                         p.Name.ToLower() ==
                                                                                         countryname.ToLower()||p.Code !=null && p.Code.ToLower()==countryname.ToLower());
                                                                             if (country == null)
                                                                             {
                                                                                 country =
                                                                                     _ctx.tblCountry.FirstOrDefault();
                                                                             }
                                                                             if(country==null)
                                                                             {
                                                                                 var res = new List<ValidationResult>
                                                                                               {
                                                                                                   new ValidationResult(string.Format("Country with Name={0} not found",SetFieldValue(row, 3)))
                                                                                               };
                                                                                 vris.Add(new ImportValidationResultInfo()
                                                                                 {
                                                                                     Results = res
                                                                                 });
                                                                                
                                                                                 return null;
                                                                             }
                                                                             return new RegionDTO()
                                                                                        {
                                                                                            Name = SetFieldValue(row, 1),
                                                                                            CountryMasterId = country.id,
                                                                                            Description =SetFieldValue(row, 2)

                                                                                        };
                                                                         }));
                validationResultInfos.AddRange(vris);

                return items.Where(n=>n!=null);
            });

        }
    }
}
