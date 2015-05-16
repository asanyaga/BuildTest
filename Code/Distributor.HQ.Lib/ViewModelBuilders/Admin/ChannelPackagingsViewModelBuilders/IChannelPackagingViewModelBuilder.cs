using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.ChannelPackagingsViewModels;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders.Impl;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders
{
   public interface IChannelPackagingViewModelBuilder
    {
       IList<ChannelPackagingViewModel> GetAll();
       void Save(string[] cb);
       [Obsolete("WTF")]
       ChannelPackagingViewModel GetPackaging();
       [Obsolete("WTF")]
       ChannelPackagingViewModel GetOutLetTypes();
       List<CPItem> ChannelPackagingItems();

       ChannelPackagingViewModel Get();

    }
   public class CPItem
   {
       public Guid Id { get; set; }
       public Guid PackagingId { get; set; }
       public Guid OutletTypeId { get; set; }
       public bool Checked { get; set; }
   }
}
