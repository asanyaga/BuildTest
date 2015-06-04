using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Mobile.Core.Data.Sequences;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Distributr.Mobile.Login.Settings;

namespace Distributr.Mobile.Data
{
    public static class DatabaseConfig
    {
        // Stuff that gets synced through the masterdata/sync endpoint. We also create a 
        // table for each of these entities.
        private static readonly List<Type> MasterDataTypes = new List<Type>
        {
            // User
            typeof (User),

            // Asset
            typeof (AssetCategory),
            typeof (Asset),
            typeof (AssetStatus),
            typeof (AssetType),

//            // Bank
            typeof (BankBranch),
            typeof (Bank),

//            // Cost Centre
            typeof (Area),
            typeof (Contact),
            typeof (ContactType),
            typeof (Region),
            typeof (Country),
            typeof (District),
            typeof (Territory),
            typeof (Distributor),
            typeof (DistributorPendingDispatchWarehouse),
            typeof (DistributorSalesman),
            typeof (SalesmanSupplier),
            typeof (SalesmanRoute),

            typeof (OutletCategory),
            typeof (Route),
            typeof (Outlet),
            typeof (ShipToAddress),
            typeof (OutletPriority),
            typeof (OutletType),
            typeof (OutletVisitDay),
            typeof (OutletVisitReasonsType),

            //Competitor
            typeof (Competitor),
            typeof (CompetitorProducts),

            typeof (Producer),
            typeof (SocioEconomicStatus),
              
            // Targets
            typeof (Target),
            typeof (TargetPeriod),

            // Product
            typeof (CertainValueCertainProductDiscount),
            typeof (CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem),
            typeof (ChannelPackaging),
            typeof (DiscountGroup),

            typeof (FreeOfChargeDiscount),
            typeof (ProductBrand),
            typeof (ProductDiscount),
            typeof (ProductDiscount.ProductDiscountItem),
            typeof (PromotionDiscount),
            typeof (PromotionDiscount.PromotionDiscountItem),
            typeof (ProductFlavour),
            typeof (ProductGroupDiscount),
            
            //Reorder
            typeof (ReOrderLevel),

            typeof (ProductPackaging),
            typeof (ProductPackagingType),
            typeof (ProductPricing),
            typeof (ProductPricing.ProductPricingItem),
            typeof (ProductPricingTier),
            typeof (ProductType),
            typeof (ReturnableProduct),
            typeof (SaleProduct),
            typeof (SaleValueDiscount),
            typeof (SaleValueDiscount.SaleValueDiscountItem),
            typeof (VATClass),
            typeof (VATClass.VATClassItem),
//
//            // Retire
            typeof (RetireSettingDTO),
//
//            // Settings
            typeof (AppSettings),
//
//            // Supplier
            typeof (Supplier),
//
//            //Financials
            typeof (UnderBankingDTO),
//            typeof (UnderBankingItemDTO),
            typeof (PaymentTracker),
//
//            //Inventory
            typeof (Inventory)
        };

        private static readonly List<Type> LocalTypes = new List<Type>()
        {
            typeof (Order),
            typeof (Payment),
            typeof (ProductLineItem),
            typeof (ReturnableLineItem),
            typeof (LocalCommandEnvelope),
            typeof (DatabaseSequence),
            typeof (IncomingEnvelopeLog),
            typeof (SyncLog),
            //TODO move this into MasterDataTypes on receiving test data
            typeof (Containment)
        };

        // Tables common to all users which are not cleaned when a new user signs in
        private static readonly List<Type> PermenantTypes = new List<Type>
        {
            typeof (LastLoggedInUser),
            typeof (LoginSettings),             
        };

        public static IEnumerable<Type> GetMasteDataTypes()
        {
            return MasterDataTypes;
        }

        // Stuff that get's blown away when a new user logs in
        public static IEnumerable<Type> GetTransientTypes()
        {
            return MasterDataTypes.Concat(LocalTypes);
        }

        public static IEnumerable<Type> GetPersistentTypes()
        {
            return PermenantTypes.Concat(MasterDataTypes).Concat(LocalTypes);
        }

        public static IList<Type> GetSyncableTypes()
        {
            return MasterDataTypes;
        }
    }
}