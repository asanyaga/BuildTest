using System;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Factory.SourcingDocuments
{
   public interface ICommodityPurchaseNoteFactory
    {
       CommodityPurchaseNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre,
                                        User documentIssuerUser, string documentReference, Guid documentParentId, string deliveredBy,CommodityProducer commodityProducer,CommoditySupplier commoditySupplier, CommodityOwner owner,DateTime documentDate,DateTime documentDateIssued,string description="",string note="");
       CommodityPurchaseLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description,decimal containersCount,decimal tareWeight);
       

    }
}
