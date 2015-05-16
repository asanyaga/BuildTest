using System;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public interface IReceiveAndProcessPendingRemoteCommandEnvelopesService
    {
        Task<bool> ReceiveAndProcessNextEnvelopesAsync(Guid costCentreApplicationId);
    

    }
    [Obsolete("Command Envelope Refactoring")]
    public interface IReceiveAndProcessPendingRemoteCommandsService
    {
        Task<bool> BatchReceiveAndProcessNextCommandAsync(Guid costCentreApplicationId);
        Task<bool> ProcessUnExecutedCommandAsync(Guid costCentreApplicationId);
  
    }
}
