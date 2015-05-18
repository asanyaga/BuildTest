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

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class PricingImportService :MasterDataImportServiceBase, IPricingImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public PricingImportService()
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
                                        Enumerable.Select<IEnumerable<ImportEntity>, List<ImportEntity>>(importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize), x => Enumerable.ToList<ImportEntity>(x)).ToList();
                                    validationResultInfos.Clear();

                                    #region Construct Pricings

                                    var pricingTaskArray = new Task<ProductPricingDTO[]>[productImports.Count];
                                    var results = new List<ProductPricingDTO>();
                                    try
                                    {
                                        for (int i = 0; i < pricingTaskArray.Length; i++)
                                        {
                                            var current = productImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                pricingTaskArray[i] =
                                                    Task<ProductPricingDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
                                                productImports.Remove(current);
                                            }
                                        }

                                        foreach (var result in pricingTaskArray.Select(n => n.Result).ToList())
                                        {
                                            results.AddRange(result);
                                        }
                                    }
                                    catch (AggregateException ex)
                                    {
                                    }

                                    #endregion

                                    #region validate pricings
                                    var validationResults = new List<ImportValidationResultInfo>();
                                    if (validationResultInfos.Any())
                                    {
                                        validationResults.AddRange(validationResultInfos);
                                        validationResultInfos.Clear();
                                    }
                                    if (results.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*results.Count);
                                        var productPricings =
                                            results.OrderBy(p => p.SellingPrice).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();
                                        var validationTaskArray =
                                            new Task<ImportValidationResultInfo[]>[productPricings.Count];

                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = productPricings.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                                            () => MapAndValidate(current));
                                                    productPricings.Remove(current);
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

                                    #region Save pricings

                                    var validatedPricings =
                                        validationResults.Where(n => n.Entity is ProductPricing && n.IsValid).Select
                                            (n => (ProductPricing) n.Entity).ToList();
                                    if (validatedPricings.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedPricings.Count);
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
                                                        Task<Guid[]>.Factory.StartNew(() => SaveProductPricings(current));
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

                                    response.ValidationResults = validationResults;

                                    return response;
                                });
        }

        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<ProductPricingDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblPricing.
                        FirstOrDefault(
                            p =>
                            p.ProductRef != null && p.ProductRef == dto.ProductMasterId &&
                            p.Tier == dto.ProductPricingTierMasterId);

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasProductPricingChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<IProductPricingRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description ="Pricing item",
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }

        private bool HasProductPricingChanged(ProductPricing pricing)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var item = context.GetInstance<IProductPricingRepository>().GetById(pricing.Id);
                if (item == null) return true;

                var currentPrice = Math.Round((decimal) item.CurrentSellingPrice, 2);
                var currentfactory = Math.Round((decimal) item.CurrentExFactory, 2);

                var pPrice = Math.Round((decimal) pricing.CurrentSellingPrice, 2);
                var pExF = Math.Round((decimal) pricing.CurrentExFactory, 2);
                if ((currentPrice != pPrice) || (currentfactory != pExF))
                    return true;

                return false;
            }
        }

        private Guid[] SaveProductPricings(IEnumerable<ProductPricing> pricings)
        {
            return (from pricing in pricings
                    let res = ObjectFactory.GetInstance<IProductPricingRepository>().Save(pricing, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
       
        private ProductPricingDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductPricingDTO>();
            var vri = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                var productCodeName = SetFieldValue(row, 1);
                var pricingTierNameCode = SetFieldValue(row, 2);
                var sellingP = SetFieldValue(row, 3);
                var startDateValue = SetFieldValue(row, 4);
                var exfactory = SetFieldValue(row, 5);
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
                    vri.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                 
                var tier = GetPricingTier(pricingTierNameCode);
                if (tier == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "Pricing Tier with code {0} or Name={1} not found",
                                                  productCodeName,productCodeName))
                                      };
                    vri.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }

                decimal sPrice = GetDecimal(sellingP);
                decimal xPrice = GetDecimal(exfactory);

                if (xPrice != 0m)
                    UpdateExfactory(product.id, xPrice);
                DateTime startDate = GetDatetime(startDateValue);

                return new ProductPricingDTO()
                           {
                               SellingPrice = sPrice, //for ordering purpose only
                               EffectiveDate = startDate,//for ordering purpose only
                               ProductMasterId = product.id,
                               ProductPricingTierMasterId = tier.id,
                               ProductPricingItems = new List<ProductPricingItemDTO>()
                                                         {
                                                             new ProductPricingItemDTO()
                                                                 {
                                                                     ExFactoryRate =xPrice==0m?product.ExFactoryPrice:xPrice,
                                                                     SellingPrice = sPrice,
                                                                     EffectiveDate = startDate
                                                                 }
                                                         }

                           };
            }));
            //In the class scope:
            Object lockMe = new Object();

            //In the function
            lock (lockMe)
            {
                validationResultInfos.AddRange(vri);
            }
            return items.Where(p=>p !=null).ToArray();

        }
        private void UpdateExfactory(Guid productId,decimal exf)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                var prod = ctx.tblProduct.FirstOrDefault(p => p.id == productId);
                if(prod !=null && prod.ExFactoryPrice==0m)
                {
                    try
                    {
                       ctx.tblProduct.FirstOrDefault(p => p.id == productId).ExFactoryPrice=exf;
                        ctx.SaveChanges();
                       
                    }
                    catch
                    {
                        
                    }
                }
            }

        }
       

       
    }
}
