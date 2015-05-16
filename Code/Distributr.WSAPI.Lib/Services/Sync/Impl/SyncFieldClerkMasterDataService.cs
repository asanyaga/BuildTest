using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncFieldClerkMasterDataService: SyncMasterDataBase, ISyncFieldClerkMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncFieldClerkMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<PurchasingClerkDTO> GetFieldClerk(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<PurchasingClerkDTO>();
            response.MasterData = new SyncMasterDataInfo<PurchasingClerkDTO>(); 
            response.MasterData.EntityName = MasterDataCollective.FieldClerk.ToString();
            try
            {
                var cct = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int)CostCentreType.PurchasingClerk
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > q.From);
                List<Guid> userCostCentreIds;
                switch (cct.CostCentreType)
                {
                    case CostCentreType.Hub:
                        var costCentreIds = _context.tblCostCentre.Where(n => n.CostCentreType == (int)CostCentreType.PurchasingClerk
                            && n.ParentCostCentreId != null && n.ParentCostCentreId == cct.Id).Distinct().Select(n => n.Id).ToList();
                        userCostCentreIds = _context.tblUsers.Where(n => n.CostCenterId == cct.Id || costCentreIds.Contains(n.CostCenterId))
                            .Select(n => n.CostCenterId).ToList();
                        query = query.Where(n => userCostCentreIds.Contains(n.Id)).OrderBy(n => n.IM_DateCreated);
                        break;
                    case CostCentreType.PurchasingClerk:
                        userCostCentreIds = _context.tblUsers.Where(n => n.CostCenterId == cct.Id).Select(n => n.CostCenterId).ToList();
                        query = query.Where(n => userCostCentreIds.Contains(n.Id)).OrderBy(n => n.IM_DateCreated);
                        break;
                }
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
        private PurchasingClerkDTO Map(tblCostCentre tbl)
        {
            var dto = new PurchasingClerkDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Name = tbl.Name,
                CostCentreCode = tbl.Cost_Centre_Code,
                ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                CostCentreTypeId = tbl.CostCentreType ?? 0,
                UserId = Guid.Empty,
                UserDto =  new UserDTO(),
                PurchasingClerkRoutes = new List<PurchasingClerkRouteDTO>(),

            };
            var entity = tbl.tblUsers.FirstOrDefault(n => n.CostCenterId == dto.MasterId);
            if (entity != null)
                dto.UserId = entity.Id;

            var usr = tbl.tblUsers.FirstOrDefault(n => n.Id == dto.UserId);
            if (usr != null)
            {
                dto.UserDto = new UserDTO 
                {
                    MasterId = usr.Id,
                    DateCreated = usr.IM_DateCreated,
                    DateLastUpdated = usr.IM_DateLastUpdated ?? new DateTime(),
                    StatusId = usr.IM_Status,
                    Username = usr.UserName,
                    CostCentre = usr.CostCenterId,
                    Password = usr.Password,
                    PIN = usr.PIN,
                    UserTypeId = usr.UserType,
                    Mobile = usr.Mobile,
                    GroupMasterId = usr.GroupId ?? Guid.Empty,
                    TillNumber = usr.TillNumber
                };
            }
            var items = _context.tblPurchasingClerkRoute.Where(a => a.PurchasingClerkId == dto.MasterId && a.IM_Status == (int)EntityStatus.Active);
                
            foreach (var item in items)
            {
                var _item = new PurchasingClerkRouteDTO
                {
                    MasterId = item.Id,
                    DateCreated = item.IM_DateCreated,
                    DateLastUpdated = item.IM_DateLastUpdated,
                    StatusId = item.IM_Status,
                    RouteId = item.RouteId,
                    PurchasingClerkId = item.PurchasingClerkId

                };
                dto.PurchasingClerkRoutes.Add(_item);
            }
            return dto;
        }
    }
}
