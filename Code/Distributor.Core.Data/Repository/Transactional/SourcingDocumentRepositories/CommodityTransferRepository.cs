﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
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
    internal class CommodityTransferRepository : SourcingDocumentRepository, ICommodityTransferRepository
    {
        private IStoreRepository _storeRepository;
        public CommodityTransferRepository(IContainerTypeRepository equipmentRepository, CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository, IStoreRepository storeRepository)
            : base(equipmentRepository, ctx, costCentreRepository, userRepository, commodityRepository, commodityOwnerRepository, commodityProducerRepository)
        {
            _storeRepository = storeRepository;
            documents = _ctx.tblSourcingDocument.Where(s => s.DocumentTypeId == (int)DocumentType.CommodityTransferNote);
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
            doc = new CommodityTransferNote(tbldoc.Id);
            _Map(tbldoc, doc);
            var transferNote = doc as CommodityTransferNote;
            if (tbldoc.DocumentTypeId2 != null )
                transferNote.TransferNoteTypeId = (CommodityTransferNote.CommodityTransferNoteTypeId) tbldoc.DocumentTypeId2;
            transferNote.WareHouseToStore = tbldoc.DocumentOnBehalfOfCostCentreId.HasValue ? _storeRepository.GetById(tbldoc.DocumentOnBehalfOfCostCentreId.Value) : null;
            transferNote._SetLineItems(tbldoc.tblSourcingLineItem.Select(MapCommodityStorageLineItem).ToList());
            doc = transferNote;
            return doc;

        }

        private CommodityTransferLineItem MapCommodityStorageLineItem(tblSourcingLineItem tblItem)
        {
            return new CommodityTransferLineItem(tblItem.Id)
            {

                Commodity = _commodityRepository.GetById(tblItem.CommodityId.Value),
                CommodityGrade = _commodityRepository.GetAll(true).SelectMany(s => s.CommodityGrades).FirstOrDefault(s => tblItem.GradeId != null && s.Id == tblItem.GradeId.Value),
                ContainerNo = tblItem.ContainerNo,//_equipmentRepository.GetById(tblItem.ContainerId.Value) as SourcingContainer,
                Description = tblItem.Description,
                Note = tblItem.Note,
                ParentLineItemId = tblItem.ParentId.HasValue ? tblItem.ParentId.Value : Guid.Empty,
                ParentDocId = tblItem.tblSourcingDocument.Id,
                ContainerType = tblItem.ContainerId.HasValue ? _containerTypeRepository.GetById(tblItem.ContainerId.Value) : null,
                Weight = tblItem.Weight.HasValue ? tblItem.Weight.Value : 0,
            };
        }

        public List<SourcingDocument> GetAll()
        {
            return documents.OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }

        public List<SourcingDocument> GetAll(DateTime startDate, DateTime endDate)
        {
            DateTime dfrom = new DateTime(startDate.Year, startDate.Month, startDate.Day, 00, 00, 00, 000);
            DateTime dto = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, 999);
            return documents.Where(s => s.IM_DateCreated >= dfrom && s.IM_DateCreated <= dto).OrderByDescending(n => n.DocumentDate).Select(Map).ToList();
        }

        public int GetCount()
        {
            return documents.Count();
        }



    }
}
