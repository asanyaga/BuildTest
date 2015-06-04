using Distributr.Core.Domain.Master.CostCentreEntities;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Login
{
    public class User : Distributr.Core.Domain.Master.UserEntities.User
    {
        public string CostCentreApplicationId { get; set; }

        [Ignore]
        public bool IsNewUser { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public DistributorSalesman DistributorSalesman { get; set; }
    }
}