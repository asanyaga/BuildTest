using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel
{
    public class CoolerViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "CoolerType is a Required Field!")]
        public string CoolerType { get; set; }
        [Required(ErrorMessage = "CoolerType is a Required Field!")]
        public Guid CoolerTypeId { get; set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Capacity is a Required Field!")]/*Issues 2626: No required field*/
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Capacity { get; set; }
        [Required(ErrorMessage = "Serial Number is a Required Field!")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string SerialNo { get; set; }
        [Required(ErrorMessage = "Asset Number is a Required Field!")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string AssetNo { get; set; }
        public bool IsActive { get; set; }
    }
}
