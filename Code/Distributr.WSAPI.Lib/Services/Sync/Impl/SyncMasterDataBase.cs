using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
   public class SyncMasterDataBase
    {
       private readonly CokeDataContext _context;

       public SyncMasterDataBase(CokeDataContext context)
       {
           _context = context;
       }
       public SyncCostCentre GetSyncCostCentre(Guid applicationId)
       {
           if (applicationId == Guid.Empty)
               return null;
           var app = _context.tblCostCentreApplication.FirstOrDefault(n => n.id == applicationId);
           if (app == null)
               return null;

           var costCentreId = app.costcentreid;
           var costCentre = _context.tblCostCentre.FirstOrDefault(n => n.Id == costCentreId);
           if (costCentre == null)
               return null;
           return new SyncCostCentre
                      {
                          CostCentreType = (CostCentreType) costCentre.CostCentreType.Value,
                          Id = costCentre.Id,
                          TblCostCentre = costCentre
                      };

       }
       public List<Guid> GetOutletIds(SyncCostCentre syncCostCentre)
       {
           var query = _context.tblCostCentre.AsQueryable();
           query = query.Where(s => s.CostCentreType == (int)CostCentreType.Outlet);
           if(syncCostCentre.CostCentreType==CostCentreType.Distributor)
           {
               query = query.Where(n => n.ParentCostCentreId == syncCostCentre.Id 
                   && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.DistributorSalesman)
           {
               var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncCostCentre.Id 
                   && n.IM_Status == (int)EntityStatus.Active).Select(n => n.RouteId).Distinct().ToList();
               query = query.Where(n => routeIds.Contains(n.RouteId.Value));
           }
           return query.Select(s => s.Id).ToList();
       }

       public List<Guid> GetRouteIds(SyncCostCentre syncCostCentre)
       {
           var routeIds = new List<Guid>();
           if (syncCostCentre.CostCentreType == CostCentreType.Distributor || syncCostCentre.CostCentreType == CostCentreType.Hub)
           {
               routeIds = _context.tblRoutes.Where(n => n.RegionId == syncCostCentre.TblCostCentre.Distributor_RegionId
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)) 
                   .Select(n => n.RouteID).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.DistributorSalesman)
           {
               routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Active).Select(n => n.RouteId).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.PurchasingClerk)
           {
               routeIds = _context.tblPurchasingClerkRoute.Where(n => n.PurchasingClerkId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Active).Select(n => n.RouteId).Distinct().ToList();
           }
           return routeIds;
       }


      


       public List<Guid> GetOutletIds(SyncCostCentre syncCostCentre, DateTime from)
       {
           var query = _context.tblCostCentre.AsQueryable();
           query = query.Where(s => s.CostCentreType == (int)CostCentreType.Outlet);
           if (syncCostCentre.CostCentreType == CostCentreType.Distributor)
           {
               query = query.Where(n => n.ParentCostCentreId == syncCostCentre.Id
                   && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive) 
                   && n.IM_DateLastUpdated > from);
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.DistributorSalesman)
           {
               var routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncCostCentre.Id
                   && n.IM_Status == (int)EntityStatus.Active && n.IM_DateLastUpdated > from)
                   .Select(n => n.RouteId).Distinct().ToList();
               query = query.Where(n => routeIds.Contains(n.RouteId.Value));
           }
           return query.Select(s => s.Id).ToList();
       }

       public List<Guid> GetRouteIds(SyncCostCentre syncCostCentre, DateTime from)
       {
           var routeIds = new List<Guid>();
           if (syncCostCentre.CostCentreType == CostCentreType.Distributor || syncCostCentre.CostCentreType == CostCentreType.Hub)
           {
               routeIds = _context.tblRoutes.Where(n => n.RegionId == syncCostCentre.TblCostCentre.Distributor_RegionId 
                   && (n.IM_Status == (int) EntityStatus.Active || n.IM_Status == (int) EntityStatus.Inactive) 
                   && n.IM_DateLastUpdated > from).Select(n => n.RouteID).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.DistributorSalesman)
           {
               var changedoutletRoutesIds =_context.tblCostCentre
                   .Where(s => s.CostCentreType == (int) CostCentreType.Outlet && s.IM_DateLastUpdated > from)
                   .Select(s => s.RouteId.Value).ToList();
               var changedRouteIds = _context.tblRoutes
                  .Where(s =>  s.IM_DateLastUpdated > from)
                  .Select(s => s.RouteID).ToList();
               changedoutletRoutesIds.AddRange(changedRouteIds);
               changedoutletRoutesIds = changedoutletRoutesIds.Distinct().ToList();
               routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Active && (changedoutletRoutesIds.Contains(n.RouteId) || n.IM_DateLastUpdated > from))
                    .Select(n => n.RouteId).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.PurchasingClerk)
           {
               routeIds = _context.tblPurchasingClerkRoute.Where(n => n.PurchasingClerkId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Active && n.IM_DateLastUpdated > from)
                    .Select(n => n.RouteId).Distinct().ToList();
           }
           return routeIds;
       }
    
    public List<Guid> GetRouteIdsToDelete(SyncCostCentre syncCostCentre, DateTime from)
       {
           var routeIds = new List<Guid>();
           if (syncCostCentre.CostCentreType == CostCentreType.Distributor || syncCostCentre.CostCentreType == CostCentreType.Hub)
           {
               routeIds = _context.tblRoutes.Where(n => n.RegionId == syncCostCentre.TblCostCentre.Distributor_RegionId 
                   && (n.IM_Status == (int) EntityStatus.Deleted ) 
                   && n.IM_DateLastUpdated > from).Select(n => n.RouteID).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.DistributorSalesman)
           {
               var changedoutletRoutesIds =_context.tblCostCentre
                   .Where(s => s.CostCentreType == (int) CostCentreType.Outlet && s.IM_DateLastUpdated > from)
                   .Select(s => s.RouteId.Value).ToList();
               var changedRouteIds = _context.tblRoutes
                  .Where(s =>  s.IM_DateLastUpdated > from)
                  .Select(s => s.RouteID).ToList();
               changedoutletRoutesIds.AddRange(changedRouteIds);
               changedoutletRoutesIds = changedoutletRoutesIds.Distinct().ToList();
               routeIds = _context.tblSalemanRoute.Where(n => n.SalemanId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Deleted && (changedoutletRoutesIds.Contains(n.RouteId) || n.IM_DateLastUpdated > from))
                    .Select(n => n.RouteId).Distinct().ToList();
           }
           else if (syncCostCentre.CostCentreType == CostCentreType.PurchasingClerk)
           {
               routeIds = _context.tblPurchasingClerkRoute.Where(n => n.PurchasingClerkId == syncCostCentre.Id
                    && n.IM_Status == (int)EntityStatus.Deleted && n.IM_DateLastUpdated > from)
                    .Select(n => n.RouteId).Distinct().ToList();
           }
           return routeIds;
       }
    }

    public class SyncCostCentre
    {
        public Guid Id { get; set; }
        public tblCostCentre TblCostCentre { get; set; }
        public CostCentreType CostCentreType { get; set; }
    }
}
