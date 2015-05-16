using System;
using System.Configuration;
using Distributr.Core;
using Distributr.Core.ClientApp;

namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public class Config : BasicConfig
    {
       
        public string CostCentreApplicationDescription { get; set; }

        public bool IsApplicationInitialized { get; set; }
        public DateTime DateInitialized { get; set; }

        public string WebServiceUrl { get; set; }
        public int ApplicationStatus { get; set; }
        public VirtualCityApp AppId { get; set; }

        public static readonly string UserKey = "cui_{0}";


         public string DbEndpoint { get; set; }
        public string Key { get; set; }
        public string DbName { get; set; }
        public string VAOKey { get; set; }
        public string WREndPoint { get; set; }
        public static Config Init()
        {
            return new Config
            {
                DbEndpoint = ConfigurationManager.AppSettings["dbendpoint"],
                DbName = ConfigurationManager.AppSettings["dbname"],
                Key = ConfigurationManager.AppSettings["key"],
                VAOKey = ConfigurationManager.AppSettings["vaoadminkey"],
                WREndPoint = ConfigurationManager.AppSettings["wrendpoint"]
            };

        }

        public static string ApiUri
        {
            get { return ConfigurationManager.AppSettings["API_URI"]; }

        }

        public static string AdminKey
        {
            get { return ConfigurationManager.AppSettings["ADMIN_KEY"]; }

        }
    }
    public class ClientApplication
    {
        public Guid Id { get; set; }
        public string HostName { get; set; }
        public bool CanSync { get; set; }
        public DateTime DateInitialized { get; set; }
        public VirtualCityApp AppId { get; set; }
    }
}
