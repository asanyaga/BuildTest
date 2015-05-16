using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.Services.Sync.Impl;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public class SyncOutletMasterDataService :SyncMasterDataBase, ISyncOutletMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncOutletMasterDataService(CokeDataContext context):base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<OutletDTO> GetOutlet(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<OutletDTO>();
            response.MasterData = new SyncMasterDataInfo<OutletDTO>();
            response.MasterData.EntityName = MasterDataCollective.Outlet.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int)CostCentreType.Outlet
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive || n.IM_Status == (int)EntityStatus.New));

                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.Outlet
                    && (n.IM_Status == (int)EntityStatus.Deleted));
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            query = query.Where(n => n.ParentCostCentreId == costCentre.Id
                                && n.IM_DateLastUpdated > myQuery.From);
                            deletedQuery = deletedQuery.Where(n => n.ParentCostCentreId == costCentre.Id
                                && n.IM_DateLastUpdated > myQuery.From);
                            break;
                        case CostCentreType.DistributorSalesman:
                            var routeIds = GetRouteIds(costCentre, myQuery.From);
                            query = query.Where(n => routeIds.Contains(n.RouteId.Value));
                            deletedQuery = deletedQuery.Where(n => routeIds.Contains(n.RouteId.Value));
                            break;
                    }
                }
                query = query.OrderBy(s => s.IM_DateCreated);
                deletedQuery = deletedQuery.OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }

                var list = new ArrayList();
                foreach (var item in query.ToArray())
                {
                    list.Add(Map(item));
                }
                response.MasterData.MasterDataItems = list.Cast<OutletDTO>().ToArray();
               // response.MasterData.MasterDataItems = query.ToList().Select(n => Map(n)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private OutletDTO Map(tblCostCentre tbl)
        {
            var dto = new OutletDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              CostCentreCode = tbl.Cost_Centre_Code,
                              ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                              CostCentreTypeId = tbl.CostCentreType ?? 0,
                              RouteMasterId = tbl.RouteId ?? Guid.Empty,
                              OutletCategoryMasterId = tbl.Outlet_Category_Id ?? Guid.Empty,
                              OutletTypeMasterId = tbl.Outlet_Type_Id ?? Guid.Empty,
                              DiscountGroupMasterId = tbl.Outlet_DiscountGroupId ?? Guid.Empty,
                              OutletProductPricingTierMasterId = tbl.Tier_Id ?? Guid.Empty,
                              VatClassMasterId = tbl.VATClass_Id ?? Guid.Empty,
                              Latitude = tbl.StandardWH_Latitude,
                              Longitude = tbl.StandardWH_Longtitude,
                              IsApproved = tbl.IM_Status == (int)EntityStatus.Active,
                              ShippingAddresses = new List<ShipToAddressDTO>(),
                              SpecialPricingTierMasterId = tbl.SpecialPricingTierId ?? Guid.Empty,

                          };
            foreach (var item in tbl.tblShipToAddress.Where(n => n.IM_Status == (int)EntityStatus.Active))
            {
                var addressitem = new ShipToAddressDTO
                                      {
                                          OutletId = dto.MasterId,
                                          MasterId = item.Id,
                                          DateCreated = item.IM_DateCreated,
                                          DateLastUpdated = item.IM_DateLastUpdated,
                                          StatusId = item.IM_Status,
                                          Name = item.Name,
                                          Code = item.Code,
                                          Description = item.Description,
                                          PostalAddress = item.PostalAddress,
                                          PhysicalAddress = item.PhysicalAddress,
                                          Longitude = item.Longitude ?? 0,
                                          Latitude = item.Latitude ?? 0
                                      };
                dto.ShippingAddresses.Add(addressitem);
            }
            return dto;
        }
    }
}
