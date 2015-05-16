using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
    public class ReceivedDeliveryRepository : SourcingDocumentRepository, IReceivedDeliveryRepository
    {
        public ReceivedDeliveryRepository(CokeDataContext ctx, IContainerTypeRepository containerTypeRepository, ICostCentreRepository costCentreRepository, 
            IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, 
            ICommodityProducerRepository commodityProducerRepository)
            : base(containerTypeRepository, ctx, costCentreRepository, userRepository, commodityRepository, commodityOwnerRepository, commodityProducerRepository)
        {
            documents = _ctx.tblSourcingDocument.Where(s => s.DocumentTypeId == (int)DocumentType.ReceivedDelivery);
        }

        public SourcingDocument GetById(Guid Id)
        {
            _log.DebugFormat("Getting by Id:{0}", Id);
            tblSourcingDocument tbldoc = documents.FirstOrDefault(n => n.Id == Id);
            if (tbldoc == null)
                return null;
            SourcingDocument ord = Map(tbldoc);
            return ord;
        }

        private SourcingDocument Map(tblSourcingDocument tbldoc)
        {
            SourcingDocument doc = null;
            doc = PrivateConstruct<ReceivedDeliveryNote>(tbldoc.Id);
            doc.DisableAddCommands();
            var receivedDeliveryNote = doc as ReceivedDeliveryNote;
            tbldoc.tblSourcingLineItem.Select(MapLineItem).ToList().ForEach(s => receivedDeliveryNote.AddLineItem(s));
            doc = receivedDeliveryNote;
            _Map(tbldoc, doc);
            doc.EnableAddCommands();
            return doc;
        }

        private ReceivedDeliveryLineItem MapLineItem(tblSourcingLineItem tblItem)
        {
            var item = new ReceivedDeliveryLineItem(tblItem.Id);
            if (tblItem.GradeId != null)
                item.CommodityGrade = _commodityRepository.GetGradeByGradeId(tblItem.GradeId.Value);
            item.ContainerNo = tblItem.ContainerNo;
            item.Description = tblItem.Description;
            item.ParentDocId = tblItem.ParentId.HasValue ? tblItem.ParentId.Value : Guid.Empty;
            item.Weight = tblItem.Weight.HasValue ? tblItem.Weight.Value : 0;
            item.DeliveredWeight = tblItem.Weight.HasValue ? tblItem.Weight.Value : 0;
            if (tblItem.CommodityId != null) item.Commodity = _commodityRepository.GetById(tblItem.CommodityId.Value);
            if (tblItem.ContainerId != null)
                item.ContainerType = _containerTypeRepository.GetById(tblItem.ContainerId.Value);
            return item;
        }


        public List<SourcingDocument> GetAll()
        {
            return documents.OrderByDescending(n => n.DocumentDate).ToList().Select(Map).ToList();
        }

        public List<SourcingDocument> GetAll(DateTime startDate, DateTime endDate)
        {
            DateTime dfrom = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00, 000);
            DateTime dto = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);
            return documents.Where(s => s.IM_DateCreated >= dfrom && s.IM_DateCreated <= dto).OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }

       
        public int GetCount()
        {
            throw new NotImplementedException();
        }

        public List<ReceivedDeliveryNote> GetPendingStorage()
        {
            //&& p.tblSourcingLineItem.Any(s => s.LineItemStatusId.Value != (int) SourcingLineItemStatus.Stored)
            var receptionNotes = documents.Where(
                p =>
                p.DocumentStatusId == (int)DocumentSourcingStatus.Confirmed && p.tblSourcingLineItem.Any(s =>!s.LineItemStatusId.HasValue || s.LineItemStatusId.Value != (int)SourcingLineItemStatus.Stored))
                .OrderByDescending(n => n.DocumentDate);
          var  viewdata=   receptionNotes  .ToList().Select(Map);

          return viewdata.OfType<ReceivedDeliveryNote>().ToList();
        }
    }
}
