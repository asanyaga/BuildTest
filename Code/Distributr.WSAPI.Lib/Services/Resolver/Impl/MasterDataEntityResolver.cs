using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.WSAPI.Lib.Services.Resolver.Impl
{
    /* ----  May2015_Notes -----------
    Investigate if it is being used
 */
    public class MasterDataEntityResolver : IMasterDataEntityResolver
    {
        private IOutletTypeRepository _outletTypeRepository;
        private IOutletRepository _outletRepository;
        private IOutletCategoryRepository _outletCategoryRepository;
        private IVATClassRepository _vatClassRepository;
        private IDiscountGroupRepository _discountGroupRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private IRouteRepository _routeRepository;
        private IContactTypeRepository _contactTypeRepository;
       // private IMaritalStatusRepository _maritalStatusRepository;
        private IAssetCategoryRepository _assetCategoryRepository;
        private IAssetTypeRepository _assetTypeRepository;
        private IAssetStatusRepository _assetStatusRepository;
        private ITargetPeriodRepository _targetPeriodRepository;
        private ICostCentreRepository _costCentreRepository;
        private IUserGroupRepository _userGroupRepository;
        private IUserRepository _userRepository;

        public MasterDataEntityResolver(IOutletTypeRepository outletTypeRepository, IOutletRepository outletRepository, IOutletCategoryRepository outletCategoryRepository, IVATClassRepository vatClassRepository, IDiscountGroupRepository discountGroupRepository, IProductPricingTierRepository productPricingTierRepository, IRouteRepository routeRepository, IContactTypeRepository contactTypeRepository, IAssetCategoryRepository assetCategoryRepository, IAssetTypeRepository assetTypeRepository, IAssetStatusRepository assetStatusRepository, ITargetPeriodRepository targetPeriodRepository, ICostCentreRepository costCentreRepository, IUserGroupRepository userGroupRepository, IUserRepository userRepository)
        {
            _outletTypeRepository = outletTypeRepository;
            _outletRepository = outletRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _vatClassRepository = vatClassRepository;
            _discountGroupRepository = discountGroupRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _routeRepository = routeRepository;
            _contactTypeRepository = contactTypeRepository;
           // _maritalStatusRepository = maritalStatusRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetTypeRepository = assetTypeRepository;
            _assetStatusRepository = assetStatusRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _costCentreRepository = costCentreRepository;
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;
        }

        public MasterEntity Resolver(MasterBaseDTO masterBase, MasterDataDTOSaveCollective collective)
        {
            MasterEntity entity = null;
            switch(collective)
            {
                case MasterDataDTOSaveCollective.Outlet:
                    entity = GetOutletEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.Contact:
                    entity = GetContactEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.Asset:
                    entity = GetAssetEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.DistributrFile:
                    entity = GetDistributrFileEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.OutletVisitDay:
                    entity = GetOutletVisitDayEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.OutletPriority:
                    entity = GetOutletPriorityEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.Target:
                    entity = GetTargetEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.User:
                    entity = GetUserEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.DistributrSalesman:
                    entity = GetDistributorSalesmanEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.PasswordChange:
                    entity = GetChangePasswordEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.AppSettings:
                    entity = GetAppSettingsEntity(masterBase);
                    break;
                case MasterDataDTOSaveCollective.MasterDataAllocation:
                    entity = GetMasterDataAllocationEntity(masterBase);
                    break;
                default:
                    throw new Exception(string.Format("MasterColective FOR {0} NOT HANDLED ON SERVER", collective));
            }
            return entity;
        }

        private MasterEntity GetMasterDataAllocationEntity(MasterBaseDTO masterBase)
        {
            MasterDataAllocationDTO dto = masterBase as MasterDataAllocationDTO;
            MasterDataAllocation entity = entity = new MasterDataAllocation(dto.MasterId);

            if (dto is RouteCentreAllocationDTO)
            {
                entity.EntityAId = ((RouteCentreAllocationDTO) dto).RouteId;
                entity.EntityBId = ((RouteCentreAllocationDTO) dto).CentreId;
                entity.AllocationType = MasterDataAllocationType.RouteCentreAllocation;
            }

            if (dto is RouteCostCentreAllocationDTO)
            {
                entity.EntityAId = ((RouteCostCentreAllocationDTO)dto).RouteId;
                entity.EntityBId = ((RouteCostCentreAllocationDTO)dto).CostCentreId;
                entity.AllocationType = MasterDataAllocationType.RouteCostCentreAllocation;
            }

            if (dto is CommodityProducerCentreAllocationDTO)
            {
                entity.EntityAId = ((CommodityProducerCentreAllocationDTO)dto).CommodityProducerId;
                entity.EntityBId = ((CommodityProducerCentreAllocationDTO)dto).CentreId;
                entity.AllocationType = MasterDataAllocationType.CommodityProducerCentreAllocation;
            }

            if (dto is RouteRegionAllocationDTO)
            {
                entity.EntityAId = ((RouteRegionAllocationDTO)dto).RouteId;
                entity.EntityBId = ((RouteRegionAllocationDTO)dto).RegionId;
                entity.AllocationType = MasterDataAllocationType.RouteRegionAllocation;
            }

            entity._DateCreated = dto.DateCreated;
            entity._DateLastUpdated = dto.DateLastUpdated;
            entity._Status = (EntityStatus)dto.StatusId;

            return entity;
        }

        private MasterEntity GetAppSettingsEntity(MasterBaseDTO masterBase)
        {
            AppSettingsDTO dto = masterBase as AppSettingsDTO;

            AppSettings entity = new AppSettings(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.Key = (SettingsKeys)dto.Key;
            entity.Value = dto.Value;

            return entity;
        }

        private MasterEntity GetChangePasswordEntity(MasterBaseDTO masterBase)
        {
            ChangePasswordDTO dto = masterBase as ChangePasswordDTO;

            User entity = _userRepository.GetById(dto.MasterId);
            if (entity != null)
            {
                if(dto.OldPassword != entity.Password)
                {
                    throw new Exception("Old password does not match with the new password");
                }

                entity.Password = dto.NewPassword;
                return entity;
            }
            return null;
        }

        private MasterEntity GetDistributorSalesmanEntity(MasterBaseDTO masterBase)
        {
            DistributorSalesmanDTO dto = masterBase as DistributorSalesmanDTO;
            DistributorSalesman entity = new DistributorSalesman(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.CostCentreCode = dto.CostCentreCode;
            entity.CostCentreType = CostCentreType.DistributorSalesman;
            entity.Name = dto.Name;
            entity.ParentCostCentre = new CostCentreRef {Id = dto.ParentCostCentreId};
            //entity.Routes 

            return entity;
        }

        private MasterEntity GetUserEntity(MasterBaseDTO masterBase)
        {
            UserDTO dto = masterBase as UserDTO;
            User entity = new User(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.CostCentre = dto.CostCentre;// = _costCentreRepository.GetById(dto.CostCentre);
            entity.Mobile = dto.Mobile;
            entity.Password = dto.Mobile;
            entity.PIN = dto.PIN;
            entity.TillNumber = dto.TillNumber;
            entity.Username = dto.Username;
            entity.UserType = (UserType)dto.UserTypeId;
            var gp = _userGroupRepository.GetById(dto.GroupMasterId);
            if (gp != null)
                entity.Group = gp;

            return entity;
        }

        private MasterEntity GetTargetEntity(MasterBaseDTO masterBase)
        {
            TargetDTO dto = masterBase as TargetDTO;
            Target entity = new Target(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.CostCentre = _outletRepository.GetById(dto.CostCentreId);
            entity.TargetValue = dto.TargetValue;
            entity.TargetPeriod = _targetPeriodRepository.GetById(dto.TargetPeriodMasterId);
            entity.IsQuantityTarget = dto.IsQuantityTarget;

            return entity;
        }

        private MasterEntity GetOutletVisitDayEntity(MasterBaseDTO masterBase)
        {
            OutletVisitDayDTO dto = masterBase as OutletVisitDayDTO;
            OutletVisitDay entity = new OutletVisitDay(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.Day = (DayOfWeek)dto.Day;
            entity.EffectiveDate = dto.EffectiveDate;
            entity.Outlet = new CostCentreRef{Id=dto.OutletMasterId};
            return entity;
        }

        private MasterEntity GetOutletPriorityEntity(MasterBaseDTO masterBase)
        {
            OutletPriorityDTO dto = masterBase as OutletPriorityDTO;
            OutletPriority entity = new OutletPriority(masterBase.MasterId, masterBase.DateCreated, masterBase.DateLastUpdated, (EntityStatus)masterBase.StatusId);
            entity.EffectiveDate = dto.EffectiveDate;
            entity.Outlet = new CostCentreRef {Id = dto.OutletMasterId};
            entity.Priority = dto.Priority;
            entity.Route = _routeRepository.GetById(dto.RouteMasterId);
            return entity;
        }

        private MasterEntity GetDistributrFileEntity(MasterBaseDTO masterBase)
        {
            DistributrFileDTO dto = masterBase as DistributrFileDTO;
            DistributrFile entity = new DistributrFile(masterBase.MasterId);
            entity.Description = dto.Description;
            entity.FileData = dto.FileData;
            entity.FileExtension = dto.FileExtension;
            entity.FileType = (DistributrFileType)dto.FileTypeMasterId;
            return entity;
        }

        private MasterEntity GetAssetEntity(MasterBaseDTO masterBase)
        {
            AssetDTO dto = masterBase as AssetDTO;
            Asset entity = new Asset(masterBase.MasterId);
            entity.AssetCategory = _assetCategoryRepository.GetById(dto.AssetCategoryMasterId);
            entity.AssetNo = dto.AssetNo;
            entity.AssetStatus = _assetStatusRepository.GetById(dto.AssetStatusMasterId);
            entity.AssetType = _assetTypeRepository.GetById(dto.AssetTypeMasterId);
            entity.Capacity = dto.Capacity;
            entity.Code = dto.Code;
            entity.Name = dto.Name;
            entity.SerialNo = dto.SerialNo;
            
            return entity;
        }

        private Contact GetContactEntity(MasterBaseDTO masterBase)
        {
            ContactDTO dto = masterBase as ContactDTO;
            Contact entity = new Contact(masterBase.MasterId);
            entity.BusinessPhone = dto.BusinessPhone;
            entity.ChildrenNames = dto.ChildrenNames;
            entity.City = dto.City;
            entity.Company = dto.Company;
            entity.ContactClassification = (ContactClassification)dto.ContactClassificationId;
            entity.ContactOwnerMasterId = dto.ContactOwnerMasterId;
            entity.ContactOwnerType = (ContactOwnerType)dto.ContactOwnerType;
            if (dto.ContactTypeMasterId != Guid.Empty)
                entity.ContactType = _contactTypeRepository.GetById(dto.ContactTypeMasterId);
            entity.DateOfBirth = dto.DateOfBirth;
            entity.Email = dto.Email;
            entity.Fax = dto.Fax;
            entity.Firstname = dto.Firstname;
            entity.HomePhone = dto.HomePhone;
            entity.HomeTown = dto.HomeTown;
            entity.JobTitle = dto.JobTitle;
            entity.Lastname = dto.Lastname;
            entity.MobilePhone = dto.MobilePhone;
           
            entity.PhysicalAddress = dto.PhysicalAddress;
            entity.PostalAddress = dto.PostalAddress;
            entity.SpouseName = dto.SpouseName;
            entity.WorkExtPhone = dto.WorkExtPhone;
            return entity;

        }

        private Outlet GetOutletEntity(MasterBaseDTO masterBase)
        {
            OutletDTO dto = masterBase as OutletDTO;
            Outlet entity = new Outlet(dto.MasterId);
            entity.CostCentreCode = dto.CostCentreCode;
            entity.CostCentreType = CostCentreType.Outlet;
            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.Name = dto.Name;
            if (dto.OutletCategoryMasterId != Guid.Empty)
                entity.OutletCategory = _outletCategoryRepository.GetById(dto.OutletCategoryMasterId);
            if (dto.OutletTypeMasterId != Guid.Empty)
                entity.OutletType = _outletTypeRepository.GetById(dto.OutletTypeMasterId);
            if (dto.OutletTypeMasterId != Guid.Empty)
            entity.ParentCostCentre = new CostCentreRef {Id = dto.ParentCostCentreId};
            if (dto.VatClassMasterId != Guid.Empty)
                entity.VatClass = _vatClassRepository.GetById(dto.VatClassMasterId);
            if (dto.DiscountGroupMasterId != Guid.Empty)
                entity.DiscountGroup = _discountGroupRepository.GetById(dto.DiscountGroupMasterId);
            if (dto.OutletProductPricingTierMasterId != Guid.Empty)
                entity.OutletProductPricingTier =_productPricingTierRepository.GetById(dto.OutletProductPricingTierMasterId);
            if (dto.RouteMasterId != Guid.Empty)
                entity.Route = _routeRepository.GetById(dto.RouteMasterId);
            return entity;
        }
    }
}
