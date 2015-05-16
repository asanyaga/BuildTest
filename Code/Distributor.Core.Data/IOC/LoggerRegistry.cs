using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;
using log4net;

namespace Distributr.Core.Data.IOC
{
    class LoggerRegistry : Registry
    {
        public LoggerRegistry() {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
