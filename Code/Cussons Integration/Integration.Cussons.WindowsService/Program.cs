using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Integration.Cussons.WPF.Lib.ExportService;
using Integration.Cussons.WPF.Lib.ExportService.Orders;
using StructureMap;
using Topshelf;
using log4net;

namespace Integration.Cussons.WindowsService
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger("Distributr_Integration_Service Logger");
        static void Main(string[] args)
        {
            new BootStrapper().Init();
            HostFactory.Run(x =>
                                {
                                    var userName = ConfigurationManager.AppSettings["user"];
                                    var password = ConfigurationManager.AppSettings["Password"];
                                    x.RunAs(userName, password);
                                    x.UseLog4Net();
                                   

                x.Service<IntegrationsService>(s =>
                                                   {
                                                       var orderService =
                                                           ObjectFactory.GetInstance<IOrderExportService>();
                                                       s.ConstructUsing(name => new IntegrationsService(orderService));
                    
                    s.WhenStarted(tc =>
                                      {
                                          tc.Start();
                                          _log.Info("PZ Cussons Distributr Integration service");
                                          
                                      });
                    s.WhenStopped(tc =>
                                      {
                                          tc.Stop();
                                          _log.Info(string.Format("Distributr Integration Service,stopped at {0}",DateTime.Now));
                                          ObjectFactory.Container.Dispose();
                                      });
                });

               
               // x.RunAsLocalSystem();                           

                x.SetDescription("PZ Cussons Distributr Integration service");        
                x.SetDisplayName("Distributr Integration Service");                       
                x.SetServiceName("Distributr_Integration_Service");                       
            });      

            
            
        }
    }

}
