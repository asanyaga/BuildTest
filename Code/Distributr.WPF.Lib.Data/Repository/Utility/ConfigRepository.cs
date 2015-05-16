using System;
using System.Collections.Generic;
using Distributr.Core;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;

using System.Linq;
using Distributr.WPF.Lib.Data.EF;
using log4net;

namespace Distributr.WPF.Lib.Data.Repository.Utility
{
    public class ConfigRepository:IConfigRepository
    {
        private DistributrLocalContext _ctx;
        private ILog _logger = LogManager.GetLogger("ConfigRepository");
        public ConfigRepository(DistributrLocalContext ctx)
        {
            this._ctx = ctx;
            
        }

        public int Save(ConfigLocal config)
        {
            var exist = _ctx.ConfigLocal.FirstOrDefault(a => a.VirtualCityApp == config.VirtualCityApp);
            if (exist == null)
            {
                exist = new ConfigLocal();
                exist.Id = config.Id;
                exist.DateInitialized = DateTime.Now;

                _ctx.ConfigLocal.Add(exist);
            }


            exist.CostCenterId = config.CostCenterId;
            exist.CostCentreApplicationId = config.CostCentreApplicationId;
            exist.CostCentreApplicationDescription = config.CostCentreApplicationDescription;
            exist.WebServiceUrl = config.WebServiceUrl;
            exist.LastDeliveredCommandRouteItemId = config.LastDeliveredCommandRouteItemId;
            exist.ApplicationStatus = config.ApplicationStatus;
            exist.IsApplicationInitialized = config.IsApplicationInitialized;
            exist.VirtualCityApp = config.VirtualCityApp;
            //exist.IsApplicationInitialized = false;


            try
            {
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return config.Id;
        }

        public ConfigLocal GetById(int Id)
        {
            return _ctx.ConfigLocal.Cast<ConfigLocal>().FirstOrDefault(n => n.Id == Id);
        }

        public ConfigLocal Get(VirtualCityApp appId)
        {
            return _ctx.ConfigLocal.Cast<ConfigLocal>().FirstOrDefault(a => a.VirtualCityApp == (int)appId);
        }

        public void CleanLocalDB()
        {  
        }
        [Obsolete("Command Envelope Refactoring")]
        public void AddDeliveredCommand(long id)
        {
               ReceivedCommandLocal existing = new ReceivedCommandLocal();
                existing.LastDeliveredCommandRouteItemId = id;
            _ctx.ReceivedCommand.Add(existing);
            _ctx.SaveChanges();

        }
            [Obsolete("Command Envelope Refactoring")]
        public void ClearDeliveredCommand()
        {
            var received = _ctx.ReceivedCommand.ToList();
            received.ForEach(n => _ctx.ReceivedCommand.Remove(n));
            _ctx.SaveChanges();
        }
            [Obsolete("Command Envelope Refactoring")]
        public List<long> GetDeliveredCommand()
        {
            List<long> all = null;
            all = _ctx.ReceivedCommand.Select(s => s.LastDeliveredCommandRouteItemId).ToList();
            return all;
        }
        
        public List<ClientApplicationLocal> GetClientApplication(VirtualCityApp appType)
        {
            _logger.Info("VirtualCityApp == " +appType);
            List<ClientApplicationLocal> all = null;
            
            _logger.Info("ClientApplicationLocal count == " + _ctx.ClientApplicationLocal.Count());
            all = _ctx.ClientApplicationLocal.Where(s => s.AppTypeId==(int)appType).ToList();
            return all;
        }

        public void SaveClientApplication(ClientApplicationLocal application)
        {
           var exist = _ctx.ClientApplicationLocal.FirstOrDefault(a => a.HostName==application.HostName&& a.AppTypeId==application.AppTypeId);
            if (exist == null)
            {
                exist = new ClientApplicationLocal();
                exist.DateInitialized = DateTime.Now;
                _ctx.ClientApplicationLocal.Add(exist);
            }
            exist.ClientAppId = application.ClientAppId;
            exist.HostName = application.HostName;
           // exist.DateInitialized = application.DateInitialized;
            exist.CanSync = application.CanSync;
            exist.AppTypeId = application.AppTypeId;

            //Ensure that only one app can syc at a time=>Set CanSync property of all apps to false if this is true.
            if (exist.CanSync)
            {
                foreach (var app in _ctx.ClientApplicationLocal.Where(p => p.AppTypeId == exist.AppTypeId && p.CanSync).ToList())
                {
                    app.CanSync = false;
                }
            }

            _ctx.SaveChanges();
        }

        public SyncTrackerLocal GetSync(string entityName)
        {
            var synctrack = _ctx.SyncTrackerLocal.FirstOrDefault(s => s.EntityName == entityName);
            if (synctrack == null)
            {
                synctrack = new SyncTrackerLocal
                                {
                                    Id = Guid.NewGuid(),
                                    EntityName = entityName,
                                    LastPage = 0,
                                    LastSyncDateTime = new DateTime(1940, 01, 01)

                                };
                _ctx.SyncTrackerLocal.Add(synctrack);
                _ctx.SaveChanges();
            }

            return synctrack;


        }

        public void SetLastSync(string entityname,DateTime lastsynctimestamp)
        {
            var synctrack = _ctx.SyncTrackerLocal.FirstOrDefault(s => s.EntityName == entityname);
            if (synctrack != null)
            {
                synctrack.LastSyncDateTime = lastsynctimestamp;
            }
            _ctx.SaveChanges();
        }

        public void AddDeliveredCommandEnvelopeId(Guid id)
        {
            var receivedEnvelopId = new ReceivedCommandEnvelopeId
                                    {
                                        EnvelopeId = id,
                                    };
            _ctx.ReceivedCommandEnvelopeIds.Add(receivedEnvelopId);
            _ctx.SaveChanges();
        }

        public void ClearDeliveredCommandEnvelopeIds()
        {
            var received = _ctx.ReceivedCommandEnvelopeIds.ToList();
            received.ForEach(n => _ctx.ReceivedCommandEnvelopeIds.Remove(n));
            _ctx.SaveChanges();
        }

        public List<Guid> GetDeliveredCommandEnvelopeIds()
        {
            List<Guid> all = null;
            all = _ctx.ReceivedCommandEnvelopeIds.Select(s => s.EnvelopeId).ToList();
            return all;
        }


        public List<ConfigLocal> GetAll()
        {
            var all = _ctx.ConfigLocal.ToList();
            return all.ToList();
        }
    }
}
