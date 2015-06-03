using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.Agrimanagr
{
    public interface ISeasonRepository : IRepositoryMaster<Season>
    {
        QueryResult<Season> Query(QueryBase q);
    }
}
