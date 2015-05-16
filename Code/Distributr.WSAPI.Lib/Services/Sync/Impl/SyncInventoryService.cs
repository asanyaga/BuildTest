using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncInventoryService : SyncMasterDataBase, ISyncInventoryService
    {
        private readonly CokeDataContext _context;

        public SyncInventoryService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<InventoryDTO> GetInventory(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<InventoryDTO>();
            response.MasterData = new SyncMasterDataInfo<InventoryDTO>();
            response.MasterData.EntityName = "Inventory";
            try
            {
                var query = _context.tblInventory.AsQueryable();
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                         var inventoryIds =_context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.DHubInventory,costCentre.Id)).ToList();
                         query = query.Where(n => inventoryIds.Contains(n.id) ).OrderBy(s => s.IM_DateCreated);
                            break;
                        case CostCentreType.DistributorSalesman:
                            
                            break;
                    }
                }
                query = query.OrderBy(s => s.IM_DateCreated);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
                response.MasterData.MasterDataItems = query.ToList().Select(n => Map(n)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private InventoryDTO Map(tblInventory tbl)
        {
            var dto = new InventoryDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Balance = tbl.Balance.HasValue ? tbl.Balance.Value : 0,
                Value = tbl.Value.HasValue ? tbl.Value.Value : 0,
                ProductMasterID = tbl.ProductId,
                UnavailableBalance = tbl.UnavailableBalance,
                WarehouseMasterID = tbl.WareHouseId,
            };
            return dto;
        }
    }
}