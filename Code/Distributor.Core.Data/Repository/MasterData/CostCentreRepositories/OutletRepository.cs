using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility;
using Distributr.Core.Data.MappingExtensions;
using log4net;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class OutletRepository: CostCentreRepository, IOutletRepository
    {
        
        public OutletRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository _userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, _userRepository, contactRepository)
        {
           
        }

        public List<Outlet> GetByDistributor(Guid distributorId, bool includeDeactivated = false)
        {
            _log.DebugFormat("Listing Outlets by: {0}",distributorId );
              return GetByParentId(distributorId)
                .OfType<Outlet>()
                .ToList();
        }

        public List<Outlet> GetByRoute(Guid routeId, bool includeDeactivated = false)
        {
            _log.DebugFormat("Listing Outlets By RouteID: {0}", routeId);
            List<Outlet> outlets = base.GetAll(includeDeactivated)
                .OfType<Outlet>()
                .Where(n => n.Route != null && n.Route.Id == routeId)
                .ToList();

            return outlets;
        }


        public List<Outlet> GetByOutletType(Guid outletTypeId, bool includeDeactivated = false)
        {
            _log.DebugFormat("Listing Outlets By OutletType: {0}", outletTypeId);
            List<Outlet> outlets = base.GetAll(includeDeactivated)
                .OfType<Outlet>()
                .Where(n => n.OutletType != null && n.OutletType.Id == outletTypeId)
                .ToList();

            return outlets;
        }

        public List<Outlet> GetByOutletCategory(Guid categoryId, bool includeDeactivated = false)
        {
            _log.DebugFormat("Listing Outlets By OutletCategory: {0}", categoryId);
            List<Outlet> outlets = base.GetAll(includeDeactivated).
                OfType<Outlet>().
                Where(n => n.OutletCategory!=null && n.OutletCategory.Id == categoryId).
                ToList();
            
           
            return outlets ;
        }

        public List<Outlet> GetByFreetextName(string outletName, bool includeDeactivated = false)
        {
            _log.DebugFormat("Get By Free Text Name {0}", outletName);
            var outlets = base.GetAll(includeDeactivated)
                .OfType<Outlet>()
                .Where(n => n.Name.Contains(outletName));
            //var qry = _ctx.tblCostCentre as IQueryable<tblCostCentre>;
            ////chris ...
            //List<Outlet> olist = (List<Outlet>) qry.Where(
            //                                            n =>
            //                                            includeDeactivated
            //                                                ? (((int) CostCentreType.Outlet) == n.CostCentreType &&
            //                                                   n.Name.Contains(outletName))
            //                                                : (((int) CostCentreType.Outlet) == n.CostCentreType &&
            //                                                   n.Name.Contains(outletName) && n.IM_Status==(int)EntityStatus.Active))
            //    .ToList().Select(n => Map(n) as Outlet)
            //    .ToList();

            return outlets.ToList();
        }

        public QueryResult<Outlet> Query(QueryStandard q, Guid? distId = null)
        {
            IQueryable<tblCostCentre> outletQuery;
            outletQuery = _ctx.tblCostCentre.Where(n => n.CostCentreType == (int) CostCentreType.Outlet).AsQueryable();
            if (q.ShowInactive)
                outletQuery = outletQuery.Where(n => n.IM_Status != (int) EntityStatus.Deleted);
            else
                outletQuery = outletQuery.Where(m => m.IM_Status == (int) EntityStatus.Active);

            if (distId != null && distId != Guid.Empty)
            {
                outletQuery = outletQuery.Where(s => s.ParentCostCentreId == distId);
            }

            if (!string.IsNullOrWhiteSpace(q.Name))
                outletQuery = outletQuery.Where(s => s.Name.ToLower().Contains(q.Name.ToLower()));

            outletQuery = outletQuery.OrderBy(s => s.Name);

            var queryResult = new QueryResult<Outlet>();
            queryResult.Count = outletQuery.Count();

            if (q.Skip.HasValue && q.Take.HasValue)
                outletQuery = outletQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = outletQuery.ToList();

            queryResult.Data = result.Select(Map).OfType<Outlet>().ToList();

            q.ShowInactive = false;

            return queryResult;
        }
        
        //Setting an Outlet Inactive
        //public void SetInactive(CostCentre entity)
        //{
        //    _log.Debug("InActivating Outlet");
        //    tblCostCentre  cc = _ctx.tblCostCentre .First(n => n.Id == entity.Id);
        //    if (cc != null)
        //    {
        //        cc.IM_Status = (int)EntityStatus.Inactive;// false;
        //        cc.IM_DateLastUpdated = DateTime.Now;
        //        _ctx.SaveChanges();
        //    }
        //    base.SetInactive(entity);
        //    //_cacheProvider.InvalidateRegion(_cacheRegion);
        //}


        //public void SetAsDeleted(CostCentre entity)
        //{
        //    throw new NotImplementedException();
        //}
       
    }
}
