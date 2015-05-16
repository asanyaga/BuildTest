using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel
{
    public class PrinterViewModel
    {
        public Guid Id { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
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
