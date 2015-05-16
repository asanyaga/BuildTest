using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Integration.Cussons.WPF.Lib.ExportService
{
   public class DistributrIntegrationService : IDistributrIntegrationService
    {
         readonly Timer _timer;
         public DistributrIntegrationService()
         {
             _timer = new Timer(500) {AutoReset = true};
             _timer.Elapsed += (sender, eventArgs) =>Start();
         }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
