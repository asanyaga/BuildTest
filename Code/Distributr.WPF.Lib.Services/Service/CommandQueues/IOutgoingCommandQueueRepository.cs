using System.Collections.Generic;
using Distributr.Core.Notifications;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public interface IOutgoingCommandQueueRepository : ICommandQueueRepository<OutgoingCommandQueueItemLocal>
    {
        List<OutgoingCommandQueueItemLocal> GetUnSentCommands();
        void MarkCommandAsSent(int OID);
        OutgoingCommandQueueItemLocal UpdateSerializedObjectWithOID(int commandOID);
        OutgoingCommandQueueItemLocal GetFirstUnSentCommand();
       void DeleteOldCommand(int OID);
        
    }

    public interface IOutgoingNotificationQueueRepository
    {
        List<OutGoingNotificationQueueItemLocal> GetUnSent();
        void MarkAsSent(int OID);
        OutGoingNotificationQueueItemLocal GetFirstUnSent();
        void Add(NotificationBase notification);


    }
    
}
