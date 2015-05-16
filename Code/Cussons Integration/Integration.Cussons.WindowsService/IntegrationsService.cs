using System;
using Integration.Cussons.WPF.Lib.ExportService.Orders;
using Quartz;
using Quartz.Impl;
using StructureMap;
using log4net;
using Timer = System.Timers.Timer;

namespace Integration.Cussons.WindowsService
{
    public class IntegrationsService:IPzCussonsService
    {
        readonly Timer _timer;
        private readonly IOrderExportService _orderExportService=null;
        private static readonly ILog _log = LogManager.GetLogger("Distributr_Integration_Service Logger");
        private bool isOnMission = false;
      
        public IntegrationsService(IOrderExportService  orderExportService)
        {
            _timer = new Timer(120000) { AutoReset = true };
            _orderExportService=orderExportService;
            TestJob();
           _timer.Elapsed += (sender, eventArgs) =>
                                  {
                                     if (!isOnMission)
                                      {
                                          GetNextOrders();
                                          
                                          isOnMission = false;

                                      }
                                      
                                  };
        }
        public void Start() {
            _timer.Start();
            GetNextOrders();

        }

        public void Stop()
        {
            _timer.Stop();
            isOnMission = false;
            ObjectFactory.Container.Dispose();
            _log.Info(string.Format("Integration service stopped at {0}",DateTime.Now));
        }

        async void GetNextOrders()
        {
            try
            {

                Console.WriteLine("Getting Next orders from HQ=>{0} ", DateTime.Now);
                isOnMission = true;
                var itemsFound = await _orderExportService.GetAndExportOrders();

                Console.WriteLine("Files Fetched=>{0} as at {1} health Ok,Next HQ trip is at {2}", itemsFound,
                                  DateTime.Now.ToShortTimeString(),
                                  DateTime.Now.AddMilliseconds(_timer.Interval).ToShortTimeString());


            }
            catch (Exception ex)
            {
                _log.Info("Error=>" + ex.Message);
                Console.WriteLine("Error {0}", ex.Message);

            }
            finally
            {
                isOnMission = false;
            }
        }

        public static void TestJob()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler();

            IJobDetail jobDetail = JobBuilder.Create<MasterDataImportJob>()
                .WithIdentity("TestJob")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule("0 45 20 * * ?")
                .WithIdentity("TestTrigger")
                .StartNow()
                .Build();
            scheduler.ScheduleJob(jobDetail, trigger);
            scheduler.Start();
        }

    }

   
}
