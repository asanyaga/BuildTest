using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.MasterDataDTO
{
   public interface IMasterDataEnvelopeBuilder
   {
       MasterDataEnvelope BuildOutletCategory(DateTime datesince, MasterDataCollective masterdata, CostCentre cc);

       MasterDataEnvelope BuildOutlet(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildOutletType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildDistributorPendingDispatchWarehouse(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildPricingTier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSaleProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildReturnableProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildConsolidatedProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCountry(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductBrand(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildArea(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildContact(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildDistributor(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildPricing(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProducer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductFlavour(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductPackagingType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRegion(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRoute(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSocioEconomicStatus(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildTerritory(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildUser(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildVatClass(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductPackaging(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildDistributorSalesman(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildChannelPackaging(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildFreeOfChargeDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCompetitor(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCompetitorProduct(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildAsset(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildAssetType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildDistrict(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProvince(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildReorderLevel(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildTargetPeriod(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildTarget(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSaleValueDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildDiscountGroup(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildPromotionDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCvcpDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildProductGroupDiscount(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSalesmanRoute(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildUserGroup(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildUserGroupRole(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildBank(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildBankBranch(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSupplier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildContactType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildAssetCategory(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildAssetStatus(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildOutletPriority(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildOutletVisitDay(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildTargetItem(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSetting(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRetireSetting(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodityType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodity(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodityOwnerType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodityProducer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommoditySupplier(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodityOwner(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCentreType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCentre(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildHub(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildStore(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildFieldClerk(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildContainerType(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildPrinter(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildWeighScale(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildSourcingContainer(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildVehicle(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       bool IsUpdated(MasterDataCollective masterdata, DateTime dateSince);

       //MasterDataEnvelope BuildMasterDataAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRouteCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildCommodityProducerCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRouteCostCentreAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);

       MasterDataEnvelope BuildRouteRegionAllocation(DateTime dateSince, MasterDataCollective masterdata, CostCentre cct);
   }
}
