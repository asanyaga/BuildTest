using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.InventoryRepository
{
    internal class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        CokeDataContext _ctx;
        IInventoryRepository _inventoryRepository;
        ICacheProvider _cacheProvider;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public InventoryTransactionRepository(CokeDataContext ctx, IInventoryRepository inventoryRepository, ICacheProvider cacheProvider)
        {
           
            _ctx = ctx;
            _inventoryRepository = inventoryRepository;
            _cacheProvider = cacheProvider;
            
        }

        public InventoryTransaction Map(tblInventoryTransaction tblInvTr)
        {
            var inv = _inventoryRepository.GetById(tblInvTr.InventoryId);
            InventoryTransaction accTr = new InventoryTransaction(tblInvTr.Id)
            {
                 CostCentreId = inv.Warehouse.Id, 
                Inventory = inv,
                NoItems = tblInvTr.NoItems.Value,
                NetValue = (decimal)tblInvTr.NetValue,
                GrossValue = (decimal)tblInvTr.GrossValue,
                DocumentType = (DocumentType)tblInvTr.DocumentType,
                DocumentId = tblInvTr.DocumentId,
                DateInserted = tblInvTr.DateInserted
            };
            accTr._SetDateCreated(tblInvTr.IM_DateCreated);
            accTr._SetDateLastUpdated(tblInvTr.IM_DateLastUpdated);
            accTr._SetStatus((EntityStatus)tblInvTr.IM_Status);

            return accTr;

        }
        public Guid Add(InventoryTransaction inventoryTransaction)
        {
            ValidationResultInfo vri = Validate(inventoryTransaction);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed To Validate InventoryTransaction");
            }
            tblInventoryTransaction tblInvTr = _ctx.tblInventoryTransaction.FirstOrDefault(n => n.Id == inventoryTransaction.Id); ;
            DateTime dt = DateTime.Now;
            if (tblInvTr == null)
            {
                tblInvTr = new tblInventoryTransaction();
                tblInvTr.Id = inventoryTransaction.Id;
                tblInvTr.IM_DateCreated = dt;
                tblInvTr.IM_Status = (int)EntityStatus.Active; //true;
                _ctx.tblInventoryTransaction.AddObject(tblInvTr);
                _log.DebugFormat("inventoryTransaction.Id == 0");
            }
            var entityStatus = (inventoryTransaction._Status == EntityStatus.New) ? EntityStatus.Active : inventoryTransaction._Status;
            if (tblInvTr.IM_Status != (int)entityStatus)
                tblInvTr.IM_Status = (int)inventoryTransaction._Status;
              


            tblInvTr.InventoryId = inventoryTransaction.Inventory.Id;
            tblInvTr.NoItems = inventoryTransaction.NoItems;
            tblInvTr.NetValue = inventoryTransaction.NetValue;
            tblInvTr.GrossValue = inventoryTransaction.GrossValue;
            tblInvTr.DocumentType = (int)inventoryTransaction.DocumentType;
            tblInvTr.DocumentId = inventoryTransaction.DocumentId;
            tblInvTr.DateInserted = inventoryTransaction.DateInserted;
            tblInvTr.IM_DateLastUpdated = dt;
            _log.DebugFormat("Saving/Updating InventoryTransaction");

            _ctx.SaveChanges();
            return tblInvTr.Id;
        }

        public InventoryTransaction GetById(Guid id)
        {
            tblInventoryTransaction tblInvTr = _ctx.tblInventoryTransaction.FirstOrDefault(n => n.Id == id);
            if (tblInvTr == null)
            {
                return null;
            }
            InventoryTransaction invTr = Map(tblInvTr);
            return invTr;
        }
        public List<InventoryTransaction> GetByWarehouse(Guid wareHouseId)
        {
            List<InventoryTransaction> qry = _ctx.tblInventoryTransaction.Where(n => n.tblInventory.WareHouseId == wareHouseId).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        public List<InventoryTransaction> GetByDate(DateTime startDate, DateTime endDate)
        {
            List<InventoryTransaction> qry = _ctx.tblInventoryTransaction.Where(n => n.DateInserted >= startDate && n.IM_DateLastUpdated <= endDate).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        public List<InventoryTransaction> GetByWarehouse(Guid wareHouseId, Inventory inventory, DocumentType? documentType)
        {
            List<InventoryTransaction> qry = null;
            if (documentType == null)
            {
                qry = _ctx.tblInventoryTransaction
                    .Where(n => n.tblInventory.WareHouseId == wareHouseId && n.tblInventory.id == inventory.Id
                                            )
                                            .ToList().Select(n => Map(n)).ToList();
            }
            else
            {
                int docType = (int)documentType;
                qry = _ctx.tblInventoryTransaction
                    .Where(n => n.tblInventory.WareHouseId == wareHouseId && n.tblInventory.id == inventory.Id)
                                            .ToList().Select(n => Map(n)).ToList();
            }

            return qry;
        }

        public List<InventoryTransaction> GetByWarehouse(Guid wareHouseId, Inventory inventory, DocumentType? documentType, DateTime startDate, DateTime endDate)
        {
            List<InventoryTransaction> qry = null;
            if (documentType == null)
            {
                qry = _ctx.tblInventoryTransaction.Where(n => n.tblInventory.WareHouseId == wareHouseId && n.tblInventory.id == inventory.Id
                                            && n.DateInserted >= startDate && n.IM_DateLastUpdated <= endDate).ToList().Select(n => Map(n)).ToList();
            }
            else
            {
                int docType = (int)documentType;
                qry = _ctx.tblInventoryTransaction.Where(n => n.tblInventory.WareHouseId == wareHouseId && n.tblInventory.id == inventory.Id
                                             && n.DateInserted >= startDate && n.IM_DateLastUpdated <= endDate).ToList().Select(n => Map(n)).ToList();
            }

            return qry;
        }

        public ValidationResultInfo Validate(InventoryTransaction itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }
    }
}
