using System.Linq;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly Database database;

        public LoginRepository(Database database)
        {
            this.database = database;
        }

        public User FindUser(string username)
        {
            return database.GetAll<User>().FirstOrDefault(user => (user.Username == username));
        }

        public LastLoggedInUser GetLastUser()
        {
            return database.Table<LastLoggedInUser>().FirstOrDefault();
        }

        public void SetLastUser(string username, string costCentreApplicationId)
        {
            database.DeleteAll<LastLoggedInUser>();
            var last = new LastLoggedInUser
            {
                LastUser = username,
                CostCentreApplicationId = costCentreApplicationId
            };
            database.Insert(last);
        }
    }
}