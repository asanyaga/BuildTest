using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Akavache;
using System.Timers;
using log4net;
using System.Diagnostics;
namespace MongoEnvelopeMigrate
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = log4net.LogManager.GetLogger("MAIN");
            var l = log.Logger;

            log.Info("Starting the migration application ==================================================");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                var settings = new AppSettings();
                BlobCache.ApplicationName = settings.LocalCache;
                int noDocumentIdsSaved = new SaveMongoCommandsToLocalCache().Go().Result;
                int noDocumentsProcessed = new MigrateDocumentCommands().Go().Result;
            }
            catch (AggregateException ex)
            {
                log.Error("Unhandled aggregate error");
                log.Error(ex.Flatten().Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            sw.Stop();
            log.InfoFormat("Total time taken to complete {0} seconds", sw.Elapsed.TotalSeconds);
            log.Info("Press any key to continue");
            Console.ReadLine();

        }

        static void SetupLogging()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

    }



}
