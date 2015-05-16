using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncUserMasterDataService : SyncMasterDataBase, ISyncUserMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncUserMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<UserDTO> GetUser(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<UserDTO>();
            response.MasterData = new SyncMasterDataInfo<UserDTO>();
            response.MasterData.EntityName = MasterDataCollective.User.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblUsers.AsQueryable();
                query = query.Where(s => s.tblCostCentre.IM_Status == (int)EntityStatus.Active  || s.tblCostCentre.IM_Status==(int)EntityStatus.Inactive).Where(n => n.IM_DateLastUpdated > myQuery.From
                    || n.PassChange==myQuery.PassChange && (n.IM_Status == (int) EntityStatus.Active || n.IM_Status == (int) EntityStatus.Inactive));
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            var userids =_context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.DHubUsers,
                                    costCentre.Id)).ToList();
                               
                            query = query.Where(n =>  userids.Contains(n.Id));
                            break;
                        case CostCentreType.DistributorSalesman:
                            if(costCentre.TblCostCentre.CostCentreType2==(int)DistributorSalesmanType.Stockist)
                            {
                                
                                var suserids = _context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.MobileUsers,
                                  costCentre.Id, (int)DistributorSalesmanType.Stockist)).ToList();
                                query = query.Where(n => suserids.Contains(n.Id));
                            }
                            else if (costCentre.TblCostCentre.CostCentreType2 == (int)DistributorSalesmanType.StockistSalesman)
                            {
                                var suserids = _context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.MobileUsers,
                                 costCentre.Id, (int)DistributorSalesmanType.StockistSalesman)).ToList();
                                query = query.Where(n => suserids.Contains(n.Id));
                            }
                            else
                            {
                                var suserids = _context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.MobileUsers,
                                 costCentre.Id, 0)).ToList();
                                query = query.Where(n => suserids.Contains(n.Id));
                            }
                           
                            break;
                        case CostCentreType.PurchasingClerk:
                            var userCcIds = new List<Guid>();
                            var driverCcIds = _context.tblUsers.Where(n => n.UserType == (int) UserType.Driver)
                                .Select(n => n.CostCenterId).ToList();
                            userCcIds.AddRange(driverCcIds);
                            userCcIds.Add(costCentre.Id);
                            query = query.Where(n => userCcIds.Contains(n.CostCenterId));
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
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

        private UserDTO Map(tblUsers tbl)
        {
            var dto = new UserDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated ?? DateTime.Now,
                              StatusId = tbl.IM_Status,
                              CostCentre = tbl.CostCenterId,
                              GroupMasterId = tbl.GroupId ?? Guid.Empty,
                              Mobile = tbl.Mobile,
                              PIN = tbl.PIN,
                              Password = tbl.Password,
                              PassChange = tbl.PassChange != null ?(int)tbl.PassChange : 0 ,
                              TillNumber = tbl.TillNumber,
                              UserTypeId = tbl.UserType,
                              Username = tbl.UserName
                          };
            return dto;
        }
    }
}
