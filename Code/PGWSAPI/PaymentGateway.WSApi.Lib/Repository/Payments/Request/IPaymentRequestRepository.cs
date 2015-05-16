using System;
using System.Collections.Generic;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;

namespace PaymentGateway.WSApi.Lib.Repository.Payments.Request
{
    public interface IPaymentRequestRepository//<T> where T : class
    {
        Guid Save(ClientRequestResponseBase entity);
        ClientRequestResponseBase GetById(Guid Id);
        IEnumerable<ClientRequestResponseBase> GetByTransactionRefId(string id);
        IEnumerable<ClientRequestResponseBase> GetByReceiptNumber(string receiptNumber);
        IEnumerable<ClientRequestResponseBase> GetAll();
        IEnumerable<ClientRequestResponseBase> GetAllTimedOutPayments(Guid serviceProviderId, DateTime startDate = new DateTime(), DateTime endDate = new DateTime());
    }
}
