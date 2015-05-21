using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using System.Diagnostics;
using System.Media;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class UpdateMasterDataService : IUpdateMasterDataService
    {
        private ILog _logger = LogManager.GetLogger("UpdateMasterDataService");
        private IWebApiProxy _webApiProxy;
        IUpdateLocalDBService _updateLocalDBService;
        private IConfigRepository _configRepository;
        private IGeneralSettingRepository _settingRepository;
        public UpdateMasterDataService(IUpdateLocalDBService updateLocalDBService, IWebApiProxy webApiProxy, IConfigRepository configRepository, IGeneralSettingRepository settingRepository)
        {
            _updateLocalDBService = updateLocalDBService;
            _tablesToSync = new List<TableSyncInfo>();
            _webApiProxy = webApiProxy;
            _logger.Debug("##### -------> UpdateMasterDataService ctor <----------#######");
            _configRepository = configRepository;
            _settingRepository = settingRepository;
        }


        public class TableSyncInfo
        {
            public string TableToSync { get; set; }
            public bool SyncComplete { get; set; }
        }

        public List<TableSyncInfo> _tablesToSync = null;
        string tableToSync = "";


        public async Task<bool> DoesCostCenreNeedToSyncAsync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            return await _webApiProxy.DoesCostCenreNeedToSyncAsync(costCentreApplicationId, vcAppId);
        }

        public async Task<string[]> ListMasterDataEntitiesAsync()
        {
            return await _webApiProxy.ListMasterDataEntitiesAsync();
        }

        public async Task<bool> GetAndUpdateEntityMasterDataAsync(Guid costCentreApplicationid, string entityName)
        {
            ResponseMasterDataInfo info = await _webApiProxy.GetEntityMasterDataAsync(costCentreApplicationid, entityName);
            if (info == null)
                return false;
            await Task.Run(() => _updateLocalDBService.UpdateLocalDB(info)).ConfigureAwait(false);
            return true;
        }



        public async Task<bool> GetByBatchAndUpdateEntityMasterDataAsync(Guid costCentreApplicationid, string entityName)
        {
            Stopwatch s1 = Stopwatch.StartNew();
            var syncPageSizeSetting = _settingRepository.GetByKey(GeneralSettingKey.SyncPageSize);
            var syncPageSize = syncPageSizeSetting != null ? Convert.ToInt32(syncPageSizeSetting.SettingValue) : 10000;

            Messenger.Default.Send("\tStart syncing " + entityName);
            var syntracker = _configRepository.GetSync(entityName);
            bool status = true;
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            // const int pagesize =10000;
            int page = 1;
            while (true)
            {
                Stopwatch s2 = Stopwatch.StartNew();

                ResponseMasterDataInfo info = await _webApiProxy.GetEntityMasterDataAsync(costCentreApplicationid,
                                                     entityName, page, syncPageSize,
                                                      syntracker.LastSyncDateTime);
                TraceTest("Fetch api",entityName, s2.ElapsedMilliseconds);
                if (info == null || info.MasterData == null || (!info.MasterData.MasterDataItems.Any() && !info.DeletedItems.Any()))
                {
                    _configRepository.SetLastSync(entityName, info.MasterData.LastSyncTimeStamp);
                    status = true;
                    break;
                }
                Messenger.Default.Send("\t\t Pulled " + info.MasterData.MasterDataItems.Count() + " record(s)");
                await Task.Run(() => _updateLocalDBService.UpdateLocalDB(info)).ConfigureAwait(false);
                page++;
            }
            TraceTest("Total time taken",entityName, s1.ElapsedMilliseconds);
            return status;
        }

        private void TraceTest(string info, string entity, long elapsedMS)
        {
            string logT = "[TI] UpdateMasterDataService {0}_{1}_{2}ms elapsed";
            System.Diagnostics.Trace.WriteLine(string.Format(logT,info,entity,elapsedMS));
        }

        public async Task<bool> GetAndUpdateInventoryAsync(Guid costCentreApplicationId)
        {
            //ResponseMasterDataInfo info = await _webApiProxy.GetInventoryAsync(costCentreApplicationId);
            //if (info == null)
            //    return false;
            //await Task.Run(() => _updateLocalDBService.UpdateInventoryDB(info)).ConfigureAwait(false);
            //return true;

            bool status = true;
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            const int pagesize = 500;
            int page = 1;
            while (true)
            {
                ResponseMasterDataInfo info = await _webApiProxy.GetInventoryAsync(costCentreApplicationId, page, pagesize);
                if (info == null || info.MasterData == null || (!info.MasterData.MasterDataItems.Any() && !info.DeletedItems.Any()))
                {
                    _configRepository.SetLastSync("Inventory", info.MasterData.LastSyncTimeStamp);
                    status = true;
                    break;
                }
                Messenger.Default.Send("\t\t Pulled " + info.MasterData.MasterDataItems.Count() + " record(s)");
                await Task.Run(() => _updateLocalDBService.UpdateInventoryDB(info)).ConfigureAwait(false);
                page++;
            }
            return status;
        }

        public async Task<bool> GetAndUpdateUnderBankingAsync(Guid costCentreApplicationId)
        {
            ResponseMasterDataInfo info = await _webApiProxy.GetUnderBankingAsync(costCentreApplicationId);
            if (info == null)
                return false;
            await Task.Run(() => _updateLocalDBService.UnderBankingDB(info)).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> GetAndUpdatePaymentAsync(Guid costCentreApplicationId)
        {
            //ResponseMasterDataInfo info = await _webApiProxy.GetPaymentsAsync(costCentreApplicationId);
            //if (info == null)
            //    return false;
            //await Task.Run(() => _updateLocalDBService.UpdatePaymentDB(info)).ConfigureAwait(false);
            //return true;

            bool status = true;
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            const int pagesize = 500;
            int page = 1;
            while (true)
            {
                ResponseMasterDataInfo info = await _webApiProxy.GetPaymentsAsync(costCentreApplicationId, page, pagesize);
                if (info == null || info.MasterData == null || (!info.MasterData.MasterDataItems.Any() && !info.DeletedItems.Any()))
                {
                    _configRepository.SetLastSync("Payments", info.MasterData.LastSyncTimeStamp);
                    status = true;
                    break;
                }
                Messenger.Default.Send("\t\t Pulled " + info.MasterData.MasterDataItems.Count() + " record(s)");
                await Task.Run(() => _updateLocalDBService.UpdatePaymentDB(info)).ConfigureAwait(false);
                page++;
            }
            return status;
        }
    }
}
