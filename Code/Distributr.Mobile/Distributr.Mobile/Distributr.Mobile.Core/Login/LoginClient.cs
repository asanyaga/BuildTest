using System;
using System.Net.Http;
using System.Threading.Tasks;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Login.Settings;
using Newtonsoft.Json;

namespace Distributr.Mobile.Login
{
    public class LoginClient : ILoginClient
    {
        public const string LoginEndpoint = "Login/LoginGet";
        public const string SuccessfulResponse = "Success";
        public const string CostCentreApplicationIdEndpoint = "CostCentreApplication/GetCreateCostCentreApplication";

        private readonly LoginSettingsRepository loginSettingsRepository;

        public LoginClient(LoginSettingsRepository loginSettingsRepository)
        {
            this.loginSettingsRepository = loginSettingsRepository;
        }

        public async Task<Result<User>> Login(string username, string password)
        {
            var settings = loginSettingsRepository.GetSettings();

            using (var client = new HttpClient())
            {
                client.Timeout = Timeouts.DefaultHttpTimeout();
                client.BaseAddress = new Uri(settings.ServerUrl);

                var costCentre = await GetCostCentreId(username, password, client);

                if (costCentre.ErrorInfo != SuccessfulResponse)
                {
                    return Result<User>.Failure(costCentre.ErrorInfo);
                }

                var costCentreApplicationId = await GetCostCentreApplicationId(costCentre.CostCentreId, client);

                if (costCentreApplicationId.ErrorInfo != SuccessfulResponse)
                {
                    return Result<User>.Failure(costCentreApplicationId.ErrorInfo);
                }
                
                var user = new User
                {
                    Username = username,
                    Password = password,
                    CostCentre = new Guid(costCentre.CostCentreId),
                    IsNewUser = true,
                    CostCentreApplicationId = costCentreApplicationId.CostCentreApplicationId
                };

                return Result<User>.Success(user);
            }
        }

        private async Task<CostCentreIdResult> GetCostCentreId(string username, string password, HttpClient client)
        {
            var httpParams = new HttpParams
            {
                {"Username", username},
                {"Password", password},
                {"usertype", "DistributorSalesman"}
            };

            var response = await client.GetAsync(LoginEndpoint + httpParams);
            var content = response.Content;

            var text = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CostCentreIdResult>(text);
        }

        private async Task<CostCentreApplicationIdResult> GetCostCentreApplicationId(string costCentreId, HttpClient client)
        {
            var httpParams = new HttpParams
            {
                {"costCentreId", costCentreId},
                {"applicationDescription", "Android_Application"}
            };

            var response = await client.GetAsync(CostCentreApplicationIdEndpoint + httpParams);
            var content = response.Content;

            var text = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CostCentreApplicationIdResult>(text);
        }
    }

    internal class CostCentreApplicationIdResult
    {
        public string CostCentreApplicationId { get; set; }
        public string ErrorInfo { get; set; }
    }

    internal class CostCentreIdResult
    {
        public string CostCentreId { get; set; }
        public string ErrorInfo { get; set; }
    }
}