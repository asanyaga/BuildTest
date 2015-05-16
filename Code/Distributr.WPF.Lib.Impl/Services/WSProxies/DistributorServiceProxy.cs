using System.Diagnostics;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility.Mapping;
using Newtonsoft.Json;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.Impl.Services.WSProxies
{
    public class DistributorServiceProxy : IDistributorServiceProxy
    {
        protected IWarehouseReceiptClient WarehouseReceiptClient;

        private IConfigService _configService;
        private ILog _log = LogManager.GetLogger("DistributorServiceProxy");

        private IMasterDataToDTOMapping _masterDataToDTOMapping;

        public DistributorServiceProxy(IConfigService configService, IMasterDataToDTOMapping masterDataToDtoMapping)
        {
            _configService = configService;
            _masterDataToDTOMapping = masterDataToDtoMapping;
        }

        private HttpClient setupHttpClient()
        {
            return new HttpClient { BaseAddress = new Uri(_configService.Load().WebServiceUrl) };

        }


        public async Task<List<OutletItem>> DistributorOutletListAsync(Guid distributorId)
        {
            string url = string.Format("api/distributorservices/distributoroutletlist/{0}", distributorId);
            var httpClient = setupHttpClient();
            var response = await httpClient.GetAsync(url);
            var outlets = await response.Content.ReadAsAsync<List<OutletItem>>();
            return outlets;
        }

        public async Task<ResponseBool> OutletAddAsync(OutletItem outletItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = httpClient.PostAsJsonAsync(url, outletItem).Result;
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when saving the outlet.\nCause: " + ex.Message;
                _log.Error("Failed to add outlet", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> OutletUpdateAsync(OutletItem outletItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, outletItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when updating the outlet.\nCause: " + ex.Message;
                _log.Error("Failed to update outlet", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> OutletsApproveAsync(List<Guid> outletIds)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletsapprove");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, outletIds);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when approving the outlet.\n cause" + ex.Message;
                _log.Error("Failed to approve outlet", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> OutletDeactivateAsync(Guid outletId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletdeactivate/{0}", outletId);
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate outlet");
                _response.ErrorInfo = "Error: An error occurred when deactivating the outlet.\nCause" + ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> OutletActivateAsync(Guid outletId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletactivate/{0}", outletId);
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex) //any other
            {
                _log.Error("Failed to Activate outlet");
                _response.ErrorInfo = "Error: An error occurred when activating the outlet.\n Cause: " + ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> OutletDeleteAsync(Guid outletId)
        {

            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/outletdelete/{0}", outletId);
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex) //any other
            {
                _log.Error("Failed to delete outlet");
                _response.ErrorInfo = "Error: An error occurred when deleting the outlet.\n" + ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> TargetAddAsync(CostCentreTargetItem targetItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/targetadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, targetItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add outlet item", ex);
                _response.ErrorInfo = "Failed to add outlet item\n" + ex;
            }
            return _response;
        }

        public async Task<ResponseBool> TargetUpdateAsync(CostCentreTargetItem targetItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/targetupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, targetItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update outlet item", ex);
                _response.ErrorInfo = "Failed to update outlet item\n" + ex;
            }
            return _response;
        }

        public async Task<ResponseBool> TargetDeactivateAsync(Guid targetId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/targetdeactivate/{0}", targetId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate target", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> TargetActivateAsync(Guid targetId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/targetactivate/{0}", targetId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to activate target", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> TargetDeleteAsync(Guid targetId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/targetdelete/{0}", targetId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to activate target", ex);
            }
            return _response;
        }

        public async Task<List<RouteItem>> RouteListAsync(Guid distributorId)
        {
            string url = string.Format("api/distributorservices/routelist/{0}", distributorId);
            var httpClient = setupHttpClient();
            var response = await httpClient.GetAsync(url);
            var routes = await response.Content.ReadAsAsync<List<RouteItem>>();
            return routes;
        }

        public async Task<ResponseBool> RouteAddAsync(RouteItem routeItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/routeadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {

                var response = await httpClient.PostAsJsonAsync(url, routeItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when saving the route.\nCause: " + ex.Message;
                _log.Error("Failed to add bank", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> RouteUpdateAsync(RouteItem routeItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/routeupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, routeItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when saving the route.\nCause: " + ex.Message;
                _log.Error("Error: An error occurred when saving the route.", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> RouteDeactivateAsync(Guid routeId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/routedeactivate/{0}", routeId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deactivating the route.\nCause: " + ex.Message;
                _log.Error("Error: An error occurred when deactivating the route.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> RouteDeleteAsync(Guid routeId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/routedelete/{0}", routeId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the route.\nCause: " + ex.Message;
                _log.Error("Failed to delete route.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> RouteActivateAsync(Guid routeId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/routeactivate/{0}", routeId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the route\nCause: " + ex.Message;
                _log.Error("Error: An error occurred when deleting the route.", ex);
            }
            return _response;
        }

        public async Task<List<UserItem>> UserListAsync(Guid distributorId)
        {
            List<UserItem> userItems = null;
            string url = string.Format("api/distributorservices/userlist/{0}", distributorId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                userItems = await response.Content.ReadAsAsync<List<UserItem>>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to retrieve user list for distributor id " + distributorId);
            }

            return userItems;

        }

        public async Task<ResponseBool> UserAddAsync(UserItem userItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/useradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, userItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when adding user.\n" + ex.Message;
                _log.Error("Error: An error occurred when adding user.", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> UserUpdateAsync(UserItem userItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/userupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, userItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to update user\nCause:" + ex.Message;
                _log.Error("Failed to update user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> UserDeactivateAsync(Guid userId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/userdeactivate/{0}", userId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to deactivate user\nCause:" + ex.Message;
                _log.Error("Failed to deactivate user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> UserActivateAsync(Guid userId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/useractivate/{0}", userId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to activate user\nCause:" + ex.Message;
                _log.Error("Failed to activate user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> UserDeleteAsync(Guid userId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/userdelete/{0}", userId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to delete user\nCause:" + ex.Message;
                _log.Error("Failed to delete user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanAddAsync(DistributorSalesmanItem distributorSalesmanItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, distributorSalesmanItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add salesman", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanUserAddAsync(DistributorSalesmanItem distributorSalesmanItem,
            UserItem userItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanuseradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                SalesmanAddDTO dto = new SalesmanAddDTO { Salesman = distributorSalesmanItem, User = userItem };
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add salesman /user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanUpdateAsync(DistributorSalesmanItem distributorSalesmanItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, distributorSalesmanItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update salesman", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanDeactivateAsync(Guid salesmanId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmandeactivate/{0}", salesmanId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanRoutesUpdateAsync(List<DistributorSalesmanRouteItem> routes)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanroutesupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, routes);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update salesman", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanActivateAsync(Guid salesmanId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanactivate/{0}", salesmanId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanDeleteAsync(Guid salesmanId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmandelete/{0}", salesmanId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to delete user", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanRouteDeactivateAsync(Guid salesmanRouteId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanroutedeactivate/{0}", salesmanRouteId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate salesman route", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanRouteActivateAsync(Guid salesmanRouteId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanrouteactivate/{0}", salesmanRouteId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to activate salesman route", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanRouteDeleteAsync(Guid salesmanRouteId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/salesmanroutedelete/{0}", salesmanRouteId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to delete salesman route", ex);
                _response.ErrorInfo = "Failed to delete salesman route.\n" +
                                      ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkAddAsync(PurchasingClerkItem purchasingClerkItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, purchasingClerkItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when adding purchasing clerk.\n" + ex.Message;
                _log.Error("Error: An error occurred when adding purchasing clerk.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the purchasing clerk.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the purchasing clerk.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to delete commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkRouteAddAsync(
            List<PurchasingClerkRouteItem> purchasingClerkRouteItems)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkrouteadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, purchasingClerkRouteItems);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update purchasing clerk routes", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkRouteActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkrouteactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the purchasing clerk.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit purchasing clerk.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> PurchasingCerkRouteDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/purchasingclerkroutekdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the purchasing clerk.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit purchasing clerk.", ex);
            }
            return _response;
        }

        public async Task<List<ContactItem>> ContactListAsync(Guid contactOwnerId)
        {
            List<ContactItem> userItems = null;
            string url = string.Format("api/distributorservices/contactlist/{0}", contactOwnerId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                userItems = await response.Content.ReadAsAsync<List<ContactItem>>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to retrieve user list for distributor id " + contactOwnerId);
            }

            return userItems;
        }

        public async Task<ResponseBool> ContactsAddAsync(List<ContactItem> contactItems)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/contactsadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, contactItems);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Error: An error occurred when saving the contact.", ex);
                _response.ErrorInfo = "Error: An error occurred when saving the contact.\n" +
                                      ex.Message;
            }

            return _response;
        }

        public async Task<ResponseBool> ContactUpdateAsync(ContactItem contactItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/contactupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, contactItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();


            }
            catch (Exception ex)
            {
                _log.Error("Failed to update contacts", ex);
                _response.ErrorInfo = "Error: An error occurred when saving the contact.\n" +
                                      ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> ContactDeactivateAsync(Guid contactId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/contactdeactivate/{0}", contactId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when saving the contact.\n" + ex.Message;
                _log.Error("Failed to save contact(s)", ex);
            }

            return _response;
        }

        public async Task<ResponseBool> ContactActivateAsync(Guid contactId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/contactactivate/{0}", contactId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when activating the contact.\n" + ex.Message;
                _log.Error("Failed to activate contact(s)", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ContactDeleteAsync(Guid contactId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/contactdelete/{0}", contactId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the contact.\n" + ex.Message;
                _log.Error("Failed to delete contact(s)", ex);
            }

            return _response;
        }

        public async Task<List<BankItem>> BankListAsync()
        {
            List<BankItem> bankItems = null;
            string url = "api/distributorservices/banklist/";
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                bankItems = await response.Content.ReadAsAsync<List<BankItem>>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to retrieve bank list  ");
            }

            return bankItems;
        }

        public async Task<ResponseBool> BankAddAsync(BankItem bankItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/bankadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, bankItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add bank", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> BankUpdateAsync(BankItem bankItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/bankupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, bankItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update bank", ex);
            }
            return _response;
        }

        public async Task<List<BankBranchItem>> BankBranchListAsync(Guid bankId)
        {
            List<BankBranchItem> bankItems = null;
            string url = string.Format("api/distributorservices/bankbranchlist/{0}", bankId);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                bankItems = await response.Content.ReadAsAsync<List<BankBranchItem>>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to retrieve bank list  ");
            }

            return bankItems;
        }

        public async Task<ResponseBool> BankBranchUpdateAsync(BankBranchItem bankBranchItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            bool success = false;
            string url = string.Format("api/distributorservices/bankbranchupdate");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, bankBranchItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
                if (response.IsSuccessStatusCode)
                    success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update bank branch", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> BankBranchAddAsync(BankBranchItem bankBranchItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/bankbranchadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, bankBranchItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _log.Error("Failed to add bank branch", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommoditySupplierAddAsync(CommoditySupplier commoditySupplier)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commoditysupplieradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commoditySupplier);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommoditySupplierAddAsync(CommoditySupplierDTO commoditySupplierdto)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/commoditysupplier/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commoditySupplierdto);
                var _responseBasic = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_responseBasic != null && _responseBasic.ErrorInfo == "OK")
                {
                    _response = new ResponseBool { Success = true };

                }
                else
                {
                    _response = new ResponseBool { Success = false, ErrorInfo = _responseBasic.ErrorInfo ?? "" };
                }
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity supplier.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommoditySupplierActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commoditysupplieractivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity supplier.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommoditySupplierDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commoditysupplierdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the commodity supplier.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to delete commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerAddAsync(CommodityProducerDTO commodityProducerdto)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/commodityproducer/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityProducerdto);
                var _responseBasic = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_responseBasic != null && _responseBasic.ErrorInfo == "OK")
                {
                    _response = new ResponseBool { Success = true };

                }
                else
                {
                    _response = new ResponseBool { Success = false, ErrorInfo = _responseBasic.ErrorInfo ?? "" };
                }
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity supplier.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerAddAsync(CommodityProducer commodityProducer)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityproduceradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityProducer);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update commodity producer.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerListAddAsync(List<CommodityProducer> commodityProducerList)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityproducerlistadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityProducerList);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update commodity producers.", ex);
            }
            return _response;
            throw new NotImplementedException();
        }

        public async Task<ResponseBool> CommodityProducerActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityproduceractivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity producer.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity producer.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityproducerdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the commodity producer.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to delete commodity producer.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityOwnerAddAsync(CommodityOwnerDTO commodityOwnerdto)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/commodityowner/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityOwnerdto);
                var _responseBasic = await response.Content.ReadAsAsync<ResponseBasic>();
                if (_responseBasic != null && _responseBasic.ErrorInfo == "OK")
                {
                    _response = new ResponseBool { Success = true };

                }
                else
                {
                    _response = new ResponseBool { Success = false, ErrorInfo = _responseBasic.ErrorInfo ?? "" };
                }
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity supplier.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to edit commodity supplier.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityOwnerAddAsync(CommodityOwnerItem commodityOwnerItem)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityowneradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityOwnerItem);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update commodity owner.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityOwnerListAddAsync(List<CommodityOwnerItem> commodityOwnerItemList)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityownerlistadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, commodityOwnerItemList);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update commodity owner.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityOwnerActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityowneractivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the commodity owner.\nCause: " + ex.Message;
                _log.Error("Failed to edit commodity owner.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityOwnerDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/commodityownerdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the commodity owner.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to delete commodity owner.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CentreAddAsync(Centre centre)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/centreadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, centre);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update centre.\nCause:", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CentreActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/centreactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the centre.\nCause: " + ex.Message;
                _log.Error("Failed to edit centre.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CentreDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/centredelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the centre .\nCause: " + ex.Message;
                _log.Error("Failed to delete centre.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> StoreAddAsync(Store store)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/storeadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, store);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update store.\nCause:", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> StoreActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/storeactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the store.\nCause: " + ex.Message;
                _log.Error("Failed to edit centre.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> StoreDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/storedelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the store.\nCause: " + ex.Message;
                _log.Error("Failed to delete store.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ContainerAddAsync(SourcingContainer container)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/containeradd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, container);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update container.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ContainerActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/containeractivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the container.\nCause: " + ex.Message;
                _log.Error("Failed to edit container.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ContainerDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/containerdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the container.\nCause: " + ex.Message;
                _log.Error("Failed to delete container.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> EquipmentAddAsync(Equipment equipment)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/equipmentadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, equipment);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update equipment.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> EquipmentActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/equipmentactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the equipment.\nCause: " + ex.Message;
                _log.Error("Failed to edit equipment.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> EquipmentDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/equipmentdelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the equipment.\nCause: " + ex.Message;
                _log.Error("Failed to delete equipment.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> VehicleAddAsync(Vehicle vehicle)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/vehicleadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, vehicle);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update store.\nCause:", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> VehicleActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/vehicleactivateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the vehicle.\nCause: " + ex.Message;
                _log.Error("Failed to edit vehicle.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> VehicleDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/vehicledelete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the vehicle.\nCause: " + ex.Message;
                _log.Error("Failed to delete vehicle.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SupplierMappingSaveAsync(CostCentreMapping mapping)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/commoditysuppliermapping");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, mapping);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save supplier outlet Mapping.\n" + ex.Message;
                _log.Error("Failed tosave supplier outlet Mapping", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SalesmanSupplierSaveAsync(List<SalesmanSupplierDTO> salesmanSuppliers)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };


            string url = string.Format("api/pushmasterdata/salesmansupplier/save");
            var httpClient = setupHttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, salesmanSuppliers);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save salesmansupplier DTO.\n" + ex.Message;
                _log.Error("Failed to save salesmansupplier DTO", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> InfectionSaveAsync(Infection infection)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            var dto = _masterDataToDTOMapping.Map(infection);
            if (dto == null)
                return _response;

            string url = string.Format("api/pushmasterdata/infection/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save infection DTO.\n" + ex.Message;
                _log.Error("Failed to save infection DTO", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> InfectionDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/infection/delete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the infection.\nCause: " + ex.Message;
                _log.Error("Failed to delete infection.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> InfectionActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/infection/activateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the infection.\nCause: " + ex.Message;
                _log.Error("Failed to editing infection.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> SeasonSaveAsync(Season season)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            var dto = _masterDataToDTOMapping.Map(season);
            if (dto == null)
                return _response;

            string url = string.Format("api/pushmasterdata/season/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save season DTO.\n" + ex.Message;
                _log.Error("Failed to save season DTO", ex);
            }
            return _response;

        }

        public async Task<ResponseBool> SeasonDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/season/delete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the season.\nCause: " + ex.Message;
                _log.Error("Failed to delete infection.", ex);
            }
            return _response;
        }



        public async Task<ResponseBool> SeasonActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/season/activateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the season.\nCause: " + ex.Message;
                _log.Error("Failed to editing season.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerServiceSaveAsync(
            CommodityProducerService commodityProducerService)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            var dto = _masterDataToDTOMapping.Map(commodityProducerService);
            if (dto == null)
                return _response;

            string url = string.Format("api/pushmasterdata/commodityproducerservice/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save Commodity Producer Service DTO.\n" + ex.Message;
                _log.Error("Failed to save Commodity Producer Service DTO", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerServiceDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/commodityproducerservice/delete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo =
                    "Error: An error occurred when deleting the commodity producer service.\nCause: " + ex.Message;
                _log.Error("Failed to delete commodity producer service.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> CommodityProducerServiceActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/commodityproducerservice/activateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the Commodity Producer Service.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to editing Commodity Producer Service.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ShiftSaveAsync(Shift shift)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            var dto = _masterDataToDTOMapping.Map(shift);
            if (dto == null)
                return _response;

            string url = string.Format("api/pushmasterdata/shift/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save shift DTO.\n" + ex.Message;
                _log.Error("Failed to save shift DTO", ex);
            }
            return _response;

        }

        public async Task<ResponseBool> ShiftDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/shift/delete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the shift.\nCause: " + ex.Message;
                _log.Error("Failed to delete shift.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ShiftActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/shift/activateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the shift.\nCause: " + ex.Message;
                _log.Error("Failed to editing shift.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ServiceProviderSaveAsync(ServiceProvider serviceProvider)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            var dto = _masterDataToDTOMapping.Map(serviceProvider);
            if (dto == null)
                return _response;

            string url = string.Format("api/pushmasterdata/serviceprovider/save");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, dto);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to save service provider DTO.\n" + ex.Message;
                _log.Error("Failed to save service provider DTO", ex);
            }
            return _response;

        }

        public async Task<ResponseBool> ServiceProviderDeleteAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/serviceprovider/delete/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the Service Provider.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to delete Service Provider.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> ServiceProviderActivateOrDeactivateAsync(Guid id)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/pushmasterdata/serviceprovider/activateordeactivate/{0}", id);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when editing the Service Provider.\nCause: " +
                                      ex.Message;
                _log.Error("Failed to editing Service Provider.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> MasterDataAllocationAddAsync(List<MasterDataAllocation> masterDataAllocationList)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/masterdataallocationadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, masterDataAllocationList);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update allocation.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> MasterDataAllocationDeleteAsync(List<Guid> ids)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
            string url = string.Format("api/distributorservices/masterdataallocationdelete/{0}", ids);
            var httpClient = setupHttpClient();
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, ids);
                _response = await response.Content.ReadAsAsync<ResponseBool>();
            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Error: An error occurred when deleting the allocation.\nCause: " + ex.Message;
                _log.Error("Failed to delete allocation.", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> OutletPriorityAddAsync(List<OutletPriorityItem> outletPriorityItems)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/outletpriorityadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, outletPriorityItems);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _response.ErrorInfo = "Failed to add outlet priority.\n" + ex.Message;
                _log.Error("Failed to add outlet priority", ex);
            }
            return _response;
        }

        public async Task<ResponseBool> OutletPriorityDeactivateAsync(Guid outletId)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/outletprioritydeactivate/{0}", outletId);
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.GetAsync(url);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _log.Error("Failed to deactivate priority", ex);
                _response.ErrorInfo = "Failed to deactivate priority\n" + ex.Message;
            }
            return _response;
        }

        public async Task<ResponseBool> OutletVisitAddAsync(List<OutletVisitDayItem> visitDays)
        {
            ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };

            string url = string.Format("api/distributorservices/outletvisitdayadd");
            var httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, visitDays);
                _response = await response.Content.ReadAsAsync<ResponseBool>();

            }
            catch (Exception ex)
            {
                _log.Error("Failed to add outletvisit", ex);
                _response.ErrorInfo = "Failed to add outletvisit \n" + ex.Message;
            }
            return _response;
        }

       
      



       
        #region AgriUser

        public Task<List<UserItem>> AgriUserListAsync(Guid hubId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBool> AgriUserAddAsync(UserItem user, ContactItem contact, List<RouteItem> routes)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBool> AgriUserDeactivateAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBool> AgriUserActivateAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBool> AgriUserDeleteAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
