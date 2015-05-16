using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities.Impl
{
    internal class CommodityOwnerTypeImportService : ICommodityOwnerTypeImportService
    {
        private readonly ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;

        public CommodityOwnerTypeImportService(ICommodityOwnerTypeRepository commodityOwnerTypeRepository)
        {
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
        }

        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<CommodityOwnerTypeImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var commodityTypes = await ConstructCommodityownerTypes(entities);
                int count = 0;
                foreach (var product in commodityTypes)
                {
                    var res = await ValidateCommodityOwnerTypeAsync(product);
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
        public async Task<bool> SaveAsync(IEnumerable<CommodityOwnerType> entities)
        {
            return await Task.Run(() =>
            {
                entities.ToList().ForEach(n => _commodityOwnerTypeRepository.Save(n));
                return true;
            });
        }

        private async Task<IEnumerable<CommodityOwnerType>> ConstructCommodityownerTypes(IEnumerable<CommodityOwnerTypeImport> entities)
        {

            return await Task.Run(() =>
            {
                var temp = new List<CommodityOwnerType>();
                var existing = _commodityOwnerTypeRepository.GetAll(true).ToList();
                foreach (var commodityTypeImport in entities)
                {
                    var commodityType = existing.FirstOrDefault(p => p.Code != null && p.Code.Equals(commodityTypeImport.Code, StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals(commodityTypeImport.Name, StringComparison.CurrentCultureIgnoreCase)) ??
                                        new CommodityOwnerType(Guid.NewGuid());

                    commodityType.Code = commodityTypeImport.Code;
                    commodityType.Description = commodityTypeImport.Description;
                    commodityType.Name = commodityTypeImport.Name;
                    commodityType._Status=EntityStatus.Active;

                    temp.Add(commodityType);
                }

                return temp;
            });

        }
        
        private async Task<ImportValidationResultInfo> ValidateCommodityOwnerTypeAsync(CommodityOwnerType commodityType)
        {
            return await Task.Run(() =>
            {
                var res = _commodityOwnerTypeRepository.Validate(commodityType);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    // Description = "Row-" + count,
                    Entity = commodityType
                };
            });

        }

    }
}
