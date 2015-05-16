using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr_Middleware.WPF.Lib.Utils;
using PzIntegrations.Lib.ImportEntities;
using StructureMap;

namespace PzIntegrations.Lib.MasterDataImports.Outlets
{
    internal class OutletImportService : IOutletImportService 
    {
        
       private List<OutletImport> imports;

        public OutletImportService()
        {
          
            imports = new List<OutletImport>();
        }

        public async Task<IEnumerable<OutletImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                 imports = new List<OutletImport>();
                var tempFolder =
                    Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                if (File.Exists(tempFolder))
                    File.Delete(tempFolder);
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
                OutletCode = SetFieldValue(dataRow, 1),
                Name = SetFieldValue(dataRow, 2),
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

       string SetFieldValue(string[] dataRow, int index)
       {
           index = index - 1;
           return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
       }

        public IList<ImportValidationResultInfo> ValidateAndSave(List<OutletImport> entities)
        {
            int batchSize = Convert.ToInt32(0.2*entities.Count);
            var outletImports = entities.OrderBy(p => p.OutletCode).Batch(batchSize).Select(x => x.ToList()).ToList();

            #region Contruct outlets
            Task<Outlet[]>[] taskArray = new Task<Outlet[]>[outletImports.Count];
            var results = new List<Outlet>();
            try
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    var current = outletImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        taskArray[i] = Task<Outlet[]>.Factory.StartNew(() => ContsructEntities(current));
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
                batchSize = Convert.ToInt32(0.2*results.Count);
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
            var validatedOutlets = validationResults.Where(n=>n.IsValid).Select(n =>(Outlet)n.Entity).ToList();
            if(validatedOutlets.Any())
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
        private EntityStatus GetStatus(string state)
        {
            if (!string.IsNullOrEmpty(state))
            {
                if (state == "0")
                    return EntityStatus.Inactive;
            }
            return EntityStatus.Active;

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
                                   Entity = outlet
                               }).ToArray();
        }

        
       private Outlet[] ContsructEntities(IEnumerable<OutletImport> entities)
        {
           
                var temp = new List<Outlet>();
                var defaultDistributr =ObjectFactory.GetInstance<ICostCentreRepository>()
                    .GetByCode("PZ Cussons EA", CostCentreType.Distributor);
                   
                var allRoutes = ObjectFactory.GetInstance<IRouteRepository>().GetAll().ToList();
                var outletCategories = ObjectFactory.GetInstance<IOutletCategoryRepository>().GetAll().ToList();
                foreach (var importentity in entities)
                {
                    var domainEntity =
                        ObjectFactory.GetInstance<ICostCentreRepository>().GetByCode(importentity.OutletCode,
                                                                                     CostCentreType.Outlet, true) as
                        Outlet;

                    bool IsNew = false;
                    if(domainEntity==null)
                    {
                        domainEntity = new Outlet(Guid.NewGuid());
                        IsNew = true;
                    }
                    #region Routes

                    if (domainEntity != null && domainEntity.Route == null)
                    {
                        domainEntity.Route = allRoutes.FirstOrDefault(
                           p =>
                           p.Name != null &&
                           p.Name.ToLower().Trim() == importentity.RouteName.Trim().ToLower());

                    }
                   
                    if (domainEntity.Route != null && !string.IsNullOrEmpty(importentity.RouteName))
                    {
                       var newRoute = allRoutes.FirstOrDefault(p => p.Name.Trim().ToLower() == importentity.RouteName.Trim().ToLower());
                        if (newRoute != null)
                            domainEntity.Route = newRoute;
                    }
                   
                    if (domainEntity.Route == null && importentity.RouteName.StartsWith("SALES VA"))
                        domainEntity.Route =
                            allRoutes.FirstOrDefault(p => p.Name.Contains("SALES VA"));

                    if (domainEntity.Route == null && !string.IsNullOrEmpty(importentity.RouteName))
                    {
                        var region = ObjectFactory.GetInstance<IRegionRepository>().GetAll(true).FirstOrDefault(p => p.Name == "Region A");
                        if (region == null)
                        {
                            region = new Region(Guid.NewGuid())
                            {
                                Country = ObjectFactory.GetInstance<ICountryRepository>().GetAll(true).FirstOrDefault(p => p.Name == "Kenya"),
                                Name = "",
                                Description = ""
                            };
                            try
                            {
                                ObjectFactory.GetInstance<IRegionRepository>().Save(region);
                            }
                            catch
                            {

                            }

                        }
                        var route = new Route(Guid.NewGuid())
                        {
                            Name = importentity.RouteName,
                            Code = importentity.RouteName,
                            Region = region
                        };
                        try
                        {
                            ObjectFactory.GetInstance<IRouteRepository>().Save(route);
                        }
                        catch
                        {

                        }
                        domainEntity.Route = route;
                    }
                   
                    try
                    {
                        ObjectFactory.GetInstance<IRouteRepository>().Save(domainEntity.Route);
                    }
                    catch
                    {
                    } 
                    
                    #endregion

                    if (defaultDistributr == null) throw new ArgumentNullException("distributor");
                    domainEntity.ParentCostCentre = new CostCentreRef() { Id = defaultDistributr.Id };
                   

                    if (domainEntity.OutletCategory == null)
                    {

                        OutletCategory category = outletCategories
                            .FirstOrDefault(
                                s => s.Name != null &&
                                     s.Name == "defaultoutletcategory");
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
                                ObjectFactory.GetInstance<IOutletCategoryRepository>().Save(category,true);
                            }
                            catch
                            {
                            }
                        }
                        domainEntity.OutletCategory = category;
                    }
                    
                    if (domainEntity.OutletType == null)
                    {
                        OutletType type =ObjectFactory.GetInstance<IOutletTypeRepository>().GetAll(true).FirstOrDefault(
                                p =>
                                p.Name != null &&
                                p.Name == "defaultoutlettype");

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
                                ObjectFactory.GetInstance<IOutletTypeRepository>().Save(type);
                            }
                            catch
                            {
                            }
                        }
                        domainEntity.OutletType = type;
                    }
                   
                    if (domainEntity.OutletProductPricingTier == null)
                    {
                        ProductPricingTier tire =ObjectFactory.GetInstance<IProductPricingTierRepository>()
                            .GetAll(true).FirstOrDefault(
                                p => p.Code != null && p.Code.Trim() == importentity.Currency.Trim());

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
                                ObjectFactory.GetInstance<IProductPricingTierRepository>().Save(tire);
                            }
                            catch
                            {
                            }
                        }

                        domainEntity.OutletProductPricingTier = tire;
                    }
                    if (domainEntity.VatClass == null)
                    {
                        VATClass productVat = ObjectFactory.GetInstance<IVATClassRepository>().GetAll(true).
                            FirstOrDefault(
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
                                ObjectFactory.GetInstance<IVATClassRepository>().Save(productVat);
                            }
                            catch
                            {
                            }
                        }
                        domainEntity.VatClass = productVat;
                    }
                    domainEntity._Status =EntityStatus.Active; //GetStatus(importentity.Status);
                    domainEntity.Name = importentity.Name.Trim();
                    domainEntity.CostCentreCode = importentity.OutletCode.Trim();
                        temp.Add(domainEntity);

                }
                return temp.ToArray();
           
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

               return false;
       }
    }
}
