using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CentreEntity
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CentreType : MasterEntity
    {
        public CentreType(Guid id) : base(id) { }
        public CentreType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive) { }

        public string Code { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
