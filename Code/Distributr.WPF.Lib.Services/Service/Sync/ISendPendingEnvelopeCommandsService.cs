using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public interface ISendPendingEnvelopeCommandsService
    {
        Task<int> SendPendingEnvelopeCommandsAsync(int maxNoCommands = 100);
    }
}