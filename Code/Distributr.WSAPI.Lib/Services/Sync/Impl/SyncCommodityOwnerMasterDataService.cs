using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
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
    public class SyncCommodityOwnerMasterDataService: SyncMasterDataBase, ISyncCommodityOwnerMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCommodityOwnerMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<CommodityOwnerDTO> GetCommodityOwner(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CommodityOwnerDTO>();
            response.MasterData = new SyncMasterDataInfo<CommodityOwnerDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.CommodityOwner.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCommodityOwner.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));

                var deletedQuery = _context.tblCommodityOwner.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Deleted));

                List<Guid> supplierIds;
                switch (costCentre.CostCentreType)
                {
                    case CostCentreType.Hub:
                        supplierIds = _context.tblCostCentre.Where(n => n.CostCentreType == (int) CostCentreType.CommoditySupplier
                            && n.ParentCostCentreId == costCentre.Id && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                            .Select(n => n.Id).ToList();
                        query = query.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(s => s.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(s => s.IM_DateCreated);
                        break;
                    case CostCentreType.PurchasingClerk:
                        supplierIds = _context.tblCostCentre.Where(n => n.CostCentreType == (int) CostCentreType.CommoditySupplier
                            && n.ParentCostCentreId == costCentre.TblCostCentre.ParentCostCentreId 
                            && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                            .Select(n => n.Id).ToList();
                        query = query.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(s => s.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(s => s.IM_DateCreated);
                        break;
                }
                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }

                response.MasterData.MasterDataItems = query.ToList().Select(Map).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }
        private CommodityOwnerDTO Map(tblCommodityOwner tbl)
        {
            var dto = new CommodityOwnerDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Code = tbl.Code,
                Surname = tbl.Surname,
                FirstName = tbl.FirstName,
                LastName = tbl.LastName,
                IdNo = tbl.IdNo,
                PinNo = tbl.PINNo,
                GenderId = tbl.Gender,
                PhysicalAddress = tbl.PhysicalAddress,
                PostalAddress = tbl.PostalAddress,
                Email = tbl.Email,
                PhoneNumber = tbl.PhoneNo,
                BusinessNumber = tbl.BusinessNo,
                FaxNumber = tbl.FaxNo,
                OfficeNumber = tbl.OfficeNo,
                Description = tbl.Description,
                DateOfBirth = tbl.DOB,
                MaritalStatusId = tbl.MaritalStatus ?? 0,
                CommoditySupplierId = tbl.CostCentreId,
                CommodityOwnerTypeId = tbl.CommodityOwnerTypeId
            };
            return dto;
        }
    }
}
