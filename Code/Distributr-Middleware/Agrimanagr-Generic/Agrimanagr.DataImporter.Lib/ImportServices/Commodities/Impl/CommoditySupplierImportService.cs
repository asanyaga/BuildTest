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
  internal  class CommoditySupplierImportService:ICommoditySupplierImportService
  {
      private readonly ICommoditySupplierRepository _commoditySupplierRepository;

      public CommoditySupplierImportService(ICommoditySupplierRepository commoditySupplierRepository)
      {
          _commoditySupplierRepository = commoditySupplierRepository;
      }

      public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<CommoditySupplierImport> entityImports)
      {
          return await Task.Run(async () =>
          {
              var results = new List<ImportValidationResultInfo>();
              var entities = await ConstructEntities(entityImports);
              int count = 0;
              foreach (var product in entities)
              {
                  var res = await ValidateEntityeAsync(product);
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

      public async Task<bool> SaveAsync(IEnumerable<CommoditySupplier> entities)
      {
          return await Task.Run(() =>
                                    {
                                        entities.ToList().ForEach(n => _commoditySupplierRepository.Save(n));
                                        return true;
                                    });
      }


      private async Task<IEnumerable<CommoditySupplier>> ConstructEntities(IEnumerable<CommoditySupplierImport> entities)
      {

          return await Task.Run(() =>
          {
              var temp = new List<CommoditySupplier>();
              var existing = _commoditySupplierRepository.GetAll(true).OfType<CommoditySupplier>().ToList();
              foreach (var entity in entities)
              {
                  var commoditySupplier =
                      existing.FirstOrDefault(
                          p => p.Name != null && p.Name.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase) || p.CostCentreCode != null && p.CostCentreCode.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase)) ??
                      new CommoditySupplier(Guid.NewGuid());

                  commoditySupplier.CostCentreCode = entity.Code;
                  commoditySupplier.CostCentreType=CostCentreType.CommoditySupplier;
                  commoditySupplier.AccountNo = entity.AccountNo;
                  commoditySupplier.BankBranchName = entity.BankBranchName;
                  commoditySupplier.BankName = entity.BankName;
                  commoditySupplier.CommoditySupplierType = (CommoditySupplierType)entity.CommoditySupplierType;
                  DateTime date;
                  commoditySupplier.JoinDate =DateTime.TryParse(entity.JoinDate.ToString(), out date)? date:DateTime.Now;
                  commoditySupplier.Name = entity.Name;
                commoditySupplier._Status=EntityStatus.Active;
                  temp.Add(commoditySupplier);
              }

              return temp;
          });

      }

      private async Task<ImportValidationResultInfo> ValidateEntityeAsync(CommoditySupplier commodityType)
      {
          return await Task.Run(() =>
          {
              var res = _commoditySupplierRepository.Validate(commodityType);
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
