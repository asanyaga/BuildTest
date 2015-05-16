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
    public class PromotionDiscountImportService : MasterDataImportServiceBase, IPromotionDiscountImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public PromotionDiscountImportService()
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
                int batchSize = Convert.ToInt32(0.2 * importEntities.Count());
                var promoImports =
                    importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                        x => x.ToList()).ToList();
                validationResultInfos.Clear();

                #region Construct

                var taskArray = new Task<PromotionDiscountDTO[]>[promoImports.Count];
                var results = new List<PromotionDiscountDTO>();
                try
                {
                    for (int i = 0; i < taskArray.Length; i++)
                    {
                        var current = promoImports.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            taskArray[i] =
                                Task<PromotionDiscountDTO[]>.Factory.StartNew(
                                    () => ConstructDtOs(current));
                            promoImports.Remove(current);
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
                    batchSize = Convert.ToInt32(0.2 * results.Count);
                    var promoDiscounts =
                        results.OrderBy(p => p.ProductMasterId).Batch(batchSize).Select(
                            x => x.ToList()).ToList();
                    var validationTaskArray =
                        new Task<ImportValidationResultInfo[]>[promoDiscounts.Count];

                    try
                    {
                        for (int i = 0; i < validationTaskArray.Length; i++)
                        {
                            var current = promoDiscounts.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                validationTaskArray[i] =
                                    Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                        () => MapAndValidate(current));
                                promoDiscounts.Remove(current);
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

                var validatedPromotions =
                    validationResultInfos.Where(n => n.Entity is PromotionDiscount && n.IsValid).Select
                        (n => (PromotionDiscount)n.Entity).ToList();
                if (validatedPromotions.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * validatedPromotions.Count);
                    var promotions =
                        validatedPromotions.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).
                            Select(x => x.ToList()).ToList();

                    var saveTasksArray = new Task<Guid[]>[promotions.Count];
                    try
                    {
                        for (int i = 0; i < saveTasksArray.Length; i++)
                        {
                            var current = promotions.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                saveTasksArray[i] =
                                    Task<Guid[]>.Factory.StartNew(() => SavePromotions(current));
                                promotions.Remove(current);
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

                response.ValidationResults = validationResults;

                return response;
            });
        }

        private Guid[] SavePromotions(IEnumerable<PromotionDiscount> current)
        {
            return (from item in current
                    let res = ObjectFactory.GetInstance<IPromotionDiscountRepository>().Save(item, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }

        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<PromotionDiscountDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblPromotionDiscount.
                        FirstOrDefault(p =>p.ProductRef == dto.ProductMasterId);

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasPromotionDiscountChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<IPromotionDiscountRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Promotion discount",
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }

        private bool HasPromotionDiscountChanged(PromotionDiscount entity)
        {
           
                var repo = ObjectFactory.GetInstance<IPromotionDiscountRepository>();
                var promo = repo.GetById(entity.Id);
                if (promo == null) return true;
                if (promo.CurrentDiscountRate != entity.CurrentDiscountRate ||
                    promo.CurrentFreeOfChargeProduct != entity.CurrentFreeOfChargeProduct ||
                    promo.CurrentFreeOfChargeQuantity != entity.CurrentFreeOfChargeQuantity ||
                    promo.CurrentParentProductQuantity != entity.CurrentParentProductQuantity) return true;
            return false;

        }
        private PromotionDiscountDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
           var items = new List<PromotionDiscountDTO>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                var productNameCode = SetFieldValue(row, 1);
                string parentQuantity = SetFieldValue(row, 2);
                string freeQuantity = SetFieldValue(row, 3);
                string effectiveDateValue = SetFieldValue(row, 4);
                string endDateValue = SetFieldValue(row, 5);
                var freeProductCode = SetFieldValue(row, 6);

                var product = GetProduct(productNameCode);
                if (product == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "Product  with code {0} or Name={1} not found",
                                                  productNameCode,productNameCode))
                                      };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                var freeproduct = GetProduct(freeProductCode);

                int freeQuantityValue = 0;
                if (!string.IsNullOrEmpty(freeQuantity))
                {
                    try
                    {
                        freeQuantityValue = Convert.ToInt32(freeQuantity);
                    }
                    catch
                    {
                        freeQuantityValue = 0;
                    }
                }
               
                DateTime effectiveDate = DateTime.Now;
                if (!string.IsNullOrEmpty(effectiveDateValue))
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
                        endDate = Convert.ToDateTime(effectiveDate);
                    }
                    catch
                    {
                        endDate = DateTime.Now.AddMonths(12);
                    }
                }

                int parentQuantityvalue = 0;
                if (!string.IsNullOrEmpty(parentQuantity))
                {
                    try
                    {
                        parentQuantityvalue = Convert.ToInt32(parentQuantity);
                    }
                    catch
                    {
                        parentQuantityvalue = 0;
                    }
                }

                return new PromotionDiscountDTO()
                {
                    ProductMasterId = product.id,
                    
                  PromotionDiscountItems = new List<PromotionDiscountItemDTO>()
                                                   {
                                                       new PromotionDiscountItemDTO()
                                                           {
                                                               EffectiveDate = effectiveDate,
                                                               EndDate = endDate,
                                                               DiscountRate =freeQuantityValue,
                                                               ProductMasterId = freeproduct !=null?freeproduct.id:default(Guid),
                                                               ParentQuantity = parentQuantityvalue,
                                                               FreeQuantity = freeQuantityValue
                                                               
                                                           }
                                                   }

                };
            }));
            return items.ToArray();


        }

       

        
    }
}
