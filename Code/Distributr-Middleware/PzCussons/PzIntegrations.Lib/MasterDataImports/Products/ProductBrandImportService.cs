using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using Distributr_Middleware.WPF.Lib.Utils;
using PzIntegrations.Lib.ImportEntities;
using StructureMap;

namespace PzIntegrations.Lib.MasterDataImports.Products
{
    public class ProductBrandImportService : IProductBrandImportService
    {
      
        public async Task<IEnumerable<ProductBrandImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var productBrandImports = new List<ProductBrandImport>();
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
                            productBrandImports.Add(MapProductBrandImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return productBrandImports;
            });
        }

        private ProductBrandImport MapProductBrandImport(string[] dataRow)
        {
            return new ProductBrandImport
            {
                Code = SetFieldValue(dataRow, 1),
                Name = SetFieldValue(dataRow, 2),
                Description = SetFieldValue(dataRow, 3),
                SupplierCode = SetFieldValue(dataRow, 4)
            };

        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }


        public IList<ImportValidationResultInfo> ValidateAndSave(List<ProductBrandImport> entities = null)
        {
            int batchSize = Convert.ToInt32(0.2 * entities.Count);
            var brandImports = entities.OrderBy(p => p.Code).Batch(batchSize).Select(x => x.ToList()).ToList();
           
            #region Construct Items
            var taskArray = new Task<ProductBrand[]>[brandImports.Count];
            var results = new List<ProductBrand>();
            try
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    var current = brandImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        taskArray[i] = Task<ProductBrand[]>.Factory.StartNew(() => ConstructEntities(current));
                        brandImports.Remove(current);
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

            #region Validate Items

            var validationResults = new List<ImportValidationResultInfo>();
            if (results.Any())
            {
                batchSize = Convert.ToInt32(0.2 * results.Count);
                var brands = results.OrderBy(p => p.Code).Batch(batchSize).Select(x => x.ToList()).ToList();
                var validationTaskArray = new Task<ImportValidationResultInfo[]>[brands.Count];


                try
                {
                    for (int i = 0; i < validationTaskArray.Length; i++)
                    {
                        var current = brands.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            validationTaskArray[i] =
                                Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidateProductBrands(current));
                            brands.Remove(current);
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

            #region Save valid items
            var validatedBrands = validationResults.Where(n => n.IsValid).Select(n => (ProductBrand)n.Entity).ToList();
            if (validatedBrands.Any())
            {
                batchSize = Convert.ToInt32(0.2 * validatedBrands.Count);
                var brands =
                    validatedBrands.OrderBy(p => p.Code).Batch(batchSize).Select(x => x.ToList()).ToList();

                var saveTasksArray = new Task<Guid[]>[brands.Count];
                try
                {
                    for (int i = 0; i < saveTasksArray.Length; i++)
                    {
                        var current = brands.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            saveTasksArray[i] =
                                Task<Guid[]>.Factory.StartNew(() => SaveProductBrands(current));
                            brands.Remove(current);
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

        private Guid[] SaveProductBrands(IEnumerable<ProductBrand> productBrands)
        {
            return (from brand in productBrands
                    let res = ObjectFactory.GetInstance<IProductBrandRepository>().Save(brand, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }

        private ImportValidationResultInfo[] ValidateProductBrands(IEnumerable<ProductBrand> productBrands)
        {
            return (from brand in productBrands
                    let res = ObjectFactory.GetInstance<IProductBrandRepository>().Validate(brand)
                    select new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Entity = brand
                    }).ToArray();
        }

        private ProductBrand[] ConstructEntities(IEnumerable<ProductBrandImport> entities)
        {
            
                var temp = new List<ProductBrand>();
              
                foreach (var entity in entities)
                {
                    var brand = ObjectFactory.GetInstance<IProductBrandRepository>().GetAll(true).FirstOrDefault(
                            p =>p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase));

                    bool isNew = false;
                    if(brand==null)
                    {
                        brand = new ProductBrand(Guid.NewGuid());
                        isNew = true;
                    }

                    var supplier = ObjectFactory.GetInstance<ISupplierRepository>().GetAll(true).FirstOrDefault(p => p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase) || p.Code == "default-pz") ??
                                   new Supplier(Guid.NewGuid()) { Code = "default-pz", Name = "PZ-default" };

                    try
                    {
                        ObjectFactory.GetInstance<ISupplierRepository>().Save(supplier);
                    }
                    catch
                    {
                        //die silently if things screw up here
                    }
                    brand.Code = entity.Code;
                    brand.Description = entity.Description;
                    brand.Name = entity.Name;
                    brand.Supplier = supplier;
                    if(isNew || HasChanged(brand))
                    temp.Add(brand);
                }

                return temp.ToArray();
            

        }

        bool HasChanged(ProductBrand item)
        {
            var brand = ObjectFactory.GetInstance<CokeDataContext>()
                .tblProductBrand.FirstOrDefault(p=>p.id==item.Id);
            if(brand==null)return true;
           if(brand.code.Trim().ToLower() !=item.Code.Trim().ToLower())
                return true;
            if(brand.name.Trim().ToLower() !=item.Name.Trim().ToLower())
                return true;

            return false;
        }
    }
}
