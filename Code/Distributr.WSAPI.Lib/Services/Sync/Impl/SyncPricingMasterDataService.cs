using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    public class SyncPricingMasterDataService :SyncMasterDataBase, ISyncPricingMasterDataService
    {
        private readonly CokeDataContext _context;

        
        public SyncPricingMasterDataService(CokeDataContext context):base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProductPricingDTO> GetPricing(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<ProductPricingDTO>();
            response.MasterData = new SyncMasterDataInfo<ProductPricingDTO>();;
            response.MasterData.EntityName = MasterDataCollective.Pricing.ToString();
            try
            {
                var query = _context.tblPricing.AsQueryable();
                query = query.Where(n => 
                    n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive);

                var deletedQuery = _context.tblPricing.AsQueryable();
                deletedQuery = deletedQuery.Where(n =>
                    n.IM_Status == (int)EntityStatus.Deleted);

                var syncostcentre = GetSyncCostCentre(q.ApplicationId);
                if (syncostcentre != null)
                {
                    var pricingTiers = GetApplicablePricingTiers(syncostcentre);

                    query = query.Where(s => s.IM_DateLastUpdated > q.From && pricingTiers.Contains(s.Tier));
                    deletedQuery = deletedQuery.Where(s => s.IM_DateLastUpdated > q.From && pricingTiers.Contains(s.Tier));
                }
                query = query.OrderBy(s => s.IM_DateCreated);
                deletedQuery = deletedQuery.OrderBy(s => s.IM_DateCreated);

                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }

                var list = new ArrayList();
                foreach (var item in query.ToArray())
                {
                    list.Add(Map(item));
                }
                response.MasterData.MasterDataItems = list.Cast<ProductPricingDTO>().ToArray();
                
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }
        private List<Guid?> GetApplicablePricingTiers(SyncCostCentre syncostcentre)
        {
            var outLetTiers = new List<Guid?>();
            if (syncostcentre.CostCentreType == CostCentreType.Distributor)
            {
                var tiers =
                         _context.tblCostCentre.Where(s => s.CostCentreType == 5
                                                           && s.ParentCostCentreId == syncostcentre.Id
                                                           &&
                                                           (s.IM_Status == (int)EntityStatus.Active ||
                                                            s.IM_Status == (int)EntityStatus.Inactive))
                             .Select(s => new { s.Tier_Id, s.SpecialPricingTierId }).ToList();
                outLetTiers = tiers.Select(n => n.Tier_Id).Distinct().ToList();
                outLetTiers.AddRange(tiers.Select(n => n.SpecialPricingTierId).Distinct().ToList());
            }
            else if (syncostcentre.CostCentreType == CostCentreType.DistributorSalesman)
            {
                var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncostcentre.Id
                                                                   && n.IM_Status == (int)EntityStatus.Active).Select(
                                                                       n => n.RouteId).Distinct().ToList();
               var tiers = _context.tblCostCentre.Where(s => routeIds.Contains(s.RouteId.Value) &&
                                                                   (s.IM_Status == (int)EntityStatus.Active ||
                                                                    s.IM_Status ==
                                                                    (int)EntityStatus.Inactive))
                    .Select(s => new { s.Tier_Id, s.SpecialPricingTierId }).ToList();
               outLetTiers = tiers.Select(n => n.Tier_Id).Distinct().ToList();
               outLetTiers.AddRange(tiers.Select(n => n.SpecialPricingTierId).Distinct().ToList());
               outLetTiers = outLetTiers.Where(p => p.HasValue && p.Value !=Guid.Empty).Distinct().ToList();
            }
            return outLetTiers;
        }
        private List<Guid?> GetChangeOutletiers(SyncCostCentre syncostcentre,DateTime fromdate)
        {
            var outLetTiers = new List<Guid?>();
            if (syncostcentre.CostCentreType == CostCentreType.Distributor)
            {
                var tiers =
                         _context.tblCostCentre.Where(s => s.IM_DateLastUpdated > fromdate && s.CostCentreType == 5
                                                           && s.ParentCostCentreId == syncostcentre.Id
                                                           &&
                                                           (s.IM_Status == (int)EntityStatus.Active ||
                                                            s.IM_Status == (int)EntityStatus.Inactive))
                             .Select(s => new { s.Tier_Id, s.SpecialPricingTierId }).ToList();
                outLetTiers = tiers.Select(n => n.Tier_Id).Distinct().ToList();
                outLetTiers.AddRange(tiers.Select(n => n.SpecialPricingTierId).Distinct().ToList());
            }
            else if (syncostcentre.CostCentreType == CostCentreType.DistributorSalesman)
            {
                var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncostcentre.Id
                                                                   && n.IM_Status == (int)EntityStatus.Active).Select(
                                                                       n => n.RouteId).Distinct().ToList();
                var tiers = _context.tblCostCentre.Where(s =>s.IM_DateLastUpdated > fromdate && routeIds.Contains(s.RouteId.Value) &&
                                                                    (s.IM_Status == (int)EntityStatus.Active ||
                                                                     s.IM_Status ==
                                                                     (int)EntityStatus.Inactive))
                     .Select(s => new { s.Tier_Id, s.SpecialPricingTierId }).ToList();
                outLetTiers = tiers.Select(n => n.Tier_Id).Distinct().ToList();
                outLetTiers.AddRange(tiers.Select(n => n.SpecialPricingTierId).Distinct().ToList());
                outLetTiers = outLetTiers.Where(p => p.HasValue && p.Value != Guid.Empty).Distinct().ToList();
            }
            return outLetTiers;
        }
        public SyncResponseMasterDataInfo<ProductPricingDTO> GetHubPricing(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductPricingDTO>
                               {MasterData = new SyncMasterDataInfo<ProductPricingDTO>()};
            response.MasterData.EntityName = MasterDataCollective.Pricing.ToString();
            try
            {

                var query = _context.ExecuteStoreQuery<ProductPricingDTO>(SyncQueryStringsUtil.GetProductPricingQuery).AsQueryable();

                var syncostcentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (syncostcentre != null)
                {
                    var pricingTiers = GetApplicablePricingTiers(syncostcentre);
                    var changeOutletiers = GetChangeOutletiers(syncostcentre, myQuery.From);
                    query = query.Where(s =>( s.DateLastUpdated > myQuery.From || changeOutletiers.Contains(s.ProductPricingTierMasterId)) && pricingTiers.Contains(s.ProductPricingTierMasterId));
                }
                query = query.OrderBy(s => s.DateCreated);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                response.MasterData.MasterDataItems = query.ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;

            return response;
        }

       

        private ProductPricingDTO Map(tblPricing tbl)
        {
            var dto = new ProductPricingDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              ProductMasterId = tbl.ProductRef,
                              ProductPricingTierMasterId = tbl.Tier,
                              ProductPricingItems = new List<ProductPricingItemDTO>(),
                              StatusId = tbl.IM_Status
                          };
            var items = tbl.tblPricingItem
                .Where(s => s.IM_Status == (int) EntityStatus.Active)
                .OrderByDescending(n => n.EffecitiveDate).ToList();
            var item = items.FirstOrDefault();
            if(item != null)
            {
                var dtoitem = new ProductPricingItemDTO
                                  {
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      EffectiveDate = item.EffecitiveDate,
                                      ExFactoryRate = item.Exfactory,
                                      MasterId = item.id,
                                      ProductPricingMasterId = item.PricingId,
                                      SellingPrice = item.SellingPrice,
                                      StatusId = item.IM_Status
                                  };
                dto.ProductPricingItems.Add(dtoitem);
            }
            return dto;
        }
    }
}
