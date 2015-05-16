using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    [Obsolete("Command Envelope Refactoring")]
    public interface IIncomingCommandQueueRepository : ICommandQueueRepository<IncomingCommandQueueItemLocal>
    {
        List<IncomingCommandQueueItemLocal> GetUnProcessedCommands();
        void DeleteOldCommand();
        void MarkAsProcessed(Guid commandId);
        void IncrimentRetryCounter(Guid commandId);
        bool ExitOldCommand();

    }
}
