using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using Distributr.WSAPI.Lib.Services.Imports.Impl;

namespace Distributr.WSAPI.Lib.Services.AgrimanagrImports.Impl
{
    public class CommodityOwnerImporterService : BaseImporterService, ICommodityOwnerImporterService
    {
        private CokeDataContext _ctx;
        private readonly ICommodityOwnerRepository _commodityOwnerRepository;
        private readonly ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private readonly ICommoditySupplierRepository _commoditySupplierRepository;
        private int count;

        public CommodityOwnerImporterService(CokeDataContext ctx, ICommodityOwnerRepository commodityOwnerRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommoditySupplierRepository commoditySupplierRepository)
        {
            _ctx = ctx;
            _commodityOwnerRepository = commodityOwnerRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            count = 300;
        }

        public ImportResponse Save(List<CommodityOwnerImport> imports)
        {
            var mappingValidationList = new List<string>();
            List<CommodityOwner> commodityOwners = imports.Select(k => Map(k, mappingValidationList)).ToList();

            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };
            }

            List<ValidationResultInfo> validationResult = commodityOwners.Select(Validate).ToList();

            if (validationResult.Any(j => !j.IsValid))
            {
                var invalidEntities = validationResult.Where(k => !k.IsValid).ToList();

                return new ImportResponse() { Status = false, Info = "Commodity owner error" + ValidationResultsInfo(invalidEntities) };
            }

            List<CommodityOwner> changedCommodityOwners = HasChanged(commodityOwners);

            foreach (var changedCommodityOwner in changedCommodityOwners)
            {
                _commodityOwnerRepository.Save(changedCommodityOwner);
            }

            return new ImportResponse() { Status = true, Info = changedCommodityOwners.Count + " Commodity suppliers Successfully Imported" };
        }

        private List<CommodityOwner> HasChanged(List<CommodityOwner> commodityOwners)
        {
            var changedCommodityOwner = new List<CommodityOwner>();
            foreach (var commodityOwner in commodityOwners)
            {
                var entity = _commodityOwnerRepository.GetById(commodityOwner.Id);
                if (entity == null)
                {
                    changedCommodityOwner.Add(commodityOwner);
                    continue;
                }

                bool hasChanged = !String.Equals(entity.Surname, commodityOwner.Surname, StringComparison.CurrentCultureIgnoreCase) || 
                    !String.Equals(entity.Description, commodityOwner.Description, StringComparison.CurrentCultureIgnoreCase) ||
                    !String.Equals(entity.FirstName, commodityOwner.FirstName, StringComparison.CurrentCultureIgnoreCase) ||
                    !String.Equals(entity.PhysicalAddress, commodityOwner.PhysicalAddress, StringComparison.CurrentCultureIgnoreCase) ||
                    !String.Equals(entity.PostalAddress, commodityOwner.PostalAddress, StringComparison.CurrentCultureIgnoreCase) ||
                    !String.Equals(entity.Email, commodityOwner.Email, StringComparison.CurrentCultureIgnoreCase) ||
                    !String.Equals(entity.BusinessNumber, commodityOwner.BusinessNumber, StringComparison.CurrentCultureIgnoreCase) ||
                    !string.Equals(entity.FaxNumber, commodityOwner.FaxNumber) ||
                    !String.Equals(entity.OfficeNumber, commodityOwner.OfficeNumber, StringComparison.CurrentCultureIgnoreCase);
                if (hasChanged)
                    changedCommodityOwner.Add(entity);
            }
            return changedCommodityOwner;
        }

        private ValidationResultInfo Validate(CommodityOwner commodityOwner)
        {
            return _commodityOwnerRepository.Validate(commodityOwner);
        }

        private CommodityOwner Map(CommodityOwnerImport commodityOwnerImport, List<string> mappingValidationList)
        {
            var exists = _ctx.tblCommodityOwner.FirstOrDefault(l => l.Code == commodityOwnerImport.Code);
            var commoditySupplierExists =
                _ctx.tblCostCentre.FirstOrDefault(l => l.Name.ToLower().Equals(commodityOwnerImport.CommoditySupplier) || l.Cost_Centre_Code.ToLower().Equals(commodityOwnerImport.CommoditySupplier));
            var commodityTypeExists =
                _ctx.tblCommodityOwnerType.FirstOrDefault(
                    k => k.Name.ToLower().Equals(commodityOwnerImport.CommodityOwnerType) || k.Code.ToLower().Equals(commodityOwnerImport.CommodityOwnerType));
            if (commoditySupplierExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid commodity supplier Code {0}",
                    commodityOwnerImport.CommoditySupplier));
            }
            else if (commodityTypeExists == null)
            {
                mappingValidationList.Add(string.Format("Invalid commodity owner type Code {0}",
                    commodityOwnerImport.CommodityOwnerType));
            }
            else
            {
                Guid id = exists != null ? exists.Id : Guid.NewGuid();
                count++;

                return new CommodityOwner(id)
                {
                    Code = commodityOwnerImport.Code,
                    Description = commodityOwnerImport.Description,
                    Surname = commodityOwnerImport.Surname,
                    FirstName = commodityOwnerImport.Firstname,
                    IdNo = commodityOwnerImport.IdNo,
                    PinNo = commodityOwnerImport.Pin,
                    DateOfBirth = commodityOwnerImport.DOB,
                    MaritalStatus = MaritalStatas.Unknown,
                    Gender = Gender.Unknown,
                    PhysicalAddress = commodityOwnerImport.PhysicalAddress,
                    PostalAddress = commodityOwnerImport.PostalAddress,
                    Email = commodityOwnerImport.Email,
                    PhoneNumber = commodityOwnerImport.Mobile,
                    BusinessNumber = commodityOwnerImport.BusinessNumber,
                    FaxNumber = commodityOwnerImport.FaxNumber,
                    OfficeNumber = commodityOwnerImport.OfficeNumber,
                    CommodityOwnerType = _commodityOwnerTypeRepository.GetById(commodityTypeExists.Id),
                    _Status = EntityStatus.Active,
                    CommoditySupplier = _commoditySupplierRepository.GetById(commoditySupplierExists.Id) as CommoditySupplier
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
                    var delete = _ctx.tblCommodityOwner.FirstOrDefault(l => l.Code == deletedCode);

                    if (delete != null)
                    {
                        var actualDelete = _commodityOwnerRepository.GetById(delete.Id);
                        if (actualDelete != null)
                        {
                            _commodityOwnerRepository.SetAsDeleted(actualDelete);
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
