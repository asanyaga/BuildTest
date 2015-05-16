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
    public class SyncProductGroupDiscountMasterDataService :SyncMasterDataBase, ISyncProductGroupDiscountMasterDataService
    {
        private readonly CokeDataContext _context;
        


        public SyncProductGroupDiscountMasterDataService(CokeDataContext context):base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<ProductGroupDiscountDTO> GetHubProductGroupDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductGroupDiscountDTO>
                                 {
                                     MasterData =
                                         new SyncMasterDataInfo<ProductGroupDiscountDTO>
                                             {EntityName = MasterDataCollective.ProductGroupDiscount.ToString()}
                                 };
          
              try
              {
                 
                  //var deletedData = _context.ExecuteStoreQuery<ProductGroupDiscountDTO>(SyncQueryStringsUtil.GetDeletedProductGroupDiscountQuery).AsQueryable();

                  var syncostcentre = GetSyncCostCentre(myQuery.ApplicationId);
                  if (syncostcentre != null)
                  {
                      string querysql = string.Format(SyncResources.SyncResource.DHubGroupProdcutDiscount,
                          syncostcentre.Id, myQuery.From.ToString("yyyy-MM-dd HH:mm:ss"));
                      var data = _context.ExecuteStoreQuery<ProductGroupDiscountDTO>(querysql).AsQueryable();
                      if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                          data = data.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value).AsQueryable();



                      response.MasterData.MasterDataItems = data.ToArray();
                      response.ErrorInfo = "Success";

                  }

                
              }
              catch (Exception ex)
              {
                  response.ErrorInfo = ex.Message;
              }
              response.MasterData.LastSyncTimeStamp = DateTime.Now;
            
              return response;
            
        }

        public SyncResponseMasterDataInfo<ProductGroupDiscountDTO> GetProductGroupDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductGroupDiscountDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<ProductGroupDiscountDTO>
                                           {EntityName = MasterDataCollective.ProductGroupDiscount.ToString()}
                               };
            try
            {

                var query = _context.tblProductDiscountGroup.Where(s=>s.ProductRef.HasValue).AsQueryable();
                var deletedQuery = _context.tblProductDiscountGroup.AsQueryable();
                var syncostcentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (syncostcentre != null)
                {
                    var discountGroups = DiscountGroups(syncostcentre);
                    query = query.Where(s => discountGroups.Contains(s.DiscountGroup));
                    deletedQuery = deletedQuery.Where(s => discountGroups.Contains(s.DiscountGroup));
                }
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);


                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }

                var list = new ArrayList();
                foreach (var item in query.ToArray())
                {
                    list.Add(Map(item));
                }
                response.MasterData.MasterDataItems = list.Cast<ProductGroupDiscountDTO>().ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;

            return response;
            
        }

        private List<Guid?> OutletDiscountGroups(SyncCostCentre syncostcentre,DateTime from)
        {
            var discountGroups=new List<Guid?>();
            if (syncostcentre.CostCentreType == CostCentreType.Distributor)
            {
                discountGroups =
                    _context.tblCostCentre.Where(s => s.CostCentreType == 5 && s.IM_DateLastUpdated > from
                                                      && s.ParentCostCentreId == syncostcentre.Id
                                                      &&
                                                      (s.IM_Status == (int) EntityStatus.Active ||
                                                       s.IM_Status == (int) EntityStatus.Inactive) &&
                                                      s.Outlet_DiscountGroupId != Guid.Empty)
                        .Select(s => s.Outlet_DiscountGroupId).Distinct().ToList();
            }
            else if (syncostcentre.CostCentreType == CostCentreType.DistributorSalesman)
            {
                var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncostcentre.Id
                                                                   && n.IM_Status == (int) EntityStatus.Active).Select(
                                                                       n => n.RouteId).Distinct().ToList();
                discountGroups = _context.tblCostCentre.Where(s => routeIds.Contains(s.RouteId.Value) && s.IM_DateLastUpdated > from && 
                                                                   (s.IM_Status == (int) EntityStatus.Active ||
                                                                    s.IM_Status ==
                                                                    (int) EntityStatus.Inactive))
                    .Select(s => s.Outlet_DiscountGroupId).ToList();
                discountGroups = discountGroups.Where(p => p.HasValue && p.Value != Guid.Empty).Distinct().ToList();
            }
            return discountGroups;
        }

        private List<Guid?> DiscountGroups(SyncCostCentre syncostcentre)
        {
            var discountGroups=new List<Guid?>();
            if (syncostcentre.CostCentreType == CostCentreType.Distributor)
            {
                discountGroups =
                    _context.tblCostCentre.Where(s => s.CostCentreType == 5
                                                      && s.ParentCostCentreId == syncostcentre.Id
                                                      &&
                                                      (s.IM_Status == (int) EntityStatus.Active ||
                                                       s.IM_Status == (int) EntityStatus.Inactive) &&
                                                      s.Outlet_DiscountGroupId != Guid.Empty)
                        .Select(s => s.Outlet_DiscountGroupId).Distinct().ToList();
            }
            else if (syncostcentre.CostCentreType == CostCentreType.DistributorSalesman)
            {
                var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncostcentre.Id
                                                                   && n.IM_Status == (int) EntityStatus.Active).Select(
                                                                       n => n.RouteId).Distinct().ToList();
                discountGroups = _context.tblCostCentre.Where(s => routeIds.Contains(s.RouteId.Value) &&
                                                                   (s.IM_Status == (int) EntityStatus.Active ||
                                                                    s.IM_Status ==
                                                                    (int) EntityStatus.Inactive))
                    .Select(s => s.Outlet_DiscountGroupId).ToList();
                discountGroups = discountGroups.Where(p => p.HasValue && p.Value != Guid.Empty).Distinct().ToList();
            }
            return discountGroups;
        }

        
        List<ProductGroupDiscountItemDTO> MapItem(tblProductDiscountGroup pdg)
        {
            return
                pdg.tblProductDiscountGroupItem.Where(p => p.IM_Status == (int)EntityStatus.Active).Select(
                    item => new ProductGroupDiscountItemDTO
                    {
                        MasterId = item.id,
                        DateCreated = item.IM_DateCreated,
                        DateLastUpdated = item.IM_DateLastUpdated,
                        StatusId = item.IM_Status,
                        DiscountRate = item.DiscountRate,
                        EffectiveDate = item.EffectiveDate,
                        EndDate = item.EndDate ?? DateTime.Now,
                        ProductMasterId = item.ProductRef
                    }).ToList();

        }

        private ProductGroupDiscountDTO Map(tblProductDiscountGroup tbl)
        {
            var dto = new ProductGroupDiscountDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                DiscountGroupMasterId = tbl.DiscountGroup,
                Quantity = tbl.Quantity.Value,
                DiscountRate = tbl.DiscountRate.Value,
                EffectiveDate = tbl.EffectiveDate.Value,
                EndDate = tbl.EndDate.Value,
                ProductMasterId = tbl.ProductRef.Value,


               
            };
           
            
        
            return dto;
        }
    }
}
