using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CoolerEntities;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel
{
    public class AssetCategoryViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public Guid AssetTypeId { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string AssetTypeName { get; set; }
        public bool IsActive { get; set; }
        public IPagination<AssetCategoryViewModel> aCategoryPagedList { get; set; }
    }
}
