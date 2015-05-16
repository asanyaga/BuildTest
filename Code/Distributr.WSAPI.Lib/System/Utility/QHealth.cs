using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RabbitMQ.Client;
using log4net;
using System.Configuration;

namespace Distributr.WSAPI.Utility
{
    public class QHealth
    {
        private static readonly ILog _log = LogManager.GetLogger("QUtility");
        static readonly object _locker = new object();
         static int _noItemsInQ;
        private static bool _isQueueAlive;
        public static int NoItemsInQ
        {
            get { return _noItemsInQ; }
            set
            {
                lock(_locker)
                {
                    _noItemsInQ = value;
                }
            }
        }

        public static bool IsQueueHealthy
        {
            get { return IsQueueAlive && NoItemsInQ < 10000; }
        }

        public static bool IsQueueAlive
        {
            get { return _isQueueAlive; }
            set
            {
                lock (_locker)
                {
                    _isQueueAlive = value;
                }
            }
        }

        public static void CheckQHealth()
        {
            string processingQ = "Q" + ConfigurationManager.AppSettings["MQName"];
            _log.Info("Check queue health");
            var connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            QueueDeclareOk queueDeclareOk = null;
           
            try
            {
                using (IConnection connection = connectionFactory.CreateConnection())
                {
                    using (IModel model = connection.CreateModel())
                    {
                        queueDeclareOk = model.QueueDeclare(processingQ, true, false, false, null);
                        IsQueueAlive = true;
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Error("Failed to read from Q", ex);
                IsQueueAlive = false;
            }
            if(queueDeclareOk == null)
            {
                IsQueueAlive = false;
                NoItemsInQ = 0;
            }
           
            NoItemsInQ = (int) queueDeclareOk.MessageCount;
            
        }

    }
}