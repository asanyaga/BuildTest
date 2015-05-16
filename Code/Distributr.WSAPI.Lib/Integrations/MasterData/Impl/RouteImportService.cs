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

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class RouteImportService : MasterDataImportServiceBase, IRouteImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IRouteRepository _repository;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;
        public RouteImportService(IDTOToEntityMapping mappingService, IRouteRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
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
                    if (res.IsValid)
                    {
                        var route = res.Entity as Route;
                        if (HasRouteChanged(route))
                            _repository.Save(route, true);
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
        private bool HasRouteChanged(Route route)
        {
            var item = _repository.GetById(route.Id);
            if (item == null) return true;
            return item.Code.ToLower() != route.Code.ToLower() || item.Name.ToLower() != route.Name.ToLower() ||
                   item.Region.Id != route.Region.Id;
        }
        private async Task<ImportValidationResultInfo> MapAndValidate(RouteDTO dto, int index)
        {
            return await Task.Run(() =>
            {
                if (dto == null) return null;
                var entity = _mappingService.Map(dto);
                var exist = _ctx.tblRoutes.FirstOrDefault(p => p.Name.ToLower() == dto.Name.ToLower()||p.Code !=null &&p.Code.ToLower()==dto.Code.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.RouteID;

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

        private async Task<IEnumerable<RouteDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var vris = new List<ImportValidationResultInfo>();
            var items = new List<RouteDTO>();
            return await Task.Run(() =>
                                      {
                                          items.AddRange(entities
                                                             .Select(n => n.Fields)
                                                             .Select(row =>
                                                                         {
                                                                             var name = SetFieldValue(row, 1);
                                                                             var code = SetFieldValue(row, 2);
                                                                             var regionname = SetFieldValue(row, 3);
                                                                             if (string.IsNullOrEmpty(name) ||
                                                                                 string.IsNullOrEmpty(code))
                                                                             {
                                                                                 var res = new List<ValidationResult>
                                                                                               {
                                                                                                   new ValidationResult(
                                                                                                       "Name or code is either null or empty")
                                                                                               };
                                                                                 vris.Add(new ImportValidationResultInfo
                                                                                              ()
                                                                                              {
                                                                                                  Results=res
                                                                                              });
                                                                                 return null;

                                                                             }
                                                                            
                                                                             tblRegion region = null;
                                                                             if (!string.IsNullOrEmpty(regionname))
                                                                                 region =
                                                                                     _ctx.tblRegion.FirstOrDefault(
                                                                                         p =>
                                                                                         p.Name.ToLower() ==
                                                                                         regionname.ToLower());
                                                                             if (region == null)
                                                                             {
                                                                                 var res = new List<ValidationResult>
                                                                                               {
                                                                                                   new ValidationResult(
                                                                                                       string.Format(
                                                                                                           "Region with Name={0} not found",
                                                                                                           regionname))
                                                                                               };
                                                                                 validationResultInfos.Add(new ImportValidationResultInfo
                                                                                                               ()
                                                                                                               {
                                                                                                                   Results = res
                                                                                                               });
                                                                                 return null;
                                                                             }
                                                                             return new RouteDTO()
                                                                                        {
                                                                                            Name = name,
                                                                                            RegionId = region.id,
                                                                                            Code = code
                                                                                        };
                                                                         }));
                                          validationResultInfos.AddRange(vris);
                return items;
            });

        }
    }
}
