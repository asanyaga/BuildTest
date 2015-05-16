using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public enum MasterDataCollective : int
    {
        ContainerType = 1,
        WeighScale = 2,

        Region = 3,
        Area = 4,
        Province = 5,
        District = 6,

        SocioEconomicStatus = 7,

        ContactType = 9,
        OutletCategory = 10,
        OutletType = 11,

        Bank = 12,
        BankBranch = 13,
        AssetStatus = 14,
        AssetType = 15,
        AssetCategory = 16,
        Asset = 17,

        Setting = 18,
        RetireSetting = 19,

        UserGroup = 20,
        UserGroupRole = 21,
        Supplier = 22,

        PricingTier = 23,
        VatClass = 24,
        ProductType = 25,
        ProductBrand = 26,
        ProductFlavour = 27,
        ProductPackagingType = 28,

        ReturnableProduct = 29,
        SaleProduct = 30,
        ConsolidatedProduct = 31,
        ProductPackaging = 32,
        ChannelPackaging = 33,
        Pricing = 34,
        

        DiscountGroup = 35,
        ProductGroupDiscount = 36,
        ProductDiscount = 37,
        SaleValueDiscount = 38,
        PromotionDiscount = 39,
        CertainValueCertainProductDiscount = 40,
        FreeOfChargeDiscount = 41,

        //Transporter = 42,
        Producer = 43,
        Distributor = 44,
        DistributorPendingDispatchWarehouse = 45,
        DistributorSalesman = 46,
        Route = 47, //check
        Outlet = 48,
        OutletVisitDay = 49,
        OutletPriority = 50,

        Competitor = 51,
        User = 52,
        Contact = 53,
        SalesmanRoute = 54,
        TargetPeriod = 55,
        Target = 56,
        TargetItem = 57,
        ReorderLevel = 58,
        CompetitorProduct = 59,

        CommodityType = 60,
        Commodity = 61,
        CentreType = 62,
        Centre = 63,
        Hub = 64,
        Store = 65,
        CommoditySupplier = 66,
        CommodityProducer = 67,
        CommodityOwnerType = 68,
        CommodityOwner = 69,
        FieldClerk = 70,
        Territory = 71,

        Printer = 72,

        Country = 73,
        SourcingContainer = 74,

        //MasterDataAllocation,
        RouteCentreAllocation = 75,
        CommodityProducerCentreAllocation = 76,
        RouteCostCentreAllocation = 77,
        RouteRegionAllocation = 78,

        Vehicle = 79,
        ServiceProvider = 80,
        Service = 81,
        Shift = 82,
        Infection = 83,
        Season = 84,
        ActivityType=85,
        ShipTo = 86,

        OutletVisitReasonsType=87,

        SalesmanSupplier = 88
    }
}




