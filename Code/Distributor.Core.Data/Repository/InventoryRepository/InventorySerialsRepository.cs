using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.InventoryRepository
{
    public class InventorySerialsRepository : IInventorySerialsRepository
    {
        private CokeDataContext _ctx;

        public InventorySerialsRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public Guid AddInventorySerial(InventorySerials invSerials)
        {
            ValidationResultInfo vri = Validate(invSerials);
            if (!vri.IsValid)
            {
                string exmessag = string.Join(",", vri.Results.Select(n => n.ErrorMessage));
                throw new DomainValidationException(vri, "Failed to validate Inventory -->" + exmessag);
            }

            DateTime now = DateTime.Now;
            tblInventorySerials tblIs = _ctx.tblInventorySerials.FirstOrDefault(n => n.Id == invSerials.Id);
            if (tblIs == null)
            {
                tblIs = new tblInventorySerials
                            {
                                Id = invSerials.Id,
                                IM_DateCreated = now,
                                IM_Status = (int)EntityStatus.Active
                            };
                _ctx.tblInventorySerials.AddObject(tblIs);
            }
            var entityStatus = (invSerials._Status == EntityStatus.New) ? EntityStatus.Active : invSerials._Status;
            if (tblIs.IM_Status != (int)entityStatus)
                tblIs.IM_Status = (int)invSerials._Status;
            tblIs.IM_DateLastUpdated = now;
            tblIs.ProductId = invSerials.ProductRef.ProductId;
            tblIs.CostCentreId = invSerials.CostCentreRef.Id;
            tblIs.DocumentId = invSerials.DocumentId;
            tblIs.From = invSerials.From;
            tblIs.To = invSerials.To;

            _ctx.SaveChanges();

            return tblIs.Id;
        }

        public InventorySerials GetById(Guid id)
        {
            tblInventorySerials item = _ctx.tblInventorySerials.FirstOrDefault(n => n.Id == id);
            if (item != null)
                return Map(item);
            return null;
        }

        public List<InventorySerials> GetAll()
        {
            var items = _ctx.tblInventorySerials.Select(n => Map(n)).ToList();
            return items;
        }

        public List<InventorySerials> GetByProductId(Guid productId)
        {
            var items = _ctx.tblInventorySerials.Where(n => n.ProductId == productId).Select(n => Map(n)).ToList();
            return items;
        }

        public List<InventorySerials> GetByDocumentId(Guid documentId)
        {
            var items = _ctx.tblInventorySerials.Where(n => n.DocumentId == documentId).Select(n => Map(n)).ToList();
            return items;
        }

        public List<InventorySerials> GetByCostCentreId(Guid costCentreId)
        {
            var items = _ctx.tblInventorySerials.Where(n => n.CostCentreId == costCentreId).Select(n => Map(n)).ToList();
            return items;
        }

        protected InventorySerials Map(tblInventorySerials tblInvSerials)
        {
            InventorySerials inv = new InventorySerials(tblInvSerials.Id, tblInvSerials.IM_DateCreated,
                                                        tblInvSerials.IM_DateLastUpdated,
                                                        (EntityStatus)tblInvSerials.IM_Status)
                                       {
                                           CostCentreRef = new CostCentreRef { Id = tblInvSerials.CostCentreId },
                                           DocumentId = tblInvSerials.DocumentId,
                                           ProductRef = new ProductRef { ProductId = tblInvSerials.ProductId },
                                           From = tblInvSerials.From,
                                           To = tblInvSerials.To,
                                       };
            return inv;
        }

        public ValidationResultInfo Validate(InventorySerials itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (string.IsNullOrEmpty(itemToValidate.From))
                vri.Results.Add(new ValidationResult(@"Must have 'From' serial number"));
            if (string.IsNullOrEmpty(itemToValidate.To))
                vri.Results.Add(new ValidationResult(@"Must have 'To' serial number"));
            if (_ctx.tblCostCentre.FirstOrDefault(n => n.Id == itemToValidate.CostCentreRef.Id) == null)
                vri.Results.Add(new ValidationResult("Cost Centre specified in CostCentreRef does not exist"));
            //if (_ctx.tblDocument.FirstOrDefault(n => n.Id == itemToValidate.DocumentId) == null)
            //    vri.Results.Add(new ValidationResult("Document in for the serials does not exist"));
            if (_ctx.tblProduct.FirstOrDefault(n => n.id == itemToValidate.ProductRef.ProductId) == null)
                vri.Results.Add(new ValidationResult("Product specified in ProductRef does not exist"));
            return vri;
        }
    }
}
