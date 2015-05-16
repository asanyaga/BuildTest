using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.ChannelPackagingsViewModels
{
   public class ChannelPackagingViewModel
    {
       public ChannelPackagingViewModel()
       {
           OutletTypes = new List<OutletTypeVM>();
           Packagings = new List<PackagingVM>();
           chanId = new List<ChannelPacksVm>();
           PageSize = 10;
       }

       public Guid ChannelPId { get; set; }
       public Guid PackageId { get; set; }
       public Guid OutletTypeId { get; set; }
       public bool IsChecked { get; set; }

       public int PageSize { get; set; }
      
       public List<OutletTypeVM> OutletTypes { get; set; }
       public List<PackagingVM> Packagings { get; set; }
       public List<ChannelPacksVm> chanId { get; set; }

       public List<RowItem[]> RowItems { get; set; }

       public class RowItem
       {
           public string CPLookup { get; set; }
           public bool IsChecked { get; set; }
           public string Check { get { if (IsChecked) return "CHECKED"; else return ""; } }
       }
       public class OutletTypeVM
       {
           public Guid Id { get; set; }
           public string OutletTypeName { get; set; }
       }
       public class PackagingVM
       {
           public Guid Id { get; set; }
           public string PackName { get; set; }
           
       }
       public class ChannelPacksVm
       {
           public Guid cId { get; set; }
       }

    }
}
