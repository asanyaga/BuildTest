//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.Core.Repository.Master;
//using Distributr.Core.MasterDataDTO;
//using Distributr.Core.Domain.Master;
//using Distributr.WSAPI.Lib.API.Model.MasterDataDTO;
//using Distributr.WSAPI.Lib.Services.Mapping;
//using Distributr.WSAPI.Lib.Services.Mapping.impl;
//using Distributr.Core.Domain.Master.ProductEntities;
//using Distributr.Core.Domain.Master.UserEntities;
//using Distributr.Core.Domain.Master.CostCentreEntities;
//using Distributr.WSAPI.Lib.CommandResults;

//namespace Distributr.WSAPI.ModelBuilder.impl
//{
//    public class ClientMasterDataManager : IClientMasterDataManager
//    {
//        IClientMasterDataTrackerRepository _clientMasterDataTrackerRepository;
//        IMasterDataToDTOMapping masterDataToDTOMapping;
//        public ClientMasterDataManager(IClientMasterDataTrackerRepository clientMasterDataTrackerRepository,IMasterDataToDTOMapping masterDataToDTOMapping )
//        {
//            _clientMasterDataTrackerRepository = clientMasterDataTrackerRepository;
//            this.masterDataToDTOMapping = masterDataToDTOMapping;
//        }

//        public ResponseSyncRequired DoesCostCentreApplicationNeedToSync(int costCentreApplicationId)
//        {
//            bool requiresToSync = false;
//            string errorInfo = "Success";

//            try
//            {
//                requiresToSync = _clientMasterDataTrackerRepository.DoesCostCentreApplicationNeedToSync(costCentreApplicationId);
//            }
//            catch (Exception e)
//            {
//                errorInfo = e.Message;
//            }

//            return new ResponseSyncRequired()
//            {
//                ErrorInfo = errorInfo,
//                RequiresToSync = requiresToSync
//            };
//        }

//        public ResponseCostCentreSyncTables RepositoryList()
//        {
//            string[] repoList = new string[0];
//            string errorInfo = "Success";

//            try
//            {
//                repoList = _clientMasterDataTrackerRepository.RepositoryList();
//            }
//            catch (Exception e)
//            {
//                errorInfo = e.Message;
//            }

//            return new ResponseCostCentreSyncTables()
//            {
//                ErrorInfo = errorInfo,
//                TablesToSync = repoList
//            };
//        }

//        public ResponseMasterDataInfo GetTableContents(int costCentreApplicationId, string entityName)
//        {
//            MasterDataCollective mdc = (MasterDataCollective)Enum.Parse(typeof(MasterDataCollective), entityName, true);

//            MasterDataInfo mdi = new MasterDataInfo { EntityName = entityName };
//            int entityId = (int)mdc;

//            MasterDataEnvelope envelope = null;
//            //try
//            //{
//                envelope = _clientMasterDataTrackerRepository.GetTableContents(costCentreApplicationId, entityId);
//            //}
//            //catch (Exception e)
//            //{
//            //    return new ResponseMasterDataInfo()
//            //    {
//            //        MasterData = new MasterDataInfo()
//            //        {
//            //            EntityName = "",
//            //            MasterDataItems = new MasterBaseDTO[0]
//            //        },
//            //        ErrorInfo = e.Message
//            //    };
//            //}

//            List<MasterBaseDTO> masterBaseDTOList = new List<MasterBaseDTO>();
//            if (envelope == null)
//            {
//                envelope = new MasterDataEnvelope()
//                {
//                    masterData = new List<MasterEntity>(),
//                    masterDataName = ""
//                };
//            }
//            foreach (MasterEntity entity in envelope.masterData)
//            {
//                MasterBaseDTO masterBaseDTO = MapResolver(mdc, entity);
//                if (masterBaseDTO == null)
//                    continue;
//                masterBaseDTOList.Add(masterBaseDTO);

//            }
//            mdi.MasterDataItems = masterBaseDTOList.ToArray();

//            return new ResponseMasterDataInfo()
//            {
//                ErrorInfo = "Success",
//                MasterData = mdi
//            };
//        }

//        private int getEntityId(string entityName)
//        {
//            int entityId = 0;
//            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
//            {
//                if (entityName.Equals(masterdata.ToString()))
//                {
//                    entityId = (int)masterdata;
//                    break;
//                }
//            }
//            return entityId;
//        }

//        private string getEntityName(int entityId)
//        {
//            string entityName = "";
//            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
//            {
//                if (entityId == (int)masterdata)
//                {
//                    entityName = masterdata.ToString();
//                    break;
//                }
//            }
//            return entityName;
//        }

//        private MasterBaseDTO MapResolver(MasterDataCollective masterData, MasterEntity me)
//        {

//            switch (masterData)
//            {
//                case MasterDataCollective.SaleProduct:

//                    return masterDataToDTOMapping.Map(me as SaleProduct);
//                    break;
//                case MasterDataCollective.ReturnableProduct:
//                    return masterDataToDTOMapping.Map(me as ReturnableProduct);
//                    break;
//                case MasterDataCollective.ConsolidatedProduct:
//                    return masterDataToDTOMapping.Map(me as ConsolidatedProduct);
//                    break;

//                case MasterDataCollective.PricingTier:
//                    return masterDataToDTOMapping.Map(me as ProductPricingTier);
//                    break;
//                case MasterDataCollective.Producer:
//                    Producer p = me as Producer;
//                    if (p == null) return null;
//                    return masterDataToDTOMapping.Map(p);
//                    break;
               
//                case MasterDataCollective.Country:
//                    return masterDataToDTOMapping.Map(me as Country);
//                    break;
//                case MasterDataCollective.OutletCategory:
//                    return masterDataToDTOMapping.Map(me as OutletCategory);
//                    break;
//                case MasterDataCollective.OutletType:
//                    return masterDataToDTOMapping.Map(me as OutletType);
//                    break;
//                case MasterDataCollective.Outlet:
//                    return masterDataToDTOMapping.Map(me as Outlet);
//                    break;
//                case MasterDataCollective.Pricing:
//                    return masterDataToDTOMapping.Map(me as ProductPricing);
//                    break;
//                /*case MasterDataCollective.PricingItem:
//                    return masterDataToDTOMapping.Map(me as ProductPricing.ProductPricingItem);
//                    break;*/
//                case MasterDataCollective.ProductBrand:
//                    return masterDataToDTOMapping.Map(me as ProductBrand);
//                    break;
//                case MasterDataCollective.ProductFlavour:
//                    return masterDataToDTOMapping.Map(me as ProductFlavour);
//                    break;
//                case MasterDataCollective.ProductPackagingType:
//                    return masterDataToDTOMapping.Map(me as ProductPackagingType);
//                    break;
//                case MasterDataCollective.ProductType:
//                    return masterDataToDTOMapping.Map(me as ProductType);
//                    break;
//                case MasterDataCollective.Region:
//                    return masterDataToDTOMapping.Map(me as Region);
//                    break;
//                case MasterDataCollective.Territory:
//                    return masterDataToDTOMapping.Map(me as Territory);
//                    break;
//                case MasterDataCollective.Area://no DTO for route yet
//                    return masterDataToDTOMapping.Map(me as Area);
//                    break;
//                case MasterDataCollective.SocioEconomicStatus:
//                    return masterDataToDTOMapping.Map(me as SocioEconomicStatus);
//                    break;
//                case MasterDataCollective.VatClass:
//                    return masterDataToDTOMapping.Map(me as VATClass);
//                    break;
//                case MasterDataCollective.Distributor:
//                    return masterDataToDTOMapping.Map(me as Distributor);
//                    break;
//                case MasterDataCollective.Route:
//                    return masterDataToDTOMapping.Map(me as Route);
//                    break;
//                case MasterDataCollective.Contact:
//                    return masterDataToDTOMapping.Map(me as Contact);
//                    break;
//                case MasterDataCollective.ProductPackaging:
//                    return masterDataToDTOMapping.Map(me as ProductPackaging);
//                    break;
//                /*case MasterDataCollective.CostCentreApplication:
//                    return masterDataToDTOMapping.Map(me as CostCentreApplication);*/
//                case MasterDataCollective.DistributorSalesman:
//                    return masterDataToDTOMapping.Map(me as DistributorSalesman);
//                case MasterDataCollective.User://no DTO for user yet
//                    return masterDataToDTOMapping.Map(me as User);
//                    break;
//                default:
//                    throw new Exception("Failed to map to DTO " + masterData.ToString());
//                    break;
//            }

//            return null;
//        }

//        public ResponseCostCentreTest GetTestCostCentre()
//        {
//            TestCostCentreEnvelope testCostCentreEnvelope = _clientMasterDataTrackerRepository.GetTestCostCentre();
//            return new ResponseCostCentreTest()
//            {
//                costCentreId = testCostCentreEnvelope.costCentreId,
//                costCentreType = testCostCentreEnvelope.costCentreType,
//                ErrorInfo = "Success"
//            };
//        }
//    }
//}
