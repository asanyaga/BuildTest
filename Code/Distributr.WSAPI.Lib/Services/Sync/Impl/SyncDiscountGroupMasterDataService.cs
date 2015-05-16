using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncDiscountGroupMasterDataService :SyncMasterDataBase, ISyncDiscountGroupMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncDiscountGroupMasterDataService(CokeDataContext context):base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<DiscountGroupDTO> GetDiscountGroup(QueryMasterData qry)
        {
            var response = new SyncResponseMasterDataInfo<DiscountGroupDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<DiscountGroupDTO>
                                           {EntityName = MasterDataCollective.DiscountGroup.ToString()}
                               };       
            try
            {
                var query = _context.tblDiscountGroup.AsQueryable();
                var syncostcentre = GetSyncCostCentre(qry.ApplicationId);
                  if (syncostcentre != null)
                  {

                     // var groupIds = DiscountGroups(syncostcentre);
//query = query.Where(s => groupIds.Contains(s.id));
                    
                  }
                query = query.Where(n => n.IM_DateLastUpdated > qry.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);
                if (qry.Skip.HasValue && qry.Take.HasValue)
                    query = query.Skip(qry.Skip.Value).Take(qry.Take.Value);

                var list = new ArrayList();
                foreach (var item in query.ToArray())
                {
                    list.Add(Map(item));
                }
                response.MasterData.MasterDataItems = list.Cast<DiscountGroupDTO>().ToArray();
               // response.MasterData.MasterDataItems = query.ToList().Select(Map).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }
        private List<Guid?> DiscountGroups(SyncCostCentre syncostcentre)
        {
            var discountGroups = new List<Guid?>();
            if (syncostcentre.CostCentreType == CostCentreType.Distributor)
            {
                discountGroups =
                    _context.tblCostCentre.Where(s => s.CostCentreType == 5
                                                      && s.ParentCostCentreId == syncostcentre.Id
                                                      &&
                                                      (s.IM_Status == (int)EntityStatus.Active ||
                                                       s.IM_Status == (int)EntityStatus.Inactive) &&
                                                      s.Outlet_DiscountGroupId != Guid.Empty)
                        .Select(s => s.Outlet_DiscountGroupId).Distinct().ToList();
            }
            else if (syncostcentre.CostCentreType == CostCentreType.DistributorSalesman)
            {
                var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncostcentre.Id
                                                                   && n.IM_Status == (int)EntityStatus.Active).Select(
                                                                       n => n.RouteId).Distinct().ToList();
                discountGroups = _context.tblCostCentre.Where(s => routeIds.Contains(s.RouteId.Value) &&
                                                                   (s.IM_Status == (int)EntityStatus.Active ||
                                                                    s.IM_Status ==
                                                                    (int)EntityStatus.Inactive))
                    .Select(s => s.Outlet_DiscountGroupId).ToList();
                discountGroups = discountGroups.Where(p => p.HasValue && p.Value != Guid.Empty).Distinct().ToList();
            }
            return discountGroups;
        }
        private DiscountGroupDTO Map(tblDiscountGroup tbl)
        {
            var dto = new DiscountGroupDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Code = tbl.Code,
                              Name = tbl.Name,
                          };
            return dto;
        }
    }
}
