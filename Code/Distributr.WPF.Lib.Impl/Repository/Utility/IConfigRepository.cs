using System;
using System.Collections.Generic;
using Distributr.Core;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Impl.Repository.Utility
{
   
    public interface IConfigRepository
    {
        ConfigLocal GetById(int Id);
        int Save(ConfigLocal config);
        ConfigLocal Get(VirtualCityApp appId);
        List<ConfigLocal> GetAll();
        void CleanLocalDB();

       // List<ConfigLocal> GetAll();
        //ConfigLocal GetByCostCenterId(int ccId);
            [Obsolete("Command Envelope Refactoring")]
        void AddDeliveredCommand(long id);
          [Obsolete("Command Envelope Refactoring")]
        void ClearDeliveredCommand();
          [Obsolete("Command Envelope Refactoring")]
        List<long> GetDeliveredCommand();
        List<ClientApplicationLocal> GetClientApplication(VirtualCityApp appType);
        void SaveClientApplication(ClientApplicationLocal application);

        SyncTrackerLocal GetSync(string entityName);
        void SetLastSync(string entityname,DateTime lastsynctimestamp);

        void AddDeliveredCommandEnvelopeId(Guid id);
        void ClearDeliveredCommandEnvelopeIds();
        List<Guid> GetDeliveredCommandEnvelopeIds();

    }
}
