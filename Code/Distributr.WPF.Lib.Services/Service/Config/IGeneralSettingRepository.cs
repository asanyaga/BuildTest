using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Services.Service
{
    public interface IGeneralSettingRepository
    {
        int Save(GeneralSetting log);
        List<GeneralSetting> GetAll();
        GeneralSetting GetById(int Id);
        GeneralSetting GetByKey(GeneralSettingKey key);
    }
    public interface IPrintedReceiptsTrackerRepository
    {
        bool IsReprint(Guid receiptId);
        void Log(Guid receiptid);
    }
}
