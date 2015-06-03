using System;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Factory.ActivityDocuments
{
    public interface IActivityFactory
    {
        ActivityDocument Create(CostCentre hub,
            CostCentre clerk,
            CostCentre supplier,
            CommodityProducer commodityProducer, 
            ActivityType activityType,
            Route route,
            Centre centre,
             Season season,
            string documentReference,
            Guid documentIssueCostCentreApplicationId,
            DateTime documentDate,
            DateTime activityDate,
            string description = "", string note = "");

        ActivityInputItem CreateInputLineItem(Guid productId, decimal quantity,string serialNo,DateTime manunfactureddate, DateTime expirydate);
        ActivityServiceItem CreateServiceLineItem(Guid serviceId, Guid serviceProviderId, Guid shiftId, string description);
        ActivityInfectionItem CreateInfectionLineItem(Guid infectionId, decimal rate, string description);
        ActivityProduceItem CreateProduceLineItem(Guid commodityId, Guid gradeId, Guid serviceProviderId, decimal weight, string description);
    }
}
