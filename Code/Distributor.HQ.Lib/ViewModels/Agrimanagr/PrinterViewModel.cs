using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr
{
    public class PrinterViewModel
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
        public int IsActive { get; set; }

    }
}
