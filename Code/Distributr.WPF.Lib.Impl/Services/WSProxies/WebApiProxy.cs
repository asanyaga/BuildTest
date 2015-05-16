using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Notifications;
using Distributr.Core.Utility.Mapping;
using Distributr.Import.Entities;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using log4net;

namespace Distributr.WPF.Lib.Impl.Services.WSProxies
{
    public class WebApiProxy : IWebApiProxy
    {
        private IConfigService _configService;
        private IDeserializeJson _deserializeJson;
        private ILog _log = LogManager.GetLogger("WebApiProxy");
        private IOtherUtilities _otherUtilities;

        public WebApiProxy(IConfigService configService, IDeserializeJson deserializeJson, IOtherUtilities otherUtilities)
        {
            _configService = configService;
            _deserializeJson = deserializeJson;
            _otherUtilities = otherUtilities;
        }

        private HttpClient setupClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(90);
            client.BaseAddress = new Uri(_configService.Load().WebServiceUrl);
            return client;
        }

        public async Task<bool> DoesCostCenreNeedToSyncAsync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            string url = string.Format("api/masterdata?costcentreapplicationid={0}&vcAppId={1}", costCentreApplicationId, vcAppId);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var needsToSync = await response.Content.ReadAsAsync<ResponseSyncRequired>();

            return needsToSync.RequiresToSync;
        }

        public async Task<string[]> ListMasterDataEntitiesAsync()
        {
            string url = "api/masterdata";
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var entities = await response.Content.ReadAsAsync<ResponseCostCentreSyncTables>();

            return entities.TablesToSync;
        }

        public async Task<ResponseMasterDataInfo> GetEntityMasterDataAsync(Guid costCentreApplicationId, string entityName)
        {
            string url = string.Format("api/masterdata?costcentreapplicationid={0}&entityName={1}", costCentreApplicationId, entityName);
            var httpClient = setupClient();

            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }
        public async Task<string> GetEntityMasterDataQueryAsync(Guid costCentreApplicationId, string entityName)
        {

            string url = string.Format("api/masterdata/sync/{0}?costcentreapplicationid={1}", entityName,
                                       costCentreApplicationId);
            var httpClient = setupClient();

            var response = await httpClient.GetAsync(url);

            var json = await response.Content.ReadAsAsync<string>();
            return json;
        }

        public async Task<ResponseMasterDataInfo> GetEntityMasterDataAsync(Guid costCentreApplicationId, string entityName, int page, int pagesize, DateTime lastsynctimeTimeStamp)
        {
            string url = string.Format("api/masterdata/sync/{0}?costcentreapplicationid={1}&page={2}&pagesize={3}&syncTimeStamp={4}&isHub={5}", entityName, costCentreApplicationId, page, pagesize, lastsynctimeTimeStamp, true);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            if(entityName=="Infection")
            {
                
            }
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }



        public async Task<ResponseMasterDataInfo> GetInventoryAsync(Guid costCentreApplicationId)
        {
            string url = string.Format("api/inventory?costcentreapplicationid={0}", costCentreApplicationId);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }

        public async Task<ResponseMasterDataInfo> GetInventoryAsync(Guid costCentreApplicationId, int page, int pagesize)
        {
            string url = string.Format("api/masterdata/sync/inventory?costcentreapplicationid={0}&page={1}&pagesize={2}", costCentreApplicationId, page, pagesize);
           
           
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
             ResponseMasterDataInfo info  = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }

        public async Task<ResponseMasterDataInfo> GetUnderBankingAsync(Guid costCentreApplicationId)
        {
            string url = string.Format("api/masterdata/sync/UnderBanking?costcentreapplicationid={0}", costCentreApplicationId);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }

        public async Task<ResponseMasterDataInfo> GetPaymentsAsync(Guid costCentreApplicationId)
        {
            string url = string.Format("api/payments?costcentreapplicationid={0}", costCentreApplicationId);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }

        public async Task<ResponseMasterDataInfo> GetPaymentsAsync(Guid costCentreApplicationId,int page,int pageSize)
        {
            string url = string.Format("api/masterdata/sync/payments?costcentreapplicationid={0}&page={1}&pagesize={2}", costCentreApplicationId,page,pageSize);
            
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            ResponseMasterDataInfo info = _deserializeJson.DeserializeResponseMasterDataInfo(json);
            return info;
        }

        public async Task<bool> SendCommandAsync(DocumentCommand command)
        {
            bool success = false;
            string urlSuffix = "api/command/run";
            HttpClient client = setupClient();
            _log.InfoFormat("Attempting to post command with id {0} to {1}", command.CommandId, client.BaseAddress + urlSuffix);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                //var response = client.PostAsJsonAsync(urlSuffix, command).Result;
                var response = await client.PostAsJsonAsync(urlSuffix, command);
                response.EnsureSuccessStatusCode();
                ResponseBasic _response = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_response == null)
                {
                    _log.InfoFormat("Sent command id {0} Failed", command.CommandId);
                    return success;
                }
                if (_response.Result.Equals("Command Processed") && response.IsSuccessStatusCode)
                {
                    _log.InfoFormat("Sent command id {0} success", command.CommandId);
                    success = true;
                }
                else
                {
                    _log.InfoFormat("Sent command id {0} Failed, ResultInfo= {1}, ErrorInfo= {2} ", command.CommandId, _response.ResultInfo, _response.ErrorInfo);

                }
            }
            catch (Exception ex)
            {

                _log.ErrorFormat("Failed to send command {0}", command.CommandId);
                _log.Error("Failed to send command", ex);
                throw ex;
            }
            return success;
        }

        public async Task<bool> SendCommandEnvelope(CommandEnvelope envelope)
        {
            bool success = false;
            string urlSuffix = "api/commandenvelope/run";
            HttpClient client = setupClient();
            _log.InfoFormat("Attempting to post Envelope with id {0} to {1}", envelope.Id, client.BaseAddress + urlSuffix);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                //var response = client.PostAsJsonAsync(urlSuffix, command).Result;
                var response = await client.PostAsJsonAsync(urlSuffix, envelope);
                response.EnsureSuccessStatusCode();
                ResponseBasic _response = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_response == null)
                {
                    _log.InfoFormat("Sent Envelope id {0} Failed", envelope.Id);
                    return success;
                }
                if (_response.Result.Equals("Envelope Processed") && response.IsSuccessStatusCode)
                {
                    _log.InfoFormat("Sent Envelope id {0} success", envelope.Id);
                    success = true;
                }
                else
                {
                    _log.InfoFormat("Sent Envelope id {0} Failed, ResultInfo= {1}, ErrorInfo= {2} ", envelope.Id, _response.ResultInfo, _response.ErrorInfo);

                }
            }
            catch (Exception ex)
            {

                _log.ErrorFormat("Failed to send Envelope {0}", envelope.Id);
                _log.Error("Failed to send Envelope", ex);
                throw ex;
            }
            return success;
        }

        public async Task<bool> SendNotificationsAsync(NotificationBase notification)
        {
            bool success = false;
            string urlSuffix = "api/notification/notify";
            HttpClient client = setupClient();
            _log.InfoFormat("Attempting to post notification with id {0} to {1}", notification.Id, client.BaseAddress + urlSuffix);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {

                var response = await client.PostAsJsonAsync(urlSuffix, notification);
                response.EnsureSuccessStatusCode();
                ResponseBasic _response = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_response == null)
                {
                    _log.InfoFormat("Sent notification id {0} Failed", notification.Id);
                    return success;
                }
                if (_response.Result.Equals("Notification Processed") && response.IsSuccessStatusCode)
                {
                    _log.InfoFormat("Sent notification id {0} success", notification.Id);
                    success = true;
                }
                else
                {
                    _log.InfoFormat("Sent notification id {0} Failed, ResultInfo= {1}, ErrorInfo= {2} ", notification.Id, _response.ResultInfo, _response.ErrorInfo);

                }
            }
            catch (Exception ex)
            {

                _log.ErrorFormat("Failed to send notification {0}", notification.Id);
                _log.Error("Failed to send notification", ex);
                throw ex;
            }
            return success;
        }

        public async Task<bool> PingServerAsync()
        {
            string url = "api/ping";
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var isalive = await response.Content.ReadAsAsync<ResponseBool>();
            return isalive.Success;
        }

        public async Task<bool> InvalidateCacheAsync()
        {
            string url = "api/invalidatecache";
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var ok = await response.Content.ReadAsAsync<ResponseBool>();
            return ok.Success;
        }

        public async Task<DocumentCommandRoutingResponse> GetNextDocumentCommandAsync(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId)
        {
            string url = string.Format("api/commandrouting?costcentreapplicationid={0}&lastDeliveredCommandRouteItemId={1}", costCentreApplicationId, lastDeliveredCommandRouteItemId);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var routingResponse = await response.Content.ReadAsAsync<DocumentCommandRoutingResponse>();
            return routingResponse;
        }

        public async Task<BatchDocumentCommandRoutingResponse> GetNextBatchDocumentCommandAsync(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId, int batchSize, string batchIdsJson)
        {
            string url = string.Format("api/commandrouting?costcentreapplicationid={0}&lastDeliveredCommandRouteItemId={1}&batchsize={2}&batchidsjson={3}", costCentreApplicationId, lastDeliveredCommandRouteItemId, batchSize, batchIdsJson);
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var routingResponse = await response.Content.ReadAsAsync<BatchDocumentCommandRoutingResponse>();
            return routingResponse;
        }

        public async Task<BatchDocumentCommandEnvelopeRoutingResponse> GetNextCommandEnvelopesAsync(EnvelopeRoutingRequest request)
        {
            
            string url = string.Format("api/commandenveloperouting/GetNextEnvelopes");
            var httpClient = setupClient();
            var response = await httpClient.PostAsJsonAsync(url, request);
            var routingResponse = await response.Content.ReadAsAsync<BatchDocumentCommandEnvelopeRoutingResponse>();
            return routingResponse;
        }

        public async Task<Guid> LoginAsync(string userName, string password, string userType)
        {
            string encPw = _otherUtilities.MD5Hash(password);
            string url = string.Format("api/login?username={0}&password={1}&usertype={2}", userName, encPw,
                                       UserType.WarehouseManager.ToString());
            var httpClient = setupClient();
            var response = await httpClient.GetAsync(url);
            var _response = await response.Content.ReadAsAsync<CostCentreLoginResponse>();
            return _response.CostCentreId;
        }

        public async Task<CreateCostCentreApplicationResponse> CreateCostCentreApplicationAsync(Guid costCentreId, string applicationDescription)
        {
            string url = string.Format("api/costcentreapplication?costCentreId={0}&applicationDescription={1}",
                                       costCentreId, applicationDescription);
            var httpClient = setupClient();
            httpClient.Timeout=new TimeSpan(0,0,5,0);
            var response = await httpClient.GetAsync(url);
            var _response = await response.Content.ReadAsAsync<CreateCostCentreApplicationResponse>();
           // return _response.CostCentreApplicationId;
            return _response;
        }

        public async Task<bool> PushMasterDataAsync(MasterDataDTOSaveCollective masterDataCollective, MasterBaseDTO jsonDto)
        {
            PushMasterDataDto pushMasterDataDto = new PushMasterDataDto
                {
                    MasterDataCollective = masterDataCollective.ToString(),
                    MasterDto = jsonDto
                };
            bool success = false;
            _log.InfoFormat("Attempting to send command");
            HttpClient client = setupClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await client.PostAsJsonAsync("api/pushmasterdata/run", pushMasterDataDto);
                var _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Info("Failed to send command " + jsonDto);
                _log.Error("Failed to send command", ex);
            }
            return success;
        }

        public async Task<bool> SendInventoryImport(List<InventoryImport> list)
        {
            HttpClient client = setupClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                const string urlSuffix = "api/new/Integration/Inventory";
                var response = await client.PostAsJsonAsync(urlSuffix, list);
                response.EnsureSuccessStatusCode();
                ImportResponse _response = await response.Content.ReadAsAsync<ImportResponse>();
                if (_response != null)
                {
                    return _response.Status;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }

            
        }
    }
}
