using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using Distributr.Integrations.Imports.Impl;

namespace Distributr.Integrations.AgrimanagrImports.Impl
{
    public class CommoditySupplierImporterService : BaseImporterService, ICommoditySupplierImporterService
    {
        private readonly ICommoditySupplierRepository _commoditySupplierRepository;
        private CokeDataContext _ctx;

        public CommoditySupplierImporterService(ICommoditySupplierRepository commoditySupplierRepository, CokeDataContext ctx)
        {
            _commoditySupplierRepository = commoditySupplierRepository;
            _ctx = ctx;
        }

        public ImportResponse Save(List<CommoditySupplierImport> imports)
        {
            var mappingValidationList = new List<string>();
            List<CommoditySupplier> commoditySuppliers = imports.Select(k => Map(k, mappingValidationList)).ToList();

            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };
            }

            List<ValidationResultInfo> validationResult = commoditySuppliers.Select(Validate).ToList();

            if (validationResult.Any(j => !j.IsValid))
            {
                var invalidEntities = validationResult.Where(k => !k.IsValid).ToList();

                return new ImportResponse() { Status = false, Info = "Commodity supplier error" + ValidationResultsInfo(invalidEntities) };
            }

            List<CostCentre> changedCommoditySuppliers = HasChanged(commoditySuppliers);

            foreach (var changedCommoditySupplier in changedCommoditySuppliers)
            {
                _commoditySupplierRepository.Save(changedCommoditySupplier);
            }

            return new ImportResponse() { Status = true, Info = changedCommoditySuppliers.Count + " Commodity suppliers Successfully Imported" };
        }

        private List<CostCentre> HasChanged(List<CommoditySupplier> commoditySuppliers)
        {
            var changedCommoditySupplier = new List<CostCentre>();
            foreach (var commoditySupplier in commoditySuppliers)
            {
                var entity = _commoditySupplierRepository.GetById(commoditySupplier.Id);
                if (entity == null)
                {
                    changedCommoditySupplier.Add(commoditySupplier);
                    continue;
                }

                bool hasChanged = !String.Equals(entity.Name, commoditySupplier.Name, StringComparison.CurrentCultureIgnoreCase);
                if (hasChanged)
                    changedCommoditySupplier.Add(entity);
            }
            return changedCommoditySupplier;
        }

        private ValidationResultInfo Validate(CommoditySupplier commoditySupplier)
        {
            return _commoditySupplierRepository.Validate(commoditySupplier);
        }

        private CommoditySupplier Map(CommoditySupplierImport commoditySupplierImport, List<string> mappingValidationList)
        {
            var exists = _ctx.tblCostCentre.FirstOrDefault(l => l.Cost_Centre_Code == commoditySupplierImport.Code);
            var bankExists =
                _ctx.tblBank.FirstOrDefault(l => l.Name.ToLower().Equals(commoditySupplierImport.BankName.ToLower()));
            var bankBranchExists =
                _ctx.tblBankBranch.FirstOrDefault(
                    l => l.Name.ToLower().Equals(commoditySupplierImport.BankBranchName.ToLower()));
            var costCentreExists =
                _ctx.tblCostCentre.FirstOrDefault(
                    l =>
                        l.Name.ToLower().Contains(commoditySupplierImport.Hub.ToLower()) ||
                        l.Cost_Centre_Code.ToLower().Equals(commoditySupplierImport.Hub.ToLower()));
            if (bankBranchExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid bank branch name {0}", commoditySupplierImport.BankBranchName));
            }
            else if (bankExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid bank name {0}", commoditySupplierImport.BankName));
            }
            else if (costCentreExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid hub name/code {0}", commoditySupplierImport.Hub));
            }
            else
            {
                Guid id = exists != null ? exists.Id : Guid.NewGuid();

                return new CommoditySupplier(id)
                {
                    Name = commoditySupplierImport.Name,
                    AccountName = commoditySupplierImport.AccountName,
                    CostCentreCode = commoditySupplierImport.Code,
                    CommoditySupplierType = MapSupplierType(commoditySupplierImport.SupplierType),
                    JoinDate = commoditySupplierImport.JoinDate,
                    AccountNo = commoditySupplierImport.AccountNumber,
                    PinNo = commoditySupplierImport.PinNumber,
                    BankId = bankExists.Id,
                    BankBranchId = bankBranchExists.Id,
                    ParentCostCentre = new CostCentreRef { Id = costCentreExists.Id },
                    CostCentreType = CostCentreType.CommoditySupplier,
                    _Status = EntityStatus.Active
                };
            }
            return null;
        }

        private CommoditySupplierType MapSupplierType(string supplierType)
        {
            CommoditySupplierType commoditySupplierType = 0;
            if (supplierType == "Individual")
                return CommoditySupplierType.Individual;
            if (supplierType == "Cooperative")
            {
                return CommoditySupplierType.Cooperative;
            }
            if (supplierType == "Default")
            {
                return CommoditySupplierType.Default;
            }
            return commoditySupplierType;
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var delete = _ctx.tblCostCentre.FirstOrDefault(l => l.Cost_Centre_Code == deletedCode);

                    if (delete != null)
                    {
                        var actualDelete = _commoditySupplierRepository.GetById(delete.Id);
                        if (actualDelete != null)
                        {
                            _commoditySupplierRepository.SetAsDeleted(actualDelete);
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
