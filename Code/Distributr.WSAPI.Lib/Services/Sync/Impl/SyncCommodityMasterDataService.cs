using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncCommodityMasterDataService: ISyncCommodityMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCommodityMasterDataService(CokeDataContext context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<CommodityDTO> GetCommodity(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CommodityDTO>();
            response.MasterData = new SyncMasterDataInfo<CommodityDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.Commodity.ToString();
            try
            {
                var query = _context.tblCommodity.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

               

                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    var deletedQuery = _context.tblCommodity.AsQueryable();
                    deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                        && (n.IM_Status == (int)EntityStatus.Deleted))
                        .OrderBy(s => s.IM_DateCreated);
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }
                response.MasterData.MasterDataItems = query.ToList().Select(s => Map(s,q.From)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private CommodityDTO Map(tblCommodity tbl,DateTime from)
        {
            var dto = new CommodityDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Name = tbl.Name,
                CommodityTypeId = tbl.CommodityTypeId,
                Code = tbl.Code,
                Description = tbl.Description,
                CommodityGrades = new List<CommodityGradeDTO>(),
            };
            var items = tbl.tblCommodityGrade.Where(s => 
                s.IM_Status == (int)EntityStatus.Active && s.CommodityId == dto.MasterId);
            foreach( var item in items)
            {
                var grade = new CommodityGradeDTO
                {   
                    MasterId = item.Id,
                    DateCreated = item.IM_DateCreated,
                    DateLastUpdated = item.IM_DateLastUpdated,
                    StatusId = item.IM_Status,
                    Name = item.Name,
                    UsageTypeId = item.UsageTypeId,
                    Code = item.Code,
                    Description = item.Description,
                    CommodityId = item.CommodityId
                };
                dto.CommodityGrades.Add(grade);
            }
            dto.DeletedCommodityGradesItem = tbl.tblCommodityGrade.Where(n => n.IM_Status == (int)EntityStatus.Deleted && n.CommodityId==tbl.Id && n.IM_DateLastUpdated > from).Select(s => s.Id).ToList();
            return dto;
        }

    }
}
