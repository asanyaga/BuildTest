using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments.Impl
{
    public class ReceivedDeliveryNoteFactory : BaseSourcingDocumentFactory, IReceivedDeliveryNoteFactory
    {
        private ICommodityRepository _commodityRepository;


        public ReceivedDeliveryNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository) : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
        }

        public ReceivedDeliveryNote Create(CostCentre documentIssuerCostCentre,CostCentre documentRecepientCostCentre, Guid documentIssueCostCentreApplicationId, User documentIssuerUser, string documentReference, Guid documentParentId, DateTime documentDate, DateTime documentDateIssued,
            DateTime? vehicleArrivalTime = null,
       DateTime? vehicleDepartureTime = null, decimal? vehicleArrivalMileage = null, decimal? vehicleDepartureMileage = null, string description="")
        {
            Guid id = Guid.NewGuid();
            ReceivedDeliveryNote doc = DocumentPrivateConstruct<ReceivedDeliveryNote>(id);
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;

            doc.DocumentDate = documentDate;
            doc.DocumentDateIssued = documentDateIssued;
            doc.Description = description;
          
          Map(doc,documentIssuerCostCentre, documentIssueCostCentreApplicationId,null, documentIssuerUser, documentReference, null, null);
            doc.DocumentRecipientCostCentre = documentRecepientCostCentre;
            doc.VehicleArrivalMileage = vehicleArrivalMileage;
            doc.VehicleDepartureMileage = vehicleDepartureMileage;
            doc.VehicleArrivalTime = vehicleArrivalTime;
            doc.VehicleDepartureTime = vehicleDepartureTime;
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public ReceivedDeliveryLineItem CreateLineItem(Guid parentId, Guid gradeId, string containerNo, decimal weight, decimal deliveredWeight, string description)
        {
            var grade = _commodityRepository.GetGradeByGradeId(gradeId);
            var li = DocumentLineItemPrivateConstruct<ReceivedDeliveryLineItem>(Guid.NewGuid());
            li.Description = description;
            li.CommodityGrade = grade;
            li.Weight = weight;
            li.ContainerNo = containerNo;
            li.DeliveredWeight = deliveredWeight;
            li.ParentDocId = parentId;
           
            return li;
        }
    }
}
