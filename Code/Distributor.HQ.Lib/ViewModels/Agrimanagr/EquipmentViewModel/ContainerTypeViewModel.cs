using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel
{
    public class ContainerTypeViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
        public string Make { get; set; }
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        public string Description { get; set; }
         [Required(ErrorMessage = "Model required.")]
        public string Model { get; set; }
        [Display(Name = "Load Carriage")]
        public decimal LoadCariage { get; set; }
        [Display(Name = "Tare Weight")]
        public decimal TareWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        [Display(Name = "Bubble Space")]
        public decimal BubbleSpace { get; set; }
        public decimal Volume { get; set; }
        [Display(Name = "Freezer Temp")]
        public decimal FreezerTemp { get; set; }
        [Display(Name = "Container User Type")]
        public int ContainerUserType { get; set; }
        public Guid SelectedCommodityId { get; set; }
        [Display(Name = "Commodity Name")]
        public String SelectedCommodityName { get; set; }
        public SelectList CommodityList { get; set; }
        public Guid SelectedCommodityGradeId { get; set; }
        public String SelectedCommodityGradeName { get; set; }
        public int IsActive { get; set; }

        public SelectList CommodityGradesList { get; set; }

        public int? Count { get; set; }
    }
}
