using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
 public class OutletCategoryImporterService: BaseImporterService,IOutletCategoryImporterService
    {
        private IOutletCategoryRepository _outletCategoryRepository;

        private CokeDataContext _context;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public OutletCategoryImporterService(IOutletCategoryRepository outletCategoryRepository, CokeDataContext context)
        {
            _context = context;
            _outletCategoryRepository = outletCategoryRepository;
        }

        public ImportResponse Save(List<OutletCategoryImport> imports)
        {
            List<OutletCategory> outletCategories = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = outletCategories.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                var validationResultsInfo = ValidationResultsInfo(validationResults);
                return new ImportResponse() { Status = false, Info = validationResultsInfo };
            }
            List<OutletCategory> changedOutletCategories = HasChanged(outletCategories);

            foreach (var changedOutletCategory in changedOutletCategories)
            {
                _outletCategoryRepository.Save(changedOutletCategory);
            }

            return new ImportResponse() { Status = true, Info = changedOutletCategories.Count + " Outlet Category Successfully Imported" };

      
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var outletCategoryId = _context.tblOutletCategory.Where(p => p.Code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var outletCategory = _outletCategoryRepository.GetById(outletCategoryId);
                    if (outletCategory != null)
                    {
                        _outletCategoryRepository.SetAsDeleted(outletCategory);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Outlet Category Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Outlet Category Deleted Succesfully", Status = true };
        }


        public OutletCategory Map(OutletCategoryImport outletCategoryImport)
        {
            var exists = _context.tblOutletCategory.FirstOrDefault(p => p.Code == outletCategoryImport.Code);

            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var outletCategory = new OutletCategory(id);
            outletCategory.Name = outletCategoryImport.Name;
            outletCategory.Code = outletCategoryImport.Code;
           
            return outletCategory;
        }

     
        private ValidationResultInfo Validate(OutletCategory outletCategory)
        {
            return _outletCategoryRepository.Validate(outletCategory);
        }

        private List<OutletCategory> HasChanged(List<OutletCategory> outletCategories)
        {
            var changedOutletCategories = new List<OutletCategory>();

            foreach (var outletCategory in outletCategories)
            {
                var entity = _outletCategoryRepository.GetById(outletCategory.Id);
                if (entity == null)
                {
                    changedOutletCategories.Add(outletCategory);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != outletCategory.Name.ToLower() ||
                                  entity.Code.ToLower() != outletCategory.Code.ToLower();

                if (hasChanged)
                {
                    changedOutletCategories.Add(outletCategory);
                }
            }
            return changedOutletCategories;
        }



    }
}
