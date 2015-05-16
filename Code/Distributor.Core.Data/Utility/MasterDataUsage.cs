using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Utility
{
   
    public class MasterDataUsage : IMasterDataUsage
    {
        protected CokeDataContext _ctx;

        public MasterDataUsage(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public bool CommodityProducerHasPurchases(CommodityProducer commodityProducer)
        {
            return _ctx.tblSourcingDocument.Any(n => n.CommodityProducerId == commodityProducer.Id);
        }

        public bool CommoditySupplierHasOwnersOrProducers(CommoditySupplier commoditySupplier)
        {
            return _ctx.tblSourcingDocument.Any(n => n.DocumentOnBehalfOfCostCentreId == commoditySupplier.Id);
        }

        public bool CommodityOwnerHasProducers(CommodityOwner commodityOwner)
        {
            return _ctx.tblSourcingDocument.Any(n => n.CommodityOwnerId == commodityOwner.Id);
        }

        public bool CommodityOwnerHasPurchases(CommoditySupplier commodityOwner)
        {
            return _ctx.tblSourcingDocument.Any(n => n.DocumentIssuerCostCentreId == commodityOwner.Id || n.DocumentRecipientCostCentreId==commodityOwner.Id);
        }

        public bool CheckCommodityOwnerTypeIsUsed(CommodityOwnerType commodityOwnerType, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblCommodityOwner.Any(n => n.CommodityOwnerTypeId == commodityOwnerType.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblCommodityOwner.Any(n => n.CommodityOwnerTypeId == commodityOwnerType.Id &&
                    (n.IM_Status == (int)EntityStatus.Inactive || n.IM_Status == (int)EntityStatus.Active)))
                    return true;
            }
            return false;
        }

        public bool CheckCommoditySupplierIsUsed(CommoditySupplier commoditySupplier, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if ( _ctx.tblCommodityOwner.Any(n => n.CostCentreId == commoditySupplier.Id && n.IM_Status == (int) EntityStatus.Active))
                    return true;
                if (_ctx.tblCommodityProducer.Any(n => n.CostCentreId == commoditySupplier.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblCommodityOwner.Any(n => n.CostCentreId == commoditySupplier.Id &&
                                                    (n.IM_Status == (int) EntityStatus.Active ||
                                                     n.IM_Status == (int) EntityStatus.Inactive)
                    ))
                    return true;
                if (_ctx.tblCommodityProducer.Any(n => n.CostCentreId == commoditySupplier.Id &&
                                                       (n.IM_Status == (int) EntityStatus.Active ||
                                                        n.IM_Status == (int) EntityStatus.Inactive)
                    )) return true;
            }
            if (_ctx.tblSourcingDocument.Any(n => n.DocumentOnBehalfOfCostCentreId == commoditySupplier.Id))
                return true;

            return false;
        }

        public bool CheckAgriUserIsUsed(User user)
        {
            if (_ctx.tblSourcingDocument.Any(n => n.DocumentIssuerUserId == user.Id))
                return true;

            return false;
        }

        public bool CheckAgriRouteIsUsed(Route route, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblCentre.Any(n => n.RouteId == route.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
                if (_ctx.tblOutletPriority.Any(n => n.RouteId == route.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
                if (_ctx.tblCostCentre.Any(n => n.RouteId == route.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            if (_ctx.tblCentre.Any(n => n.RouteId == route.Id && n.IM_Status != (int)EntityStatus.Deleted))
                return true;
            if (_ctx.tblOutletPriority.Any(n => n.RouteId == route.Id && n.IM_Status != (int)EntityStatus.Deleted))
                return true;
            if (_ctx.tblCostCentre.Any(n => n.RouteId == route.Id && n.IM_Status != (int)EntityStatus.Deleted))
                return true;
            return false;
        }

        public bool CheckAgriCentreIsUsed(Centre centre, EntityStatus intendedStatus)
        {
            var allocatedToCommodityProducers = _ctx.tblMasterDataAllocation
                .Where(n => n.AllocationType == (int) MasterDataAllocationType.CommodityProducerCentreAllocation &&
                            n.EntityBId == centre.Id)
                .Select(n => _ctx.tblCommodityProducer.FirstOrDefault(p => p.Id == n.EntityAId));
            if(intendedStatus == EntityStatus.Inactive)
            {
                if (allocatedToCommodityProducers.Any(n => n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            if (intendedStatus == EntityStatus.Deleted)
            {
                if (allocatedToCommodityProducers.Any(n => n.IM_Status == (int)EntityStatus.Inactive))
                    return true;
            }
            return false;
        }

        public bool CheckAgriCentreTypeIsUsed(CentreType centreType, EntityStatus intendedStatus)
        {
            if(intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblCentre.Any(n => n.CentreTypeId == centreType.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblCentre.Any(n => n.CentreTypeId == centreType.Id &&
                    (n.IM_Status == (int)EntityStatus.Inactive || n.IM_Status == (int)EntityStatus.Active)))
                    return true;
            }
            return false;
        }

        public bool CheckSourcingContainerIsUsed(SourcingContainer container)
        {
            return false;
        }

        public bool CheckContainerTypeIsUsed(ContainerType containerType, EntityStatus intendedStatus)
        {
            if(intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblEquipment.Any(n => n.ContainerTypeId == containerType.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if(intendedStatus == EntityStatus.Deleted){
                if (_ctx.tblEquipment.Any(n => n.ContainerTypeId == containerType.Id &&
                    (n.IM_Status == (int)EntityStatus.Inactive || n.IM_Status == (int)EntityStatus.Active)))
                    return true;
            }
            if (_ctx.tblSourcingLineItem.Any(n => n.ContainerId == containerType.Id))
                return true;

            return false;
        }

        public bool CheckCommodityGradeIsUsed(Guid commodityGradeId, EntityStatus intendedStatus)
        {
            if(_ctx.tblSourcingLineItem.Any(n => n.GradeId == commodityGradeId))
                return true;
            return false;
        }

        public bool CheckCommodityIsUsed(Commodity commodity, EntityStatus intendedStatus)
        {
            if(_ctx.tblSourcingLineItem.Any(n => n.CommodityId == commodity.Id))
                return true;
            if(intendedStatus == EntityStatus.Inactive)
            {
                if(commodity.CommodityGrades.Any(n => n._Status == EntityStatus.Active))
                    return true;
            }
            else if(intendedStatus == EntityStatus.Deleted)
            {
                if (commodity.CommodityGrades.Any(n => n._Status != EntityStatus.Deleted))
                    return true;
            }
            return false;
        }

        public bool CheckCommodtiyTypeIsUsed(CommodityType commodityType, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblCommodity.Any(n => n.CommodityTypeId == commodityType.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblCommodity.Any(n => n.CommodityTypeId == commodityType.Id &&
                    (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)))
                    return true;
            }
            return false;
        }

        public bool CheckStoreIsUsed(Store store, EntityStatus intendedStatus)
        {
            if (_ctx.tblSourcingDocument.Any(n => n.DocumentRecipientCostCentreId == store.Id))
                return true;
            return false;
        }

        public bool CheckVehicleIsUsed(Vehicle vehicle, EntityStatus intendedStatus)
        {
            return false;
        }

        public bool CheckBankIsUsed(Bank bank, EntityStatus intendedStatus)
        {
            if(intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblBankBranch.Any(n => n.BankId == bank.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if(intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblBankBranch.Any(n => n.BankId == bank.Id &&
                    (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)))
                    return true;
            }
            return false;
        }

        public bool CheckAgriRegionIsUsed(Region region, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblRoutes.Any(n => n.RegionId == region.Id && n.IM_Status == (int) EntityStatus.Active))
                    return true;
                if (
                    _ctx.tblCostCentre.Any(
                        n => n.Distributor_RegionId == region.Id && n.IM_Status == (int) EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblRoutes.Any(n => n.RegionId == region.Id &&
                                            (n.IM_Status == (int) EntityStatus.Active ||
                                             n.IM_Status == (int) EntityStatus.Inactive)))
                    return true;
                if (_ctx.tblCostCentre.Any(n => n.Distributor_RegionId == region.Id &&
                                                (n.IM_Status == (int) EntityStatus.Active ||
                                                 n.IM_Status == (int) EntityStatus.Inactive)))
                    return true;
            }
            return false;
        }

        public bool CheckHubIsUsed(Hub hub, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblCostCentre.Any(n => n.ParentCostCentreId == hub.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblCostCentre.Any(n => n.ParentCostCentreId == hub.Id &&
                    (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)))
                    return true;
            }
            return false;
        }

        public bool CheckCountryIsUsed(Country country, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
            {
                if (_ctx.tblRegion.Any(n => n.Country == country.Id && n.IM_Status == (int)EntityStatus.Active))
                    return true;
            }
            else if (intendedStatus == EntityStatus.Deleted)
            {
                if (_ctx.tblRegion.Any(n => n.Country == country.Id &&
                    (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)))
                    return true;
            }
            return false;
        }

        public bool CheckOutletIsUsed(OutletVisitReasonsType outletVisitReasonsType, EntityStatus intendedStatus)
        {
            if (intendedStatus == EntityStatus.Inactive)
                if (_ctx.tblOutletVisitReasonType.FirstOrDefault(n => n.id == outletVisitReasonsType.Id && n.IM_Status == (int)EntityStatus.Active) != null)
                    return false;
            return true; 
        }


        public bool CanEditHubOrDistributrRegion(CostCentre hubOrDistributr)
        {
            if(_ctx.tblCostCentre.FirstOrDefault(n => n.Id == hubOrDistributr.Id).tblRegion.tblRoutes.Any(n => n.IM_Status != (int)EntityStatus.Deleted))
            {
                return false;
            }
            return true;
        }
    }
}
