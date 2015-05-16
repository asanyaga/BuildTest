using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
    internal class CommoditySupplierInventoryRepository : ICommoditySupplierInventoryRepository
    {
        protected IQueryable<tblCommoditySupplierInventory> CommoditySupplierInventory;
        protected CokeDataContext _ctx;
        protected IQueryable<tblCostCentre> CostCentre;
        protected IQueryable<tblCommodity> Commodity;
        protected IQueryable<tblCommodityGrade> CommodityGrade;
        
        

        public CommoditySupplierInventoryRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository)

        {
            _ctx = ctx;
            CommoditySupplierInventory = _ctx.tblCommoditySupplierInventory;
            CostCentre = _ctx.tblCostCentre;
            Commodity = _ctx.tblCommodity;
            CommodityGrade = _ctx.tblCommodityGrade;
        }



        public QueryResult<CommoditySupplierInventoryLevel> Query(QueryCommoditySupplierInventory query)
        {
            var commoditySupplierInventoryQuery = CommoditySupplierInventory.AsQueryable();

            var queryResult = new QueryResult<CommoditySupplierInventoryLevel>();

            if (query.CommoditySupplierId != Guid.Empty)
            {
              commoditySupplierInventoryQuery= commoditySupplierInventoryQuery.Where(p => p.SupplierId == query.CommoditySupplierId);
            }
            if(query.StoreId!=Guid.Empty)
            {
                commoditySupplierInventoryQuery=commoditySupplierInventoryQuery.Where(p => p.WareHouseId == query.StoreId);
            }
            queryResult.Count = commoditySupplierInventoryQuery.Count();
            commoditySupplierInventoryQuery = commoditySupplierInventoryQuery.OrderBy(s => s.CommodityId).ThenBy(s => s.GradeId);
            if (query.Skip.HasValue && query.Take.HasValue)
                commoditySupplierInventoryQuery = commoditySupplierInventoryQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            var result = commoditySupplierInventoryQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CommoditySupplierInventoryLevel>().ToList();
          
            return queryResult;
        }

        private CommoditySupplierInventoryLevel Map(tblCommoditySupplierInventory tbl)
        {
            var _warehouse = CostCentre.FirstOrDefault(p => p.CostCentreType == (int) CostCentreType.Store && p.Id==tbl.WareHouseId);
             var _commoditySupplier = CostCentre.FirstOrDefault(p => p.CostCentreType == (int) CostCentreType.CommoditySupplier && p.Id==tbl.SupplierId);
            var _commodity = Commodity.FirstOrDefault(p => p.Id == tbl.CommodityId);
            var _commodityGrade = CommodityGrade.FirstOrDefault(p => p.Id == tbl.GradeId);
            var commoditySupplierInventoryLevel = new CommoditySupplierInventoryLevel(tbl.id)
            {
                Warehouse = _warehouse!=null?_warehouse.Name:"",
                //Check if commodity supplier is the sole farmer then add his name there
                CommoditySupplier = _commoditySupplier!=null?_commoditySupplier.Name:"",
                Commodity = _commodity!=null?_commodity.Name:"",
                Grade = _commodityGrade!=null?_commodityGrade.Name:"",
                Balance = tbl.Balance.ToString(),
                
            };

            return commoditySupplierInventoryLevel;
        }

        //private SourcingDocument Map(tblSourcingDocument tbldoc)
        //{
        //    SourcingDocument doc = null;
        //    doc = PrivateConstruct<CommodityWarehouseStorageNote>(tbldoc.Id);
        //    doc.DisableAddCommands();


        //    CommodityWarehouseStorageNote commodityWarehouseStorageNote = doc as CommodityWarehouseStorageNote;
        //    if (commodityWarehouseStorageNote != null)
        //    {
        //        commodityWarehouseStorageNote.DriverName = tbldoc.DriverName;
        //        commodityWarehouseStorageNote.VehiclRegNo = tbldoc.VehicleRegNo;
        //        commodityWarehouseStorageNote.CommodityOwnerId = tbldoc.CommodityOwnerId;
        //        if (tbldoc.RouteId != null) commodityWarehouseStorageNote.RouteId = tbldoc.RouteId.Value;
        //        if (tbldoc.CentreId != null) commodityWarehouseStorageNote.CentreId = tbldoc.CentreId.Value;
        //        tbldoc.tblSourcingLineItem.Select(MapLineItem).ToList().ForEach(s => commodityWarehouseStorageNote.AddLineItem(s));
        //        doc = commodityWarehouseStorageNote;
        //    }
        //    _Map(tbldoc, doc);
        //    doc.EnableAddCommands();
        //    return doc;

        //}
      
        //public SourcingDocument GetById(Guid Id)
        //{

        //    _log.DebugFormat("Getting by Id:{0}", Id);
        //    tblSourcingDocument tbldoc = documents.FirstOrDefault(n => n.Id == Id);
        //    if (tbldoc == null)
        //        return null;
        //    SourcingDocument ord = Map(tbldoc);
        //    return ord;
        //}
    }
}