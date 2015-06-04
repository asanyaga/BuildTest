using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Utility.MasterData
{
    public enum ServiceType { Cummulative=1,Other=2}
    public class QueryActivityResult
    {
        public QueryActivityResult()
        {
            Data = new List<ActivityDocument>();
        }

        public List<ActivityDocument> Data { get; set; }
        public int Count { get; set; }

    }
    public class QueryResult
    {
        public QueryResult()
        {
            Data = new List<MasterEntity>();
        }

        public List<MasterEntity> Data { get; set; }
        public int Count { get; set; }

    }
    public class QueryResult<T> where T : class
    {
        public QueryResult()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; set; }
        public int Count { get; set; }

    }
    public abstract class QueryBase
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

    }

    public class QueryMasterData : QueryBase
    {
        public QueryMasterData()
        {
            IsFirstSync = false;
        }
        public DateTime From { get; set; }
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public int PassChange { get; set; }
        public string Description { get; set; }
        public bool IsFirstSync { get; set; }
        public Guid? CostCentreId { get; set; }

    }
    public class QueryActivity : QueryBase
    {
        public Guid? ActivityTypeId { get; set; }
    }
    public class QueryCommoditySupplier : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryStore : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryCommodityOwner : QueryBase
    {
        public Guid? SupplierId { get; set; }
        public Guid? CommodityOwnerId { get; set; }
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QuerySupplierContact : QueryBase
    {
        public Guid? SupplierId { get; set; }
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryCommodityProducer : QueryBase
    {
        public Guid? SupplierId { get; set; }
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }
    public class QueryPurchasingClerkRoute : QueryBase
    {
        public Guid? PurchasingClerkId { get; set; }
        public Guid? RouteId { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QuerySeason : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryShift : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }
    public class QueryActivityType : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }
    public class QueryCommodityProducerService : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryInfection : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryCostCentre : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }

        public Guid CostCentreId { get; set; }
        public List<int> ListOfCostCentreTypeIds { get; set; }
    }
    public class QueryServiceProvider : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryStandard:QueryBase
    {
        public Guid? SupplierId { get; set; }
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
        public int? UserType { get; set; }
    }
    public class QueryDocument : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
        public int DocumentSourcingStatusId { get; set; }
    }
    public class QueryCommoditySupplierInventory : QueryBase
    {
        public Guid CommoditySupplierId { get; set; }
        public Guid StoreId { get; set; }
    }
    public class QueryUsers : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
        public List<int> UserTypes { get; set; }
    }

    public class QueryOrders : QueryStandard
    {
        public Guid? Distributr { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public OrderType OrderType { get; set; }
        public DocumentStatus DocumentStatus { get; set; }
    }


  

    public class QueryEquipment : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
        public int EquipmentType { get; set; }
    }

    public class QueryGrade : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
        public Guid CommodityId { get; set; }
    }

    public class QueryFOCDiscount : QueryStandard
    {
        public Guid? BrandId { get; set; }
    }
    public class QueryVatClassLineItems : QueryBase
    {
        public Guid VatClassId { get; set; }
        public bool ShowInactive { get; set; }
    }

    public class QueryUnderBanking : QueryBase
    {
        public QueryUnderBanking()
        {
            
        }
        public Guid? CostCentreId { get; set; }
        public Guid? SalesmanId { get; set; }

    }

    public class ThirdPartyMasterDataQuery:QueryBase
    {
        public ThirdPartyMasterDataQuery()
        {
            SearchTextList=new List<string>();
        }

        public List<string> RequestedFields { get; set; } 
        public DateTime From { get; set; }
        public MasterDataCollective MasterCollective { get; set; }
        public List<string> SearchTextList { get; set; }
        
    }

    public class QueryOutletOrder : QueryBase
    {
        public QueryOutletOrder()
        {
            
        }
       
        public Guid OutletId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Name { get; set; }
    }

    public class QueryFarmerDetails
    {
        public string Mobile { get; set; }
        public ServiceType ServiceType { get; set; }
        public string FarmerCode { get; set; }
        public string Month { get; set; }
    }
}
