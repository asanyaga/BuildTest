﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WSAPI.Lib.API.Model.InventoriesDTO;
using Distributr.WSAPI.Lib.DTOModels.FinancialDTO;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Assets;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.CostCentre;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Retire;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Settings;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Distributr.WSAPI.Lib.API.Model.MasterDataDTO;
using Distributr.WSAPI.Lib.API.Model.MasterDataDTO.CostCentre;
using Distributr.WSAPI.Lib.API.Model.MasterDataDTO.Product;
using Distributr.WSAPI.Lib.API.Model.MasterDataDTO.User;
using Distributr.Core.Domain.Master;
using Distributr.WSAPI.Lib.CommandResults;
using Newtonsoft.Json.Converters;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Product;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Competitor;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Coolers;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Banks;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Suppliers;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.MaritalStatus;
//using log4net;

namespace Distributr.WSAPI.Lib.Services.Mapping.impl
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
                response.MasterData.EntityName = entityName;
                response.ErrorInfo = errorInfo;
                string listJson = jo["MasterData"]["MasterDataItems"].ToString();
            MasterBaseDTO[] dtos = null;
                if (entityName == "Inventory")
                {
                    dtos= DeserialiseToInventoryDTOArray(listJson);
                }
                else if (entityName == "Payment")
                {
                    dtos = DeserialiseToPaymentDTOArray(listJson);
                }
                else
                {
                    dtos= DeserialiseToDTOArray(entityName, listJson);
                }
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
                case MasterDataCollective.ChannelPackaging :
                    mbDTO = JsonConvert.DeserializeObject<ChannelPacksDTO[]>(listJson, new IsoDateTimeConverter());
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
                case MasterDataCollective.MaritalStatus:
                    mbDTO = JsonConvert.DeserializeObject<MaritalStatusDTO[]>(listJson, new IsoDateTimeConverter());
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
                response.Command = _commandDeserialize.DeserializeCommand(response.CommandType, _commandJson);
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
