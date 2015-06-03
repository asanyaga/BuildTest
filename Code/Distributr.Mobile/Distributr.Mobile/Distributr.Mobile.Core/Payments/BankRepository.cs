using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Payments
{
    public class BankRepository : BaseRepository<Bank>
    {
        public BankRepository(Database database) : base(database)
        {

        }
    }
}
