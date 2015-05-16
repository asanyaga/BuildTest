using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Agrimanagr.HQ.Areas.Agrimanagr.ViewModels
{
    public class ServiceProviderViewModel:MasterBaseDTO
    {
        [Required(ErrorMessage = "Code is a required field")]
        public string Code { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        [Required(ErrorMessage = "ID number is a required field")]
        public string IdNo { get; set; }
        [Required(ErrorMessage = "Pin number is a required field")]
        public string PinNo { get; set; }

        [Required(ErrorMessage = "Gender is a required field")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Bank is a required field")]
        public Guid BankId { get; set; }
        [Required(ErrorMessage = "Bank Branch is a required field")]
        public Guid BankBranchId { get; set; }

        public string Description { get; set; }
        [Required(ErrorMessage = "Mobile Number is a required field")]
        [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
        public string MobileNumber { get; set; }
    }
}