using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CostCentreApplication : MasterEntity 
    {


        
        public CostCentreApplication(Guid id):base(id)
        {

        }

        public CostCentreApplication(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        
        [Required(ErrorMessage="You must add cost centre id")]
        public Guid CostCentreId {get;set;}

        [Required(ErrorMessage="You must add a description -  Suggested CCName_App1")]
        public string Description { get; set; }

    }
}
