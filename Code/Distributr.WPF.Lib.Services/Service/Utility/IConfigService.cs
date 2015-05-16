using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public interface IConfigService
    {
        Config Load();
        void Save(Config config);

        void CleanLocalDB();
        
        ViewModelParameters ViewModelParameters { get; }

        string WCFEndpointConfigurationName { get; }
        
        string WCFRemoteAddress { get; }
        List<ClientApplication> GetClientApplications();
        void SaveClientApplication(ClientApplication application);
        //bool CanSync();
        Guid GetClientAppId();

        //List<>
    }
}
