using System;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Factory.SourcingDocuments
{
    public interface ICommodityReleaseNoteFactory
    {
        CommodityReleaseNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre,
            User documentIssuerUser, string documentReference, Guid documentParentId, 
           DateTime documentDate,DateTime documentDateIssued,string description="",string note="");
        CommodityReleaseLineItem CreateLineItem(Guid parentId, Guid parentLineItemId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description);
       

    }
}