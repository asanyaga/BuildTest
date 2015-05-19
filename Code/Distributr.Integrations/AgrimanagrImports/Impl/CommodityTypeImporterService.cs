using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using Distributr.Integrations.Imports.Impl;

namespace Distributr.Integrations.AgrimanagrImports.Impl
{
    public class CommodityTypeImporterService : BaseImporterService,ICommodityTypeImporterService
    {
        private CokeDataContext _ctx;
        private ICommodityTypeRepository _commodityTypeRepository;

        public CommodityTypeImporterService(CokeDataContext ctx, ICommodityTypeRepository commodityTypeRepository)
        {
            _ctx = ctx;
            _commodityTypeRepository = commodityTypeRepository;
        }

        public ImportResponse Save(List<CommodityTypeImport> imports)
        {
            List<CommodityType> commodityTypes = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResult = commodityTypes.Select(Validate).ToList();

            if (validationResult.Any(j => !j.IsValid))
            {
                var invalidEntities = validationResult.Where(k => !k.IsValid).ToList();

                return new ImportResponse(){Status = false, Info = "Commodity type error" + ValidationResultsInfo(invalidEntities)};
            }

            List<CommodityType> changedCommodityTypes = HasChanged(commodityTypes);

            foreach (var changedCommodityType in changedCommodityTypes)
            {
                _commodityTypeRepository.Save(changedCommodityType);
            }

            return new ImportResponse() { Status = true, Info = changedCommodityTypes.Count + " Commodities Successfully Imported" };
        }

        private List<CommodityType> HasChanged(List<CommodityType> commodityTypes)
        {
            var changedCommodityTypes = new List<CommodityType>();
            foreach (var commodityType in commodityTypes)
            {
                var entity = _commodityTypeRepository.GetById(commodityType.Id);
                if (entity == null)
                {
                    changedCommodityTypes.Add(commodityType);
                    continue;
                }

                bool hasChanged = !String.Equals(entity.Name, commodityType.Name, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Code, commodityType.Code, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Description, commodityType.Description, StringComparison.CurrentCultureIgnoreCase);
                if(hasChanged)
                    changedCommodityTypes.Add(entity);
            }
            return changedCommodityTypes;
        }

        private ValidationResultInfo Validate(CommodityType commodityType)
        {
            return _commodityTypeRepository.Validate(commodityType);
        }

        private CommodityType Map(CommodityTypeImport commodityTypeImport)
        {
            var exists = _ctx.tblCommodityType.FirstOrDefault(l => l.Code == commodityTypeImport.Code);

            Guid id = exists != null? exists.Id : Guid.NewGuid();

            return new CommodityType(id)
            {
                Code = commodityTypeImport.Code,
                Name = commodityTypeImport.Name,
                Description = commodityTypeImport.Description
            };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var commodityType = _ctx.tblCommodityType.FirstOrDefault(l=>l.Code == deletedCode);

                    if (commodityType != null)
                    {
                        var actualCommodityType = _commodityTypeRepository.GetById(commodityType.Id);
                        if (actualCommodityType != null)
                        {
                            _commodityTypeRepository.SetAsDeleted(actualCommodityType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ImportResponse() { Status = false, Info = "Commodity Type Delete Error" + ex };
                }

            }
            return new ImportResponse() { Info = "Commodity Type Deleted Succesfully", Status = true };
        }
    }
}
