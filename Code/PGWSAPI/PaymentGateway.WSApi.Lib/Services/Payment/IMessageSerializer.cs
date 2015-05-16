using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;

namespace PaymentGateway.WSApi.Lib.Services.Payment
{
    public interface IMessageSerializer
    {
        ClientRequestResponseBase DeserializeClientRequest(string requestType, string jsonRequest);
        ClientRequestResponseBase DeserializeSDPResponse(string jsonResponse, ClientRequestResponseType responseType);
        ServerRequestBase DeserializeSDPRequest(string requestType, string jsonRequest);
        string SerializeMessage(ClientRequestResponseBase enitity, ClientRequestResponseType entityType);
    }
}
