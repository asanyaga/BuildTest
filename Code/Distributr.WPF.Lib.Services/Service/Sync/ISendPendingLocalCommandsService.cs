using System;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public interface ISendPendingLocalCommandsService
    {
        Task<int> SendPendingCommandsAsync(int maxNoCommands = 100);

        Task<int> SendPendingNotificationAsync();
    }
}
