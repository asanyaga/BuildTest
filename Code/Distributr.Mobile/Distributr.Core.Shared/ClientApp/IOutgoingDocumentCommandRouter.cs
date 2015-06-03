using System;
using Distributr.Core.Commands;

namespace Distributr.Core.ClientApp
{
    public interface IOutgoingDocumentCommandRouter
    {
        void RouteDocumentCommand(ICommand command);

        [Obsolete("Code should be refactored to use RouteDocumentCommand - ")]
        void RouteDocumentCommandWithOutSave(ICommand command);

    }
}
