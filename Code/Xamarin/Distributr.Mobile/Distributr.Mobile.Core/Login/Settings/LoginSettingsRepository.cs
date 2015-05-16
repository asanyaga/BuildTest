using Distributr.Mobile.Data;

namespace Distributr.Mobile.Login.Settings
{
    public class LoginSettingsRepository
    {
        private readonly Database db;

        public LoginSettingsRepository(Database db)
        {
            this.db = db;
        }

        public LoginSettings GetSettings()
        {
            var settings = db.Table<LoginSettings>().FirstOrDefault();
            if (settings == null)
            {
                return new LoginSettings
                {
                    ServerUrl = "http://192.168.1.104/qa3_ws/api/"
                };
            }
            return settings;
        }

        public void Save(LoginSettings settings)
        {
            db.InsertOrReplace(settings);
        }
    }
}