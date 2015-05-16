using System;
using Distributr.Core.ClientApp;

namespace Distributr.Mobile.Core
{
    public class GetDocumentReference : IGetDocumentReference
    {
        public string GetDocReference(string docType, string orderRef)
        {
            throw new NotImplementedException();
        }

        public string GetDocReference(string docType, Guid salesmanId, Guid outletId)
        {
            throw new NotImplementedException();
        }
    }
}