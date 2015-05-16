using System;
namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ProductType : MasterEntity
    {
        public ProductType() : base(default(Guid)) { }
        public ProductType(Guid id) : base(id)
        {

        }
        public ProductType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }
}
