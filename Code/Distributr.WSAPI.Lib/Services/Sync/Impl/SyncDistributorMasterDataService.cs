using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncDistributorMasterDataService : ISyncDistributorMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncDistributorMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<DistributorDTO> GetDistributor(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<DistributorDTO>();
            response.MasterData = new SyncMasterDataInfo<DistributorDTO>();
            response.MasterData.EntityName = MasterDataCollective.Distributor.ToString();
            try
            {
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int) CostCentreType.Distributor
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.Distributor
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(n => n.IM_DateCreated);

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

        private DistributorDTO Map(tblCostCentre tbl)
        {
            var dto = new DistributorDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              CostCentreCode = tbl.Cost_Centre_Code,
                              ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                              CostCentreTypeId = tbl.CostCentreType ?? 0,
                              ProducerId = tbl.ParentCostCentreId ?? Guid.Empty,
                              Owner = tbl.Distributor_Owner,
                              PIN = tbl.Distributor_PIN,
                              AccountNo = tbl.Cost_Centre_Code,
                              RegionMasterId = tbl.Distributor_RegionId ?? Guid.Empty,
                              ASMUserMasterId = tbl.Distributor_ASM_Id,
                              SalesRepUserMasterId = tbl.SalesRep_Id,
                              SurveyorUserMasterId = tbl.Surveyor_Id,
                              ProductPricingTierMasterId = tbl.Tier_Id ?? Guid.Empty,
                              PaybillNumber = tbl.PaybillNumber,
                              MerchantNumber = tbl.MerchantNumber
                          };
            var asm = _context.tblUsers.FirstOrDefault(n => n.Id == dto.ASMUserMasterId);
            var salesRep = _context.tblUsers.FirstOrDefault(n => n.Id == dto.SalesRepUserMasterId);
            var surveyor = _context.tblUsers.FirstOrDefault(n => n.Id == dto.SurveyorUserMasterId);
            if (salesRep != null)
            {
                dto.SalesRepDTO = new UserDTO
                                      {
                                          MasterId = salesRep.Id,
                                          DateCreated = salesRep.IM_DateCreated,
                                          DateLastUpdated = salesRep.IM_DateLastUpdated ?? DateTime.Now,
                                          StatusId = salesRep.IM_Status,
                                          Username = salesRep.UserName,
                                          CostCentre = salesRep.CostCenterId,
                                          Password = salesRep.Password,
                                          PIN = salesRep.PIN,
                                          UserTypeId = salesRep.UserType,
                                          Mobile = salesRep.Mobile,
                                          GroupMasterId = salesRep.GroupId ?? Guid.Empty,
                                          TillNumber = salesRep.TillNumber
                                      };
            }
            if (asm != null)
            {
                dto.ASMDTO = new UserDTO
                                      {
                                          MasterId = asm.Id,
                                          DateCreated = asm.IM_DateCreated,
                                          DateLastUpdated = asm.IM_DateLastUpdated ?? DateTime.Now,
                                          StatusId = asm.IM_Status,
                                          Username = asm.UserName,
                                          CostCentre = asm.CostCenterId,
                                          Password = asm.Password,
                                          PIN = asm.PIN,
                                          UserTypeId = asm.UserType,
                                          Mobile = asm.Mobile,
                                          GroupMasterId = asm.GroupId ?? Guid.Empty,
                                          TillNumber = asm.TillNumber
                                      };
            }
            if (surveyor != null)
            {
                dto.SurveyorDTO = new UserDTO
                {
                    MasterId = surveyor.Id,
                    DateCreated = surveyor.IM_DateCreated,
                    DateLastUpdated = surveyor.IM_DateLastUpdated ?? DateTime.Now,
                    StatusId = surveyor.IM_Status,
                    Username = surveyor.UserName,
                    CostCentre = surveyor.CostCenterId,
                    Password = surveyor.Password,
                    PIN = surveyor.PIN,
                    UserTypeId = surveyor.UserType,
                    Mobile = surveyor.Mobile,
                    GroupMasterId = surveyor.GroupId ?? Guid.Empty,
                    TillNumber = surveyor.TillNumber
                };
            }
            return dto;
        }
    }
}
