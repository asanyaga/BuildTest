using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.DataImporter.Lib.ImportEntity;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportService.Shipping.Impl
{
    public class ShipToAddressImportService : IShipToAddressImportService
    {
        private IOutletRepository _outletRepository;
        private ICostCentreRepository _costCentreRepository;
        private List<string> _failedImpoprts;
        public ShipToAddressImportService(IOutletRepository outletRepository, ICostCentreRepository costCentreRepository)
        {
            _outletRepository = outletRepository;
            _costCentreRepository = costCentreRepository;
            _failedImpoprts=new List<string>();
        }

        public IEnumerable<ShipToAddressImport> Import(string path)
        {
             try
            {

                var inputFileDescription = new CsvFileDescription
                {
                    // cool - I can specify my own separator!
                    SeparatorChar = ',',
                    FirstLineHasColumnNames = false,
                    QuoteAllFields = true,
                    EnforceCsvColumnAttribute = true
                };

                CsvContext cc = new CsvContext();
                var importEntities = cc.Read<ShipToAddressImport>(path, inputFileDescription);
                return importEntities;
            }
             catch (FileNotFoundException ex)
             {
                 throw ex;
             }
             catch (FieldAccessException ex)
             {
                 throw ex;
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message, "Importer Error", MessageBoxButton.OK, MessageBoxImage.Error);
                 return null;
             }
        }

        public IList<ImportValidationResultInfo> Validate(List<ShipToAddressImport> entities)
        {
           IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
            int count = 1;
            foreach (var domainentity in ConstructDomainEntities(entities))
            {
                var res = _costCentreRepository.Validate(domainentity);

                var importValidationResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Row-" + count,
                    Entity = domainentity
                };
                results.Add(importValidationResult);


                count++;

            }
            return results;
        }

        public Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ShipToAddressImport> entities)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Outlet> ConstructDomainEntities(IEnumerable<ShipToAddressImport> entities)
        {
            var newOutletAddresses = new List<Outlet>();
           
            foreach (var entity in entities)
            {
                var outlet = _costCentreRepository.GetByCode(entity.OutletCode, CostCentreType.Outlet, true) as Outlet;
                if (outlet != null)
                {
                    if(outlet.ShipToAddresses.Any())
                        outlet.ShipToAddresses.Clear();
                    var shipping = new ShipToAddress(Guid.NewGuid())
                                       {
                                           Description = entity.Description,
                                           Name = entity.Description,
                                           Code = entity.Code,
                                           PhysicalAddress = entity.PhysicalAddress,
                                           PostalAddress = entity.PostalAddress,
                                           Latitude = entity.Latitude != null ? decimal.Parse(entity.Latitude) : 0,
                                           Longitude = entity.Longitude != null ? decimal.Parse(entity.Longitude) : 0

                                       };
                    outlet.AddShipToAddress(shipping);
                     newOutletAddresses.Add(outlet);
                }
                else
                {
                    if (!_failedImpoprts.Any(p => p.Equals(entity.OutletCode, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _failedImpoprts.Add(entity.OutletCode);
                    }
                }
            }
            return newOutletAddresses;
        }

        public void Save(List<Outlet> outlets)
        {
            foreach (var outlet in outlets)
            {
                _costCentreRepository.Save(outlet);    
            }
        }

        public List<string> GetNonExistingOutletCodes()
        {
            return _failedImpoprts;
        }
    }
}
