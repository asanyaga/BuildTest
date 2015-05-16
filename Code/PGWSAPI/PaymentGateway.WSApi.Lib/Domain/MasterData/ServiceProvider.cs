using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.MasterData
{
   public class ServiceProvider : MasterEntity
    {
       [Display(Name = "SDP Application Id")]
       [Required(ErrorMessage = "SDP APP ID is required")]
       public string SdpAppId { set; get; }

       [Display(Name = "SDP Password")]
       [Required(ErrorMessage = "SDP Password is required")]
       public string SdpPassword { set; get; }

       [Display(Name = "Subscriber Id")]
       [Required(ErrorMessage = "Subscriber Id is required")]
       public string SubscriberId { set; get; }

       [Display(Name = "Currency")]
       [Required(ErrorMessage = "Currency is required")]
       public string Currency { set; get; }

       [Display(Name = "Allow Partial Payment")]
       [Required(ErrorMessage = "Allow Partial Payment is required")]
       public bool AllowPartialPayment { set; get; }

       [Display(Name = "Allow Over Payment")]
       [Required(ErrorMessage = "Allow Over Payment is required")]
       public bool AllowOverPayment { set; get; }

       [Display(Name = "Name")]
       [Required(ErrorMessage = "Name is required")]
       public string Name { set; get; }

       [Display(Name = "Code")]
       [Required(ErrorMessage = "Code is required")]
       public string Code { set; get; }

       [Display(Name ="Cost Centre Id" )]//DistributorCostCenterId"SID"
       [Required(ErrorMessage = "Cost Centre Id is required")]
       public string Sid { set; get; }

       [Display(Name = "SMS Short Code")]//DistributorCostCenterId"SID"
       [Required(ErrorMessage = "SMS Short Code is required")]
       public string SmsShortCode { set; get; }

       [Required(ErrorMessage = "Company Id")]
       public Guid SPCompanyId { get; set; }//For Distributor or Factory, this will be the parent cost centre Id

       //[Required(ErrorMessage = "Web Service Url")]
       //public string SPWSUrl { get; set; }

       //[Display (Name="Select Distributor")]
       //[Required(ErrorMessage="You must select a distributor first.")]
       public List<Distributor> Distributors { get; set; }

    }

    public class Distributor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
