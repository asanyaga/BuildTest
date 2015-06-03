using System;
using System.Collections.Generic;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow
{
    public interface IRetireDocumentWFManager
    {
        void Submit(RetireDocumentCommand command, DocumentType documentType);
        RetireDocumentSetting GetSetting();
        List<Order> GetDeliveredOrders(int duration);
        List<Order> GetFullPaidOrder(int duration);
        void RetireDocument(Guid parentId);
        void RemoveOldInventoryDocument();
        List<ReturnsNote> GetClosedReturns(int duration);
        void RemoveClosedReturnsDocument(Guid parentId);
    }
}
