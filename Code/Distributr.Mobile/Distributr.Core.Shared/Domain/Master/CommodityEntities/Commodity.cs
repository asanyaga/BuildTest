using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Distributr.Core.Domain.Master.CommodityEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Commodity:MasterEntity
    {
        public Commodity(Guid id) : base(id)
        {
            CommodityGrades = new List<CommodityGrade>();

        }
        public Commodity(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : 
            base(id, dateCreated, dateLastUpdated, status){}

        public Commodity(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status, IList<CommodityGrade> commodityGrades) :
            base(id, dateCreated, dateLastUpdated, status)
        {
            CommodityGrades = commodityGrades;
        }
        
        public string Name { get; set; }
        public CommodityType CommodityType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public IList<CommodityGrade> CommodityGrades { get; internal set; }

       


    }
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CommodityGrade : MasterEntity
    {
        public CommodityGrade(Guid id)
            : base(id)
        {
        }

        public CommodityGrade(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status)
            : base(id, dateCreated, dateLastUpdated, status)
        {
        }
  

        public string Name { get; set; }
        public Commodity Commodity { get; set; }
        public int UsageTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
