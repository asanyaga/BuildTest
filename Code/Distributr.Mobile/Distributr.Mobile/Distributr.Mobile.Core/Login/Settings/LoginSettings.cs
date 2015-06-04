using SQLite.Net.Attributes;

namespace Distributr.Mobile.Login.Settings
{
    public class LoginSettings
    {
        [PrimaryKey, AutoIncrement]
        public int PrimaryKey { get; set; }

        public string ServerUrl { get; set; }
    }
}