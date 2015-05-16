using System;
using System.Collections.Generic;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

//using log4net;

namespace Distributr.Core.Utility.Mapping.impl
{
    public class DeserializeJson : IDeserializeJson
    {
        private ICommandDeserialize _commandDeserialize;
        //ILog _logger = LogManager.GetLogger("DeserializeJson");
        public DeserializeJson(ICommandDeserialize commandDeserialize)
        {
            _commandDeserialize = commandDeserialize;
        }

        public ResponseMasterDataInfo DeserializeResponseMasterDataInfo(string responseMasterDataInfo)
        {
            ResponseMasterDataInfo response= new ResponseMasterDataInfo{ MasterData = new MasterDataInfo()};
            //try 
            //{
                JObject jo = JObject.Parse(responseMasterDataInfo);
                string entityName = (string)jo["MasterData"]["EntityName"];
                string errorInfo = (string)jo["ErrorInfo"];
                var datelastsyncInfo = jo["MasterData"]["LastSyncTimeStamp"];

                if (datelastsyncInfo != null && (DateTime)datelastsyncInfo > new DateTime(1900, 01, 01))
                {
                    response.MasterData.LastSyncTimeStamp = (DateTime)datelastsyncInfo;
                }
            response.MasterData.EntityName = entityName;
                response.ErrorInfo = errorInfo;
                string listJson = jo["MasterData"]["MasterDataItems"].ToString();
                List<Guid> deletedlistJson = JsonConvert.DeserializeObject<List<Guid>>(jo["DeletedItems"].ToString(), new IsoDateTimeConverter());
            MasterBaseDTO[] dtos = null;
                if (entityName == "Inventory")
                {
                    dtos= DeserialiseToInventoryDTOArray(listJson);
                }
                else if (entityName == "Payment")
                {
                    dtos = DeserialiseToPaymentDTOArray(listJson);
                }
                else if (entityName == "UnderBanking")
                {
                    dtos = DeserialiseToUnderBankingDTOArray(listJson);
                }
                else
                {
                    dtos= DeserialiseToDTOArray(entityName, listJson);
                }
            response.DeletedItems = deletedlistJson;
            response.MasterData.MasterDataItems = dtos;

            //}
            //catch(Exception ex)
            //{
             //   _logger.Error(ex);
            //}
            return response;
        }

        private MasterBaseDTO[] DeserialiseToInventoryDTOArray(string listJson)
        {
            MasterBaseDTO[] mbDTO = null;
            mbDTO = JsonConvert.DeserializeObject<InventoryDTO[]>(listJson, new IsoDateTimeConverter());
            return mbDTO;
        }
        private MasterBaseDTO[] DeserialiseToPaymentDTOArray(string listJson)
        {
            MasterBaseDTO[] mbDTO = null;
            mbDTO = JsonConvert.DeserializeObject<PaymentTrackerDTO[]>(listJson, new IsoDateTimeConverter());
            return mbDTO;
        }
        private MasterBaseDTO[] DeserialiseToUnderBankingDTOArray(string listJson)
        {
            MasterBaseDTO[] mbDTO = null;
            mbDTO = JsonConvert.DeserializeObject<UnderBankingDTO[]>(listJson, new IsoDateTimeConverter());
            return mbDTO;
        }

        MasterBaseDTO[] DeserialiseToDTOArray(string entityName, string listJson)
        {
            MasterDataCollective mdc = (MasterDataCollective) Enum.Parse(typeof(MasterDataCollective), entityName,true) ;
            //if (mdc == null)
            //    throw new Exception("Failed to identify type for deserialization");

            MasterBaseDTO[] mbDTO = null;
            switch (mdc)
            {
                case MasterDataCollective.Country:
                    mbDTO = JsonConvert.DeserializeObject<CountryDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.OutletCategory:
                    mbDTO = JsonConvert.DeserializeObject<OutletCategoryDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Territory:
                    mbDTO = JsonConvert.DeserializeObject<TerritoryDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.OutletType:
                    mbDTO = JsonConvert.DeserializeObject<OutletTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.PricingTier:
                    mbDTO = JsonConvert.DeserializeObject<ProductPricingTierDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductFlavour:
                    mbDTO = JsonConvert.DeserializeObject<ProductFlavourDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductType:
                    mbDTO = JsonConvert.DeserializeObject<ProductTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductBrand:
                    mbDTO = JsonConvert.DeserializeObject<ProductBrandDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductPackagingType:
                    mbDTO = JsonConvert.DeserializeObject<ProductPackagingTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.SocioEconomicStatus:
                    mbDTO = JsonConvert.DeserializeObject<SocioEconomicStatusDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Region:
                    mbDTO = JsonConvert.DeserializeObject<RegionDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Area:
                    mbDTO = JsonConvert.DeserializeObject<AreaDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Distributor:
                    mbDTO = JsonConvert.DeserializeObject<DistributorDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Producer:
                    mbDTO = JsonConvert.DeserializeObject<ProducerDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Outlet:
                    mbDTO = JsonConvert.DeserializeObject<OutletDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                /*case MasterDataCollective.Transporter:
                    mbDTO = JsonConvert.DeserializeObject<TransporterDTO[]>(listJson);
                    break;*/
                case MasterDataCollective.Contact:
                    mbDTO = JsonConvert.DeserializeObject<ContactDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Route:
                    mbDTO = JsonConvert.DeserializeObject<RouteDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.User:
                    mbDTO = JsonConvert.DeserializeObject<UserDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Pricing:
                    mbDTO = JsonConvert.DeserializeObject<ProductPricingDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                /*case MasterDataCollective.PricingItem:
                    mbDTO = JsonConvert.DeserializeObject<ProductPricingItemDTO[]>(listJson);
                    break;*/
                case MasterDataCollective.SaleProduct:
                    mbDTO = JsonConvert.DeserializeObject<SaleProductDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ReturnableProduct:
                    mbDTO = JsonConvert.DeserializeObject<ReturnableProductDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ConsolidatedProduct:
                    mbDTO = JsonConvert.DeserializeObject<ConsolidatedProductDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.VatClass:
                    mbDTO = JsonConvert.DeserializeObject<VATClassDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductPackaging:
                    mbDTO = JsonConvert.DeserializeObject<ProductPackagingDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                /*case MasterDataCollective.ConsolidatedProductItem:
                    mbDTO = JsonConvert.DeserializeObject<ConsolidatedProductProductDetailDTO[]>(listJson);
                    break;*/
                case MasterDataCollective.DistributorSalesman:
                    mbDTO = JsonConvert.DeserializeObject<DistributorSalesmanDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.DistributorPendingDispatchWarehouse:
                    mbDTO = JsonConvert.DeserializeObject<DistributorPendingDispatchWarehouseDTO[]>(listJson,
                    new IsoDateTimeConverter
                        ());
                    break;
                case MasterDataCollective.ChannelPackaging:
                    mbDTO = JsonConvert.DeserializeObject<ChannelPackagingDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.FreeOfChargeDiscount:
                    mbDTO = JsonConvert.DeserializeObject<FreeOfChargeDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.Competitor :
                    mbDTO = JsonConvert.DeserializeObject<CompetitorDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CompetitorProduct :
                    mbDTO = JsonConvert.DeserializeObject<CompetitorProductDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Asset :
                    mbDTO = JsonConvert.DeserializeObject<AssetDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.AssetType :
                    mbDTO = JsonConvert.DeserializeObject<AssetTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.District :
                    mbDTO = JsonConvert.DeserializeObject<DistrictDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Province :
                    mbDTO = JsonConvert.DeserializeObject<ProvinceDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ReorderLevel :
                    mbDTO = JsonConvert.DeserializeObject<ReorderLevelDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                //case MasterDataCollective.Returnables :
                //    mbDTO = JsonConvert.DeserializeObject<ReturnablesDTO[]>(listJson, new IsoDateTimeConverter());
                //    break;
                //case MasterDataCollective.Shells :
                //    mbDTO = JsonConvert.DeserializeObject<ShellsDTO[]>(listJson, new IsoDateTimeConverter());
                //    break;
                case MasterDataCollective.TargetPeriod :
                    mbDTO = JsonConvert.DeserializeObject<TargetPeriodDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Target :
                    mbDTO = JsonConvert.DeserializeObject<TargetDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductDiscount:
                    mbDTO = JsonConvert.DeserializeObject<ProductDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.SaleValueDiscount:
                    mbDTO = JsonConvert.DeserializeObject<SaleValueDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.DiscountGroup:
                    mbDTO = JsonConvert.DeserializeObject<DiscountGroupDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CertainValueCertainProductDiscount:
                    mbDTO = JsonConvert.DeserializeObject<CertainValueCertainProductDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ProductGroupDiscount:
                    mbDTO = JsonConvert.DeserializeObject<ProductGroupDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.PromotionDiscount:
                    mbDTO = JsonConvert.DeserializeObject<PromotionDiscountDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
               
                case MasterDataCollective.SalesmanRoute:
                    mbDTO = JsonConvert.DeserializeObject<SalesmanRouteDTO[]>(listJson, new IsoDateTimeConverter());
                    break;

                case MasterDataCollective.SalesmanSupplier:
                    mbDTO = JsonConvert.DeserializeObject<SalesmanSupplierDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.UserGroup:
                    mbDTO = JsonConvert.DeserializeObject<UserGroupDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.UserGroupRole:
                    mbDTO = JsonConvert.DeserializeObject<UserGroupRoleDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Bank :
                    mbDTO = JsonConvert.DeserializeObject<BankDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.BankBranch:
                    mbDTO = JsonConvert.DeserializeObject<BankBranchDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Supplier:
                    mbDTO=JsonConvert.DeserializeObject<SupplierDTO[]>(listJson,new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ContactType:
                    mbDTO = JsonConvert.DeserializeObject<ContactTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
              
                case MasterDataCollective.AssetCategory:
                        mbDTO = JsonConvert.DeserializeObject<AssetCategoryDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.AssetStatus:
                        mbDTO = JsonConvert.DeserializeObject<AssetStatusDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.OutletVisitDay:
                        mbDTO = JsonConvert.DeserializeObject<OutletVisitDayDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.OutletPriority:
                        mbDTO = JsonConvert.DeserializeObject<OutletPriorityDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.TargetItem:
                        mbDTO = JsonConvert.DeserializeObject<TargetItemDTO[]>(listJson, new IsoDateTimeConverter());
                        break;
                case MasterDataCollective.Setting:
                    mbDTO = JsonConvert.DeserializeObject<AppSettingsDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.RetireSetting:
                    mbDTO = JsonConvert.DeserializeObject<RetireSettingDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommodityType:
                    mbDTO = JsonConvert.DeserializeObject<CommodityTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Commodity:
                    mbDTO = JsonConvert.DeserializeObject<CommodityDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommodityOwner:
                    mbDTO = JsonConvert.DeserializeObject<CommodityOwnerDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommodityOwnerType:
                    mbDTO = JsonConvert.DeserializeObject<CommodityOwnerTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommodityProducer:
                    mbDTO = JsonConvert.DeserializeObject<CommodityProducerDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommoditySupplier:
                    mbDTO = JsonConvert.DeserializeObject<CommoditySupplierDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Centre:
                    mbDTO = JsonConvert.DeserializeObject<CentreDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CentreType:
                    mbDTO = JsonConvert.DeserializeObject<CentreTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Hub:
                    mbDTO = JsonConvert.DeserializeObject<HubDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Store:
                    mbDTO = JsonConvert.DeserializeObject<StoreDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.FieldClerk:
                    mbDTO = JsonConvert.DeserializeObject<PurchasingClerkDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ContainerType:
                    mbDTO = JsonConvert.DeserializeObject<ContainerTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Printer:
                    mbDTO = JsonConvert.DeserializeObject<PrinterDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.WeighScale:
                    mbDTO = JsonConvert.DeserializeObject<WeighScaleDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.SourcingContainer:
                    mbDTO = JsonConvert.DeserializeObject<SourcingContainerDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Vehicle:
                    mbDTO = JsonConvert.DeserializeObject<VehicleDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.RouteCentreAllocation:
                    mbDTO = JsonConvert.DeserializeObject<RouteCentreAllocationDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.RouteCostCentreAllocation:
                    mbDTO = JsonConvert.DeserializeObject<RouteCostCentreAllocationDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.CommodityProducerCentreAllocation:
                    mbDTO = JsonConvert.DeserializeObject<CommodityProducerCentreAllocationDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.RouteRegionAllocation:
                    mbDTO = JsonConvert.DeserializeObject<RouteRegionAllocationDTO[]>(listJson, new IsoDateTimeConverter());
                    break;


                case MasterDataCollective.ServiceProvider:
                    mbDTO = JsonConvert.DeserializeObject<ServiceProviderDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Service:
                    mbDTO = JsonConvert.DeserializeObject<ServiceDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Shift:
                    mbDTO = JsonConvert.DeserializeObject<ShiftDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Season:
                    mbDTO = JsonConvert.DeserializeObject<SeasonDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.Infection:
                    mbDTO = JsonConvert.DeserializeObject<InfectionDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
                case MasterDataCollective.ActivityType:
                    mbDTO = JsonConvert.DeserializeObject<ActivityTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;

                case MasterDataCollective.OutletVisitReasonsType:
                    mbDTO = JsonConvert.DeserializeObject<OutletVisitReasonTypeDTO[]>(listJson, new IsoDateTimeConverter());
                    break;
            }

            return mbDTO;
        }

        public DocumentCommandRoutingResponse DeserializeDocumentCommandRoutingResponse(string jsonString)
        {
            DocumentCommandRoutingResponse response = new DocumentCommandRoutingResponse();
            JObject jo = JObject.Parse(jsonString);
            string _commandType =(string) jo["CommandType"];
            string _commandRouteItemId = (jo["CommandRouteItemId"]).ToString(); ;
            string _commandJson =   jo["Command"].ToString();
            string _errorInfo = (string) jo["ErrorInfo"];
            response.CommandType = _commandType;
            response.CommandRouteItemId = Int32.Parse(_commandRouteItemId);
            response.ErrorInfo = _errorInfo;
            try
            {
              //  response.Command = _commandDeserialize.DeserializeCommand(response.CommandType, _commandJson);
            }catch
            {
                response.Command = null;
                
            }
            return response;
        }

        public BatchDocumentCommandRoutingResponse DeserializeBatchDocumentCommandRoutingResponse(string jsonString)
        {
            BatchDocumentCommandRoutingResponse responce = new BatchDocumentCommandRoutingResponse();
            JObject jo = JObject.Parse(jsonString);
            string RoutingResponse = (jo["RoutingCommands"]).ToString();
           
            if (RoutingResponse!="null")
            {
                foreach (JObject j in jo["RoutingCommands"])
                {
                    responce.RoutingCommands.Add(DeserializeDocumentCommandRoutingResponse(j.ToString()));
                }
            }
            string commandCount =(jo["CommandRoutingCount"]).ToString();
            string LastcommandRouteItemId = (jo["LastCommandRouteItemId"]).ToString();
            responce.LastCommandRouteItemId = long.Parse(LastcommandRouteItemId);
            responce.CommandRoutingCount = int.Parse(commandCount);
            responce.ErrorInfo = (jo["ErrorInfo"]).ToString();
            return responce;

        }
    }
}
