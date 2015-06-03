using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Transactional.RecollectionRepositories
{
    public interface IReCollectionRepository
    {
        List<UnderBankingItem> Query(QueryMasterData q);
        List<UnderBankingItemSummary> QuerySummary(QueryUnderBanking q);
        List<UnderBankingItemReceived> UnderBankingItemReceived(Guid id);
    }

}
