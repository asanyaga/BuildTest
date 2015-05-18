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
    public class AreaImporterService:BaseImporterService,IAreaImporterService
    {
        private IAreaRepository _areaRepository;
        private IRegionRepository _regionRepository;
        private readonly CokeDataContext _context;


          protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public AreaImporterService(CokeDataContext context, IAreaRepository areaRepository, IRegionRepository regionRepository)
        {
            _areaRepository = areaRepository;
            _regionRepository = regionRepository;
            _context = context;
           
        }


        public ImportResponse Save(List<AreaImport> imports)
        {
            List<Area> areas = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = areas.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
               
            }
            List<Area> changedAreas = HasChanged(areas);

            foreach (var changedArea in changedAreas)
            {
                _areaRepository.Save(changedArea);
            }
            return new ImportResponse() { Status = true, Info = changedAreas.Count + " Areas Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
           foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var areaId = _context.tblArea.Where(p => p.Name == deletedCode).Select(p => p.id).FirstOrDefault();

                    var area = _areaRepository.GetById(areaId);
                    if (area != null)
                    {
                        _areaRepository.SetAsDeleted(area);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Area Delete Error" + ex.ToString());
                }
               
            }
            return new ImportResponse() { Info = "Area Deleted Succesfully", Status = true };
        }
        

        private List<Area> HasChanged(List<Area> areas)
        {
            var changedAreas = new List<Area>();
            foreach (var area in areas)
            {
                var entity = _areaRepository.GetById(area.Id);
                if(entity==null)
                {
                    changedAreas.Add(area);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != area.Name.ToLower() ||
                                  entity.region != area.region;
                
                if(hasChanged)
                {
                    changedAreas.Add(area);
                }
            }
            return changedAreas;
        }

        protected ValidationResultInfo Validate(Area area)
        {
            return _areaRepository.Validate(area);
        }

        protected Area Map(AreaImport areaImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblArea, p => p.Name == areaImport.Name);
            Guid id = exists != null ? exists.id : Guid.NewGuid();


            var regionId = Queryable.Where(_context.tblRegion, p => p.Name == areaImport.RegionCode).Select(p=>p.id).FirstOrDefault();
            var region = _regionRepository.GetById(regionId);
           
            var area = new Area(id);
            area.Name = areaImport.Name;
            area.Description = areaImport.Description;
            area.region = region;


            return area;

        }
    }
}
