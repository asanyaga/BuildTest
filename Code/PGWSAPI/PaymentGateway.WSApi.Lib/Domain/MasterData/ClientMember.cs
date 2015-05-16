using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.WSApi.Lib.Domain.MasterData
{
    public  enum  ClientMemberType
    {
        
        Distributor = 2,
        DistributorSalesman = 4,
        Outlet = 5,
        Hub = 8,
        CommoditySupplier = 9,
        PurchasingClerk = 10,
      
    }
    public class ClientMember : MasterEntity
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { set; get; }

        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required")]
        public string Code { set; get; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Type is required")]
        public ClientMemberType MemberType { set; get; }

        public Client Client { set; get; }
        public string ExternalId { get; set; }
    }
}