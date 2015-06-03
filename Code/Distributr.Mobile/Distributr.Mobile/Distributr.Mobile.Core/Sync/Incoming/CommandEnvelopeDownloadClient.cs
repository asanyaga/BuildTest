using System;
using System.Net.Http;
using System.Text;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Login.Settings;
using Newtonsoft.Json;

namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class CommandEnvelopeDownloadClient
    {
        public const string NextEnvelopeEndpoint = "commandenveloperouting/GetNextEnvelopes";
        private const string NextEnvelopeEndpointContentType = "application/json";

        private readonly LoginSettingsRepository loginSettingsRepository;

        public CommandEnvelopeDownloadClient(LoginSettingsRepository loginSettingsRepository)
        {
            this.loginSettingsRepository = loginSettingsRepository;
        }

        public DownloadEnvelopesResponse DownloadCommandEnvelopes(DownloadEnvelopeRequest request)
        {
            var settings = loginSettingsRepository.GetSettings();

            using (var client = new HttpClient())
            {
                client.Timeout = Timeouts.DefaultHttpTimeout();
                client.BaseAddress = new Uri(settings.ServerUrl);

                var response = client.PostAsync(NextEnvelopeEndpoint, CreateContent(request)).Result;

                var text = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<DownloadEnvelopesResponse>(text);
            }              
        }

        private HttpContent CreateContent(DownloadEnvelopeRequest request)
        {
            return new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, NextEnvelopeEndpointContentType);
        }
    }
}
