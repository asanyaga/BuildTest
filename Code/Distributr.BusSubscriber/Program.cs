using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.BusSubscriber;
using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.Script;
using Distributr.WSAPI.Lib.Services.Bus;
using EasyNetQ;
using EasyNetQ.Topology;
using StructureMap;
using Topshelf;
using log4net;
using System.Configuration;
using System.Diagnostics;

namespace Distributr.BusSubscriber
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger("Subscription Logger");

        static void Main(string[] args)
        {
            _log.Info("Entered Main in Subscriber Program/Service");
            new Bootstrap().Init();
            DistributrDataHelper.Migrate();

           
            _log.Info("Bootstrap complete");
            
            HostFactory.Run(x =>
                {
                   
                x.Service<IDistributrSubscriberService>(s =>
                {
                    
                    s.ConstructUsing(name =>
                                         {
                                             IDistributrSubscriberService service = null;
                                            
                                             try
                                             {
                                                 service = ObjectFactory.GetInstance<IDistributrSubscriberService>();
                                              
                                             }
                                             catch(Exception ex)
                                             {
                                                 _log.Error("Failed to load IOC", ex);
                                             }
                                             return service;
                                         });
                    s.WhenStarted(tc =>
                                      {
                                          _log.Info("Starting.......");
                                          tc.Start();
                                      });
                    s.WhenStopped(tc =>
                    {
                        tc.Stop();
                        _log.Info("Ending Bus Subscriber");
                        ObjectFactory.Container.Dispose();
                    });
                });

                x.RunAsLocalSystem();
                
                x.SetDescription(ConfigurationManager.AppSettings["ServiceDescription"]);
                x.SetDisplayName(ConfigurationManager.AppSettings["ServiceDisplayName"]);
                x.SetServiceName(ConfigurationManager.AppSettings["ServiceName"]);
            });
        }
    }
}
