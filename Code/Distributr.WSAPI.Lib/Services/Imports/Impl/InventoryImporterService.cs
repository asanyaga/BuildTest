using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Import.Entities;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class InventoryImporterService:BaseImporterService,IInventoryImporterService
    {
        private CokeDataContext _context;
        private IInventoryRepository _inventoryRepository;
        public InventoryImporterService(CokeDataContext context)
        {
            _context = context;
        }

        public ImportResponse Save(List<InventoryImport> imports)
        {
            List<tblInventoryImports> import = imports.Select(Map).ToList();

            foreach (var imp in import)
            {
                _context.tblInventoryImports.AddObject(imp);
                _context.SaveChanges();
            }
           //import.ForEach(p=>_context.tblInventoryImports.AddObject(p));

           // _context.SaveChanges();
           return new ImportResponse() { Status = true, Info = " Inventory Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            throw new NotImplementedException();
        }


        private tblInventoryImports Map(InventoryImport  import)
        {
            var inventoryImport = new tblInventoryImports();
            inventoryImport.id = Guid.NewGuid();
            inventoryImport.ProductCode = import.ProductCode;
            inventoryImport.Quantity = import.Quantity;
            inventoryImport.Ref = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            inventoryImport.WarehouseCode = import.WarehouseCode;
            inventoryImport.ImportStatus = (int)ProcessStatus.New;
            inventoryImport.IM_DateCreated = DateTime.Now;
            inventoryImport.IM_DateLastUpdated = DateTime.Now;
            inventoryImport.IM_Status = (int) EntityStatus.Active;

            return inventoryImport;
            

        }
    }
}
