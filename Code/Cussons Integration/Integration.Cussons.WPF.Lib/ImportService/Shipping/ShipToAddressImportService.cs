using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportService.Shipping
{
    internal class ShipToAddressImportService : IShipToAddressImportService
    {
        private readonly ICostCentreRepository _costCentreRepository;
        private CokeDataContext _ctx;
        private IList<ImportValidationResultInfo> validations;
        public ShipToAddressImportService(ICostCentreRepository costCentreRepository, CokeDataContext ctx)
        {
            _costCentreRepository = costCentreRepository;
            _ctx = ctx;
            validations=new List<ImportValidationResultInfo>();
        }

        public async Task<IEnumerable<ShipToAddressImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var imports = new List<ShipToAddressImport>();
                var tempFolder = Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                File.Copy(path, tempFolder);
                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                {
                    parser.SetDelimiters("\t");
                    string[] currentRow = null;

                    while (!parser.EndOfData)
                    {
                        currentRow = parser.ReadFields();
                        if (currentRow != null && currentRow.Length > 0)
                        {
                            imports.Add(MapShipToAddressImport(currentRow));
                        }
                    }

                }

                return imports;
            });
        }

        ShipToAddressImport MapShipToAddressImport(string[] dataRow)
        {
            var item = new ShipToAddressImport()
            {
                OutletCode = SetFieldValue(dataRow, 1),
                OutletName = SetFieldValue(dataRow, 2),
                ShipToCode = SetFieldValue(dataRow, 3),
                ShipToName = SetFieldValue(dataRow, 4),
                PostalAddress = SetFieldValue(dataRow, 5),
            };
            return item;
        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length > index && string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }
        public async Task<IEnumerable<ShipToAddressImport>> Import(string path,bool generic=true)
        {
            return await Task.Factory.StartNew(() =>
                                                   {
                                                       IEnumerable<ShipToAddressImport> imports;
                                                       try
                                                       {

                                                           var inputFileDescription = new CsvFileDescription
                                                                                          {
                                                                                              // cool - I can specify my own separator!
                                                                                              SeparatorChar = '\t',
                                                                                              //tab delimited
                                                                                              FirstLineHasColumnNames =
                                                                                                  false,
                                                                                              QuoteAllFields = true,
                                                                                              EnforceCsvColumnAttribute
                                                                                                  =
                                                                                                  true
                                                                                          };

                                                           var cc = new CsvContext();

                                                           imports = cc.Read<ShipToAddressImport>(path,
                                                                                                  inputFileDescription);


                                                       }
                                                       catch (FileNotFoundException ex)
                                                       {
                                                           MessageBox.Show("File not found on specified path:\n" + path);
                                                           return null;
                                                       }
                                                       catch (FieldAccessException ex)
                                                       {
                                                           MessageBox.Show(
                                                               "File cannot be accessed,is it in use by another application?",
                                                               "Importer Error", MessageBoxButton.OK,
                                                               MessageBoxImage.Stop);
                                                           return null;
                                                       }
                                                       catch (Exception ex)
                                                       {
                                                           MessageBox.Show("Unknown Error:Details\n" + ex.Message,
                                                                           "Importer Error",
                                                                           MessageBoxButton.OK, MessageBoxImage.Error);
                                                           return null;
                                                       }
                                                       return imports;
                                                   });
        }
       
        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ShipToAddressImport> entities)
        {
            return await Task.Run(async () =>
            {
                 await ConstructEntities(entities);
                return validations;
            });
        }
        public async Task<bool> SaveAsync()
        {
            return await Task.Run(() =>
                                      {
                                          try
                                          {
                                              _ctx.SaveChanges();
                                          }
                                          catch (DbEntityValidationException ex)
                                          {
                                              foreach (var error in ex.EntityValidationErrors)
                                              {
                                                  Console.WriteLine("====================");

                                                  FileUtility.LogError(string.Format("Entity {0} in state {1} has validation errors:",
                                                      error.Entry.Entity.GetType().Name, error.Entry.State));
                                                  foreach (var ve in error.ValidationErrors)
                                                  {
                                                      Console.WriteLine("\tProperty: {0}, Error: {1}",
                                                          ve.PropertyName, ve.ErrorMessage);
                                                  }
                                                  Console.WriteLine();
                                              }
                                              throw;
                                          }
                                         
                                          return true;
                                      });
        }

        public async Task<bool> SaveAsync(List<CostCentre> entities)
        {
            return await Task.Run(() =>
                                      {
                                          int batchSize = 10;
                                          var outlets = entities.OrderBy(p => p.CostCentreCode).Batch(batchSize).Select(x => x.ToList()).ToList();
                                          while (outlets.Any())
                                          {
                                              var current = outlets.FirstOrDefault();
                                              if (current == null) continue;
                                              current.ForEach(entity => _costCentreRepository.Save(entity));
                                              outlets.Remove(current);
                                          }
                                         
              
                return true;
            });
        }

     
       
        
        private async Task<IEnumerable<tblShipToAddress>> ConstructEntities(IEnumerable<ShipToAddressImport> entities)
        {
            return await Task.Run(() =>
                                      {
                                          var newOutletAddresses = new List<tblShipToAddress>();
                                         

                                          foreach (var entity in entities)
                                          {
                                              var outlet =
                                                  _costCentreRepository.GetByCode(entity.OutletCode,
                                                                                  CostCentreType.Outlet, true) as
                                                  Outlet;
                                              if (outlet == null)
                                              {
                                                  var result=new List<ValidationResult>
                                                                 {
                                                                     new ValidationResult(
                                                                         string.Format("outlet code=>{0} not found",
                                                                                       entity.OutletCode))
                                                                 };

                                                  validations.Add(new ImportValidationResultInfo()
                                                                      {
                                                                          Description = entity.OutletName,
                                                                          Entity = new ShipToAddress(Guid.NewGuid()),
                                                                          Results = result
                                                                      });
                                              }
                                              else
                                              {

                                                  var address =
                                                      _ctx.tblShipToAddress.FirstOrDefault(
                                                          n =>
                                                          n.CostCentreId == outlet.Id &&n.Code !=null && n.Code.ToLower()==entity.ShipToCode.ToLower() && n.Name != null &&
                                                          (n.Name.ToLower() == entity.ShipToName.ToLower()) &&
                                                          (n.PostalAddress != null &&
                                                           (n.PostalAddress.ToLower() == entity.PostalAddress.ToLower())));
                                                  var dt = DateTime.Now;
                                                  if (address == null)
                                                  {
                                                      address = new tblShipToAddress
                                                                    {
                                                                        IM_DateCreated = dt,
                                                                        IM_Status = (int) EntityStatus.Active
                                                                        ,
                                                                        Id = Guid.NewGuid()
                                                                    };
                                                      _ctx.tblShipToAddress.AddObject(address);
                                                  }
                                                  address.CostCentreId = outlet.Id;
                                                  address.Name = entity.ShipToName ?? outlet.Name;
                                                  address.PhysicalAddress = entity.PostalAddress;
                                                  address.PostalAddress = entity.PostalAddress;
                                                  address.Description = entity.ShipToCode ?? string.Empty;
                                                  address.Code = entity.ShipToCode ?? string.Empty;
                                                  address.Latitude = 0;
                                                  address.Longitude = 0;
                                                  address.IM_DateLastUpdated = dt;
                                                  
                                                  newOutletAddresses.Add(address);
                                              }
                                          }
                                          return newOutletAddresses;
                                      });
        }

        
    }
}
