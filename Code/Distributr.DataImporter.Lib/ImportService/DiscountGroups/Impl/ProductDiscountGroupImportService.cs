using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Experimental;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using StructureMap;

namespace Distributr.DataImporter.Lib.ImportService.DiscountGroups.Impl
{
    public class ProductDiscountGroupImportService : IProductDiscountGroupImportService
    {
        private readonly IProductDiscountGroupRepository _productDiscountGroupRepository;
        private IProductDiscountGroupMapper _productDiscountGroupMapper;
        private List<string> _failedProductCodes;

        public ProductDiscountGroupImportService(IProductDiscountGroupRepository productDiscountGroupRepository, IProductDiscountGroupMapper productDiscountGroupMapper, IGroupDiscountMapper groupDiscountMapper, CokeDataContext ctx)
        {
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _productDiscountGroupMapper = productDiscountGroupMapper;
        }

        public ProductDiscountGroupImportService(IProductDiscountGroupRepository productDiscountGroupRepository, IProductDiscountGroupMapper productDiscountGroupMapper, IGroupDiscountMapper groupDiscountMapper, List<string> failedProductCodes, CokeDataContext ctx, IProductDiscountGroupFactory productDiscountGroupFactory)
        {
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _productDiscountGroupMapper = productDiscountGroupMapper;
            _failedProductCodes = failedProductCodes;
        }

        public IEnumerable<ProductDiscountGroupImport> Import(string path)
        {
            try
            {
                var productImports = new List<ProductDiscountGroupImport>();
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
                            productImports.Add(MapImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return productImports;
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

        private ProductDiscountGroupImport MapImport(string[] dataRow)
        {
            var discountvaluestring = SetFieldValue(dataRow, 3);
            decimal discountvalue = 0m;
            try
            {
                if (!string.IsNullOrEmpty(discountvaluestring))
                    discountvalue = Convert.ToDecimal(discountvaluestring);

            }
            catch
            {
                discountvalue = 0m;
            }
            return new ProductDiscountGroupImport
            {
                DiscontGroupCode = SetFieldValue(dataRow, 1),
                ProductCode = SetFieldValue(dataRow, 2),
                DiscountValue = discountvalue
            };
        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }


        public IList<ImportValidationResultInfo> Validate(List<ProductDiscountGroupImport> entities)
        {
            IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
            int count = 1;
            foreach (var domainentity in ConstructDomainEntities(entities))
            {
                var res = _productDiscountGroupRepository.Validate(domainentity);

                var importValidationResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Row-" + count,
                    Entity = domainentity,
                    EntityNameOrCode = domainentity.GroupDiscount !=null? domainentity.GroupDiscount.Code ?? domainentity.GroupDiscount.Name: ""
                };
                results.Add(importValidationResult);
                count++;
            }
            return results;
        }

        public Task<IList<ImportValidationResultInfo>> ValidateAsync(List<ProductDiscountGroupImport> entities)
        {
            throw new NotImplementedException();
        }

      

        public List<string> GetNonExistingProductCodes()
        {
            return _failedProductCodes;
        }

        public int GetUpdatedItems()
        {
            return updatedItems;
        }

        public IList<ImportValidationResultInfo> ValidateAndSave(List<ProductDiscountGroupImport> entities = null)
        {
            int batchSize = Convert.ToInt32(0.2 * entities.Count);
            var productDiscountImports = entities.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();

            #region Contruct Items
            var taskArray = new Task<IEnumerable<ProductGroupDiscount>>[productDiscountImports.Count];
            var results = new List<ProductGroupDiscount>();
            try
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    var current = productDiscountImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        taskArray[i] = Task<IEnumerable<ProductGroupDiscount>>.Factory.StartNew(() => ConstructDomainEntities(current));
                        productDiscountImports.Remove(current);
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

            #region Validate
            var validationResults = new List<ImportValidationResultInfo>();
            if (results.Any())
            {
                batchSize = Convert.ToInt32(0.2 * results.Count);
                var productDiscounts = results.OrderBy(p => p._DateLastUpdated).Batch(batchSize).Select(x => x.ToList()).ToList();
                var validationTaskArray = new Task<ImportValidationResultInfo[]>[productDiscounts.Count];


                try
                {
                    for (int i = 0; i < validationTaskArray.Length; i++)
                    {
                        var current = productDiscounts.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            validationTaskArray[i] =
                                Task<ImportValidationResultInfo[]>.Factory.StartNew(() => ValidateDiscounts(current));
                            productDiscounts.Remove(current);
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

            #region Save valid Items
            var validatedProductDiscounts = validationResults.Where(n => n.IsValid).Select(n => (ProductGroupDiscount)n.Entity).ToList();
            if (validatedProductDiscounts.Any())
            {
                batchSize = Convert.ToInt32(0.2 * validatedProductDiscounts.Count);
                var products =
                    validatedProductDiscounts.OrderBy(p => p._DateLastUpdated).Batch(batchSize).Select(x => x.ToList()).ToList();

                var saveTasksArray = new Task<Guid[]>[products.Count];
                try
                {
                    for (int i = 0; i < saveTasksArray.Length; i++)
                    {
                        var current = products.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            saveTasksArray[i] =
                                Task<Guid[]>.Factory.StartNew(() => SaveDiscounts(current));
                            products.Remove(current);
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
            #endregion


            return validationResults.Where(p => !p.IsValid).ToList();
        }


        private int updatedItems = 0;

       private IEnumerable<ProductGroupDiscount> ConstructDomainEntities(IEnumerable<ProductDiscountGroupImport> entities)
        {
            var productGroupDiscounts = new List<ProductGroupDiscount>();
            
            foreach (var importentity in entities)
            {

                decimal rate = Convert.ToDecimal((importentity.DiscountValue / 100));
                var product = ObjectFactory.GetInstance<IProductDiscountGroupMapper>().GetProduct(importentity.ProductCode);
                if (product != null && !string.IsNullOrEmpty(importentity.DiscontGroupCode))
                {
                    var tblD = ObjectFactory.GetInstance<CokeDataContext>().tblDiscountGroup.FirstOrDefault(p => p.Code.ToLower() == importentity.DiscontGroupCode.ToLower());
                    DiscountGroup discountGroup = null;
                    
                    if (tblD == null)
                    {
                        try
                        {
                            ObjectFactory.GetInstance<IDiscountGroupRepository>().Save(new DiscountGroup(Guid.NewGuid())
                                                                                         {
                                                                                             Name =
                                                                                                 importentity.
                                                                                                 DiscontGroupCode,
                                                                                             Code =
                                                                                                 importentity.
                                                                                                 DiscontGroupCode
                                                                                         });

                            discountGroup =
                                ObjectFactory.GetInstance<IGroupDiscountMapper>().FindByCode(
                                    importentity.DiscontGroupCode);



                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        discountGroup = new DiscountGroup(tblD.id);
                    }


                  var  currentPGroupDiscount =ObjectFactory.GetInstance<CokeDataContext>().tblProductDiscountGroup.FirstOrDefault(
                           p =>
                           p.DiscountGroup == discountGroup.Id &&
                           p.tblProductDiscountGroupItem.Any(n => n.ProductRef == product.id));
                    if (currentPGroupDiscount != null)
                    {
                        if(CanUpdate(currentPGroupDiscount,product.id,rate))
                        {
                            var item =
                            currentPGroupDiscount.tblProductDiscountGroupItem.FirstOrDefault(
                                p => p.ProductRef == product.id);
                            if (item != null)
                            {
                                item.DiscountRate = rate;
                                item.EffectiveDate = DateTime.Now;
                                item.IM_DateLastUpdated = DateTime.Now;
                                ObjectFactory.GetInstance<IProductDiscountGroupMapper>().Update(item);
                                updatedItems++;
                            }
                        }
                    }
                    else
                    {

                        var currentProductGroupDiscount =
                            ObjectFactory.GetInstance<IProductDiscountGroupFactory>().CreateProductGroupDiscount(
                                discountGroup,
                                new ProductRef() {ProductId = product.id},
                                rate,
                                DateTime.Now,
                                DateTime.Now.AddMonths(12), false, 0);
                        productGroupDiscounts.Add(currentProductGroupDiscount);
                    }
                }
                else
                {
                    if (!_failedProductCodes.Any(p => p.Equals(importentity.ProductCode, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _failedProductCodes.Add(importentity.ProductCode);
                    }
                }
            }
           
            return productGroupDiscounts;
        }
        
       private bool CanUpdate(tblProductDiscountGroup currentPGroupDiscount, Guid productid, decimal discountValue)
        {
            var item =
                currentPGroupDiscount.tblProductDiscountGroupItem.OrderByDescending(p=>p.IM_DateLastUpdated).FirstOrDefault(
                    n => n.ProductRef == productid);
           var t1 = Math.Round(discountValue, 6);
           var t2 = item != null ? Math.Round(item.DiscountRate, 6) : 0;
           var equal=Decimal.Compare(t1,t2) != 0;
           return item == null || equal;

        }

       private Guid[] SaveDiscounts(IEnumerable<ProductGroupDiscount> discounts)
       {
           return (from discount in discounts
                   let res = ObjectFactory.GetInstance<IProductDiscountGroupRepository>().Save(discount, true)
                   select new Guid(res.ToString())
                   {

                   }).ToArray();
       }
       private ImportValidationResultInfo[] ValidateDiscounts(IEnumerable<ProductGroupDiscount> products)
       {
           return (from product in products
                   let res = ObjectFactory.GetInstance<IProductDiscountGroupRepository>().Validate(product)
                   select new ImportValidationResultInfo()
                   {
                       Results = res.Results,
                       Entity = product
                   }).ToArray();
       }

        public void Save(List<ProductGroupDiscount> groupDiscounts)
        {
            try
            {
                _productDiscountGroupMapper.Insert(groupDiscounts);
            }catch(Exception ex)
            {
                FileUtility.LogError(ex.Message);
            }

        }

        
    }
}
