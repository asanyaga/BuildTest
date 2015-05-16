using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using Distributr.WSAPI.Lib.Services.Imports.Impl;

namespace Distributr.WSAPI.Lib.Services.AgrimanagrImports.Impl
{
    public class CommodityImporterService : BaseImporterService, ICommodityImporterService
    {
        private CokeDataContext _ctx;
        private ICommodityRepository _commodityRepository;

        public CommodityImporterService(CokeDataContext ctx, ICommodityRepository commodityRepository)
        {
            _ctx = ctx;
            _commodityRepository = commodityRepository;
        }

        public ImportResponse Save(List<CommodityImport> imports)
        {
            var mappingValidationList = new List<string>();
            List<Commodity> commodities = imports.Select(l => Map(l, mappingValidationList)).ToList();

            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };
            }

            List<ValidationResultInfo> validationResult = commodities.Select(Validate).ToList();

            if (validationResult.Any(j => !j.IsValid))
            {
                var invalidEntities = validationResult.Where(k => !k.IsValid).ToList();

                return new ImportResponse() { Status = false, Info = "Commodity error" + ValidationResultsInfo(invalidEntities) };
            }

            List<Commodity> changedCommodities = HasChanged(commodities);

            foreach (var changedCommodity in changedCommodities)
            {
                _commodityRepository.Save(changedCommodity);
            }

            return new ImportResponse() { Status = true, Info = changedCommodities.Count + " Commodities Successfully Imported" };
        }

        private List<Commodity> HasChanged(List<Commodity> commodities)
        {
            var changedCommodities = new List<Commodity>();
            foreach (var commodity in commodities)
            {
                var entity = _commodityRepository.GetById(commodity.Id);
                if (entity == null)
                {
                    changedCommodities.Add(commodity);
                    continue;
                }

                bool hasChanged = !String.Equals(entity.Name, commodity.Name, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Code, commodity.Code, StringComparison.CurrentCultureIgnoreCase) ||
                                  !String.Equals(entity.Description, commodity.Description, StringComparison.CurrentCultureIgnoreCase) ||
                                  !Guid.Equals(entity.CommodityType.Id, commodity.CommodityType.Id);
                if (hasChanged)
                    changedCommodities.Add(entity);
            }
            return changedCommodities;
        }

        private ValidationResultInfo Validate(Commodity commodity)
        {
            return _commodityRepository.Validate(commodity);
        }

        private Commodity Map(CommodityImport commodityImport, List<string> mappingValidationList)
        {
            var exists = _ctx.tblCommodity.FirstOrDefault(l => l.Code == commodityImport.Code);
            var commodityTypeExists =
                _ctx.tblCommodityType.FirstOrDefault(
                    p => p.Name.ToLower() == commodityImport.CommodityType);
            if (commodityTypeExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid commodity type name {0}", commodityImport.CommodityType));
            }
            else
            {
                Guid id = exists != null ? exists.Id : Guid.NewGuid();
                var commodityType = new CommodityType(commodityTypeExists.Id)
                {
                    Code = commodityTypeExists.Code,
                    Name = commodityTypeExists.Name,
                    Description = commodityTypeExists.Description,
                    Id = commodityTypeExists.Id
                };
                return new Commodity(id)
                {
                    Code = commodityImport.Code,
                    Name = commodityImport.Name,
                    Description = commodityImport.Description,
                    CommodityType =  commodityType
                };
            }
            return null;
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var delete = _ctx.tblCommodity.FirstOrDefault(l => l.Code == deletedCode);

                    if (delete != null)
                    {
                        var actualDelete = _commodityRepository.GetById(delete.Id);
                        if (actualDelete != null)
                        {
                            _commodityRepository.SetAsDeleted(actualDelete);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ImportResponse() { Status = false, Info = "Commodity Delete Error" + ex };
                }

            }
            return new ImportResponse() { Info = "Commodity Deleted Succesfully", Status = true };
        }
    }
}
