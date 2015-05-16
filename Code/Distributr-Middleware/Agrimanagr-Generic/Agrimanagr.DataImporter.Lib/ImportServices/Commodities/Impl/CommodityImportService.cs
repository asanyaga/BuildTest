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
  internal class CommodityImportService:ICommodityImportService
  {
      private readonly ICommodityRepository _commodityRepository;
      private readonly ICommodityTypeRepository _commodityTypeRepository;

      public CommodityImportService(ICommodityRepository commodityRepository)
      {
          _commodityRepository = commodityRepository;
      }

      public CommodityImportService(ICommodityRepository commodityRepository, ICommodityTypeRepository commodityTypeRepository)
      {
          _commodityRepository = commodityRepository;
          _commodityTypeRepository = commodityTypeRepository;
      }

      public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<CommodityImport> entities)
      {
          return await Task.Run(async () =>
          {
              var results = new List<ImportValidationResultInfo>();
              var commodityTypes = await ConstructCommodity(entities);
              int count = 0;
              foreach (var product in commodityTypes)
              {
                  var res = await ValidateCommodityAsync(product);
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
      
      public async Task<bool> SaveAsync(IEnumerable<Commodity> entities)
      {
          return await Task.Run(() =>
          {
              entities.ToList().ForEach(n => _commodityRepository.Save(n));
              return true;
          });
      }

      private async Task<IEnumerable<Commodity>> ConstructCommodity(IEnumerable<CommodityImport> entities)
      {

          return await Task.Run(() =>
          {
              var temp = new List<Commodity>();
              var existing = _commodityRepository.GetAll(true).ToList();
              foreach (var entity in entities)
              {
                  var commodity = existing.FirstOrDefault(p => p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase)) ??
                                      new Commodity(Guid.NewGuid());

                  commodity.Code = entity.Code;
                  commodity.Description = entity.Description;
                  commodity.Name = entity.Name;
                  commodity.CommodityType =
                      _commodityTypeRepository.GetAll(true).FirstOrDefault(
                          p => p.Code == entity.CommodityTypeCode);
                  commodity._Status = EntityStatus.Active;
                 
                  temp.Add(commodity);
              }

              return temp;
          });

      }

      private async Task<ImportValidationResultInfo> ValidateCommodityAsync(Commodity commodity)
      {
          return await Task.Run(() =>
          {
              var res = _commodityRepository.Validate(commodity);
              return new ImportValidationResultInfo()
              {
                  Results = res.Results,
                  // Description = "Row-" + count,
                  Entity = commodity
              };
          });

      } 

  
    }
}
