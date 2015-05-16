namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public enum SyncStatus
    {
        AbandonSync = -1,
        MasterDataInProgress = 1,
        MasterDataComplete = 2,
        SendPendingLocalCommandsInProgress = 3,
        SendPendingLocalCommandsComplete = 4,
        ReceiveAndProcessPendingRemoteCommandsInProgress = 5,
        ReceiveAndProcessPendingRemoteCommandComplete = 6,
        SendPendingLocalMasterDataComplete = 7,
        SendPendingLocalMasterDataInProgress = 8,
       InventoryInProgress=9,
       InventoryInComplete=10,
       PaymentInProgress = 11,
       PaymentInComplete = 12,

    }
}
