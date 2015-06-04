using System;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Workflow
{
    public interface ISourcingDocumentFactoryParameters
    {
        CostCentre DocumentIssuerCostCentre { get; set; }
        User DocumentIssuerUser { get; set; }
        CostCentre DocumentRecipientCostCentre { get; set; }
        DocumentStatus Status { get; set; }
    }

    public interface ISourcingDocumentLineItemFactoryParameters
    {
        Guid ParentId { get; set; }
        Commodity Commodity { get; set; }
        CommodityGrade CommodityGrade { get; set; }
        decimal Weight { get; set; }
        decimal ActualWeight { get; set; }
        decimal TareWeight { get; set; }
        SourcingContainer Container { get; set; }
        string Note { get; set; }
        string Description { get; set; }
    }

    public interface ISourcingWFManager<T> where T : SourcingDocument
    {
        void SubmitChanges(T document);
    }
    public interface IActivityDocumentWFManager<T> where T : ActivityDocument
    {
        void SubmitChanges(T document);
    }
    public interface IReCollectionWFManager
    {
        void SubmitChanges(ReCollection document);
    }
   
}
