using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.WPF.Lib.Data.IOC;
using StructureMap;

namespace ClientTestHarness
{
    public class Services
    {
        public static void Setup()
        {
            DTOToEntityMapping.SetupAutomapperMappings();
            ObjectFactory.Initialize(x =>
                {
                    x.AddRegistry<WPFRegistry>();
                    x.AddRegistry<RepositoryRegistry>();
                    x.AddRegistry<ServiceRegistry>();
                    x.AddRegistry<CommandHandlerRegistry>();
                    x.AddRegistry<WorkflowRegistry>();
                });

        }

        public static T Using<T>() where T : class
        {
           return  ObjectFactory.Container.GetInstance<T>();
        }

    }
}
