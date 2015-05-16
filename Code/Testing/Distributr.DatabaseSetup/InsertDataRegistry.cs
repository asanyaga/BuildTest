using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;
using Distributr.DatabaseSetup;

namespace RebuildDB
{
    public class InsertDataRegistry : Registry
    {

        public InsertDataRegistry()
        {

            //For<IInsertTestData>().Use<InsertPHDTestData>();
            For<IInsertTestData>().Use<InsertTestData>();
         //For<IInsertTestData>().Use<InsertDeploymentData>();
        }
    }
}
