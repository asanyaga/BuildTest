using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.Master.CompetitorManagement
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class CompetitorProducts:MasterEntity
    {
       public CompetitorProducts(Guid id)
           : base(id)
       { 
       }
       public CompetitorProducts(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       
       }
       public string ProductName { get; set; }
       public string ProductDescription { get; set; }
       public Competitor Competitor { get; set; }
       public ProductBrand Brand{get;set;}
       public ProductPackaging Packaging { get; set; }
       public ProductType ProductType { get; set; }
       public ProductPackagingType PackagingType { get; set; }
       public ProductFlavour Flavour { get; set; }
    }
}
