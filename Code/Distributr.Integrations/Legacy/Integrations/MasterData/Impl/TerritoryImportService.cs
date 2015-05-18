using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class TerritoryImportService :MasterDataImportServiceBase, ITerritoryImportService
    {
        private readonly IDTOToEntityMapping _mappingService;

        private readonly ITerritoryRepository _territoryRepository;
        private List<Territory> _existingteritories; 

        public TerritoryImportService(IDTOToEntityMapping mappingService, ITerritoryRepository territoryRepository)
        {
            _mappingService = mappingService;
            _territoryRepository = territoryRepository;
            _existingteritories = _territoryRepository.GetAll(true).ToList();
        }

        
        private async Task<ImportValidationResultInfo> MapAndValidate(TerritoryDTO dto,int index)
        {
            return await Task.Run(() =>
                                      {
                                          if (dto == null) return null;
                                          var territory = _mappingService.Map(dto);
                                          var exist =
                                              _existingteritories.FirstOrDefault(
                                                  p => p.Name.ToLower() == territory.Name.ToLower());
                                          territory.Id = exist == null ? Guid.NewGuid() : exist.Id;

                                          var res = _territoryRepository.Validate(territory);
                                          var vResult = new ImportValidationResultInfo()
                                                            {
                                                                Results = res.Results,
                                                                Description =
                                                                    string.Format("Row-{0} code or name=>{1}", index,
                                                                                  territory.Name),
                                                                Entity =territory
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
                        _territoryRepository.Save(res.Entity as Territory, true);
                    }
                    vResults.ValidationResults.Add(res);
                    index++;
                }

                return vResults;
            });
        }
       
        /// <summary>
        /// Territory fields has Territory name is index 0
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private async Task<IEnumerable<TerritoryDTO>> ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<TerritoryDTO>();
            return await Task.Run(() =>
                                      {
                                          items.AddRange(entities.Select(n => n.Fields)
                                                             .Select(row => new TerritoryDTO()
                                                                                {
                                                                                    Name = SetFieldValue(row, 1)
                                                                                }));

                                          return items;
                                      });
            
        }

        
    }
}
