using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CompetitorManagement
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Competitor:MasterEntity
    {
       public Competitor(Guid id)
           : base(id)
       { 
       
       }
       public Competitor(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       
       }
       public string Name { get; set; }
       public string PhysicalAddress { get; set; }
       public string PostalAddress { get; set; }
       public string Telephone { get; set; }
       public string ContactPerson { get; set; }
       public string City { get; set; }
       public string Longitude { get; set; }
       public string Lattitude { get; set; }
    }
}
