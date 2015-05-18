using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using Distributr.Integrations.Imports.Impl;

namespace Distributr.Integrations.AgrimanagrImports.Impl
{
    public class CommodityOwnerTypeImporterService : BaseImporterService, ICommodityOwnerTypeImporterService
    {
        private CokeDataContext _ctx;
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;

        public CommodityOwnerTypeImporterService(CokeDataContext ctx, ICommodityOwnerTypeRepository commodityOwnerTypeRepository)
        {
            _ctx = ctx;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
        }

        public ImportResponse Save(List<CommodityOwnerTypeImport> imports)
        {
            List<CommodityOwnerType> commodityOwnerTypes = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResult = commodityOwnerTypes.Select(Validate).ToList();

            if (validationResult.Any(j => !j.IsValid))
            {
                var invalidEntities = validationResult.Where(k => !k.IsValid).ToList();

                return new ImportResponse() { Status = false, Info = "Commodity owner type error" + ValidationResultsInfo(invalidEntities) };
            }

            List<CommodityOwnerType> changedCommodityTypes = HasChanged(commodityOwnerTypes);

            foreach (var changedCommodityType in changedCommodityTypes)
            {
                _commodityOwnerTypeRepository.Save(changedCommodityType);
            }

            return new ImportResponse() { Status = true, Info = changedCommodityTypes.Count + " Commodity Owner Type Successfully Imported" };
        }

        private List<CommodityOwnerType> HasChanged(List<CommodityOwnerType> commodityOwnerTypes)
        {
            var changedCommodityOwnerTypes = new List<CommodityOwnerType>();
            foreach (var commodityOwnerType in commodityOwnerTypes)
            {
                var entity = _commodityOwnerTypeRepository.GetById(commodityOwnerType.Id);
                if (entity == null)
                {
                    changedCommodityOwnerTypes.Add(commodityOwnerType);
                    continue;
                }

                bool hasChanged = !String.Equals(entity.Name, commodityOwnerType.Name, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Code, commodityOwnerType.Code, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Description, commodityOwnerType.Description, StringComparison.CurrentCultureIgnoreCase);
                if (hasChanged)
                    changedCommodityOwnerTypes.Add(entity);
            }
            return changedCommodityOwnerTypes;
        }

        private ValidationResultInfo Validate(CommodityOwnerType commodityTypeImport)
        {
           return _commodityOwnerTypeRepository.Validate(commodityTypeImport);
        }

        private CommodityOwnerType Map(CommodityOwnerTypeImport commodityOwnerType)
        {
            var exists = _ctx.tblCommodityOwnerType.FirstOrDefault(l => l.Code == commodityOwnerType.Code);

            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            return new CommodityOwnerType(id)
            {
                Code = commodityOwnerType.Code,
                Name = commodityOwnerType.Name,
                Description = commodityOwnerType.Description
            };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            throw new NotImplementedException();
        }
    }
}
