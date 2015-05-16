using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel
{
    public class AssetTypeViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is a required field")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is a required field")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public IPagination<AssetTypeViewModel> aTypePagedList { get; set; }
    }
}
