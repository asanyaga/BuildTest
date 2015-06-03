using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.SettingsEntities;

namespace Distributr.Core.Repository.Master.SettingsRepositories
{
    public interface ISettingsRepository : IRepositoryMaster<AppSettings>
    {
        AppSettings GetByKey(SettingsKeys key);
    }
}
