using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Utility.MasterData
{
    public interface IMasterDataUsage
    {
        bool CommodityProducerHasPurchases(CommodityProducer commodityProducer);
        bool CommodityOwnerHasPurchases(CommoditySupplier commodityOwner);
        bool CheckCommodityOwnerTypeIsUsed(CommodityOwnerType commodityOwnerType, EntityStatus intendedStatus);
        bool CheckCommoditySupplierIsUsed(CommoditySupplier commoditySupplier, EntityStatus intendedStatus);
        bool CheckAgriUserIsUsed(User user);
        bool CheckAgriRouteIsUsed(Route route, EntityStatus intendedStatus);
        bool CheckAgriCentreIsUsed(Centre centre, EntityStatus intendedStatus);
        bool CheckAgriCentreTypeIsUsed(CentreType centreType, EntityStatus intendedStatus);
        bool CheckSourcingContainerIsUsed(SourcingContainer container);
        bool CheckContainerTypeIsUsed(ContainerType containerType, EntityStatus intendedStatus);
        bool CheckCommodityGradeIsUsed(Guid commodityGradeId, EntityStatus intendedStatus);
        bool CheckCommodityIsUsed(Commodity commodity, EntityStatus intendedStatus);
        bool CheckCommodtiyTypeIsUsed(CommodityType commodityType, EntityStatus intendedStatus);
        bool CheckStoreIsUsed(Store store, EntityStatus intendedStatus);
        bool CheckVehicleIsUsed(Vehicle vehicle, EntityStatus intendedStatus);
        bool CheckBankIsUsed(Bank bank, EntityStatus intendedStatus);
        bool CheckAgriRegionIsUsed(Region region, EntityStatus intendedStatus);
        bool CheckHubIsUsed(Hub hub, EntityStatus intendedStatus);
        bool CheckCountryIsUsed(Country country, EntityStatus intendedStatus);
        bool CheckOutletIsUsed(OutletVisitReasonsType outletVisitReasonsType, EntityStatus intendedStatus);
        bool CanEditHubOrDistributrRegion(CostCentre hubOrDistributr);
        bool CommoditySupplierHasOwnersOrProducers(CommoditySupplier commoditySupplier);
        bool CommodityOwnerHasProducers(CommodityOwner commodityOwner);
    }
}
