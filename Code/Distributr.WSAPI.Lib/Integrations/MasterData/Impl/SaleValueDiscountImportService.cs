using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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
    public class SaleValueDiscountImportService : MasterDataImportServiceBase, ISaleValueDiscountImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public SaleValueDiscountImportService()
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
                var salevaluediscountImports =
                    importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                        x => x.ToList()).ToList();
                validationResultInfos.Clear();

                #region Construct 

                var taskArray = new Task<SaleValueDiscountDTO[]>[salevaluediscountImports.Count];
                var results = new List<SaleValueDiscountDTO>();
                try
                {
                    for (int i = 0; i < taskArray.Length; i++)
                    {
                        var current = salevaluediscountImports.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            taskArray[i] =
                                Task<SaleValueDiscountDTO[]>.Factory.StartNew(
                                    () => ConstructDtOs(current));
                            salevaluediscountImports.Remove(current);
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
                    batchSize = Convert.ToInt32(0.2 * results.Count);
                    var salevalueDiscounts =
                        results.OrderBy(p => p.TierMasterId).Batch(batchSize).Select(
                            x => x.ToList()).ToList();
                    var validationTaskArray =
                        new Task<ImportValidationResultInfo[]>[salevalueDiscounts.Count];

                    try
                    {
                        for (int i = 0; i < validationTaskArray.Length; i++)
                        {
                            var current = salevalueDiscounts.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                validationTaskArray[i] =
                                    Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                        () => MapAndValidate(current));
                                salevalueDiscounts.Remove(current);
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

                var validatedPricings =
                    validationResultInfos.Where(n => n.Entity is SaleValueDiscount && n.IsValid).Select
                        (n => (SaleValueDiscount)n.Entity).ToList();
                if (validatedPricings.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * validatedPricings.Count);
                    var pricings =
                        validatedPricings.OrderBy(p => p.CurrentEffectiveDate).Batch(batchSize).
                            Select(x => x.ToList()).ToList();

                    var saveTasksArray = new Task<Guid[]>[pricings.Count];
                    try
                    {
                        for (int i = 0; i < saveTasksArray.Length; i++)
                        {
                            var current = pricings.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                saveTasksArray[i] =
                                    Task<Guid[]>.Factory.StartNew(() => SaveSaleValueDiscounts(current));
                                pricings.Remove(current);
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

        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<SaleValueDiscountDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblSaleValueDiscount.
                        FirstOrDefault(
                            p =>
                            p.TierId ==dto.TierMasterId);

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasSaleValueChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<ISaleValueDiscountRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description = "Pricing item",
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }

        private Guid[] SaveSaleValueDiscounts(IEnumerable<SaleValueDiscount> sDiscounts)
        {
            return (from item in sDiscounts
                    let res = ObjectFactory.GetInstance<ISaleValueDiscountRepository>().Save(item, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }

        private bool HasSaleValueChanged(SaleValueDiscount saleValueDiscount)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var item = context.GetInstance<ISaleValueDiscountRepository>().GetById(saleValueDiscount.Id);
                if (item == null) return true;

                var currentRate = Math.Round(item.CurrentRate, 2);
                var currentfactory = Math.Round(item.CurrentSaleValue, 2);

                var pRate = Math.Round(saleValueDiscount.CurrentRate, 2);
                var psalevalue = Math.Round(saleValueDiscount.CurrentSaleValue, 2);
                return (currentRate != pRate) || (currentfactory != psalevalue);
            }
        }

        private SaleValueDiscountDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<SaleValueDiscountDTO>();

            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
               var pricingTierNameCode = SetFieldValue(row, 1);
               string saleValue = SetFieldValue(row, 2);
               string effectiveDateValue = SetFieldValue(row, 3);
               string endDateValue = SetFieldValue(row, 4);
               string discountRValue = SetFieldValue(row, 5);

                var tier = GetPricingTier(pricingTierNameCode);

                if (tier == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "Tier  with code {0} or Name={1} not found",
                                                  pricingTierNameCode,pricingTierNameCode))
                                      };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                
               
             
                decimal svalue = 0m;
                if (!string.IsNullOrEmpty(saleValue))
                {
                    try
                    {
                        svalue = Convert.ToDecimal(saleValue);
                    }
                    catch
                    {
                        svalue = 0m;
                    }
                }
                if (svalue == 0)
                    return null;
                
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
                
                decimal discountRate = 0m;
                if (!string.IsNullOrEmpty(discountRValue))
                {
                    try
                    {
                        discountRate = Convert.ToDecimal(discountRValue);
                    }
                    catch
                    {
                        discountRate = 0m;
                    }
                }

                return new SaleValueDiscountDTO()
                           {
                               TierMasterId = tier.id,
                               DiscountItems = new List<SaleValueDiscountItemDTO>()
                                                   {
                                                       new SaleValueDiscountItemDTO()
                                                           {
                                                               EffectiveDate = effectiveDate,
                                                               EndDate = endDate,
                                                               DiscountValue = discountRate,
                                                               DiscountThreshold = svalue

                                                           }
                                                   }

                           };
            }));
            return items.ToArray();

        }

        private tblPricingTier GetPricingTier(string itemName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblPricingTier pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx.tblPricingTier.FirstOrDefault(p => p.Name.ToLower() == itemName.ToLower() || p.Code != null && p.Code.ToLower() == itemName.ToLower());
                return pitem;
            }
        }
    }
}
