using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities.Impl
{
    internal class CommodityOwnerImportService : ICommodityOwnerImportService
    {
        private readonly ICommodityOwnerRepository _commodityOwnerRepository;
        private readonly ICommoditySupplierRepository _commoditySupplierRepository;
        private readonly ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;

        public CommodityOwnerImportService(ICommodityOwnerRepository commodityOwnerRepository, ICommoditySupplierRepository commoditySupplierRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository)
        {
            _commodityOwnerRepository = commodityOwnerRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
        }

        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<CommodityOwnerImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var commodityOwners = await ConstructEntities(entities);
                int count = 0;
                foreach (var owner in commodityOwners)
                {
                    var res = await ValidateEntityeAsync(owner);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description = "Row-" + count,
                        Entity = owner
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
        }

        public async Task<bool> SaveAsync(IEnumerable<CommodityOwner> entities)
        {
            return await Task.Run(() =>
            {
                entities.ToList().ForEach(n => _commodityOwnerRepository.Save(n));
                return true;
            });
        }

        private async Task<IEnumerable<CommodityOwner>> ConstructEntities(IEnumerable<CommodityOwnerImport> entities)
        {
            
            return await Task.Run(() =>
            {
                var temp = new List<CommodityOwner>();
                var existing = _commodityOwnerRepository.GetAll(true).ToList();
                foreach (var entity in entities)
                {
                    if(string.IsNullOrEmpty(entity.Code))
                    {
                        MessageBox.Show(string.Format("Supply valid code for farmer {0}", entity.FirstName));
                        break;
                    }
                    var commodityOwner =
                        existing.FirstOrDefault(p => p.Code != null && p.Code.Trim().ToLower() == entity.Code.Trim().ToLower()) ?? new CommodityOwner(Guid.NewGuid());

                    commodityOwner.Code = entity.Code;
                    commodityOwner.Email = entity.Email;
                    commodityOwner.PinNo = entity.PinNo;
                    commodityOwner.Surname = entity.Surname;
                    commodityOwner.CommoditySupplier = _commoditySupplierRepository.GetAll(true).OfType<CommoditySupplier>().FirstOrDefault(p=>p.Name.ToLower()==entity.CommoditySupplierName.ToLower());
                    commodityOwner.CommodityOwnerType =
                        _commodityOwnerTypeRepository.GetAll(true).FirstOrDefault(
                            p => p.Name.ToLower() == entity.CommodityOwnerTypeName.ToLower()|| p.Code.ToLower() == entity.CommodityOwnerTypeName.ToLower());
                    commodityOwner.OfficeNumber = string.IsNullOrEmpty(entity.OfficeNumber)
                                                      ? entity.PhoneNumber
                                                      : entity.OfficeNumber;
                    commodityOwner.PhoneNumber = entity.PhoneNumber;
                    commodityOwner.PostalAddress = entity.PostalAddress;
                    commodityOwner.LastName = entity.LastName;
                    commodityOwner.FirstName = entity.FirstName;
                    commodityOwner.FaxNumber = string.IsNullOrEmpty(entity.FaxNumber)?entity.PhoneNumber:entity.FaxNumber;
                    commodityOwner.DateOfBirth = entity.DateOfBirth;
                    commodityOwner.Gender = (Gender)entity.GenderEnum;
                    commodityOwner.IdNo = entity.IdNo;
                    commodityOwner._Status=EntityStatus.Active;
                    
                    temp.Add(commodityOwner);
                }

                return temp;
            });

        }
        private async Task<ImportValidationResultInfo> ValidateEntityeAsync(CommodityOwner entity)
        {
            return await Task.Run(() =>
            {
                var res = _commodityOwnerRepository.Validate(entity);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    // Description = "Row-" + count,
                    Entity = entity
                };
            });

        }
    }
}
