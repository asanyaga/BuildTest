using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncUnderBankingService :SyncMasterDataBase, ISyncUnderBankingService
    {
         private CokeDataContext _context;

         public SyncUnderBankingService(CokeDataContext context):base(context)
        {
            _context = context;
        }
         public SyncResponseMasterDataInfo<UnderBankingDTO> GetUnderBanking(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<UnderBankingDTO>();
            response.MasterData = new SyncMasterDataInfo<UnderBankingDTO>();;
            response.MasterData.EntityName = "UnderBanking";
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblRecollection.AsQueryable();
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            var disCostcentreId =
                                _context.tblCostCentre.Where(s => s.ParentCostCentreId == costCentre.Id).Select(
                                    s => s.Id).ToList();
                            query = query.Where(n => disCostcentreId.Contains(n.FromCostCentreId) && n.IM_DateLastUpdated > q.From);
                            break;
                        case CostCentreType.DistributorSalesman:
                            var outletIds =
                                _context.tblSalemanRoute.Where(s => s.SalemanId == costCentre.Id).SelectMany(
                                    s => s.tblRoutes.tblCostCentre)
                                    .Where(s => s.CostCentreType == 5).Select(s => s.Id).ToList();
                            query = query.Where(n => n.CostCentreId==costCentre.Id);
                            break;
                    }
                }
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);
                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);
                response.MasterData.MasterDataItems = query.ToList().Select(s => Map(s)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

         private UnderBankingDTO Map(tblRecollection tbl)
         {
             var costcentre = _context.tblCostCentre.FirstOrDefault(s => s.Id == tbl.FromCostCentreId);
             var dto = new UnderBankingDTO();
             dto.MasterId = tbl.Id;
             dto.DateCreated = tbl.IM_DateCreated;
             dto.DateLastUpdated = tbl.IM_DateLastUpdated;
             dto.StatusId = tbl.IM_Status;
             dto.Description = tbl.Description;
             dto.Amount = tbl.Amount;
             dto.CostCentreId = tbl.FromCostCentreId;
             var items = tbl.tblRecollectionItem.ToList();
             dto.TotalReceivedAmount = items.Sum(s => s.Amount);
             dto.Items = items.Select(s => new UnderBankingItemDTO{Amount = s.Amount,Id = s.Id,IsConfirmed = s.IsComfirmed, UnderBankingId = s.RecollectionId} ).ToArray();
             dto.CostCentreName = costcentre != null ? costcentre.Name : "";
             return dto;
         }
    }
}
