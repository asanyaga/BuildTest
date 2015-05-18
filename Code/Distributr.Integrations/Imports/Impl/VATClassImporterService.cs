using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class VATClassImporterService:BaseImporterService,IVATClassImporterService
    {
        private IVATClassRepository _vatClassRepository;

        private CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public VATClassImporterService(IVATClassRepository vatClassRepository, CokeDataContext context)
        {
            _vatClassRepository = vatClassRepository;
            _context = context;
        }

        public ImportResponse Save(List<VATClassImport> imports)
        {
            List<VATClass> vatClasses = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = vatClasses.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<VATClass> changedVatClasses = HasChanged(vatClasses);

            foreach (var changedVatClass in changedVatClasses)
            {
                _vatClassRepository.Save(changedVatClass);
            }
            return new ImportResponse() { Status = true, Info = changedVatClasses.Count + " VAT Class Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var vatClassItemId = _context.tblVATClass.Where(p => p.Class == deletedCode).Select(p => p.id).FirstOrDefault();

                    var vatClass = _vatClassRepository.GetById(vatClassItemId);
                    if (vatClass != null)
                    {
                        _vatClassRepository.SetAsDeleted(vatClass);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("VAT Delete Error" + ex.ToString());
                }
               
            }
            return new ImportResponse() { Info = "VAT Class Deleted Succesfully", Status = true };
        }

        private ValidationResultInfo Validate(VATClass vatClass)
        {
           return _vatClassRepository.Validate(vatClass);
        }

        private List<VATClass> HasChanged(List<VATClass> vatClasses)
        {
            var changedVatClasses = new List<VATClass>();
            foreach (var vatClass in vatClasses)
            {
                var entity = _vatClassRepository.GetById(vatClass.Id);
                if (entity == null)
                {
                    changedVatClasses.Add(vatClass);
                    continue;
                }
                bool hasChanged = false;
                if(entity.Name.ToLower() != vatClass.Name.ToLower() || entity.VatClass.ToLower() != vatClass.VatClass.ToLower())
                {
                    hasChanged = true;
                }

                var currentRate = Math.Round(vatClass.CurrentRate, 2);
                var currentEffectiveDate = vatClass.CurrentEffectiveDate;

                var previousRate = Math.Round(entity.CurrentRate, 2);
                var previousEffectiveDate = entity.CurrentEffectiveDate;

                bool hasEffectiveDateChanged = (currentEffectiveDate != previousEffectiveDate);

                if ((currentRate != previousRate))// || hasEffectiveDateChanged) //Add this date if this is currenteffective date is not set to now.
                    hasChanged= true;

                if (hasChanged)
                {
                    changedVatClasses.Add(vatClass);
                }
            }
            return changedVatClasses;
        }

        private VATClass  Map(VATClassImport vatClassImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblVATClass, p => p.Name == vatClassImport.Name);


            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var vatClass = new VATClass(id);
            var vatClassItemsList = new List<VATClass.VATClassItem>();


            vatClass.Name = vatClassImport.Code;
            vatClass.VatClass = vatClassImport.Name;


            var classItemExists = Queryable.FirstOrDefault(_context.tblVATClassItem, p => p.VATClassID == id && p.Rate==vatClassImport.CurrentRate);


            Guid classItemId = classItemExists != null ? classItemExists.id : Guid.NewGuid();

            var vatClassItem = new VATClass.VATClassItem(classItemId)
            {
                
                EffectiveDate = vatClassImport.CurrentEffectiveDate,
                Rate = vatClassImport.CurrentRate,
               //Math.Round(Convert.ToDecimal((vatClassImport.CurrentRate / 100)), 4)
            };

            vatClassItemsList.Add(vatClassItem);
            vatClass.AddVatClassItems(vatClassItemsList);

            return vatClass;
        }
    }
}
