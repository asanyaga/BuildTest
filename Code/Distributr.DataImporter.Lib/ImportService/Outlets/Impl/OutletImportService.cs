using System;
using System.Collections.Generic;
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
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using StructureMap;

namespace Distributr.DataImporter.Lib.ImportService.Outlets.Impl
{
  public  class OutletImportService : IOutletImportService
  {

      public OutletImportService()
      {

      }

      public IEnumerable<OutletImport> Import(string path)
      {
          try
          {
              var imports = new List<OutletImport>();
              var tempFolder =
                  Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
              if (File.Exists(tempFolder))
                  File.Delete(tempFolder);
              File.Copy(path, tempFolder);
              using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
              {
                  parser.SetDelimiters(",");
                  string[] currentRow = null;

                  while (!parser.EndOfData)
                  {
                      currentRow = parser.ReadFields();
                      if (currentRow != null && currentRow.Length > 0)
                      {
                          imports.Add(MapOutletImport(currentRow));
                      }
                  }

              }
              File.Delete(tempFolder);
              return imports;
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

      private OutletImport MapOutletImport(string[] dataRow)
      {
          var item = new OutletImport
          {
              OutletCode = SetFieldValue(dataRow, 1),
              Name = SetFieldValue(dataRow, 2),
              Address = SetFieldValue(dataRow,3),
              PhoneNo = SetFieldValue(dataRow,4),
              ContactPerson = SetFieldValue(dataRow,5),
              DiscountGroupCode = SetFieldValue(dataRow,6),
              RouteCode = SetFieldValue(dataRow,7),
              TierCode = SetFieldValue(dataRow,8),
              SpecialPrice = SetFieldValue(dataRow,9),
              Discount=SetFieldValue(dataRow,10),
              VatClass=SetFieldValue(dataRow,11),
              Credit = SetFieldValue(dataRow,12),
              DistributorCode = SetFieldValue(dataRow,13)
          };

          return item;

      }

      string SetFieldValue(string[] dataRow, int index)
      {
          index = index - 1;
          return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
      }

      public IList<ImportValidationResultInfo> ValidateAndSave(List<OutletImport> entities)
      {
          int batchSize = Convert.ToInt32(0.2 * entities.Count);
          var outletImports = entities.OrderBy(p => p.OutletCode).Batch(batchSize).Select(x => x.ToList()).ToList();

          #region Contruct outlets
          Task<IEnumerable<Outlet>>[] taskArray = new Task<IEnumerable<Outlet>>[outletImports.Count];
          var results = new List<Outlet>();
          try
          {
              for (int i = 0; i < taskArray.Length; i++)
              {
                  var current = outletImports.FirstOrDefault();
                  if (current != null && current.Any())
                  {
                      taskArray[i] = Task<IEnumerable<Outlet>>.Factory.StartNew(() => ConstructDomainEntities(current));
                      outletImports.Remove(current);
                  }
              }

              foreach (var result in taskArray.Select(n => n.Result).ToList())
              {
                  results.AddRange(result);
              }
          }
          catch (AggregateException ex)
          {
          }
          #endregion

          #region Validate Outlets
          var validationResults = new List<ImportValidationResultInfo>();
          if (results.Any())
          {
              batchSize = Convert.ToInt32(0.2 * results.Count);
              var outlets = results.OrderBy(p => p.CostCentreCode).Batch(batchSize).Select(x => x.ToList()).ToList();
              var validationTaskArray = new Task<ImportValidationResultInfo[]>[outlets.Count];


              try
              {
                  for (int i = 0; i < validationTaskArray.Length; i++)
                  {
                      var current = outlets.FirstOrDefault();
                      if (current != null && current.Any())
                      {
                          validationTaskArray[i] =
                              Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidateOutlets(current));
                          outlets.Remove(current);
                      }
                  }

                  foreach (var result in validationTaskArray.Select(n => n.Result).ToList())
                  {
                      validationResults.AddRange(result);
                  }
              }
              catch (AggregateException ex)
              {
              }

          }
          #endregion

          #region Save valid outlets
          var validatedOutlets = validationResults.Where(n => n.IsValid).Select(n => (Outlet)n.Entity).ToList();
          if (validatedOutlets.Any())
          {
              batchSize = Convert.ToInt32(0.2 * validatedOutlets.Count);
              var outlets =
                  validatedOutlets.OrderBy(p => p.CostCentreCode).Batch(batchSize).Select(x => x.ToList()).ToList();

              var saveTasksArray = new Task<Guid[]>[outlets.Count];
              try
              {
                  for (int i = 0; i < saveTasksArray.Length; i++)
                  {
                      var current = outlets.FirstOrDefault();
                      if (current != null && current.Any())
                      {
                          saveTasksArray[i] =
                              Task<Guid[]>.Factory.StartNew(() => SaveOutlets(current));
                          outlets.Remove(current);
                      }
                  }
                  var savedResults = new List<Guid>();
                  foreach (var result in saveTasksArray.Select(n => n.Result).ToList())
                  {
                      savedResults.AddRange(result);
                  }
              }
              catch (AggregateException ex)
              {
              }
          }
          return validationResults.Where(p => !p.IsValid).ToList();

          #endregion

      }

      private IEnumerable<Outlet> ConstructDomainEntities(IEnumerable<OutletImport> entities)
      {

          var newOutlet = new List<Outlet>();

          var defaultDistributr = ObjectFactory.GetInstance<ICostCentreRepository>().GetByCode("87878", CostCentreType.Distributor) as Distributor;
          var discountGroups = ObjectFactory.GetInstance<IDiscountGroupRepository>().GetAll(true).ToList();
          foreach (var importentity in entities)
          {
              var domainEntity = ObjectFactory.GetInstance<ICostCentreRepository>().GetByCode(importentity.OutletCode, CostCentreType.Outlet, true) as Outlet;
              bool isNew = false;
              if(domainEntity==null)
              {
                  domainEntity = new Outlet(Guid.NewGuid());
                  isNew = true;
              }
             
              if (!string.IsNullOrEmpty(importentity.DistributorCode))
              {
                  defaultDistributr = ObjectFactory.GetInstance<ICostCentreRepository>().GetByCode(importentity.DistributorCode, CostCentreType.Distributor) as Distributor ?? defaultDistributr;

              }

              domainEntity.ParentCostCentre = defaultDistributr != null ? new CostCentreRef() { Id = defaultDistributr.Id } : null;

              #region routes
              var route = ObjectFactory.GetInstance<IRouteRepository>().GetAll(true).FirstOrDefault(
                      s =>
                      s.Code != null && s.Code == importentity.RouteCode ||
                      s.Name != null && s.Name.Equals(importentity.RouteCode, StringComparison.CurrentCultureIgnoreCase));

              if (route == null && !string.IsNullOrEmpty(importentity.RouteCode))
              {
                  route = new Route(Guid.NewGuid())
                  {
                      Code = importentity.RouteCode,
                      Name = importentity.RouteCode,
                      Region = defaultDistributr.Region
                  };
                  try
                  {
                      ObjectFactory.GetInstance<IRouteRepository>().Save(route);

                  }
                  catch (Exception ex)
                  {
                      FileUtility.LogError(ex.Message);
                  }
              }
              if (string.IsNullOrEmpty(importentity.RouteCode))
              {
                  route = ObjectFactory.GetInstance<IRouteRepository>().GetAll(true).FirstOrDefault(p => p.Code == "default");
                  if (route == null)
                  {
                      route = new Route(Guid.NewGuid())
                      {
                          Code = "default",
                          Name = "default",
                          Region = defaultDistributr.Region
                      };
                      try
                      {
                          ObjectFactory.GetInstance<IRouteRepository>().Save(route);

                      }
                      catch (Exception ex)
                      {
                          FileUtility.LogError(ex.Message);
                      }
                  }

              }
              domainEntity.Route = route;
              #endregion

              #region Type and Category
              var allCategories = ObjectFactory.GetInstance<IOutletCategoryRepository>().GetAll();
              var category = allCategories.FirstOrDefault(s =>
                  s.Code != null && s.Code == importentity.OutletCategoryName || s.Name != null && s.Name == importentity.OutletCategoryName);
              if (category == null)
              {
                  category = allCategories.FirstOrDefault(s =>
                      s.Name != null && s.Name.Equals("defaultoutletcategory", StringComparison.CurrentCultureIgnoreCase));
                  if (category == null)
                  {
                      category = new OutletCategory(Guid.NewGuid())
                      {
                          Name = string.IsNullOrEmpty(importentity.OutletCategoryName)
                                     ? "defaultoutletcategory"
                                     : importentity.OutletCategoryName,
                          Code = string.IsNullOrEmpty(importentity.OutletCategoryName)
                                     ? "defaultoutletcategory"
                                     : importentity.OutletCategoryName
                      };
                      try
                      {
                          ObjectFactory.GetInstance<IOutletCategoryRepository>().Save(category);
                      }
                      catch
                      {
                      }
                  }

              }
              domainEntity.OutletCategory = category;
              var type = ObjectFactory.GetInstance<IOutletTypeRepository>().GetAll(true).FirstOrDefault(p =>
                  p.Code != null && p.Code == importentity.OutletTypeName || p.Name != null && p.Name == importentity.OutletTypeName);
              if (type == null)
              {
                  type = ObjectFactory.GetInstance<IOutletTypeRepository>().GetAll(true).FirstOrDefault(p =>
                      p.Name != null && p.Name.Equals("defaultoutlettype", StringComparison.InvariantCultureIgnoreCase));
                  if (type == null)
                  {
                      type = new OutletType(Guid.NewGuid())
                      {
                          Name =
                              string.IsNullOrEmpty(importentity.OutletTypeName)
                                  ? "defaultoutlettype"
                                  : importentity.OutletTypeName,
                          Code =
                              string.IsNullOrEmpty(importentity.OutletTypeName)
                                  ? "defaultoutlettype"
                                  : importentity.OutletTypeName
                      };
                      try
                      {
                          ObjectFactory.GetInstance<IOutletTypeRepository>().Save(type);
                      }
                      catch
                      {
                      }
                  }

              }
              domainEntity.OutletType = type;
              #endregion

              #region pricing tier

              ProductPricingTier tier = ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll(true).FirstOrDefault(p => p.Name == "DefaultPricingTier");
              if (tier == null)
              {
                  tier = new ProductPricingTier(Guid.NewGuid())
                  {
                      Name = "DefaultPricingTier",
                      Code = "DefaultPricingTier",
                      Description = "DefaultPricingTier"
                  };
                  try
                  {
                      ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(tier);
                  }
                  catch
                  {
                  }
              }

              //create special tier
              if (!string.IsNullOrEmpty(importentity.TierCode))
              {
                  var specialTier = ObjectFactory.GetInstance<IProductPricingTierRepository>().GetAll(true).FirstOrDefault(p => p.Code != null && p.Code.ToLower() == importentity.TierCode.ToLower()) ??
                      new ProductPricingTier(Guid.NewGuid())
                      {
                          Name = importentity.TierCode,
                          Code = importentity.TierCode,
                          Description = importentity.TierCode,
                      };
                  try
                  {
                      ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(specialTier);
                  }
                  catch
                  {
                  }
                  domainEntity.SpecialPricingTier = specialTier;
              }
              domainEntity.OutletProductPricingTier = tier;


              #endregion

              #region Discount Group

              if (!string.IsNullOrEmpty(importentity.DiscountGroupCode))
              {
                  var discountGroup = discountGroups.FirstOrDefault(p => p.Code == importentity.DiscountGroupCode) ??
                                   SaveDiscountGroup(new DiscountGroup(Guid.NewGuid())
                                   {
                                       Name = importentity.DiscountGroupCode,
                                       Code = importentity.DiscountGroupCode
                                   });
                  domainEntity.DiscountGroup = discountGroup;
              }

              #endregion
              
              #region VAT class
              var productVat = ObjectFactory.GetInstance<IVATClassRepository>().GetAll(true).FirstOrDefault(p =>
                  p.VatClass != null && p.VatClass == importentity.VatClass);
              if (productVat == null)
              {
                  productVat = ObjectFactory.GetInstance<IVATClassRepository>().GetAll(true).FirstOrDefault(p =>
                      p.Name != null && p.Name.Equals("defaultVAT", StringComparison.CurrentCultureIgnoreCase));
                  if (productVat == null)
                  {
                      var viatItem = new VATClass.VATClassItem(Guid.NewGuid())
                      {
                          EffectiveDate = DateTime.Now,
                          Rate = 0,

                      };
                      productVat = new VATClass(Guid.NewGuid())
                      {
                          Name =
                              string.IsNullOrEmpty(importentity.VatClass)
                                  ? "defaultVAT"
                                  : importentity.VatClass,
                          VatClass =
                              string.IsNullOrEmpty(importentity.VatClass)
                                  ? "defaultVAT"
                                  : importentity.VatClass,
                      };
                      productVat.VATClassItems.Add(viatItem);
                      try
                      {
                          ObjectFactory.GetInstance<IVATClassRepository>().Save(productVat);
                      }
                      catch
                      {
                      }
                  }
              }

              domainEntity.VatClass = productVat;
              #endregion

              domainEntity.Name = importentity.Name;
              domainEntity.CostCentreCode = importentity.OutletCode;
              domainEntity._Status = EntityStatus.Active;
              if (isNew || OutletHasChanged(domainEntity))
                  newOutlet.Add(domainEntity);
          }
          return newOutlet.ToList();
      }
      private Guid[] SaveOutlets(IEnumerable<Outlet> outlets)
      {
          return (from outlet in outlets
                  let res = ObjectFactory.GetInstance<ICostCentreRepository>().Save(outlet, true)
                  select new Guid(res.ToString())
                  {

                  }).ToArray();
      }
      private ImportValidationResultInfo[] ValidateOutlets(IEnumerable<Outlet> outlets)
      {
          return (from outlet in outlets
                  let res = ObjectFactory.GetInstance<ICostCentreRepository>().Validate(outlet)
                  select new ImportValidationResultInfo()
                  {
                      Results = res.Results,
                      Entity = outlet,
                      Description = string.Format("Outlet code {0} or name {1}",outlet.CostCentreCode,outlet.Name)
                  }).ToArray();
      }
      private bool OutletHasChanged(Outlet item)
      {
          var outlet = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(item.Id) as Outlet;

          if (outlet == null) return true;
          if (outlet.CostCentreCode.Trim() != item.CostCentreCode.Trim())
              return true;
          if (outlet.Name.Trim().ToLower() != item.Name.Trim().ToLower())
              return true;
          if (outlet.Route.Id != item.Route.Id)
              return true;
          if (outlet.VatClass.Id != item.VatClass.Id)
              return true;
          if (outlet.OutletProductPricingTier.Id != item.OutletProductPricingTier.Id)
              return true;
          if ((outlet.SpecialPricingTier == null && item.SpecialPricingTier != null) || (outlet.SpecialPricingTier != null && item.SpecialPricingTier == null))
              return true;
          if (outlet.SpecialPricingTier != null && item.SpecialPricingTier != null && outlet.SpecialPricingTier.Id !=item.SpecialPricingTier.Id)
              return true;
          if (outlet.ParentCostCentre.Id != item.ParentCostCentre.Id)
              return true;
          if ((outlet.DiscountGroup == null && item.DiscountGroup != null) || (outlet.DiscountGroup != null && item.DiscountGroup == null))
              return true;
          if (outlet.DiscountGroup != null && item.DiscountGroup != null && outlet.DiscountGroup.Id != item.DiscountGroup.Id)
              return true;

          return false;
      }

      #region old code
      public IList<ImportValidationResultInfo> Validate(List<OutletImport> entities)
        {
            IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
            int count = 1;
            foreach (var domainentity in ConstructDomainEntities(entities))
            {
                var res = ObjectFactory.GetInstance<ICostCentreRepository>().Validate(domainentity);

                var importValidationResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Row-" + count,
                    Entity = domainentity,
                    EntityNameOrCode=domainentity.CostCentreCode??domainentity.Name
                };
                results.Add(importValidationResult);


                count++;

            }
            return results;
        }

      public Task<IList<ImportValidationResultInfo>> ValidateAsync(List<OutletImport> entities)
      {
          throw new NotImplementedException();
      }
      
      private DiscountGroup SaveDiscountGroup(DiscountGroup discountGroup)
      {
          try
          {
              var exists = ObjectFactory.GetInstance<IDiscountGroupRepository>().GetAll(true).FirstOrDefault(p => p.Code == discountGroup.Code);
              if (exists !=null)
              {
                  exists.Code = discountGroup.Code;
                  exists.Name = discountGroup.Name;
                  ObjectFactory.GetInstance<IDiscountGroupRepository>().Save(exists);
                  return exists;
              }
              ObjectFactory.GetInstance<IDiscountGroupRepository>().Save(discountGroup);
              return discountGroup;

          }
          catch (Exception ex)
          {
          }
          return null;
      }

      public void Save(List<Outlet> entities)
        {
            foreach (var entity in entities)
            {
                ObjectFactory.GetInstance<ICostCentreRepository>().Save(entity);
            }
        }
      #endregion
  }
}
