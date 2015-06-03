using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Value;

namespace Distributr.Core.Factory.Master.Impl
{
    public class CostCentreFactory : ICostCentreFactory
    {
        private Guid _id = Guid.Empty;
        public CostCentre CreateCostCentre(Guid id, CostCentreType costCentreType, CostCentre parentCostCentre)
        {
            _id = id;
            switch (costCentreType)
            {
                case CostCentreType.Producer:
                    return CreateProducer();
                case CostCentreType.Transporter:
                    return CreateTransporter(parentCostCentre);
                case CostCentreType.Distributor:
                    return CreateDistributor(parentCostCentre);
                case CostCentreType.DistributorSalesman:
                    return CreateDistributorSalesmanWarehouse(parentCostCentre);
                case CostCentreType.Outlet:
                    return CreateOutlet(parentCostCentre);
                case CostCentreType.DistributorPendingDispatchWarehouse:
                    return CreateDistributorPendingDispatchWarehouse(parentCostCentre);
                case CostCentreType.Hub:
                    return CreateHub(parentCostCentre);
                case CostCentreType.CommoditySupplier:
                    return CreateCommoditySupplier(parentCostCentre);
                case CostCentreType.PurchasingClerk:
                    return CreatePurchasingClerk(parentCostCentre);
                case  CostCentreType.Store:
                    return CreateStore(parentCostCentre);
            }
            return null;
        }

        private CostCentre CreateCommoditySupplier(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating CommoditySupplier entity");
            if (CostCentreType.Hub != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a CommoditySupplier must be a Hub");
            return new CommoditySupplier(_id)
            {
                CostCentreType = CostCentreType.CommoditySupplier,
                ParentCostCentre = ParentRef(parentCostCentre)
            }; 
        }

        CostCentre CreateHub(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating hub entity");
            if (CostCentreType.Producer != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a hub must be a producer");
            return new Hub(_id)
            {
                CostCentreType = CostCentreType.Hub,
                ParentCostCentre = ParentRef(parentCostCentre)
            };
        }

        CostCentre CreateProducer()
        {
            return new Producer (_id)
            {
                Contact = new List<Contact>(),
                CostCentreType = CostCentreType.Producer,
            };
        }

        CostCentre CreateTransporter(CostCentre parentCostCentre)
        {
            if(parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating transporter entity");
            if( CostCentreType.Producer != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a transporter must be a producer");
            return new Transporter(_id)
            {
                Contact = new List<Contact>(),

                CostCentreType = CostCentreType.Transporter,
                ParentCostCentre = ParentRef( parentCostCentre)
            };
        }

        CostCentre CreateDistributor(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating distributor entity");
            if (CostCentreType.Producer != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a distribtor must be a producer");
            return new Distributr.Core.Domain.Master.CostCentreEntities.Distributor(_id) 
            {
                Contact = new List<Contact>(),

                CostCentreType = CostCentreType.Distributor ,
                ParentCostCentre = ParentRef( parentCostCentre)
            };
        }

        CostCentre CreateDistributorSalesmanWarehouse(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating Distributor salesman warehouse entity");
            if (CostCentreType.Distributor != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a Distributor salesman warehouse must be a distributor");
            return new DistributorSalesman(_id)
            {
                Contact = new List<Contact>(),

                CostCentreType = CostCentreType.DistributorSalesman,
                ParentCostCentre = ParentRef( parentCostCentre)
            };
        }
        
        CostCentre CreateDistributorPendingDispatchWarehouse(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException(
                    "A distributor warehouse must be supplied as the parent cost centre when creating a distributor pending dispatch warehouse");
            if (CostCentreType.Distributor != parentCostCentre.CostCentreType)
                throw new ArgumentException("A distributor warehouse must be supplied as the parent cost centre when creating a distributor pending dispatch warehouse");
            return new DistributorPendingDispatchWarehouse(_id)
                       {
                           Contact = new List<Contact>(),
                           CostCentreType = CostCentreType.DistributorPendingDispatchWarehouse,
                           ParentCostCentre = ParentRef(parentCostCentre)
                       };
        }

        CostCentre CreateOutlet(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating outlet entity");
            if (CostCentreType.Distributor != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of an outlet must be a distributor");
            return new Outlet(_id) 
            {
                Contact = new List<Contact>(),

                CostCentreType = CostCentreType.Outlet,
                ParentCostCentre = ParentRef( parentCostCentre)
            };
           
        }

        CostCentreRef ParentRef(CostCentre parentCostCentre)
        {
            return new CostCentreRef { Id = parentCostCentre.Id };
        }

        CostCentre CreatePurchasingClerk(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating PurchasingClerk warehouse entity");
            if (CostCentreType.Hub != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a PurchasingClerk warehouse must be a Hub");
            return new PurchasingClerk(_id)
            {
                Contact = new List<Contact>(),
                CostCentreType = CostCentreType.PurchasingClerk,
                ParentCostCentre = ParentRef(parentCostCentre)
            };
        }

        CostCentre CreateStore(CostCentre parentCostCentre)
        {
            if (parentCostCentre == null)
                throw new ArgumentException("A parent cost centre must be supplied when creating hub entity");
            if (CostCentreType.Hub != parentCostCentre.CostCentreType)
                throw new ArgumentException("The parent cost centre of a Store must be a Hub");
            return new Store(_id)
            {
                CostCentreType = CostCentreType.Store,
                ParentCostCentre = ParentRef(parentCostCentre)
            };
        }
    }
}
