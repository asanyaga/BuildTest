using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel
{
    public class RetireSettingViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "RetireType is a Required Field!")]
        public int RetireTypeId { get; set; }
        public string RetireTypeName { get; set; }
        [Required(ErrorMessage = "Duration is a Required Field!")]
        public int Duration { get; set; }
        public bool IsActive { get; set; }
       
    }
}
