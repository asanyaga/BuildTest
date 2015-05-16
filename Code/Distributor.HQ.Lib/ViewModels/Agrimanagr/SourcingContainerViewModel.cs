using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr
{
    public class SourcingContainerViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string EquipmentNumber { get; set; }
        public string Name { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int EquipmentType { get; set; }
        public string Description { get; set; }
        public Guid CostCentre { get; set; }
        public decimal LoadCariage { get; set; }
        public decimal TareWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal BubbleSpace { get; set; }
        public decimal Volume { get; set; }
        public decimal FreezerTemp { get; set; }
        public Guid CommodityId { get; set; }
        public Guid CommodityGrade { get; set; }
        public int IsActive { get; set; }
    }
}
