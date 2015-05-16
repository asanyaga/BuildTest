using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPUtilityLib.Masterdata;
using SAPUtilityLib.Masterdata.Impl;
using StructureMap.Configuration.DSL;

namespace SAPUtilityLib.IoC
{
    public class SAPUtilsRegistry : Registry
    {
        public SAPUtilsRegistry()
        {
            For<IPullMasterdataService>().Use<PullMasterdataService>();
        }
    }
}
