using System;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public interface IAutoSyncService
    {
        bool ShowMessageBox { get; }
        void SetShowMessageBox(bool canShow);
        void UploadCommandSyncCompleted();
        void DownloadCommandSyncCompleted();
        void MasterDataSyncCompleted();
        void BeginMasterDataSync();
        void BeginUploadCommandSync();
       
        void BeginBatchDownloadCommandSync();
        void StartAutomaticSync();
        void StopAutomaticSync();
        void RestartAutomaticSync();
    }


    /// <summary>
    /// Factor out Sync stuff from autosyncservice
    /// </summary>
    public interface ISyncService
    {
        bool CanSync();
        Task<bool> UpdateMasterData(IProgress<string> progress );
        Task<bool> UpdateInventory(IProgress<string> progress );
        Task<bool> UpdatePayments(IProgress<string> progress);
        Task<bool> DownloadCommandEnvelopesSync();
        Task<int> UploadCommandEnvelopesSync();
        Config AppInitialize(Guid costCentreApplicationId, Guid costCentreId, string serverUrl);
    }
}
