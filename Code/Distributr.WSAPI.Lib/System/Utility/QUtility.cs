using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Distributr.WSAPI.Lib.System.Utility;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.WSAPI.Utility
{
    public class QUtility
    {
        private static readonly ILog _log = LogManager.GetLogger("QUtility");
        
        public  void SetupTimer()
        {
            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                (state, timeout) =>
                {
                    ILog _log = LogManager.GetLogger("QUtility called");

                    try
                    {
                        QHealth.CheckQHealth();
                        if(QHealth.IsQueueHealthy)
                        {
                            new QAddRetryCommand().Go();
                        }

                    }
                    catch (Exception ex)
                    {
                        _log.Info("Failed to execute q timer delegate", ex);
                    }
                },
                null,
                TimeSpan.FromMinutes(1),
                false
            );
        }
    }
}