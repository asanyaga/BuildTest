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
        private const string LOGIN_ENDPOINT = "Login/LoginGet";

        private const string COST_CENTRE_APPLICATION_ID_ENDPOINT = "CostCentreApplication/GetCreateCostCentreApplication";

        private readonly LoginSettingsRepository repository;

        public LoginClient(LoginSettingsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<User>> Login(string username, string password)
        {
            var settings = repository.GetSettings();

            using (var client = new HttpClient())
            {
                client.Timeout = Timeouts.DefaultHttpTimeout();
                client.BaseAddress = new Uri(settings.ServerUrl);

                var costCentreId = await GetCostCentreId(username, password, client);

                if (costCentreId == null)
                {
                    return Result<User>.Failure(string.Format("Cannot get Cost Centre Id for User {0}", username));
                }

                var costCentreApplicationId = await GetCostCentreApplicationId(costCentreId, client);

                if (costCentreApplicationId == null)
                {
                    return
                        Result<User>.Failure(string.Format("Cannot get Cost Centre Application Id for User {0}", username));
                }

                var user = new User
                {
                    Username = username,
                    Password = password,
                    CostCentre = new Guid(costCentreId),
                    IsNewUser = true,
                    CostCentreApplicationId = costCentreApplicationId
                };

                return Result<User>.Success(user);
            }
        }

        private async Task<string> GetCostCentreId(string username, string password, HttpClient client)
        {
            var httpParams = new HttpParams();
            httpParams.Add("Username", username);
            httpParams.Add("Password", password);
            httpParams.Add("usertype", "DistributorSalesman");

            var response = await client.GetAsync(LOGIN_ENDPOINT + httpParams);
            var content = response.Content;

            var text = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CostCentreIdResult>(text).CostCentreId;
        }

        private async Task<string> GetCostCentreApplicationId(string costCentreId, HttpClient client)
        {
            var httpParams = new HttpParams();
            httpParams.Add("costCentreId", costCentreId);
            httpParams.Add("applicationDescription", "Android_Application");

            var response = await client.GetAsync(COST_CENTRE_APPLICATION_ID_ENDPOINT + httpParams);
            var content = response.Content;

            var text = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CostCentreApplicationIdResult>(text).CostCentreApplicationId;
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