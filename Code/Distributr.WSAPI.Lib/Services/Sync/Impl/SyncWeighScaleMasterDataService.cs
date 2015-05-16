using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncWeighScaleMasterDataService :SyncMasterDataBase, ISyncWeighScaleMasterDataService
    {
        private readonly CokeDataContext _context;


        public SyncWeighScaleMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<WeighScaleDTO> GetWeighScale(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<WeighScaleDTO>();
            response.MasterData = new SyncMasterDataInfo<WeighScaleDTO>();
            response.MasterData.EntityName = MasterDataCollective.WeighScale.ToString();
            try
            {
               

                var query = _context.tblEquipment.Where(s => s.EquipmentType == (int)EquipmentType.WeighingScale).AsQueryable();

                var deletedQuery = _context.tblEquipment.Where(s => s.EquipmentType == (int)EquipmentType.WeighingScale).AsQueryable();

                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        
                        case CostCentreType.Hub:
                            query = query.Where(n => n.CostCentreId == costCentre.Id);
                            deletedQuery = deletedQuery.Where(n => n.CostCentreId == costCentre.Id);
                            break;
                        case CostCentreType.PurchasingClerk:
                           
                            query = query.Where(n => n.CostCentreId==costCentre.TblCostCentre.ParentCostCentreId);
                            deletedQuery = deletedQuery.Where(n => n.CostCentreId == costCentre.TblCostCentre.ParentCostCentreId);
                            break;
                    }
                }
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(n => n.IM_DateCreated);

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

        private WeighScaleDTO Map(tblEquipment tbl)
        {
            var dto = new WeighScaleDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Name = tbl.Name,
                Code = tbl.Code,
                Description = tbl.Description,
                EquipmentNumber = tbl.EquipmentNumber,
                EquipmentTypeId = tbl.EquipmentType,
                HubId = tbl.CostCentreId,
                Make = tbl.Make,
                Model = tbl.Model
            };
            return dto;
        }
    }
}
