using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class ProductGroupDiscountImportService : MasterDataImportServiceBase, IProductGroupDiscountImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public ProductGroupDiscountImportService()
        {
            validationResultInfos=new List<ImportValidationResultInfo>();
        }
        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

        public Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return Task.Run(() =>
                                {
                                    var response = new MasterDataImportResponse();
                                    var importEntities = imports as List<ImportEntity> ?? imports.ToList();
                                    int batchSize = Convert.ToInt32(0.2*importEntities.Count());
                                    var productImports =
                                        importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                                            x => x.ToList()).ToList();
                                    validationResultInfos.Clear();

                                    #region Construct

                                    var taskArray = new Task<ProductGroupDiscountDTO[]>[productImports.Count];
                                    var results = new List<ProductGroupDiscountDTO>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = productImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<ProductGroupDiscountDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
                                                productImports.Remove(current);
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

                                    #region validate
                                    var validationResults = new List<ImportValidationResultInfo>();
                                    if (validationResultInfos.Any())
                                    {
                                        validationResults.AddRange(validationResultInfos);
                                        validationResultInfos.Clear();
                                    }
                                    if (results.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*results.Count);
                                        var productGroupDiscounts =
                                            results.OrderBy(p => p.ProductMasterId).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();
                                        var validationTaskArray =
                                            new Task<ImportValidationResultInfo[]>[productGroupDiscounts.Count];

                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = productGroupDiscounts.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                                            () => MapAndValidate(current));
                                                    productGroupDiscounts.Remove(current);
                                                }
                                            }

                                            foreach (var result in validationTaskArray.Select(n => n.Result).ToList())
                                            {
                                                validationResultInfos.AddRange(result);
                                            }
                                        }
                                        catch (AggregateException ex)
                                        {
                                        }

                                    }

                                    #endregion

                                    #region Save

                                    var validatedProductDGroups =
                                        validationResultInfos.Where(n => n.Entity is ProductGroupDiscount && n.IsValid)
                                            .Select
                                            (n => (ProductGroupDiscount) n.Entity).ToList();
                                    if (validatedProductDGroups.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedProductDGroups.Count);
                                        var discounts =
                                            validatedProductDGroups.OrderBy(p => p._DateLastUpdated).Batch(batchSize).
                                                Select(x => x.ToList()).ToList();

                                        var saveTasksArray = new Task<Guid[]>[discounts.Count];
                                        try
                                        {
                                            for (int i = 0; i < saveTasksArray.Length; i++)
                                            {
                                                var current = discounts.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    saveTasksArray[i] =
                                                        Task<Guid[]>.Factory.StartNew(
                                                            () => SaveProductDiscountGroups(current));
                                                    discounts.Remove(current);
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

                                    response.ValidationResults = validationResultInfos;

                                    return response;
                                });
        }
        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<ProductGroupDiscountDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblProductDiscountGroup.
                        FirstOrDefault(p =>p.DiscountGroup ==dto.DiscountGroupMasterId && p.tblProductDiscountGroupItem.Any(n=>n.ProductRef==dto.ProductMasterId));

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<IProductDiscountGroupRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} Description or code=>{1}", index,
                                      "product discount group item"),
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }
        private Guid[] SaveProductDiscountGroups(IEnumerable<ProductGroupDiscount> items)
        {
            return (from item in items
                    let res = ObjectFactory.GetInstance<IProductDiscountGroupRepository>().Save(item, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
        private ProductGroupDiscountDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductGroupDiscountDTO>();
            var vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                string discountGroupCode = SetFieldValue(row, 1);
                string productCodeName = SetFieldValue(row, 2);
                string discountValue = SetFieldValue(row, 3);
                string effectiveDateValue = SetFieldValue(row, 4);
                string endDateValue = SetFieldValue(row, 5);

                var discountGroup = GetDiscountGroup(discountGroupCode);
                var product = GetProduct(productCodeName);
                if (product == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "product  with code {0} or Name={1} not found",
                                                  productCodeName,productCodeName))
                                      };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                if (discountGroup == null)
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format(
                                              "Discount group  with code or Name {0} not found",
                                              discountGroupCode))
                                  };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                 
                
                decimal value = 0m;
                if (!string.IsNullOrEmpty(discountValue))
                {
                    try
                    {
                        value = Convert.ToDecimal(discountValue);
                    }
                    catch
                    {
                        value = 0m;
                    }
                }
                 
                  DateTime effectiveDate = DateTime.Now;
                if(!string.IsNullOrEmpty(effectiveDateValue))
                {
                    try
                    {
                        effectiveDate = Convert.ToDateTime(effectiveDateValue);
                    }
                    catch
                    {
                        effectiveDate = DateTime.Now;
                    }
                }
                
                  DateTime endDate = DateTime.Now.AddMonths(12);
                  if (!string.IsNullOrEmpty(endDateValue))
                {
                    try
                    {
                        endDate = Convert.ToDateTime(endDateValue);
                    }
                    catch
                    {
                        endDate =  DateTime.Now.AddMonths(12);
                    }
                }

                return new ProductGroupDiscountDTO()
                           {
                               ProductMasterId = product.id,
                               DiscountGroupMasterId = discountGroup.id,
                             
                                                                    EffectiveDate = effectiveDate,
                                                                    EndDate = endDate,
                                                                    DiscountRate = value,
                                                                   
                                                    

                           };
            }));
            Object lockMe = new Object();
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
            return items.ToArray();

        }
        private bool HasChanged(ProductGroupDiscount groupD)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                var dGroup = ctx.tblProductDiscountGroup.FirstOrDefault(p => p.id == groupD.Id);
                if (dGroup == null) return true;
                var productId = groupD.Product.ProductId;
                var discountValue = groupD.DiscountRate;

                var item = dGroup.tblProductDiscountGroupItem.OrderBy(p=>p.IM_DateLastUpdated).FirstOrDefault(p => p.ProductRef == productId);

                return item == null || item.DiscountRate.ToString("0.00") != discountValue.ToString("0.00");
            }
        }
        private tblDiscountGroup GetDiscountGroup(string itemName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblDiscountGroup pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx
                        .tblDiscountGroup.FirstOrDefault(
                            p => p.Code != null &&
                            p.Code.ToLower() == itemName.ToLower() || p.Name.ToLower() == itemName.ToLower());
                return pitem;
            }
        }
        private tblProduct GetProduct(string itemName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblProduct pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx
                        .tblProduct.FirstOrDefault(
                            p =>p.ProductCode != null &&
                            p.ProductCode.ToLower() == itemName.ToLower()|| p.Description.ToLower() == itemName.ToLower());
                return pitem;
            }
        }
    }
}
