using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Distributr.Core.Domain.Master.AssetEntities;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel
{
    public class AssetViewModel
    {
        public Guid Id { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.type.required")]
        public Guid AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.category.required")]
        public Guid AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.status.required")]
        public Guid AssetStatusId { get; set; }
        public string AssetStatus { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.code.required")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.name.required")]
        public string Name { get; set; }
        //[LocalizedRequired(ErrorMessage = "hq.vm.asset.capacity.required")]
        public string Capacity { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.serial.required")]
        public string SerialNo { get; set; }
        [LocalizedRequired(ErrorMessage = "hq.vm.asset.assetno.required")]
        public string AssetNo { get; set; }
        public bool IsActive { get; set; }
    }
}
