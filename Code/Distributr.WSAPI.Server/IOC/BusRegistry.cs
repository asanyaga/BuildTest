using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.EasyNetQ;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using StructureMap.Configuration.DSL;

namespace Distributr.WSAPI.Server.IOC
{
    public class BusRegistry : Registry
    {
        public BusRegistry()
        {
            //choose bus
            bool useStubbedBus = true;
            bool.TryParse(ConfigurationSettings.AppSettings["UseStubbedBus"], out useStubbedBus);
            if (useStubbedBus)
                For<IBusPublisher>().Use<StubbedBusPublisher>();
            else
            {
                For<IBusPublisher>().Use<EasyNetQBusPublisher>()
                    .Ctor<string>("mqname")
                    .EqualToAppSetting("MQName");
                For<IControllerBusPublisher>().Use<EasyNetQBusPublisher>()
                    .Ctor<string>("mqname")
                    .EqualToAppSetting("MQName");
            }
            For<IBusSubscriber>().Use<MainBusSubscriber>();
        }
    }
}
