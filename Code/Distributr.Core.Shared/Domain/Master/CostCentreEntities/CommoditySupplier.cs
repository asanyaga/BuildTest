using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum  CommoditySupplierType{Individual=0,Cooperative=1,Default=2}
#if !SILVERLIGHT
   [Serializable]
#endif
   public class CommoditySupplier: CostCentre
    {
       public CommoditySupplier(Guid id)
           : base(id)
       {
           CostCentreType = CostCentreEntities.CostCentreType.CommoditySupplier;
       }

       public CommoditySupplier(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive) : base(id, dateCreated, dateLastUpdated, isActive)
       {
           CostCentreType = CostCentreEntities.CostCentreType.CommoditySupplier;
       }
       public CommoditySupplierType CommoditySupplierType { get; set; }
       public DateTime JoinDate { get; set; }
       [Required(ErrorMessage="Account Number is Required")]
    
       public string AccountNo { get; set; }
       public string PinNo { get; set; }
		#if !__ANDROID__ 
       [DataMember(IsRequired=true)]
       #endif
		[Required(ErrorMessage = "Bank is Required")]
       public Guid BankId { get; set; }

       [Required(ErrorMessage = "Bank Branch is Required")]
       public Guid BankBranchId { get; set; }

       [Required(ErrorMessage = "Account Name is Required")]
       public string AccountName { get; set; }
    }
}
