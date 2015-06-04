using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CommodityEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CommodityType: MasterEntity
    {
        public CommodityType(Guid id) : base(id){}

        public CommodityType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status){ }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
