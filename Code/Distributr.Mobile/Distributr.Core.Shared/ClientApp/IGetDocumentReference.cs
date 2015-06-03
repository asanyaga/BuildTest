using System;

namespace Distributr.Core.ClientApp
{
    public interface IGetDocumentReference
    {
        //string GetDocReference(string docType, string salesmanUserName, string outletCode);
        string GetDocReference(string docType, string orderRef);
        string GetDocReference(string docType, Guid salesmanId, Guid outletId);
    }
}
