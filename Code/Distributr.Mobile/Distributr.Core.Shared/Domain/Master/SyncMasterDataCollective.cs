using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.SuppliersEntities;

namespace Distributr.Core.Domain.Master
{
    public static class SyncMasterDataCollective
    {
        public static List<MasterDataCollective> GetMasterDataCollective(VirtualCityApp app)
        {
            var collective = new List<MasterDataCollective>();
            if (app == VirtualCityApp.Ditributr)
                collective = GetDistributrCollective();
            else if (app == VirtualCityApp.Agrimanagr)
                collective = GetAgrimanagrCollective();
            return collective;
        }

        private static List<MasterDataCollective> GetAgrimanagrCollective()
        {
            var collective = new List<MasterDataCollective>();
            
            //collective.Add(MasterDataCollective.MasterDataAllocation);
            collective.Add(MasterDataCollective.Territory);
            collective.Add(MasterDataCollective.Country);
            collective.Add(MasterDataCollective.Region);
            collective.Add(MasterDataCollective.Area);
            collective.Add(MasterDataCollective.Province);
            collective.Add(MasterDataCollective.District);
            
            collective.Add(MasterDataCollective.SocioEconomicStatus);
            collective.Add(MasterDataCollective.ContactType);

            collective.Add(MasterDataCollective.Bank);
            collective.Add(MasterDataCollective.BankBranch);
            
            collective.Add(MasterDataCollective.Setting);
            collective.Add(MasterDataCollective.RetireSetting);

            collective.Add(MasterDataCollective.UserGroup);
            collective.Add(MasterDataCollective.UserGroupRole);
           
            collective.Add(MasterDataCollective.CommodityProducerCentreAllocation);
            collective.Add(MasterDataCollective.RouteCentreAllocation);
            collective.Add(MasterDataCollective.RouteRegionAllocation);
            collective.Add(MasterDataCollective.RouteCostCentreAllocation);

            collective.Add(MasterDataCollective.Producer);
            collective.Add(MasterDataCollective.Hub);
            collective.Add(MasterDataCollective.Route); 

            collective.Add(MasterDataCollective.FieldClerk);
            collective.Add(MasterDataCollective.User);

            collective.Add(MasterDataCollective.Contact);
            //collective.Add(MasterDataCollective.SalesmanRoute);

            collective.Add(MasterDataCollective.CommodityType);
            collective.Add(MasterDataCollective.Commodity);
            collective.Add(MasterDataCollective.CentreType);
            collective.Add(MasterDataCollective.Centre);
            collective.Add(MasterDataCollective.Store);
            collective.Add(MasterDataCollective.CommoditySupplier);
            collective.Add(MasterDataCollective.CommodityProducer);
            collective.Add(MasterDataCollective.CommodityOwnerType);
            collective.Add(MasterDataCollective.CommodityOwner);
            collective.Add(MasterDataCollective.ContainerType);
            collective.Add(MasterDataCollective.Printer);
            collective.Add(MasterDataCollective.WeighScale);
            collective.Add(MasterDataCollective.SourcingContainer);
            collective.Add(MasterDataCollective.Vehicle);

            collective.Add(MasterDataCollective.ServiceProvider);
            collective.Add(MasterDataCollective.Shift);
            collective.Add(MasterDataCollective.Season);
            collective.Add(MasterDataCollective.Service);
            collective.Add(MasterDataCollective.Infection);
            collective.Add(MasterDataCollective.ActivityType);

            collective.Add(MasterDataCollective.VatClass);
            collective.Add(MasterDataCollective.Supplier);
            collective.Add(MasterDataCollective.ProductType);
            collective.Add(MasterDataCollective.ProductBrand);
            collective.Add(MasterDataCollective.ProductFlavour);
            collective.Add(MasterDataCollective.ProductPackagingType);

            collective.Add(MasterDataCollective.ProductPackaging);
            collective.Add(MasterDataCollective.ReturnableProduct);
            collective.Add(MasterDataCollective.SaleProduct);
            return collective;
        }

        private static List<MasterDataCollective> GetDistributrCollective()
        {
            var collective = new List<MasterDataCollective>();
            
            collective.Add(MasterDataCollective.Territory);
            collective.Add(MasterDataCollective.Country);
            collective.Add(MasterDataCollective.Region);
            collective.Add(MasterDataCollective.Area);
            collective.Add(MasterDataCollective.Province);
            collective.Add(MasterDataCollective.District);

            collective.Add(MasterDataCollective.SocioEconomicStatus);
            //collective.Add(MasterDataCollective.MaritalStatus);
            collective.Add(MasterDataCollective.ContactType);
            collective.Add(MasterDataCollective.OutletCategory);
            collective.Add(MasterDataCollective.OutletType);

            collective.Add(MasterDataCollective.Bank);
            collective.Add(MasterDataCollective.BankBranch);
            collective.Add(MasterDataCollective.AssetStatus);
            collective.Add(MasterDataCollective.AssetType);
            collective.Add(MasterDataCollective.AssetCategory);
            collective.Add(MasterDataCollective.Asset);

            collective.Add(MasterDataCollective.Setting);
            collective.Add(MasterDataCollective.RetireSetting);

            collective.Add(MasterDataCollective.UserGroup);
            collective.Add(MasterDataCollective.UserGroupRole);
            collective.Add(MasterDataCollective.Supplier);

            collective.Add(MasterDataCollective.PricingTier);
            collective.Add(MasterDataCollective.VatClass);
            collective.Add(MasterDataCollective.ProductType);
            collective.Add(MasterDataCollective.ProductBrand);
            collective.Add(MasterDataCollective.ProductFlavour);
            collective.Add(MasterDataCollective.ProductPackagingType);

            collective.Add(MasterDataCollective.ProductPackaging);
            collective.Add(MasterDataCollective.ReturnableProduct);
            collective.Add(MasterDataCollective.SaleProduct);
            collective.Add(MasterDataCollective.ConsolidatedProduct);
            //collective.Add(MasterDataCollective.ChannelPackaging);
            collective.Add(MasterDataCollective.Pricing);

            collective.Add(MasterDataCollective.DiscountGroup);
            collective.Add(MasterDataCollective.ProductGroupDiscount);
            collective.Add(MasterDataCollective.ProductDiscount);
            collective.Add(MasterDataCollective.SaleValueDiscount);
            collective.Add(MasterDataCollective.PromotionDiscount);
            collective.Add(MasterDataCollective.CertainValueCertainProductDiscount);
            collective.Add(MasterDataCollective.FreeOfChargeDiscount);

            //MasterDataCollective.Transporter = 42,
            collective.Add(MasterDataCollective.Producer);
            collective.Add(MasterDataCollective.Distributor);
            collective.Add(MasterDataCollective.DistributorPendingDispatchWarehouse);
            collective.Add(MasterDataCollective.DistributorSalesman);
            collective.Add(MasterDataCollective.Route); //check
            collective.Add(MasterDataCollective.Outlet);
            collective.Add(MasterDataCollective.OutletVisitDay);
            collective.Add(MasterDataCollective.OutletPriority);

            collective.Add(MasterDataCollective.Competitor);
            collective.Add(MasterDataCollective.User);
            collective.Add(MasterDataCollective.SalesmanRoute);
            collective.Add(MasterDataCollective.SalesmanSupplier);
            collective.Add(MasterDataCollective.Contact);
            
            collective.Add(MasterDataCollective.TargetPeriod);
            collective.Add(MasterDataCollective.Target);
            collective.Add(MasterDataCollective.TargetItem);
            collective.Add(MasterDataCollective.ReorderLevel);
            collective.Add(MasterDataCollective.CompetitorProduct);

            //collective.Add(MasterDataCollective.MasterDataAllocation);
            collective.Add(MasterDataCollective.RouteRegionAllocation);
            collective.Add(MasterDataCollective.RouteCostCentreAllocation);
            collective.Add(MasterDataCollective.OutletVisitReasonsType);

            return collective;
        }
    }
}
