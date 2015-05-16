using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PaymentGateway.WSApi.Lib.Domain.MasterData
{
   public class User:MasterEntity
    {
       [Display(Name="Username")]
       [Required(ErrorMessage="Username is required")]
       public string Username { set; get; }

       [Display(Name = "Full Names")]
       [Required(ErrorMessage = "Full Names is required")]
       public string FullName { set; get; }

       [Display(Name = "Email")]
       [Required]
       [DataType(DataType.EmailAddress,ErrorMessage = "Email is required")]
       public string Email { set; get; }

       [Display(Name = "Phone Number")]
       [Required(ErrorMessage = "Phone Number is required")]
       public string PhoneNo { set; get; }

       [Display(Name = "Last Login")]
       [Required(ErrorMessage = "Last Login is required")]
       public DateTime LastLogin { set; get; }

       [Display(Name = "Password")]
       [Required(ErrorMessage = "Password is required")]
       public string Password { set; get; }
       
       
       public bool HasChangePassword { get; set; }
    }
}
