using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class OutletImportService : MasterDataImportServiceBase, IOutletImportService
    {
        private List<ImportValidationResultInfo> validationResultInfos;
        public OutletImportService()
        {
            validationResultInfos = new List<ImportValidationResultInfo>();
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
                                    var outletImports =
                                        Enumerable.Select<IEnumerable<ImportEntity>, List<ImportEntity>>(importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize), x => Enumerable.ToList<ImportEntity>(x)).ToList();
                                   
                                    #region Construct Items

                                    var taskArray = new Task<OutletDTO[]>[outletImports.Count];
                                    var results = new List<OutletDTO>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = outletImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<OutletDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
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
                                        batchSize = Convert.ToInt32(0.2*results.Count);
                                        var outlets =
                                            results.OrderBy(p => p.CostCentreCode).Distinct().Batch(batchSize).Select(
                                                x => x.ToList()).
                                                ToList();
                                        var validationTaskArray = new Task<ImportValidationResultInfo[]>[outlets.Count];


                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = outlets.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                                            () => MapAndValidate(current));
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
                                            foreach (var error in ex.InnerExceptions)
                                            {
                                                response.ErrorInfo += error;
                                            }
                                        }

                                    }

                                    #endregion

                                    #region Save valid items

                                    var validatedOutlets =
                                        validationResults.Where(n => n.IsValid).Select(n => (Outlet) n.Entity).
                                            ToList();
                                    if (validatedOutlets.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedOutlets.Count);
                                        var outlets =
                                            validatedOutlets.OrderBy(p => p.CostCentreCode).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();

                                        var saveTasksArray = new Task<Guid[]>[outlets.Count];
                                        try
                                        {
                                            for (int i = 0; i < saveTasksArray.Length; i++)
                                            {
                                                var current = outlets.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    saveTasksArray[i] =
                                                        Task<Guid[]>.Factory.StartNew(() => SaveItems(current));
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
                                        result.EntityItem = "Outlet";
                                    }
                                  
                                    return response;
                                });


        }
        
        private Guid[] SaveItems(IEnumerable<Outlet> current)
        {
            return (from outlet in current
                    let res = ObjectFactory.GetInstance<ICostCentreRepository>().Save(outlet, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
        }
        private ImportValidationResultInfo[] MapAndValidate(List<OutletDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =
                    ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblCostCentre.
                        FirstOrDefault(p =>
                                       p.Cost_Centre_Code != null &&
                                       p.Cost_Centre_Code.ToLower().Trim() == dto.CostCentreCode.ToLower().Trim() &&
                                       p.CostCentreType == (int) CostCentreType.Outlet);

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;
                if (OutletHasChanged(entity))
                {
                    var res =
                    ObjectFactory.Container.GetNestedContainer()
                        .GetInstance<ICostCentreRepository>().Validate(entity);
                    var vResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description =
                            string.Format("Row-{0} Description or code=>{1}", index,
                                          entity.Name ?? entity.CostCentreCode),
                        Entity = entity
                    };
                    result.Add(vResult);
                }
                
            }
            return result.ToArray();
        }
        private bool OutletHasChanged(Outlet item)
        {
            var outlet = ObjectFactory.GetInstance<ICostCentreRepository>().GetById(item.Id) as Outlet;

            if (outlet == null) return true;
            if (outlet.CostCentreCode.Trim() != item.CostCentreCode.Trim())
                return true;
            if (outlet.Name.Trim().ToLower() != item.Name.Trim().ToLower())
                return true;
            if (outlet.Route!=null && outlet.Route.Id != item.Route.Id)
                return true;
            var oldVatId = outlet.VatClass != null ? outlet.VatClass.Id : Guid.Empty;
            var newVatId = item.VatClass != null ? item.VatClass.Id : Guid.Empty;
            if (oldVatId != newVatId)
                return true;
            if (outlet.OutletProductPricingTier != null && item.OutletProductPricingTier!=null && outlet.OutletProductPricingTier.Id != item.OutletProductPricingTier.Id)
                return true;

            return false;
        }
        private OutletDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<OutletDTO>();
            var vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                var outletCode = SetFieldValue(row, 1);
                var name = SetFieldValue(row, 2);
                var distributrCode = SetFieldValue(row, 3);
                var routeCode = SetFieldValue(row, 4);
                var outletCategory = SetFieldValue(row, 5);
                var outletype = SetFieldValue(row, 6);
                var discountGroupCode = SetFieldValue(row, 7);
                var productPricingTierCode = SetFieldValue(row, 8);
                var vatClassCode = SetFieldValue(row, 9);
                var specialPricingTierCode = SetFieldValue(row, 10);
                var lat = SetFieldValue(row, 11);
                var longt = SetFieldValue(row, 12);
                if (string.IsNullOrEmpty(outletCode) || string.IsNullOrEmpty(name))
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Outlet name or code is null"))
                                      };
                    var vri = new ImportValidationResultInfo()
                                  {
                                      Results = res
                                  };
                    vris.Add(vri);
                    return null;
                }
                if (string.IsNullOrEmpty(distributrCode))
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Distributr Code is required for salesman {0}",distributrCode))
                                      };
                   vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }

                if (string.IsNullOrEmpty(routeCode))
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "Route code is empty or null"))
                                      };
                    
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;

                }

                var distributr = GetDistributr(distributrCode);
                if (distributr == null)
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format(
                                              "Distributor with code={0} not found",
                                              distributrCode))
                                  };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }

                var route = GetRoute(routeCode);
                if (route == null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format(
                                                  "Route with code={0} not found",routeCode))
                                      };
                    vris.Add(new ImportValidationResultInfo()
                                 {
                                     Results = res
                                 });
                    return null;
                }
                var category = GetOutletCategory(outletCategory);
                var type = GetOutletType(outletype);
                var vatclass = GetVATClass(vatClassCode);
                var pricingtier = GetPricingTier(productPricingTierCode);
                var specialTier = GetPricingTier(specialPricingTierCode);
                var discountGroup = GetDiscountGroup(discountGroupCode);
                return new OutletDTO()
                {
                   CostCentreCode = outletCode,
                   DiscountGroupMasterId = discountGroup !=null?discountGroup.id:default(Guid),
                   Name = name,
                   ParentCostCentreId = distributr.Id,
                   RouteMasterId = route.RouteID,
                   OutletCategoryMasterId = category.id,
                   OutletProductPricingTierMasterId = pricingtier != null ? pricingtier.id : default(Guid),
                   OutletTypeMasterId = type.id,
                   VatClassMasterId =vatclass!=null? vatclass.id:Guid.Empty,
                   SpecialPricingTierMasterId = specialTier != null ? specialTier.id : default(Guid),
                   Latitude = lat,
                   Longitude = longt,
                   CostCentreTypeId = (int)CostCentreType.Outlet,
                   IsApproved = true,
                   StatusId = (int)EntityStatus.Active

                };
            }));
            //In the class scope:
            Object lockMe = new Object();

            //In the function
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
           
          
            return items.Where(p=>p !=null).ToArray();

        }
        private tblPricingTier GetPricingTier(string tierCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblPricingTier tier= null;
                if (!string.IsNullOrEmpty(tierCode))
                    tier = ctx
                        .tblPricingTier.FirstOrDefault(
                            p => p.Code != null &&
                            p.Code.ToLower() == tierCode.ToLower() || p.Name != null && p.Name.ToLower() == tierCode.ToLower());
                return tier;
            }
        }
        private tblVATClass GetVATClass(string vatNameCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblVATClass vatClass = null;
                if (!string.IsNullOrEmpty(vatNameCode))
                {
                    vatClass = ctx
                        .tblVATClass.FirstOrDefault(
                            p => p.Class != null &&
                                 p.Class.ToLower() == vatNameCode.ToLower() ||
                                 p.Name != null && p.Name.ToLower() == vatNameCode.ToLower());
                }
                //if (vatClass == null)
                //    vatClass = ctx.tblVATClass.FirstOrDefault(p => p.Class == "default");

                if (vatClass == null)
                {
                    return null;
                    var item = new tblVATClass()
                                   {
                                       Class = "default",
                                       Name = "default",
                                       id = Guid.NewGuid(),
                                       IM_DateCreated = DateTime.Now,

                                       IM_DateLastUpdated = DateTime.Now,
                                       IM_Status = (int) EntityStatus.Active,
                                       tblVATClassItem = new EntityCollection<tblVATClassItem>()
                                                             {
                                                                 new tblVATClassItem()
                                                                     {
                                                                         id = Guid.NewGuid(),
                                                                         Rate = 0m,
                                                                         EffectiveDate = DateTime.Now,
                                                                         IM_DateCreated = DateTime.Now,
                                                                         IM_DateLastUpdated = DateTime.Now,
                                                                         IM_Status = (int) EntityStatus.Active
                                                                     }
                                                             }
                                   };

                    ctx.tblVATClass.AddObject(item);
                    ctx.SaveChanges();
                }
                return vatClass;
            }
        }
        private tblRoutes GetRoute(string routeCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblRoutes route = null;
                if (!string.IsNullOrEmpty(routeCode))
                    route = ctx
                        .tblRoutes.FirstOrDefault(
                            p => p.Code != null &&
                            p.Code.ToLower() == routeCode.ToLower() || p.Name != null && p.Name.ToLower() == routeCode.ToLower());
                return route;
            }
        }
        private tblDiscountGroup GetDiscountGroup(string discountGroupCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblDiscountGroup discountGroup = null;
                if (!string.IsNullOrEmpty(discountGroupCode))
                {
                    discountGroup = ctx
                        .tblDiscountGroup.FirstOrDefault(
                            p => p.Code != null &&
                                 p.Code.ToLower() == discountGroupCode.ToLower() ||
                                 p.Name != null && p.Name.ToLower() == discountGroupCode.ToLower());
                }
               
                return discountGroup;
            }
        }
        private tblOutletCategory GetOutletCategory(string outletcategorycode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblOutletCategory outletcategory = null;
                if (!string.IsNullOrEmpty(outletcategorycode))
                {
                    outletcategory = ctx
                        .tblOutletCategory.FirstOrDefault(
                            p => p.Code != null &&
                                 p.Code.ToLower() == outletcategorycode.ToLower() ||
                                 p.Name != null && p.Name.ToLower() == outletcategorycode.ToLower());
                }
                if (outletcategory == null)
                    outletcategory = ctx.tblOutletCategory.FirstOrDefault(p => p.Code == "default");

                if (outletcategory == null)
                {
                    var item = new tblOutletCategory()
                    {
                        Code = "default",
                        Name = "default",
                        id = Guid.NewGuid(),
                        Description = "default",
                        IM_DateCreated = DateTime.Now,
                        IM_DateLastUpdated = DateTime.Now,
                        IM_Status = (int)EntityStatus.Active

                    };
                    ctx.tblOutletCategory.AddObject(item);
                    ctx.SaveChanges();
                }
                return outletcategory;
            }
        }
        private tblOutletType GetOutletType(string outlettypeCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblOutletType outletType = null;
                if (!string.IsNullOrEmpty(outlettypeCode))
                {
                    outletType = ctx
                        .tblOutletType.FirstOrDefault(
                            p => p.Code != null &&
                                 p.Code.ToLower() == outlettypeCode.ToLower() ||
                                 p.Name != null && p.Name.ToLower() == outlettypeCode.ToLower());
                }
                if(outletType==null)
                outletType = ctx.tblOutletType.FirstOrDefault(p => p.Code == "default");

                if(outletType==null)
                {
                    var item = new tblOutletType()
                                   {
                                       Code = "default",
                                       Name = "default",
                                       id = Guid.NewGuid(),
                                       IM_DateCreated = DateTime.Now,
                                       IM_DateLastUpdated = DateTime.Now,
                                       IM_Status = (int)EntityStatus.Active
                                   };
                    ctx.tblOutletType.AddObject(item);
                    ctx.SaveChanges();
                }
                return outletType;
            }
        }
       
    }
}
