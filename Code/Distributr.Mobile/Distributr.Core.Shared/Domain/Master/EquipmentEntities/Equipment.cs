using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.EquipmentEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public  class Equipment : MasterEntity
    {
        public Equipment(Guid id) : base(id) { }
        public Equipment(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}

        public string Code { get; set; }
        public string EquipmentNumber { get; set; }
        public string Name { get; set; }        
        public string Make { get; set; }
        public string Model { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public string Description { get; set; }
        public Hub CostCentre { get; set; }
    }
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum EquipmentType{Others=0, Container = 1, Printer = 2, WeighingScale = 3,Vehicle=4 }
   
}
