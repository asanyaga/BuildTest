
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.Utils;
using LINQtoCSV;
using LumenWorks.Framework.IO.Csv;

namespace Integration.Cussons.WPF.Lib.ImportService.CostCentres.Impl
{
    internal class OutletImportService : IOutletImportService
    {
        private ICostCentreRepository _costCentreRepository;
        private IRouteRepository _routeRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private IVATClassRepository _vatClassRepository;
        private IOutletCategoryRepository _outletCategoryRepository;
        private IOutletTypeRepository _outletTypeRepository;
        private IRegionRepository _regionRepository;
        private List<string> _failedImpoprts;
        private ICountryRepository _countryRepository;

        public OutletImportService(ICostCentreRepository costCentreRepository, IRouteRepository routeRepository, IProductPricingTierRepository productPricingTierRepository, IVATClassRepository vatClassRepository, IOutletCategoryRepository outletCategoryRepository, IOutletTypeRepository outletTypeRepository, IRegionRepository regionRepository, List<string> failedImpoprts, ICountryRepository countryRepository)
        {
            _costCentreRepository = costCentreRepository;
            _routeRepository = routeRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _vatClassRepository = vatClassRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _outletTypeRepository = outletTypeRepository;
            _regionRepository = regionRepository;
            _failedImpoprts = failedImpoprts;
            _countryRepository = countryRepository;
        }


        public async Task<IEnumerable<OutletImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
                                                   {
                                                       var imports = new List<OutletImport>();
                                                       var tempFolder =
                                                           Path.Combine(FileUtility.GetApplicationTempFolder(),Path.GetFileName(path));
                                                       File.Copy(path,tempFolder);

                                                       using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                                                       {
                                                           parser.SetDelimiters("\t");
                                                           string[] currentRow = null;
                                                           
                                                           while (!parser.EndOfData)
                                                           {
                                                              currentRow = parser.ReadFields();
                                                              if (currentRow !=null && currentRow.Length > 0)
                                                               {
                                                                   imports.Add(MapOutletImport(currentRow));
                                                               }
                                                           }

                                                       }
                                                       File.Delete(tempFolder);
                                                       return imports;
                                                   });

        }

        private OutletImport MapOutletImport(string[] dataRow)
        {
            var item = new OutletImport
                           {
                               OutletCode =SetFieldValue(dataRow,1),
                               Name =SetFieldValue(dataRow,2),
                               PinNo = SetFieldValue(dataRow, 3),
                               PostalAddress = SetFieldValue(dataRow, 4),
                               PhysicalAddress = SetFieldValue(dataRow, 5),
                               Status = SetFieldValue(dataRow, 6),
                               Tax = SetFieldValue(dataRow, 7),
                               Currency = SetFieldValue(dataRow, 8),
                               CreditLimit = SetFieldValue(dataRow, 9),
                               SalesmanCode = SetFieldValue(dataRow, 10),
                               RouteName = SetFieldValue(dataRow, 11),
                           };

            return item;

        }

         string SetFieldValue(string[] dataRow,int index)
        {
            index = index - 1;
            return (dataRow.Length>index && string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }

        public async Task<IEnumerable<OutletImport>> Import(string path,bool falsed=false)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEnumerable<OutletImport> imports;
                try
                {

                    var inputFileDescription = new CsvFileDescription
                    {
                        // cool - I can specify my own separator!
                        SeparatorChar = '\t',
                        //tab delimited
                        FirstLineHasColumnNames =
                            false,
                        QuoteAllFields =false,
                        EnforceCsvColumnAttribute
                            =true
                    };
                    var cc = new CsvContext();

                    imports = cc.Read<OutletImport>(path,inputFileDescription);

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

        public List<string> GetNonExistingRoutesCodes()
        {
            return _failedImpoprts;
        }

        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<OutletImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var outlets = await ContsructEntities(entities);
                int count = 0;
                foreach (var outlet in outlets)
                {
                    var res = await ValidateEntityAsync(outlet);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description =string.Format("Row-{0} code=>{1}", count, outlet.CostCentreCode),
                        Entity = outlet
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
        }

        public async Task<bool> SaveAsync(List<Outlet> entities)
        {
            return await Task.Run(() =>
            {

                foreach (var outlet in entities)
                {
                    _costCentreRepository.Save(outlet);
                }
                return true;

            });
        }

        private async Task<ImportValidationResultInfo> ValidateEntityAsync(Outlet outlet)
        {
            return await Task.Run(() =>
            {
                var res = _costCentreRepository.Validate(outlet);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Entity = outlet
                };
            });
        }

        private async Task<IEnumerable<Outlet>> ContsructEntities(IEnumerable<OutletImport> entities)
        {
            return await Task.Run(() =>
                                      {


                                          var temp = new List<Outlet>();
                                          var defaultDistributr =
                                              _costCentreRepository.GetByCode("PZ Cussons EA",CostCentreType.Distributor); 
                                          var allRoutes = _routeRepository.GetAll(true).ToList();
                                          var outletCategories = _outletCategoryRepository.GetAll(true);
                                          foreach (var importentity in entities)
                                          {
                                              var domainEntity =
                                                  _costCentreRepository.GetByCode(importentity.OutletCode,CostCentreType.Outlet,true) as
                                                  Outlet?? new Outlet(Guid.NewGuid());
                                           
                                              if(domainEntity !=null && domainEntity.Route==null)
                                              {
                                                  domainEntity.Route = allRoutes.FirstOrDefault(
                                                     p =>
                                                     p.Name != null &&
                                                     p.Name.ToLower().Trim() == importentity.RouteName.Trim().ToLower());

                                                  

                                              }
                                              if (domainEntity.Route == null && importentity.RouteName.StartsWith("SALES VA"))
                                                  domainEntity.Route =
                                                      allRoutes.FirstOrDefault(p => p.Name.Contains("SALES VA"));

                                              if (domainEntity.Route == null && !string.IsNullOrEmpty(importentity.RouteName))
                                              {
                                                  var region=_regionRepository.GetAll(true).FirstOrDefault(p => p.Name == "Region A");
                                                  if(region==null)
                                                  {
                                                      region = new Region(Guid.NewGuid())
                                                                   {
                                                                       Country = _countryRepository.GetAll(true).FirstOrDefault(p => p.Name == "Kenya"),
                                                                       Name = "",
                                                                       Description = ""
                                                                   };
                                                      try
                                                      {
                                                          _regionRepository.Save(region);
                                                      }
                                                      catch
                                                      {

                                                      }
                                                      
                                                  }
                                                  var route = new Route(Guid.NewGuid())
                                                                  {
                                                                      Name = importentity.RouteName,
                                                                      Code = importentity.RouteName,
                                                                      Region =region
                                                                  };
                                                  try
                                                  {
                                                      _routeRepository.Save(route);
                                                  }
                                                  catch
                                                  {
                                                   
                                                  }
                                                  domainEntity.Route = route;

                                              }
                                              try
                                              {
                                                  _routeRepository.Save(domainEntity.Route);
                                              }catch
                                              {
                                              }

                                              if(defaultDistributr==null)throw new ArgumentNullException("distributor");
                                              domainEntity.ParentCostCentre = new CostCentreRef()
                                                                                  {Id = defaultDistributr.Id};
                                              temp.Add(domainEntity);
                                            
                                              if (domainEntity.OutletCategory == null)
                                              {

                                                  OutletCategory category = outletCategories
                                                      .FirstOrDefault(
                                                          s => s.Name != null &&
                                                               s.Name=="defaultoutletcategory");
                                                  if (category == null)
                                                  {
                                                      category = new OutletCategory(Guid.NewGuid())
                                                                     {
                                                                         Name =
                                                                             string.IsNullOrEmpty(importentity.Name)
                                                                                 ? "defaultoutletcategory"
                                                                                 : importentity.Name,
                                                                         Code =
                                                                             string.IsNullOrEmpty(importentity.Name)
                                                                                 ? "defaultoutletcategory"
                                                                                 : importentity.Name
                                                                     };
                                                      try
                                                      {
                                                          _outletCategoryRepository.Save(category);
                                                      }
                                                      catch
                                                      {
                                                      }
                                                  }
                                                  domainEntity.OutletCategory = category;
                                              }
                                             try
                                             {
                                                 //GO=>don't ask me why am doing this......
                                                 var category =
                                                     outletCategories.FirstOrDefault(
                                                         p => p.Code == domainEntity.OutletCategory.Code);
                                                 _outletCategoryRepository.Save(category);
                                                 domainEntity.OutletCategory = category;
                                             }catch
                                             {
                                                 
                                             }
                                              if (domainEntity.OutletType == null)
                                              {
                                                  OutletType type =
                                                      _outletTypeRepository.GetAll(true).FirstOrDefault(
                                                          p =>
                                                          p.Name != null &&
                                                          p.Name=="defaultoutlettype");

                                                  if (type == null)
                                                  {
                                                      type = new OutletType(Guid.NewGuid())
                                                                 {
                                                                     Name =
                                                                         string.IsNullOrEmpty(importentity.Name)
                                                                             ? "defaultoutlettype"
                                                                             : importentity.Name,
                                                                     Code =
                                                                         string.IsNullOrEmpty(importentity.Name)
                                                                             ? "defaultoutlettype"
                                                                             : importentity.Name
                                                                 };
                                                      try
                                                      {
                                                          _outletTypeRepository.Save(type);
                                                      }
                                                      catch
                                                      {
                                                      }
                                                  }
                                                  domainEntity.OutletType = type;
                                              }
                                              try
                                              {
                                                  //GO=>don't ask me why am doing this......
                                                  var update =
                                                      _outletTypeRepository.GetAll(true).FirstOrDefault(
                                                          p => p.Code == domainEntity.OutletType.Code);
                                                  _outletTypeRepository.Save(update);
                                                  domainEntity.OutletType = update;
                                              }
                                              catch
                                              {

                                              }

                                             if (domainEntity.OutletProductPricingTier == null)
                                              {
                                                  ProductPricingTier tire =
                                                      _productPricingTierRepository.GetAll(true).FirstOrDefault(
                                                          p =>p.Code !=null && p.Code.Trim()==importentity.Currency.Trim());

                                                  if (tire == null)
                                                  {
                                                      tire = new ProductPricingTier(Guid.NewGuid())
                                                                 {
                                                                     Name = importentity.Currency.Trim(),
                                                                     Code = importentity.Currency.Trim(),
                                                                     Description = importentity.Currency.Trim(),
                                                                 };
                                                      try
                                                      {
                                                          _productPricingTierRepository.Save(tire);
                                                      }
                                                      catch
                                                      {
                                                      }
                                                  }

                                                  domainEntity.OutletProductPricingTier = tire;
                                              }
                                             if (domainEntity.VatClass == null)
                                             {
                                                 VATClass productVat = _vatClassRepository.GetAll(true).FirstOrDefault(
                                                     p => p.Name != null &&
                                                          p.Name == "defaultVAT");
                                                 if (productVat == null)
                                                 {
                                                     var viatItem = new VATClass.VATClassItem(Guid.NewGuid())
                                                                        {
                                                                            EffectiveDate = DateTime.Now,
                                                                            Rate = 0,
                                                                        };
                                                     productVat = new VATClass(Guid.NewGuid())
                                                                      {
                                                                          Name = "defaultVAT",
                                                                          VatClass = "defaultVAT",
                                                                      };
                                                     productVat.VATClassItems.Add(viatItem);
                                                     try
                                                     {
                                                         _vatClassRepository.Save(productVat);
                                                     }
                                                     catch
                                                     {
                                                     }
                                                 }
                                                 domainEntity.VatClass = productVat;
                                             }
                                              domainEntity._Status =EntityStatus.Active;
                                              domainEntity.Name = importentity.Name.Trim();
                                              domainEntity.CostCentreCode = importentity.OutletCode.Trim();

                                          }
                                          return temp;
                                      });


        }
    }
}
