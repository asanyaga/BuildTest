using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;

namespace Distributr.BusSubscriber.Service
{
    public class BusBuilder
    {
        public static IAdvancedBus CreateMessageBus()
        {
            return RabbitHutch.CreateBus("host=localhost").Advanced;
        }
    }
}
