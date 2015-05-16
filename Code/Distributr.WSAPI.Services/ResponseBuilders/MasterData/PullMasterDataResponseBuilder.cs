using System;
using System.Collections.Generic;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.Domain.Master;
using Distributr.WSAPI.Lib.Services.Mapping;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.WSAPI.Lib.ResponseBuilders.MasterData
{
    public class PullMasterDataResponseBuilder : IPullMasterDataResponseBuilder
    {
        IClientMasterDataTrackerRepository _clientMasterDataTrackerRepository;
        IMasterDataToDTOMapping masterDataToDTOMapping;
        
        public PullMasterDataResponseBuilder(IInventoryRepository inventoryRepository,IClientMasterDataTrackerRepository clientMasterDataTrackerRepository, IMasterDataToDTOMapping masterDataToDTOMapping)
        {
            _clientMasterDataTrackerRepository = clientMasterDataTrackerRepository;
           
            this.masterDataToDTOMapping = masterDataToDTOMapping;
        }

        public ResponseSyncRequired DoesCostCentreApplicationNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            bool requiresToSync = false;
            string errorInfo = "Success";

            try
            {
                requiresToSync = _clientMasterDataTrackerRepository.DoesCostCentreApplicationNeedToSync(costCentreApplicationId, vcAppId);
            }
            catch (Exception e)
            {
                errorInfo = e.Message;
            }

            return new ResponseSyncRequired()
            {
                ErrorInfo = errorInfo,
                RequiresToSync = requiresToSync
            };
        }

        public ResponseCostCentreSyncTables RepositoryList()
        {
            string[] repoList = new string[0];
            string errorInfo = "Success";

            try
            {
                repoList = _clientMasterDataTrackerRepository.RepositoryList();
            }
            catch (Exception e)
            {
                errorInfo = e.Message;
            }

            return new ResponseCostCentreSyncTables()
            {
                ErrorInfo = errorInfo,
                TablesToSync = repoList
            };
        }

        public ResponseMasterDataInfo GetTableContents(Guid costCentreApplicationId, string entityName)
        {
            MasterDataCollective mdc = (MasterDataCollective)Enum.Parse(typeof(MasterDataCollective), entityName, true);

            MasterDataInfo mdi = new MasterDataInfo { EntityName = entityName };
            int entityId = (int)mdc;

            MasterDataEnvelope envelope = null;

            envelope = _clientMasterDataTrackerRepository.GetTableContents(costCentreApplicationId, entityId);

            List<MasterBaseDTO> masterBaseDTOList = new List<MasterBaseDTO>();
            if (envelope == null)
            {
                envelope = new MasterDataEnvelope()
                {
                    masterData = new List<MasterEntity>(),
                    masterDataName = ""
                };
            }

            foreach (MasterEntity entity in envelope.masterData)
            {
                MasterBaseDTO masterBaseDTO = MapResolver(mdc, entity);
                if (masterBaseDTO == null)
                    continue;
                masterBaseDTOList.Add(masterBaseDTO);

            }

            mdi.MasterDataItems = masterBaseDTOList.ToArray();

            return new ResponseMasterDataInfo()
            {
                ErrorInfo = "Success",
                MasterData = mdi
            };
        }

        private int getEntityId(string entityName)
        {
            int entityId = 0;
            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
            {
                if (entityName.Equals(masterdata.ToString()))
                {
                    entityId = (int)masterdata;
                    break;
                }
            }
            return entityId;
        }

        private string getEntityName(int entityId)
        {
            string entityName = "";
            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
            {
                if (entityId == (int)masterdata)
                {
                    entityName = masterdata.ToString();
                    break;
                }
            }
            return entityName;
        }

        private MasterBaseDTO MapResolver(MasterDataCollective masterData, MasterEntity me)
        {

            switch (masterData)
            {
                case MasterDataCollective.SaleProduct:
                    return masterDataToDTOMapping.Map(me as SaleProduct);
                case MasterDataCollective.ReturnableProduct:
                    return masterDataToDTOMapping.Map(me as ReturnableProduct);
                case MasterDataCollective.ConsolidatedProduct:
                    return masterDataToDTOMapping.Map(me as ConsolidatedProduct);
                case MasterDataCollective.PricingTier:
                    return masterDataToDTOMapping.Map(me as ProductPricingTier);
                case MasterDataCollective.Producer:
                    Producer p = me as Producer;
                    if (p == null) return null;
                    return masterDataToDTOMapping.Map(p);
                case MasterDataCollective.Country:
                    return masterDataToDTOMapping.Map(me as Country);
                case MasterDataCollective.OutletCategory:
                    return masterDataToDTOMapping.Map(me as OutletCategory);
                case MasterDataCollective.OutletType:
                    return masterDataToDTOMapping.Map(me as OutletType);
                case MasterDataCollective.Outlet:
                    return masterDataToDTOMapping.Map(me as Outlet);
                case MasterDataCollective.Pricing:
                    return masterDataToDTOMapping.Map(me as ProductPricing);
                /*case MasterDataCollective.PricingItem:
                    return masterDataToDTOMapping.Map(me as ProductPricing.ProductPricingItem);
                    break;*/
                case MasterDataCollective.ProductBrand:
                    return masterDataToDTOMapping.Map(me as ProductBrand);
                case MasterDataCollective.ProductFlavour:
                    return masterDataToDTOMapping.Map(me as ProductFlavour);
                case MasterDataCollective.ProductPackagingType:
                    return masterDataToDTOMapping.Map(me as ProductPackagingType);
                case MasterDataCollective.ProductType:
                    return masterDataToDTOMapping.Map(me as ProductType);
                case MasterDataCollective.Region:
                    return masterDataToDTOMapping.Map(me as Region);
                case MasterDataCollective.Territory:
                    return masterDataToDTOMapping.Map(me as Territory);
                case MasterDataCollective.Area://no DTO for route yet
                    return masterDataToDTOMapping.Map(me as Area);
                case MasterDataCollective.SocioEconomicStatus:
                    return masterDataToDTOMapping.Map(me as SocioEconomicStatus);
                case MasterDataCollective.VatClass:
                    return masterDataToDTOMapping.Map(me as VATClass);
                case MasterDataCollective.Distributor:
                    return masterDataToDTOMapping.Map(me as Distributor);
                case MasterDataCollective.Route:
                    return masterDataToDTOMapping.Map(me as Route);
                case MasterDataCollective.Contact:
                    return masterDataToDTOMapping.Map(me as Contact);
                case MasterDataCollective.ProductPackaging:
                    return masterDataToDTOMapping.Map(me as ProductPackaging);
                /*case MasterDataCollective.CostCentreApplication:
                    return masterDataToDTOMapping.Map(me as CostCentreApplication);*/
                case MasterDataCollective.DistributorSalesman:
                    return masterDataToDTOMapping.Map(me as DistributorSalesman);
                case MasterDataCollective.User://no DTO for user yet
                    return masterDataToDTOMapping.Map(me as User);
                case MasterDataCollective.DistributorPendingDispatchWarehouse:
                    return masterDataToDTOMapping.Map(me as DistributorPendingDispatchWarehouse);

                case MasterDataCollective.ChannelPackaging :
                    return masterDataToDTOMapping.Map(me as ChannelPackaging );
                case MasterDataCollective.Competitor :
                    return masterDataToDTOMapping.Map(me as Competitor);
                case MasterDataCollective.CompetitorProduct :
                    return masterDataToDTOMapping.Map(me as CompetitorProducts );
                case MasterDataCollective.Asset :
                    return masterDataToDTOMapping.Map(me as Asset);
                case MasterDataCollective.AssetType :
                    return masterDataToDTOMapping.Map(me as AssetType );
                case MasterDataCollective.District :
                    return masterDataToDTOMapping.Map(me as District);
                case MasterDataCollective.Province :
                    return masterDataToDTOMapping.Map(me as Province);
                case MasterDataCollective.ReorderLevel :
                    return masterDataToDTOMapping.Map(me as ReOrderLevel);
                //case MasterDataCollective.Returnables :
                //    return masterDataToDTOMapping.Map(me as Returnables);
                //case MasterDataCollective.Shells :
                //    return masterDataToDTOMapping.Map(me as Shells);
                case MasterDataCollective.TargetPeriod :
                    return masterDataToDTOMapping.Map(me as TargetPeriod);
                case MasterDataCollective.Target:
                    return masterDataToDTOMapping.Map(me as Target);
                case MasterDataCollective.ProductDiscount:
                    return masterDataToDTOMapping.Map(me as ProductDiscount);
                case MasterDataCollective.SaleValueDiscount:
                    return masterDataToDTOMapping.Map(me as SaleValueDiscount);
                case MasterDataCollective.DiscountGroup:
                    return masterDataToDTOMapping.Map(me as DiscountGroup);
                case MasterDataCollective.CertainValueCertainProductDiscount:
                    return masterDataToDTOMapping.Map(me as CertainValueCertainProductDiscount);
                case MasterDataCollective.PromotionDiscount:
                    return masterDataToDTOMapping.Map(me as PromotionDiscount);
                case MasterDataCollective.ProductGroupDiscount:
                    return masterDataToDTOMapping.Map(me as ProductGroupDiscount);
                case MasterDataCollective.FreeOfChargeDiscount:
                    return masterDataToDTOMapping.Map(me as FreeOfChargeDiscount);
                case MasterDataCollective.SalesmanRoute:
                    return masterDataToDTOMapping.Map(me as SalesmanRoute);

                case MasterDataCollective.SalesmanSupplier:
                    return masterDataToDTOMapping.Map(me as SalesmanSupplier);
                case MasterDataCollective.UserGroup:
                    return masterDataToDTOMapping.Map(me as UserGroup);
                case MasterDataCollective.UserGroupRole:
                    return masterDataToDTOMapping.Map(me as UserGroupRoles);
                case MasterDataCollective.Bank:
                    return masterDataToDTOMapping.Map(me as Bank);
                case MasterDataCollective.BankBranch:
                    return masterDataToDTOMapping.Map(me as BankBranch);
                case MasterDataCollective.Supplier:
                    return masterDataToDTOMapping.Map(me as Supplier);
               
                case MasterDataCollective.ContactType:
                    return masterDataToDTOMapping.Map(me as ContactType);
                case MasterDataCollective.AssetStatus:
                    return masterDataToDTOMapping.Map(me as AssetStatus);
                case MasterDataCollective.AssetCategory:
                    return masterDataToDTOMapping.Map(me as AssetCategory);
                case MasterDataCollective.OutletPriority:
                    return masterDataToDTOMapping.Map(me as OutletPriority);
                case MasterDataCollective.OutletVisitDay:
                    return masterDataToDTOMapping.Map(me as OutletVisitDay);
                case MasterDataCollective.TargetItem:
                    return masterDataToDTOMapping.Map(me as TargetItem);
                case MasterDataCollective.Setting:
                    return masterDataToDTOMapping.Map(me as AppSettings);
                case MasterDataCollective.RetireSetting:
                    return masterDataToDTOMapping.Map(me as RetireDocumentSetting);
                default:
                    throw new Exception("Failed to map to DTO " + masterData.ToString());
            }

        }

        public ResponseCostCentreTest GetTestCostCentre()
        {
            TestCostCentreEnvelope testCostCentreEnvelope = _clientMasterDataTrackerRepository.GetTestCostCentre();
            return new ResponseCostCentreTest()
            {
                costCentreId = testCostCentreEnvelope.costCentreId,
                costCentreType = testCostCentreEnvelope.costCentreType,
                ErrorInfo = "Success"
            };
        }

        public ResponseMasterDataInfo GetInventory(Guid costCentreApplicationId)
        {

            MasterDataInfo mdi = new MasterDataInfo { EntityName = "Inventory" };
           // int entityId = (int)mdc;

            MasterDataEnvelope envelope = null;

            envelope = _clientMasterDataTrackerRepository.GetInventory(costCentreApplicationId);

            List<MasterBaseDTO> masterBaseDTOList = new List<MasterBaseDTO>();
            if (envelope == null)
            {
                envelope = new MasterDataEnvelope()
                {
                    masterData = new List<MasterEntity>(),
                    masterDataName = ""
                };
            }

            foreach (MasterEntity entity in envelope.masterData)
            {
                MasterBaseDTO masterBaseDTO = masterDataToDTOMapping.Map(entity as Inventory);
                if (masterBaseDTO == null)
                    continue;
                masterBaseDTOList.Add(masterBaseDTO);

            }

            mdi.MasterDataItems = masterBaseDTOList.ToArray();

            return new ResponseMasterDataInfo()
            {
                ErrorInfo = "Success",
                MasterData = mdi
            };
        }

        public ResponseMasterDataInfo GetPayments(Guid costCentreApplicationId)
        {
            MasterDataInfo mdi = new MasterDataInfo { EntityName = "Payment" };
            // int entityId = (int)mdc;

            MasterDataEnvelope envelope = null;

            envelope = _clientMasterDataTrackerRepository.GetPayments(costCentreApplicationId);

            List<MasterBaseDTO> masterBaseDTOList = new List<MasterBaseDTO>();
            if (envelope == null)
            {
                envelope = new MasterDataEnvelope()
                {
                    masterData = new List<MasterEntity>(),
                    masterDataName = ""
                };
            }

            foreach (MasterEntity entity in envelope.masterData)
            {
                MasterBaseDTO masterBaseDTO = masterDataToDTOMapping.Map(entity as PaymentTracker);
                if (masterBaseDTO == null)
                    continue;
                masterBaseDTOList.Add(masterBaseDTO);

            }

            mdi.MasterDataItems = masterBaseDTOList.ToArray();

            return new ResponseMasterDataInfo()
            {
                ErrorInfo = "Success",
                MasterData = mdi
            };
        }
    }
}
