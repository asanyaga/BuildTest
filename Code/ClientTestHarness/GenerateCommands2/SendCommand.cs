using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace ClientTestHarness.GenerateCommands2
{
    public class SendCommand
    {
        public bool SendDocumentCommand(DocumentCommand command)
        {
            string serverUrl = ConfigurationManager.AppSettings["WSURL"];
            var client = new HttpClient();
            client.BaseAddress = new Uri(serverUrl  );
            HttpResponseMessage response = client.PostAsync("api/command/run", command, new JsonMediaTypeFormatter()).Result;
            return response.IsSuccessStatusCode;
        }
    }
}
