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
   public interface ICommodityReceptionNoteFactory
   {
       CommodityReceptionNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId,
                                     CostCentre documentRecipientCostCentre,
                                     User documentIssuerUser, string documentReference, Guid documentParentId,
                                     DateTime documentDate, DateTime documentDateIssued,
                                     DateTime? vehicleArrivalTime = null,
                                     DateTime? vehicleDepartureTime = null,
                                     decimal? vehicleArrivalMileage = null,
                                     decimal? vehicleDepartureMileage = null, string description = "", string note = "");
       CommodityReceptionLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description);
    }
}
