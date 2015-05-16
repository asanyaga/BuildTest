using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace PaymentGateway.WSApi.Lib.Repository.Payments.Response
{
    public interface IPaymentResponseRepository//<T> where T : class
    {
        Guid Save(ClientRequestResponseBase entity);
        ClientRequestResponseBase GetById(Guid id);
        IEnumerable<ClientRequestResponseBase> GetByTransRefId(Guid id);
        IEnumerable<ClientRequestResponseBase> GetAll();
    }
}
