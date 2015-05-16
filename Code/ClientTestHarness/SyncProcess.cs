using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace ClientTestHarness
{
    public class SyncProcess
    {
        public static int RunUploadCommands(int noCommands)
        {
            IConfigService configService = Services.Using<IConfigService>();
            ISendPendingLocalCommandsService sendPendingLocalCommandsService = Services.Using<ISendPendingLocalCommandsService>();
            Guid appId = configService.Load().CostCentreApplicationId;
            int nocommands = sendPendingLocalCommandsService.SendPendingCommandsAsync(noCommands).Result;

            Console.WriteLine("Sent {0} commands", nocommands);
            return nocommands;
        }
    }
}
