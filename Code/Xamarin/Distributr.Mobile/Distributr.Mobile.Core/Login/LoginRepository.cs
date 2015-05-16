using System;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly Database db;

        public LoginRepository(Database db)
        {
            this.db = db;
        }

        public User FindUser(string username)
        {
            return db.Table<User>().Where(user => (user.Username == username)).FirstOrDefault();
        }

        public string GetLastUser()
        {
            var last = db.Table<LastLoggedInUser>().FirstOrDefault();
            if (last == null)
            {
                return String.Empty;
            }
            return last.LastUser;
        }

        public void SetLastUser(string username)
        {
            db.DeleteAll<LastLoggedInUser>();
            var last = new LastLoggedInUser
            {
                LastUser = username
            };
            db.Insert(last);
        }
    }
}