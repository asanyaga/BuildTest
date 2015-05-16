using System;
using System.ComponentModel.DataAnnotations;
namespace Distributr.Core.Domain.Master.SettingsEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class AppSettings : MasterEntity
    {
       public AppSettings() : base(default(Guid)) { }

       public AppSettings(Guid id) : base(id) { }
       public AppSettings(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}

       [Required(ErrorMessage = "Key is required")]
       public SettingsKeys Key { get; set; }

       [Required(ErrorMessage = "Value is required")]
       public string Value { get; set; }

       public string VirtualCityAppName { get; set; }
    }
}
