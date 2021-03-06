﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
    internal class CommodityReceptionRepository :SourcingDocumentRepository, ICommodityReceptionRepository
    {
        public CommodityReceptionRepository(IContainerTypeRepository equipmentRepository, CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository)
            : base(equipmentRepository, ctx, costCentreRepository, userRepository, commodityRepository, commodityOwnerRepository, commodityProducerRepository)
        {
            documents = _ctx.tblSourcingDocument.Where(s => s.DocumentTypeId == (int)DocumentType.CommodityReceptionNote);
        }

        public List<CommodityReceptionNote> GetPendingStorage()
        {
            var receptionNotes = _ctx.tblSourcingDocument
                .Where(n => n.DocumentTypeId == (int)DocumentType.CommodityReceptionNote)
                .Where(p=>p.tblSourcingLineItem
                    .Any(q=>q.LineItemStatusId==(int)SourcingLineItemStatus.Confirmed))
                    .OrderByDescending(n => n.DocumentDate)
                    .ToList().Select(Map);

            return receptionNotes.OfType<CommodityReceptionNote>().ToList();
        }

        public int GetPendingStorageCount()
        {
            return _ctx.tblSourcingDocument
                .Where(n => n.DocumentTypeId == (int) DocumentType.CommodityReceptionNote)
                .Where(p => p.tblSourcingLineItem
                                .Any(q => q.LineItemStatusId == (int) SourcingLineItemStatus.Confirmed))
                .OrderByDescending(n => n.DocumentDate)
                .Count();
        }

       
        public bool IsLineItemStored(CommodityReceptionLineItem lineItem)
        {
            return IsLineItemStored(lineItem.ParentLineItemId);
        }

        bool IsLineItemStored(Guid lineItemParentId)
        {
            tblSourcingLineItem stored =
               _ctx.tblSourcingLineItem
                   .FirstOrDefault(n =>
                                   n.ParentId == lineItemParentId
                                   && n.tblSourcingDocument.DocumentTypeId == (int)DocumentType.CommodityStorageNote);
            return stored != null;
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
        private SourcingDocument Map(tblSourcingDocument tbldoc)
        {
            SourcingDocument doc = null;
            doc = new CommodityReceptionNote(tbldoc.Id);
            doc.DisableAddCommands();
            CommodityReceptionNote receptionNote = doc as CommodityReceptionNote;
            receptionNote._SetLineItems(tbldoc.tblSourcingLineItem.Select(MapCommodityReceptionLineItem).ToList());
            doc = receptionNote;
            _Map(tbldoc, doc);
            doc.EnableAddCommands();
            return doc;

        }
        private CommodityReceptionLineItem MapCommodityReceptionLineItem(tblSourcingLineItem tblItem)
        {
            return new CommodityReceptionLineItem(tblItem.Id)
            {
                
                Commodity = _commodityRepository.GetById(tblItem.CommodityId.Value),
                CommodityGrade = _commodityRepository.GetAll(true).SelectMany(s => s.CommodityGrades).FirstOrDefault(s => tblItem.GradeId != null && s.Id == tblItem.GradeId.Value),
                ContainerNo =tblItem.ContainerNo,// _equipmentRepository.GetById(tblItem.ContainerId.Value) as SourcingContainer,
                Description = tblItem.Description,
                Note = tblItem.Note,
                ParentLineItemId = tblItem.ParentId.HasValue ? tblItem.ParentId.Value : Guid.Empty,
                ContainerType = tblItem.ContainerId.HasValue ? _containerTypeRepository.GetById(tblItem.ContainerId.Value) : null,
                Weight = tblItem.Weight.HasValue ? tblItem.Weight.Value : 0,
                WeighType = (int) (tblItem.WeighType.HasValue ? (WeighType)tblItem.WeighType.Value : WeighType.Manual),
                LineItemStatus = tblItem.LineItemStatusId.HasValue ? (SourcingLineItemStatus) tblItem.LineItemStatusId.Value : SourcingLineItemStatus.New
            };
        }
        public  List<SourcingDocument> GetAll()
        {
            return documents.OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }
        public  List<SourcingDocument> GetAll(DateTime startDate, DateTime endDate)
        {
            DateTime dfrom = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00, 000);
            DateTime dto = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);
            return documents.Where(s => s.IM_DateCreated >= dfrom && s.IM_DateCreated <= dto).OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }


        public  int GetCount()
        {
            return documents.Count();
        }
    }
}
