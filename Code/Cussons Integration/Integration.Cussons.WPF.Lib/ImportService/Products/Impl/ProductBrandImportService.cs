using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportService.Products.Impl
{
    internal class ProductBrandImportService : IProductBrandImportService
    {
        private readonly IProductBrandRepository _productBrandRepository;
        private readonly ISupplierRepository _supplierRepository;

        public ProductBrandImportService(IProductBrandRepository productBrandRepository, ISupplierRepository supplierRepository)
        {
            _productBrandRepository = productBrandRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<ProductBrandImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
                                {
                                    IEnumerable<ProductBrandImport> productBrandImports;
                                    try
                                    {
                                        
                                        var inputFileDescription = new CsvFileDescription
                                                                                      {
                                                                                          // cool - I can specify my own separator!
                                                                                          SeparatorChar = '\t',//tab delimited
                                                                                          FirstLineHasColumnNames =
                                                                                              false,
                                                                                          QuoteAllFields = true,
                                                                                          EnforceCsvColumnAttribute =
                                                                                              true
                                                                                      };

                                        CsvContext cc = new CsvContext();

                                        productBrandImports = cc.Read<ProductBrandImport>(path, inputFileDescription);


                                    }
                                    catch (FileNotFoundException ex)
                                    {
                                        MessageBox.Show("File not found on specified path:\n"+path);
                                        return null;
                                    }
                                    catch (FieldAccessException ex)
                                    {
                                        MessageBox.Show("File cannot be accessed,is it in use by another application?","Importer Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                                        return null;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Unknown Error:Details\n" + ex.Message, "Importer Error",
                                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                        return null;
                                    }
                                    return productBrandImports;
                                });
        }
        private async Task<IEnumerable<ProductBrand>> ConstructEntities(IEnumerable<ProductBrandImport> entities)
        {
            return await Task.Run(() =>
            {
                var temp = new List<ProductBrand>();
                var existing = _productBrandRepository.GetAll(true).ToList();
                foreach (var entity in entities)
                {
                    var brand = existing.FirstOrDefault(p => p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase)) ??
                                        new ProductBrand(Guid.NewGuid());

                    var supplier = _supplierRepository.GetAll(true).FirstOrDefault(p => p.Code != null && p.Code.Equals(entity.Code, StringComparison.CurrentCultureIgnoreCase) || p.Name != null && p.Name.Equals(entity.Name, StringComparison.CurrentCultureIgnoreCase) || p.Code == "default-pz") ??
                                   new Supplier(Guid.NewGuid()) {Code = "default-pz", Name = "PZ-default"};

                    try
                    {
                        _supplierRepository.Save(supplier);
                    }catch
                    {
                        //die silently if things screw up here
                    }
                    brand.Code = entity.Code;
                    brand.Description = entity.Description;
                    brand.Name = entity.Name;
                    brand.Supplier = supplier;
                    temp.Add(brand);
                }

                return temp;
            });

        }

        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ProductBrandImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var productBrands = await ConstructEntities(entities);
                int count = 0;
                foreach (var product in productBrands)
                {
                    var res = await ValidateEntityAsync(product);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description = "Row-" + count,
                        Entity = product
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
            
        }

        
        public async Task<bool> SaveAsync(IEnumerable<ProductBrand> entities)
        {
            return await Task.Run(() =>
            {
                entities.ToList().ForEach(n => _productBrandRepository.Save(n));
                return true;
            });
        }

        private async Task<ImportValidationResultInfo> ValidateEntityAsync(ProductBrand productBrand)
        {
            return await Task.Run(() =>
            {
                var res = _productBrandRepository.Validate(productBrand);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Entity = productBrand
                };
            });

        }
        
    }
}
