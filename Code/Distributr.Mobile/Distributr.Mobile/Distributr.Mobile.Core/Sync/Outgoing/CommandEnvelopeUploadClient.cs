using System;
using System.Net.Http;
using System.Text;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login.Settings;
using Newtonsoft.Json;

namespace Distributr.Mobile.Core.Sync.Outgoing
{
    public class CommandEnvelopeUploadClient : ICommandEnvelopeUploadClient
    {
        public const string OutgoingEnvelopeEndpoint = "commandenvelope/run";
        private const string OutgoingEnvelopeEndpointContentType = "application/json";

        private readonly LoginSettingsRepository loginSettingsRepository;

        public CommandEnvelopeUploadClient(LoginSettingsRepository loginSettingsRepository)
        {
            this.loginSettingsRepository = loginSettingsRepository;
        }

        public UploadEnvelopeResponse UploadCommandEnvelope(LocalCommandEnvelope localCommandEnvelope)
        {
            var settings = loginSettingsRepository.GetSettings();

            using (var client = new HttpClient())
            {
                client.Timeout = Timeouts.DefaultHttpTimeout();
                client.BaseAddress = new Uri(settings.ServerUrl);

                var response = client.PostAsync(OutgoingEnvelopeEndpoint, CreateContent(localCommandEnvelope)).Result;

                var text = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<UploadEnvelopeResponse>(text);
            }            
        }

        private StringContent CreateContent(LocalCommandEnvelope localCommandEnvelope)
        {
            return new StringContent(localCommandEnvelope.Contents, Encoding.UTF8, OutgoingEnvelopeEndpointContentType);
        }
    }
}
