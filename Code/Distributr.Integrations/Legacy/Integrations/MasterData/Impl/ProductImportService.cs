using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class SaleProductImportService : MasterDataImportServiceBase, IProductImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public SaleProductImportService()
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
                var productImports =
                    Enumerable.Select<IEnumerable<ImportEntity>, List<ImportEntity>>(importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize), x => Enumerable.ToList<ImportEntity>(x)).ToList();
                validationResultInfos.Clear();

                #region Construct Items

                var taskArray = new Task<SaleProductDTO[]>[productImports.Count];
                var results = new List<SaleProductDTO>();
                try
                {
                    for (int i = 0; i < taskArray.Length; i++)
                    {
                        var current = productImports.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            taskArray[i] =
                                Task<SaleProductDTO[]>.Factory.StartNew(
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
                    foreach (var error in ex.InnerExceptions)
                    {
                        response.ErrorInfo += error;
                    }

                }

                #endregion

                #region Validate Items

                var validationResults = new List<ImportValidationResultInfo>();
                if (validationResultInfos.Any())
                {
                    validationResults.AddRange(validationResultInfos);
                    validationResultInfos.Clear();
                }
                if (results.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * results.Count);
                    var products =
                        results.OrderBy(p => p.ProductCode).Distinct().Batch(batchSize).Select(x => x.ToList()).
                            ToList();
                    var validationTaskArray = new Task<ImportValidationResultInfo[]>[products.Count];


                    try
                    {
                        for (int i = 0; i < validationTaskArray.Length; i++)
                        {
                            var current = products.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                validationTaskArray[i] =
                                    Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                        () => MapAndValidate(current));
                                products.Remove(current);
                            }
                        }

                        foreach (var result in validationTaskArray.Select(n => n.Result).ToList())
                        {
                            validationResults.AddRange(result);
                        }
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var error in ex.InnerExceptions)
                        {
                            response.ErrorInfo += error;
                        }
                    }

                }

                #endregion

                #region Save valid items

                var validatedProducts =
                    validationResults.Where(n => n.IsValid).Select(n => (Product)n.Entity).
                        ToList();
                if (validatedProducts.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * validatedProducts.Count);
                    var products =
                        validatedProducts.OrderBy(p => p.ProductCode).Batch(batchSize).Select(
                            x => x.ToList()).ToList();

                    var saveTasksArray = new Task<Guid[]>[products.Count];
                    try
                    {
                        for (int i = 0; i < saveTasksArray.Length; i++)
                        {
                            var current = products.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                saveTasksArray[i] =
                                    Task<Guid[]>.Factory.StartNew(() => SaveItems(current));
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
                        foreach (var error in ex.InnerExceptions)
                        {
                            response.ErrorInfo += error;
                        }
                    }
                }



                #endregion

                response.ValidationResults.AddRange(validationResults);
                foreach (var result in response.ValidationResults)
                {
                    result.Entity = null;
                    result.EntityItem = "Product";
                }
                return response;
            });
        }


        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<SaleProductDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblProduct.
                        FirstOrDefault(p =>
                                       p.ProductCode != null && p.ProductCode.ToLower() == dto.ProductCode.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;
                if (!HasProductChanged(entity)) continue;
                var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<IProductRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                                  {
                                      Results = res.Results,
                                      Description =
                                          string.Format("Row-{0} Description or code=>{1}", index,
                                                        entity.Description ?? entity.ProductCode),
                                      Entity = entity
                                  };
                result.Add(vResult);
            }
            return result.ToArray();
        }

        private Guid[] SaveItems(IEnumerable<Product> products)
        {
            return (from product in products
                    
                    let res = ObjectFactory.GetInstance<IProductRepository>().Save(product, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();

        }

        private bool HasProductChanged(SaleProduct item)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var product = context.GetInstance<IProductRepository>().GetByCode(item.ProductCode, true);

                if (product == null) return true;
                var desc = product.Description.Trim().ToLower();
                var itemdesc = item.Description.Trim().ToLower();
                var productprice = Math.Round((decimal) product.ExFactoryPrice, 2);
                var itemprice = Math.Round((decimal) product.ExFactoryPrice, 2);
                var itemVATClassId= item.VATClass!=null?item.VATClass.Id.ToString():"";
                var productVATClassId = product.VATClass != null ? product.VATClass.Id.ToString() : "";

                return (product.Id != item.Id) || (product.Brand.Id != item.Brand.Id) || (productprice != itemprice)
                       || (desc != itemdesc) || (productVATClassId!=itemVATClassId);
            }

        }
        private SaleProductDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<SaleProductDTO>();
            var vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                string pCode = SetFieldValue(row, 1);
                string pDesc = SetFieldValue(row, 2);
                decimal exfactory = GetDecimal(SetFieldValue(row, 3));
                var productTypeName = SetFieldValue(row, 4);
                var productBrandName = SetFieldValue(row, 5);
                var flavourname = SetFieldValue(row, 6);
                var packagingTypeName = SetFieldValue(row, 7);
                var vatclassName = SetFieldValue(row, 8);
                string typeString = SetFieldValue(row, 9);
                var returnableName = SetFieldValue(row, 10);
               
                var productBrand = GetProductBrand(productBrandName);
                if (productBrand == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "product Brand with Name={0} not found",
                                                  productBrandName))
                                      };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                
                var VATClass = GetVATClass(vatclassName);
                var productType = GetProductType(productTypeName);
                var productFlavour = GetProductFlavour(flavourname, productBrand);

                
                var productPackagingType = GetPackagingType(packagingTypeName);
                
                var returnable = GetReturnableProduct(returnableName);

               
                int type = 0;
                if (!string.IsNullOrEmpty(typeString))
                {
                    try
                    {
                        type = typeString.ToLower() == "default" ? 0 : Convert.ToInt32(SetFieldValue(row, 9));

                    }catch
                    {
                        type = 0;
                    }
                }
                
                return new SaleProductDTO()
                {
                    ProductCode = pCode,
                    Description = pDesc,
                    ExFactoryPrice =exfactory,
                    ProductBrandMasterId = productBrand.id,
                    ProductTypeMasterId = productType==null?Guid.Empty:productType.id,
                    ProductFlavourMasterId = productFlavour.id,
                    VatClassMasterId = VATClass==null?Guid.Empty:VATClass.id,
                    ProductPackagingTypeMasterId = productPackagingType==null?Guid.Empty:productPackagingType.id,
                    ReturnableProductMasterId = returnable==null?Guid.Empty:returnable.id,
                    ReturnableTypeMasterId = type
                    
                };
            }));
            Object lockMe = new Object();
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
            return items.ToArray();

        }
       
        private tblProduct GetReturnableProduct(string itemName)
        {
            if (string.IsNullOrEmpty(itemName) || itemName.ToLower() == "default") return null;
            using (var ctx = new CokeDataContext(Con))
            {
                tblProduct pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx
                        .tblProduct.Where(p => p.Returnable.HasValue).FirstOrDefault(
                            p =>
                            p.Description.ToLower() == itemName.ToLower() ||
                            p.ProductCode != null &&
                            p.ProductCode.ToLower() == itemName.ToLower());
                return pitem;
            }
        }

        private tblVATClass GetVATClass(string vatclassName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblVATClass pitem = null;
                if (!string.IsNullOrEmpty(vatclassName))
                    pitem =ctx.tblVATClass.FirstOrDefault(
                    p =>
                    p.Name.ToLower() == vatclassName.ToLower() 
                   
                   );
             
                if(pitem==null)
                {
                    //prefer any before resorting to default
                  pitem=  ctx.tblVATClass.FirstOrDefault(
                    p =>p.Name !="default" && p.tblVATClassItem.Any(n => n.Rate == 0 && n.EffectiveDate <= DateTime.Now));
                }
                return pitem ?? (ctx.tblVATClass.FirstOrDefault(
                    p => p.tblVATClassItem.Any(n => n.Rate == 0 && n.EffectiveDate <= DateTime.Now)));
            }
        }

        private tblProductFlavour GetProductFlavour(string itemName,tblProductBrand brand)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblProductFlavour pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                {
                    pitem = ctx
                                .tblProductFlavour.FirstOrDefault(
                                    p =>
                                    p.name.ToLower() == itemName.ToLower() ||
                                    p.code != null &&
                                    p.code.ToLower() == itemName.ToLower()) ?? ctx.tblProductFlavour.FirstOrDefault(
                                        p => p.name.ToLower() == "default");
                }
                if(pitem==null)
                {
                    var date = DateTime.Now;
                        pitem = new tblProductFlavour()
                        {
                            BrandId = brand.id,
                            id = Guid.NewGuid(),
                            code = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                            description = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                            IM_DateCreated = date,
                            IM_DateLastUpdated = date,
                            name = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                        };
                        ctx.tblProductFlavour.AddObject(pitem);
                        ctx.SaveChanges();
                }
                return pitem;
            }
        }

        private tblProductBrand GetProductBrand(string itemName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblProductBrand pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx
                        .tblProductBrand.FirstOrDefault(
                            p =>
                            p.name.ToLower() == itemName.ToLower() ||
                            p.code != null &&
                            p.code.ToLower() == itemName.ToLower())??ctx
                        .tblProductBrand.FirstOrDefault(
                            p =>
                            p.name.ToLower()=="default");

                if(pitem==null)
                {
                    var date = DateTime.Now;
                    var supp = ctx.tblSupplier.FirstOrDefault(p => p.Code == "default");
                    if(supp==null)
                    {
                        supp=new tblSupplier()
                                 {
                                     Name = "default",
                                     Code = "default",
                                     Description = "default",
                                     IM_Status = (int)EntityStatus.Active,
                                     id = Guid.NewGuid(),
                                     IM_DateCreated = date,
                                     IM_DateLastUpdated = date
                                 };
                        ctx.tblSupplier.AddObject(supp);
                        ctx.SaveChanges();
                    }
                    pitem = new tblProductBrand()
                                {
                                    IM_Status = (int) EntityStatus.Active,
                                    id = Guid.NewGuid(),
                                    name = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                                    description = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                                    code = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                                    SupplierId = supp.id,
                                    IM_DateCreated = date,
                                    IM_DateLastUpdated = date

                                };
                    ctx.tblProductBrand.AddObject(pitem);
                    ctx.SaveChanges();
                }

                
                return pitem;
            }
        }

        private tblProductPackagingType GetPackagingType(string itemName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblProductPackagingType pitem = null;
                if (!string.IsNullOrEmpty(itemName))
                    pitem = ctx
                        .tblProductPackagingType.FirstOrDefault(
                            p =>
                            p.name.ToLower() == itemName.ToLower() ||
                            p.code != null &&
                            p.code.ToLower() == itemName.ToLower());

                if (pitem == null)
                {
                    pitem = ctx.tblProductPackagingType.FirstOrDefault(p => p.name.ToLower() == "default");
                    if (pitem == null)
                    {
                        pitem = new tblProductPackagingType()
                                    {
                                        id = Guid.NewGuid(),
                                        name = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                                        code = string.IsNullOrEmpty(itemName) ? "default" : itemName,
                                        IM_DateCreated = DateTime.Now,
                                        IM_Status = (int) EntityStatus.Active,
                                        IM_DateLastUpdated = DateTime.Now,
                                    };
                        ctx.tblProductPackagingType.AddObject(pitem);
                        ctx.SaveChanges();

                    }

                }
                return pitem;
            }
        }

        private  tblProductType GetProductType(string productTypeName)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblProductType productType = null;
                if (!string.IsNullOrEmpty(productTypeName))
                {
                    productType = ctx
                       .tblProductType.FirstOrDefault(
                           p =>
                           p.name.ToLower() == productTypeName.ToLower() ||
                           p.code != null &&
                           p.code.ToLower() == productTypeName.ToLower());
                }

                if (productType == null)
                {
                    productType = ctx.tblProductType.FirstOrDefault(p => p.name.ToLower() == "default");
                    if (productType == null)
                    {
                        productType = new tblProductType()
                                          {
                                              id = Guid.NewGuid(),
                                              name = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                                              Description =
                                                  string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                                              code = string.IsNullOrEmpty(productTypeName) ? "default" : productTypeName,
                                              IM_DateCreated = DateTime.Now,
                                              IM_Status = (int) EntityStatus.Active,
                                              IM_DateLastUpdated = DateTime.Now
                                          };
                        ctx.tblProductType.AddObject(productType);
                        ctx.SaveChanges();
                    }

                }

                return productType;
            }
        }

      
    }
}
