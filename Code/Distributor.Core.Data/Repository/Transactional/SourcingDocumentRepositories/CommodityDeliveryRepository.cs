using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
    internal class CommodityDeliveryRepository :SourcingDocumentRepository, ICommodityDeliveryRepository
    {

        public CommodityDeliveryRepository(IContainerTypeRepository equipmentRepository, CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository)
            : base(equipmentRepository, ctx, costCentreRepository, userRepository, commodityRepository, commodityOwnerRepository, commodityProducerRepository)
        {
            documents = _ctx.tblSourcingDocument.Where(s => s.DocumentTypeId == (int)DocumentType.CommodityDelivery);
        }

        public  List<SourcingDocument> GetAll()
        {
            return documents.OrderByDescending(n => n.DocumentDate).ToList().Select(Map).ToList();
        }
        public override void Save(SourcingDocument documentEntity)
        {
            throw new Exception("Use ICommodityDeliveryWFManager to submit");
        }
        public  List<SourcingDocument> GetAll(DateTime startDate, DateTime endDate)
        {
            DateTime dfrom = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00, 000);
            DateTime dto = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);
            return documents.Where(s => s.IM_DateCreated >= dfrom && s.IM_DateCreated <= dto).OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }

        public  int GetCount()
        {
            throw new NotImplementedException();
        }

        public List<CommodityDeliveryNote> GetAllByStatus(DocumentSourcingStatus status)
        {
            return documents.Where(s => s.DocumentStatusId == (int)status).OrderByDescending(n => n.DocumentDate).ToList().Select(Map).OfType<CommodityDeliveryNote>().ToList();
        }

        private SourcingDocument Map(tblSourcingDocument tbldoc)
        {
            SourcingDocument doc = null;
            doc = PrivateConstruct<CommodityDeliveryNote>(tbldoc.Id);
            doc.DisableAddCommands();
           
            
            CommodityDeliveryNote deliveryNote = doc as CommodityDeliveryNote;
            deliveryNote.DriverName = tbldoc.DriverName;
            deliveryNote.VehiclRegNo = tbldoc.VehicleRegNo;
            if (tbldoc.RouteId != null) deliveryNote.RouteId = tbldoc.RouteId.Value;
            if (tbldoc.CentreId != null) deliveryNote.CentreId = tbldoc.CentreId.Value;
            tbldoc.tblSourcingLineItem.Select(MapLineItem).ToList().ForEach(s=>deliveryNote.AddLineItem(s));
            doc = deliveryNote;
            _Map(tbldoc, doc);
            doc.EnableAddCommands();
            return doc;

        }
        private CommodityDeliveryLineItem MapLineItem(tblSourcingLineItem tblItem)
        {
            var item = DocumentLineItemPrivateConstruct<CommodityDeliveryLineItem>(tblItem.Id);
           
            item.Commodity = _commodityRepository.GetById(tblItem.CommodityId.Value);
            item.CommodityGrade =
                _commodityRepository.GetAll(true).SelectMany(s => s.CommodityGrades).FirstOrDefault(
                    s => tblItem.GradeId != null && s.Id == tblItem.GradeId.Value);
            item.ContainerNo = tblItem.ContainerNo;// _equipmentRepository.GetById(tblItem.ContainerId.Value) as SourcingContainer;
            if (tblItem.ContainerId != null)
                item.ContainerType = _containerTypeRepository.GetById(tblItem.ContainerId.Value);
            item.Description = tblItem.Description;
            item.Note = tblItem.Note;
            item.ParentLineItemId = tblItem.ParentId.HasValue ? tblItem.ParentId.Value : Guid.Empty;
            item.Weight = tblItem.Weight.HasValue ? tblItem.Weight.Value : 0;
            if (tblItem.NoOfContainer != null) item.NoOfContainers = tblItem.NoOfContainer.Value;
            if (tblItem.LineItemStatusId != null)
                item.LineItemStatus = (SourcingLineItemStatus) tblItem.LineItemStatusId;
            return item;
        }

        public  SourcingDocument GetById(Guid Id)
        {

            _log.DebugFormat("Getting by Id:{0}", Id);
            tblSourcingDocument tbldoc = documents.FirstOrDefault(n => n.Id == Id);
            if (tbldoc == null)
                return null;
            SourcingDocument ord = Map(tbldoc);
            return ord;
        }
    }
}
