using System;
namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif

    public enum EagcContactType
    {
        DepositorType = 1,
        BuyerType = 2,
        VoucherAdminOrgType = 3,
        VoucherCentreClerkType = 4,
        VoucherCentreAdminType = 5,
        BankType = 6

    }
   public class ContactType:MasterEntity
    {
       public ContactType() : base(default(Guid)) { }
       public ContactType(Guid id):base(id)
       {

       }
       public ContactType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           :base(id,dateCreated,dateLastUpdated,isActive)
       {

       }
       public string Code { get; set; }
       public string Description { get; set; }
       public string Name { get; set; }
    }
}
