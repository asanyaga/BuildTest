using System;
using System.Collections.Generic;
using System.Configuration;
using Distributr.Core.Data.EF;

using StructureMap.Configuration.DSL;
using Distributr.WPF.Lib.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.WPF.Lib.Data.IOC.WPFDefaultServices;
using StructureMap;


namespace Distributr.WPF.Lib.Data.IOC
{
    public class RepositoryRegistry : Registry
    {
        /// <summary>
        /// Production default IOC setup
        /// </summary>
        public RepositoryRegistry()
        {
            //REPOSITORIES
            string currentPlatformType = "win";
            if (ConfigurationManager.AppSettings["PlatformType"] != null)
                currentPlatformType = ConfigurationManager.AppSettings["PlatformType"];
            string connectionString = ConfigurationManager.AppSettings["cokeconnectionstring"];
            if (currentPlatformType == "win")
            {
                For<CokeDataContext>()
                .Use<CokeDataContext>()
                 .Ctor<string>("connectionString")
                .Is(connectionString);
            }
            // Local set up DB
            string localConnectionString = ConfigurationManager.ConnectionStrings["DistributrLocalContext"].ConnectionString;
            For<DistributrLocalContext>().Use<DistributrLocalContext>().Ctor<string>("connectionString").Is(localConnectionString);
            For<ICacheProvider>().Use(DefaultCacheProvider.GetInstance());
            For<IDTOToEntityMapping>().Use<DTOToEntityMapping>();

            foreach (var item in WPFRepositoryDefaultServices.DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }

        }

        /// <summary>
        /// Facilitate manual configuration of container
        /// </summary>
        /// <param name="exp"></param>
        public static void ManualStructuremapSetup(IInitializationExpression exp, string hubConnectionString, string localConnectionString)
        {
            exp.For<CokeDataContext>()
                .Use<CokeDataContext>()
                 .Ctor<string>("connectionString")
                .Is(hubConnectionString);
            exp.For<DistributrLocalContext>().Use<DistributrLocalContext>().Ctor<string>("connectionString").Is(localConnectionString);
            //exp.For<ICacheProvider>().Use(DefaultCacheProvider.GetInstance());
            exp.For<IDTOToEntityMapping>().Use<DTOToEntityMapping>();

            foreach (var item in WPFRepositoryDefaultServices.DefaultServiceList())
            {
                exp.For(item.Item1).Use(item.Item2);
            }
        }


        
    }
}
