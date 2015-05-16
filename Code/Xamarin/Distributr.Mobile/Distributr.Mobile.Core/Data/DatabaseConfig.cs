using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Mobile.Core.Commands;
using Distributr.Mobile.Login;
using Distributr.Mobile.Login.Settings;
using Distributr.Mobile.Products;

namespace Distributr.Mobile.Data
{
    public static class DatabaseConfig
    {
        // Stuff that get's synced through the masterdata/sync endpoint. We also create a 
        // table for each of these entities.
        private static readonly List<Type> syncableTypes = new List<Type>
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
//
//            // Cost Centre
            typeof (Area),
            typeof (Contact),
            typeof (ContactType),
            typeof (Region),
            typeof (Country),
            typeof (Distributor),
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

            typeof (Producer),
            typeof (SocioEconomicStatus),
            //TODO not present in file
            //typeof (Territory),
//
//            // Marital Status
//            // Appears in old Android code base but doesn't appear to be used
//            // typeof(MaritalStatusDTO),

              
//            // Targets
            typeof (Target),
            //TODO not present in file
//          typeof (TargetItemDTO),
            typeof (TargetPeriod),
//
//            // Product
              typeof (CertainValueCertainProductDiscount),
              //TODO not present in file
              //typeof (ChannelPackaging),
//            typeof (ConsolidatedProductDTO),
//            typeof (DiscountGroupDTO),

            typeof (FreeOfChargeDiscount),
            typeof (ProductBrand),
            typeof (ProductDiscount),
            typeof (ProductFlavour),
            //TODO not present in file
            //typeof (ProductGroupDiscount),

            typeof (ProductPackaging),
            typeof (ProductPackagingType),
            typeof (Pricing),
            typeof (ProductPricing.ProductPricingItem),
            typeof (ProductPricingTier),
            typeof (ProductType),
            typeof (ReturnableProduct),
            typeof (SaleProduct),
            typeof (SaleValueDiscount),
            typeof (VATClass),
            typeof (VATClass.VATClassItem),
//
//            // Retire
            typeof (RetireSettingDTO),
//
//            // Settings
              //TODO not present in file
//            typeof (AppSettingsDTO),
//
//            // Supplier
            typeof (Supplier),
//
//            //Financials
            typeof (UnderBankingDTO),
            typeof (PaymentTracker),
//
//            //Inventory
            typeof (Inventory)
        };

        // These are child tables that are in many-to-one relationshsips with the synced entities above.
        private static readonly List<Type> childTypes = new List<Type>
        {
//            typeof (CertainValueCertainProductDiscountItemDTO),
//            typeof (ConsolidatedProductProductDetailDTO),
//            typeof (ProductDiscountItemDTO),
//            // Looks to be unused - the GUID in the linked ProductGroupDiscount record is all zeroes
//            // typeof(ProductGroupDiscountItemDTO),
//            typeof (ProductDiscountItemDTO),
//            typeof (SaleValueDiscountItemDTO),
//            typeof (UnderBankingItemDTO)
        };

        // Local tables 
        private static readonly List<Type> localTypes = new List<Type>
        {
            typeof (LastLoggedInUser),
            typeof (LoginSettings), 
            typeof (OutboundCommandEnvelope)
        };

        // Stuff that get's blown away when a new user logs in
        public static IEnumerable<Type> GetTransientTypes()
        {
            return childTypes.Concat(syncableTypes);
        }

        public static IEnumerable<Type> GetPersistentTypes()
        {
            return localTypes.Concat(childTypes.Concat(syncableTypes));
        }

        public static IList<Type> GetSyncableTypes()
        {
            return syncableTypes;
        }
    }
}