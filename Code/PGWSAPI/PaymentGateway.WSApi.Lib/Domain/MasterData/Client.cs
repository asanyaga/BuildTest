using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.WSApi.Lib.Domain.MasterData
{
    public class Client : MasterEntity
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }

        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required")]
        public string Code { set; get; }

        [Display(Name = "Path")]
        [Required(ErrorMessage = "Path is required")]
        public string Path { set; get; }


        [Display(Name = "Application Id")]
        [Required(ErrorMessage = "ApplicationId is required")]
        public string ApplicationId { set; get; }

        [Display(Name = "Application Password")]
        [Required(ErrorMessage = "Application Password is required")]
        public string ApplicationPassword { set; get; }

        public string ExternalId { get; set; }
    }
}