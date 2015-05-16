using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncContainerTypeMasterDataService : ISyncContainerTypeMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncContainerTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ContainerTypeDTO> GetContainerType(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ContainerTypeDTO>();
            response.MasterData = new SyncMasterDataInfo<ContainerTypeDTO>();
            response.MasterData.EntityName = MasterDataCollective.ContainerType.ToString();
            try
            {
                var query = _context.tblContainerType.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblContainerType.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }
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

        private ContainerTypeDTO Map(tblContainerType tbl)
        {
            var dto = new ContainerTypeDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Make = tbl.Make,
                              Code = tbl.Code,
                              Description = tbl.Description,
                              Model = tbl.Model,
                              LoadCariage = tbl.LoadCariage ?? 0,
                              TareWeight = tbl.TareWeight ?? 0,
                              Lenght = tbl.Lenght ?? 0,
                              Width = tbl.Width ?? 0,
                              Height = tbl.Height ?? 0,
                              BubbleSpace = tbl.BubbleSpace ?? 0,
                              Volume = tbl.Volume ?? 0,
                              FreezerTemp = tbl.FreezerTemp ?? 0,
                              CommodityGradeId = tbl.CommodityGradeId ?? Guid.Empty,
                              
                              ContainerUseTypeId = tbl.ContainerUseId ?? 0
                          };
            return dto;
        }
    }
}
