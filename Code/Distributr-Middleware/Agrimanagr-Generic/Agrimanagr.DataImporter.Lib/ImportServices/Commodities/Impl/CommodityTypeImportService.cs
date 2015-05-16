using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities.Impl
{
    internal class CommodityTypeImportService :ICommodityTypeImportService
    {
        private readonly ICommodityTypeRepository _commodityTypeRepository;

        public CommodityTypeImportService(ICommodityTypeRepository commodityTypeRepository)
        {
            _commodityTypeRepository = commodityTypeRepository;
        }

        
        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<CommodityTypeImport> entities)
        {
           return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var commodityTypes =await ConstructCommodityTypes(entities);
                int count = 0;
                foreach (var product in commodityTypes)
                {
                    var res = await ValidateCommodityTypeAsync(product);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description = "Row-" + count,
                        Entity = product
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
            
           
        }

        private async Task<IEnumerable<CommodityType>> ConstructCommodityTypes(IEnumerable<CommodityTypeImport> entities)
        {
            return  await Task.Run(() =>
                         {
                             var temp = new List<CommodityType>();
                             var existing = _commodityTypeRepository.GetAll(true).ToList();
                             foreach (var entity in entities)
                             {
                                 var commodityType = existing.FirstOrDefault(p => p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase) ||p.Name!=null && p.Name.Equals(entity.Name,StringComparison.CurrentCultureIgnoreCase)) ??
                                                     new CommodityType(Guid.NewGuid());

                                 commodityType.Code = entity.Code;
                                 commodityType.Description = entity.Description;
                                 commodityType.Name = entity.Name;
                                 commodityType._Status = EntityStatus.Active;
                                 temp.Add(commodityType);
                             }

                             return temp;
                         });
            
        }

        public async Task<bool> SaveAsync(IEnumerable<CommodityType> entities)
        {
            return await Task.Run(() =>
                                    {
                                        entities.ToList().ForEach(n=>_commodityTypeRepository.Save(n));
                                        return true;
                                    });
        }

        private async Task<ImportValidationResultInfo> ValidateCommodityTypeAsync(CommodityType commodityType)
        {
            return await Task.Run(() =>
            {
                var res = _commodityTypeRepository.Validate(commodityType);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Entity = commodityType
                };
            });
            
        }

      
    }
}
