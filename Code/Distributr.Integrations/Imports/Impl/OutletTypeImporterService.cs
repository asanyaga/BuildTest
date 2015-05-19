using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
   public class OutletTypeImporterService : BaseImporterService, IOutletTypeImporterService
    {
        private IOutletTypeRepository _outletTypeRepository;
        private CokeDataContext _context;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public OutletTypeImporterService( CokeDataContext context , IOutletTypeRepository outletTypeRepository )
        {
            _outletTypeRepository = outletTypeRepository;
            _context = context;
        }


        public ImportResponse Save(List<OutletTypeImport> imports)
        {
            List<OutletType> outletTypes = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = outletTypes.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                var validationResultsInfo = ValidationResultsInfo(validationResults);
                return new ImportResponse() { Status = false, Info = validationResultsInfo };
            }
            List<OutletType> changedOutletTypes = HasChanged(outletTypes);

            foreach (var changedOutletType in changedOutletTypes)
            {
                _outletTypeRepository.Save(changedOutletType);
            }

            return new ImportResponse() { Status = true, Info = changedOutletTypes.Count + " Outlet Type Successfully Imported" };

      
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var outletTypeId = _context.tblOutletType.Where(p => p.Code == deletedCode).Select(p => p.id ).FirstOrDefault();

                    var outletType = _outletTypeRepository.GetById(outletTypeId);
                    if (outletType != null)
                    {
                        _outletTypeRepository.SetAsDeleted(outletType);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Outlet Type Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Outlet Type Deleted Succesfully", Status = true };
       
        }


        private ValidationResultInfo Validate(OutletType outletType)
        {
            return _outletTypeRepository.Validate(outletType);
        }

        private List<OutletType> HasChanged(List<OutletType> outletTypes)
        {
            var changedOutletTypes = new List<OutletType>();

            foreach (var outletType in outletTypes)
            {
                var entity = _outletTypeRepository.GetById(outletType.Id);
                if (entity == null)
                {
                    changedOutletTypes.Add(outletType);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != outletType.Name.ToLower() ||
                                  entity.Code.ToLower() != outletType.Code.ToLower();

                if (hasChanged)
                {
                    changedOutletTypes.Add(outletType);
                }
            }
            return changedOutletTypes;
        }

        public OutletType Map(OutletTypeImport outletTypeImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblOutletType, p => p.Code == outletTypeImport.Code);

            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var outletType = new OutletType(id);
            outletType.Name = outletTypeImport.Name;
            outletType.Code = outletTypeImport.Code;
            
            return outletType;
        }


    }
}
