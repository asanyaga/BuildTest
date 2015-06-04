using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.QuickBooksDocumentRepository
{
    public interface IQuickBooksDocumentRepository : IDocumentRepository<MainOrder>
    {
        List<MainOrderSummary> GetClosedOrdersToExport(int page, int pageSize, DateTime startDate, DateTime endDate, List<Guid>exclude, OrderType orderType, string search = "");

        int GetCount(DateTime startDate, DateTime endDate, List<Guid> exclude, OrderType orderType, string search);
    }
}
