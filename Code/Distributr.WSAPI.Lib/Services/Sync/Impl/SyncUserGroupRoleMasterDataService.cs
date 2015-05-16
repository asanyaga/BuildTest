using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncUserGroupRoleMasterDataService : ISyncUserGroupRoleMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncUserGroupRoleMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<UserGroupRoleDTO> GetUserGroupRole(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<UserGroupRoleDTO>();
            response.MasterData = new SyncMasterDataInfo<UserGroupRoleDTO>();
            response.MasterData.EntityName = MasterDataCollective.UserGroupRole.ToString();
            try
            {
                var query = _context.tblUserGroupRoles.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblUserGroupRoles.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }
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

        private UserGroupRoleDTO Map(tblUserGroupRoles tbl)
        {
            var dto = new UserGroupRoleDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              CanAccess = tbl.CanAccess,
                              UserGroupMasterId = tbl.GroupId,
                              UserRoleMasterId = tbl.RoleId
                          };
            return dto;
        }
    }
}
