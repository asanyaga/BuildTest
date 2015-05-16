using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.QuickBooksDocumentRepository;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    public class QuickBooksDocumentRepository : MainOrderRepository, IQuickBooksDocumentRepository
    {
        public QuickBooksDocumentRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
        }

        public List<MainOrderSummary> GetClosedOrdersToExport(int page, int pageSize, DateTime startDate, DateTime endDate, List<Guid> exclude, OrderType orderType, string search = "")
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            search = search.ToLower();
            List<tblDocument> unexportedtbl = _ctx.tblDocument.Where(n =>
                                                    n.OrderOrderTypeId == (int) orderType &&
                                                    n.DocumentStatusId == (int) DocumentStatus.Closed &&
                                                    exclude.All(ex => ex != n.Id) &&
                                                    (n.DocumentDateIssued >= startDate &&
                                                     n.DocumentDateIssued <= endDate) &&
                                                    n.DocumentReference.ToLower().Contains(search)

                ).OrderByDescending(d => d.IM_DateCreated).Skip((page - 1)*pageSize).Take(pageSize).ToList();

            return unexportedtbl.Select(n => MapSummary(n, true)).ToList();
        }

        public int GetCount(DateTime startDate, DateTime endDate, List<Guid> exclude, OrderType orderType, string search)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            Expression<Func<tblDocument, bool>> expression = (n =>
                                                              n.OrderOrderTypeId == (int) orderType &&
                                                              n.DocumentStatusId == (int) DocumentStatus.Closed &&
                                                              exclude.All(ex => ex != n.Id) &&
                                                              (n.DocumentDateIssued >= startDate &&
                                                               n.DocumentDateIssued <= endDate) &&
                                                              n.DocumentReference.ToLower().Contains(search));
            return base.GetCount(expression);
        }
    }
}
