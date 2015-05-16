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
    public class CertainValueCertainProductDiscountImportService : MasterDataImportServiceBase, ICertainValueCertainProductDiscountImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public CertainValueCertainProductDiscountImportService()
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
                                    var importItems =
                                        importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                                            x => x.ToList()).ToList();
                                    validationResultInfos.Clear();

                                    #region Construct

                                    var taskArray =
                                        new Task<CertainValueCertainProductDiscountDTO[]>[importItems.Count];
                                    var results = new List<CertainValueCertainProductDiscountDTO>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = importItems.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<CertainValueCertainProductDiscountDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
                                                importItems.Remove(current);
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

                                    if (results.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*results.Count);
                                        var productDiscounts =
                                            results.OrderBy(p => p.InitialValue).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();
                                        var validationTaskArray =
                                            new Task<ImportValidationResultInfo[]>[productDiscounts.Count];

                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = productDiscounts.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                                            () => MapAndValidate(current));
                                                    productDiscounts.Remove(current);
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

                                    var validatedValues =
                                        validationResultInfos.Where(
                                            n => n.Entity is CertainValueCertainProductDiscount && n.IsValid).Select
                                            (n => (CertainValueCertainProductDiscount) n.Entity).ToList();
                                    if (validatedValues.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedValues.Count);
                                        var values =
                                            validatedValues.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).
                                                Select(x => x.ToList()).ToList();

                                        var saveTasksArray = new Task<Guid[]>[values.Count];
                                        try
                                        {
                                            for (int i = 0; i < saveTasksArray.Length; i++)
                                            {
                                                var current = values.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    saveTasksArray[i] =
                                                        Task<Guid[]>.Factory.StartNew(() => Save(current));
                                                    values.Remove(current);
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
        private Guid[] Save(IEnumerable<CertainValueCertainProductDiscount> items)
        {
            return (from item in items
                    let res = ObjectFactory.GetInstance<ICertainValueCertainProductDiscountRepository>().Save(item, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<CertainValueCertainProductDiscountDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var entityProduct = entity.Product.ProductId;
                
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblCertainValueCertainProductDiscount.
                        FirstOrDefault(p =>p.tblCertainValueCertainProductDiscountItem.Any(n=>n.Product==entityProduct));

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasValueChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<ICertainValueCertainProductDiscountRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Certain value certain discount",
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }
        private bool HasValueChanged(CertainValueCertainProductDiscount entity)
        {

            var repo = ObjectFactory.GetInstance<ICertainValueCertainProductDiscountRepository>();
            var promo = repo.GetById(entity.Id);
            if (promo == null) return true;
            if (promo.CertainValue != entity.CertainValue||
                promo.CurrentProduct != entity.CurrentProduct||
                promo.CurrentQuantity!= entity.CurrentQuantity||
                promo.CurrentValue !=entity.CurrentValue) return true;
            return false;

        }
        private CertainValueCertainProductDiscountDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CertainValueCertainProductDiscountDTO>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                var productNameCode = SetFieldValue(row, 1);
                string initialValueString = SetFieldValue(row, 2);
                string certainValueString = SetFieldValue(row, 3);
                string effectiveDateValue = SetFieldValue(row, 4);
                string endDateValue = SetFieldValue(row, 5);
                string quantity = SetFieldValue(row, 6);

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
             
                decimal certainValue = 0;
                if (!string.IsNullOrEmpty(certainValueString))
                {
                    try
                    {
                        certainValue = Convert.ToInt32(certainValueString);
                    }
                    catch
                    {
                        certainValue = 0;
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
                int Quantity = 0;
                if (!string.IsNullOrEmpty(quantity))
                {
                    try
                    {
                        Quantity = Convert.ToInt32(quantity);
                    }
                    catch
                    {
                        Quantity = 0;
                    }
                }


                decimal InitialValue = 0;
                if (!string.IsNullOrEmpty(initialValueString))
                {
                    try
                    {
                        InitialValue = Convert.ToDecimal(initialValueString);
                    }
                    catch
                    {
                        InitialValue = 0;
                    }
                }

                return new CertainValueCertainProductDiscountDTO()
                           {
                               InitialValue = InitialValue,
                               CertainValueCertainProductDiscountItems =
                                   new List<CertainValueCertainProductDiscountItemDTO>()
                                       {
                                           new CertainValueCertainProductDiscountItemDTO()
                                               {
                                                   CertainValue = certainValue,
                                                   Quantity = Quantity,
                                                   ProductMasterId = product.id,
                                                   EffectiveDate = effectiveDate,
                                                   EndDate = endDate
                                               }
                                       }

                           };
            }));
            return items.ToArray();


        }
    }
}
