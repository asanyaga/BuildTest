using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Validation;
using Distributr.Import.Entities;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class SupplierImportService:BaseImporterService,ISupplierImporterService
    {

        private ISupplierRepository _supplierRepository;

        private CokeDataContext _context;

        public SupplierImportService(ISupplierRepository supplierRepository, CokeDataContext context)
        {
            _supplierRepository = supplierRepository;
            _context = context;
        }

        public ImportResponse Save(List<SupplierImport> imports)
        {
            List<Supplier> suppliers = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = suppliers.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
            }

            List<Supplier> changedSuppliers = HasChanged(suppliers);

            foreach (var changedSupplier in changedSuppliers)
            {
                _supplierRepository.Save(changedSupplier);
            }

            return new ImportResponse() { Status = true, Info = changedSuppliers.Count + " Suppliers Successfully Imported" };

        }

        private List<Supplier> HasChanged(List<Supplier> suppliers)
        {
            var changedSuppliers = new List<Supplier>();
            foreach (var supplier in suppliers)
            {
                var entity = _supplierRepository.GetById(supplier.Id);
                if (entity == null)
                {
                    changedSuppliers.Add(supplier);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != supplier.Name.ToLower() || entity.Code.ToLower() != supplier.Code.ToLower();

                if (hasChanged)
                {
                    changedSuppliers.Add(supplier);
                }
            }
            return changedSuppliers;
        }

        private ValidationResultInfo Validate(Supplier supplier)
        {
            return _supplierRepository.Validate(supplier);
        }

        private Supplier Map(SupplierImport supplierImport)
        {
            var exists = _context.tblSupplier.FirstOrDefault(p => p.Code == supplierImport.Code);

            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var supplier = new Supplier(id);
            supplier.Code = supplierImport.Code;
            supplier.Name = supplierImport.Name;
            supplier.Description = supplierImport.Description;


            return supplier;
        }
    }
}
