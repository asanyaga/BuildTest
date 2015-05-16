using Distributr.WPF.Lib.Services.Service.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility.Caching;
using Distributr.WPF.Lib.Services.Service.Utility;
using log4net;
using Distributr.Core;
using Distributr.Core.Domain.Master;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class SyncService : ISyncService
    {
        private ILog _logger = LogManager.GetLogger("SyncService");
        private IConfigService _configService;
        private IUpdateMasterDataService _updateMasterDataService;
        private ICacheProvider _cacheProvider;

        private IReceiveAndProcessPendingRemoteCommandEnvelopesService _receiveAndProcessPendingRemoteCommandEnvelopesService;
        private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

        public SyncService(IConfigService configService, IUpdateMasterDataService updateMasterDataService, ICacheProvider cacheProvider, IReceiveAndProcessPendingRemoteCommandEnvelopesService receiveAndProcessPendingRemoteCommandEnvelopesService, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
        {
            _configService = configService;
            _updateMasterDataService = updateMasterDataService;
            _cacheProvider = cacheProvider;
            _receiveAndProcessPendingRemoteCommandEnvelopesService = receiveAndProcessPendingRemoteCommandEnvelopesService;
            _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
        }


        public bool CanSync()
        {
            Guid clientAppId = _configService.GetClientAppId();
            bool exist = _configService.GetClientApplications().Any(c => c.CanSync);
            if (!exist)
            {
                ClientApplication application = _configService.GetClientApplications().FirstOrDefault(s => s.Id == clientAppId);
                if (application == null)
                    return false;
                application.CanSync = true;
                _configService.SaveClientApplication(application);
                return true;
            }

            return _configService.GetClientApplications().Any(c => c.CanSync && c.Id == clientAppId);
        }

        public async Task<bool> UpdateMasterData(IProgress<string> progress)
        {
            Config config = _configService.Load();
            Guid ccAppId = config.CostCentreApplicationId;
            VirtualCityApp vcAppId = config.AppId;
            bool needTo = true; //await updateMasterDataService.DoesCostCenreNeedToSyncAsync(appId, vcAppId);
            if (needTo)
            {
                foreach (MasterDataCollective masterData in SyncMasterDataCollective.GetMasterDataCollective(vcAppId))
                {
                    string entity = masterData.ToString();
                    try
                    {
                        bool success = await _updateMasterDataService.GetByBatchAndUpdateEntityMasterDataAsync(ccAppId, entity);
                        if (success)
                        {
                            progress.Report("\n " + string.Format("==>  Successful update of {0}", entity));
                        }
                        else
                        {

                            progress.Report("\n " + string.Format("....Failed to update {0}", entity));
                            _logger.Error(string.Format("....Failed to update {0}", entity));
                            break;
                        }
                    }
                    catch (Exception exception)
                    {
                        progress.Report("\n " + string.Format("....Failed to update {0}", entity));
                        _logger.Error(string.Format("....Failed to update {0}", entity), exception);
                    }
                    
                }
            }
            _cacheProvider.Reset();
            return true;
        }

        public async Task<bool> DownloadCommandEnvelopesSync()
        {
            Guid appid = _configService.Load().CostCentreApplicationId;
            bool ok = await _receiveAndProcessPendingRemoteCommandEnvelopesService.ReceiveAndProcessNextEnvelopesAsync(appid);
            return ok;
        }

        public async Task<int> UploadCommandEnvelopesSync()
        {
            Guid appId = _configService.Load().CostCentreApplicationId;
            int noofcmdsent = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync(100);
            return noofcmdsent;
        }

        public Config AppInitialize(Guid costCentreApplicationId, Guid costCentreId, string serverUrl)
        {
            _configService.CleanLocalDB();
            Config config = _configService.Load();
            config.CostCentreApplicationId = costCentreApplicationId;
            config.DateInitialized = DateTime.Now;
            config.CostCentreId = costCentreId;
            config.WebServiceUrl = serverUrl.Trim();
            config.IsApplicationInitialized = true;
            config.ApplicationStatus = 1;
            _configService.Save(config);
            return config;
        }




        public async Task<bool> UpdateInventory(IProgress<string> progress)
        {
            Config config = _configService.Load();
            Guid ccAppId = config.CostCentreApplicationId;
            progress.Report("Begin update inventory");
            bool inventorySuccess = await _updateMasterDataService.GetAndUpdateInventoryAsync(ccAppId);
            progress.Report("Completed update inventory");
            return inventorySuccess;
        }

        public async Task<bool> UpdatePayments(IProgress<string> progress)
        {
            Config config = _configService.Load();
            Guid ccAppId = config.CostCentreApplicationId;
            progress.Report("Begin update payments");
            bool success = await _updateMasterDataService.GetAndUpdatePaymentAsync(ccAppId);
            progress.Report("Completed update payments");
            return success;
        }
    }
}
