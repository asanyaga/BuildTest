using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Factory.SourcingDocuments
{
    public interface ICommodityTransferNoteFactory
    {
        CommodityTransferNote Create(CostCentre documentIssuerCostCentre, CostCentre documentRecepientCostCentre, 
            Guid documentIssueCostCentreApplicationId, User documentIssuerUser, string documentReference, 
            Guid documentParentId, DateTime documentDate, DateTime documentDateIssued, string description);

        CommodityTransferLineItem CreateLineItem(Guid parentId, Guid parentLineItemId, Guid gradeId, Guid commodityId, decimal weight, 
           string batchNumber, string description);
    }
}
