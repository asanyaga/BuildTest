using System;
using Distributr.Core;
using Distributr.Core.Domain.Master;
using Sqo;

namespace Distributr.WPF.Lib.Impl.Model.Utility
{
    public class ConfigLocal 
    {
        public int Id { get; set; }
        public Guid CostCenterId { get; set; }
        public Guid CostCentreApplicationId { get; set; }
        public string CostCentreApplicationDescription { get; set; }
        public bool IsApplicationInitialized { get; set; }
        public DateTime DateInitialized { get; set; }
        public string WebServiceUrl { get; set; }
        public long LastDeliveredCommandRouteItemId { get; set; }
        public int ApplicationStatus { get; set; }
        public int VirtualCityApp { get; set; }
        
    }
    public class ClientApplicationLocal
    {
        public int Id { get; set; }
        public Guid ClientAppId { get; set; }
        public string HostName { get; set; }
        public bool CanSync { get; set; }
        public DateTime DateInitialized { get; set; }
        public int AppTypeId { get; set; }
    }
    public class SyncTrackerLocal
    {
        public Guid Id { get; set; }
        public string  EntityName { get; set; }
        public DateTime LastSyncDateTime { get; set; }
        public int LastPage { get; set; }
        
    }
}
