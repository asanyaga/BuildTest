using System.Collections.Generic;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Impl.Repository.Utility
{
    public interface ILogRepository
    {
        int Save(LogLocal log);
        List<LogLocal> GetAll();
        List<LogLocal> GetByType(LogType type);
        
    }
}
