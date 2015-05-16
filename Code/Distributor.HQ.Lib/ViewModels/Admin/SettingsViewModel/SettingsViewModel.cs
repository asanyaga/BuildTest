using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel
{
    public class SettingsViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Key is a Required Field!")]
        public int SettingKey { get; set; }

        public SettingsKeys Key { get; set; }
        [Required(ErrorMessage = "Value is a Required Field!")]
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public SelectList KeyList { get; set; }
    }
}
