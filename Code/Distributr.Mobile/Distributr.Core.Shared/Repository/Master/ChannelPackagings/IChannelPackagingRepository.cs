using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ChannelPackagings
{
   public interface IChannelPackagingRepository:IRepositoryMaster<ChannelPackaging>
   {
       IEnumerable<ChannelPackaging> Query(QueryStandard q);
   }
}
