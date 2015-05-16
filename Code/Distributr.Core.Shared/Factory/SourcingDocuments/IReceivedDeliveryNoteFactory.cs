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
   public interface IReceivedDeliveryNoteFactory
    {
       ReceivedDeliveryNote Create(CostCentre documentIssuerCostCentre, CostCentre documentRecepientCostCentre, Guid documentIssueCostCentreApplicationId,
                                       User documentIssuerUser, string documentReference, Guid documentParentId, DateTime documentDate, DateTime documentDateIssued,
           DateTime? vehicleArrivalTime = null,
                                     DateTime? vehicleDepartureTime = null,
                                     decimal? vehicleArrivalMileage = null,
                                     decimal? vehicleDepartureMileage = null,string description="");
       ReceivedDeliveryLineItem CreateLineItem(Guid parentId, Guid gradeId,string containerNo, decimal weight,decimal deliveredWeight, string description);
    }
}
