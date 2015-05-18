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
    public class RegionImporterService : BaseImporterService, IRegionImporterService
    {
        private IRegionRepository _regionRepository;
        private ICountryRepository _countryRepository;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CokeDataContext _context;

        public RegionImporterService(IRegionRepository regionRepository, ICountryRepository countryRepository, CokeDataContext context)
        {
            _regionRepository = regionRepository;
            _countryRepository = countryRepository;
            _context = context;
        }


        public ImportResponse Save(List<RegionImport> imports)
        {
            var mappingValidationList = new List<string>();
            List<Region> regions = imports.Select(s=>Map(s,mappingValidationList)).ToList();

            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }

            List<ValidationResultInfo> validationResults = regions.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
            }

            List<Region> changedRegions = HasChanged(regions);

            foreach (var changedRegion in changedRegions)
            {
                _regionRepository.Save(changedRegion);
            }
            return new ImportResponse() { Status = true, Info = changedRegions.Count + " Regions Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var regionId = _context.tblRegion.Where(p => p.Name == deletedCode).Select(p => p.id).FirstOrDefault();

                    var region = _regionRepository.GetById(regionId);
                    if (region != null)
                    {
                        _regionRepository.SetAsDeleted(region);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Region Error" + ex.ToString());
                }
            }
            return new ImportResponse() { Info = "Region Deleted Succesfully", Status = true };
        }



        private List<Region> HasChanged(List<Region> regions)
        {
            var changedRegions = new List<Region>();
            foreach (var region in regions)
            {
                var entity = _regionRepository.GetById(region.Id);
                if (entity == null)
                {
                    changedRegions.Add(region);
                    continue;
                }


                bool hasChanged = entity.Name.ToLower() != region.Name.ToLower() ||
                                  entity.Description.ToLower() != region.Description.ToLower() ||
                                  entity.Country != region.Country;

                if (hasChanged)
                {
                    changedRegions.Add(region);
                }
            }
            return changedRegions;
        }

        private ValidationResultInfo Validate(Region region)
        {
            return _regionRepository.Validate(region);
        }


        private Region Map(RegionImport regionImport, List<string> mappingvalidationList)
        {
            var exists = Queryable.FirstOrDefault(_context.tblRegion, p => p.Name == regionImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var country = Queryable.FirstOrDefault(_context.tblCountry, p => p.Code == regionImport.CountryCode);
            if(country==null)
            {mappingvalidationList.Add(string.Format((string) "Invalid Country Code {0}",(object) regionImport.CountryCode));}
            var countryId = country != null ? country.id : Guid.Empty;

            var countryEntity = _countryRepository.GetById(countryId);

            var region = new Region(id);
            region.Country = countryEntity;
            region.Name = regionImport.Code;
            region.Description = regionImport.Name;

            return region;

        }
    }
}
