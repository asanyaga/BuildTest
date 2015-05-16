using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class ProductBrandImportService : MasterDataImportServiceBase, IProductBrandImportService
    {
        
        private List<ImportValidationResultInfo> validationResultInfos;

        public ProductBrandImportService()
        {
            validationResultInfos=new List<ImportValidationResultInfo>();
        }

        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            return Task.Run(() =>
                                {
                                    var response = new MasterDataImportResponse();
                                    var importEntities = imports as List<ImportEntity> ??
                                                         imports.ToList();
                                    int batchSize = Convert.ToInt32(0.2*importEntities.Count()); //batch 20% of the collection and process in parallel
                                    var brandImports =
                                        importEntities.OrderBy(p => p.MasterDataCollective).
                                            Batch(batchSize).Select(
                                                x => x.ToList()).ToList();

                                    #region Construct Items

                                    var taskArray =
                                        new Task<ProductBrandDTO[]>[brandImports.Count];
                                    var results = new List<ProductBrandDTO>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = brandImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<ProductBrandDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
                                                brandImports.Remove(current);
                                            }
                                        }

                                        foreach (
                                            var result in
                                                taskArray.Select(n => n.Result).ToList())
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
                                        var brands =
                                            results.OrderBy(p => p.Code).Batch(batchSize).Select
                                                (x => x.ToList()).
                                                ToList();
                                        var validationTaskArray =
                                            new Task<ImportValidationResultInfo[]>[brands.Count];


                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = brands.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.
                                                            Factory.StartNew(
                                                                () => MapAndValidate(current));
                                                    brands.Remove(current);
                                                }
                                            }

                                            foreach (
                                                var result in
                                                    validationTaskArray.Select(n => n.Result).
                                                        ToList())
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

                                    response.ValidationResults.AddRange(validationResults);
                                    foreach (var result in response.ValidationResults)
                                    {
                                        result.Entity = new ProductBrand(Guid.Empty);
                                    }
                                    return response;
                                });
        }

        public Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            return Task.Run(() =>
                                {
                                    var response = new MasterDataImportResponse();
                                    var importEntities = imports as List<ImportEntity> ?? imports.ToList();
                                    int batchSize = Convert.ToInt32(0.2*importEntities.Count());
                                    var brandImports =
                                        importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                                            x => x.ToList()).ToList();

                                    #region Construct Items

                                    var taskArray = new Task<ProductBrandDTO[]>[brandImports.Count];
                                    var results = new List<ProductBrandDTO>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = brandImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<ProductBrandDTO[]>.Factory.StartNew(
                                                        () => ConstructDtOs(current));
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
                                        var brands =
                                            results.OrderBy(p => p.Code).Batch(batchSize).Select(x => x.ToList()).
                                                ToList();
                                        var validationTaskArray = new Task<ImportValidationResultInfo[]>[brands.Count];


                                        try
                                        {
                                            for (int i = 0; i < validationTaskArray.Length; i++)
                                            {
                                                var current = brands.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    validationTaskArray[i] =
                                                        Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                                            () => MapAndValidate(current));
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
                                            foreach (var error in ex.InnerExceptions)
                                            {
                                                response.ErrorInfo += error;
                                            }
                                        }

                                    }

                                    #endregion

                                    #region Save valid items

                                    var validatedBrands =
                                        validationResults.Where(n => n.IsValid).Select(n => (ProductBrand) n.Entity).
                                            ToList();
                                    if (validatedBrands.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedBrands.Count);
                                        var brands =
                                            validatedBrands.OrderBy(p => p.Code).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();

                                        var saveTasksArray = new Task<Guid[]>[brands.Count];
                                        try
                                        {
                                            for (int i = 0; i < saveTasksArray.Length; i++)
                                            {
                                                var current = brands.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    saveTasksArray[i] =
                                                        Task<Guid[]>.Factory.StartNew(() => SaveItems(current));
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
                                        result.EntityItem = "ProductBrand";
                                    }
                                    return response;
                                });
        }


        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<ProductBrandDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var entity = ObjectFactory.Container.GetNestedContainer().GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist =ObjectFactory.Container.GetNestedContainer().GetInstance<CokeDataContext>().tblProductBrand.FirstOrDefault(p => p.name.ToLower() == dto.Name.ToLower()||
                    p.code !=null&& p.code.ToLower() == dto.Code.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.id;

                var res =
                    ObjectFactory.Container.GetNestedContainer()
                    .GetInstance<IProductBrandRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.Name ?? entity.Code),
                    Entity = entity
                };
                result.Add(vResult);
            }
            return result.ToArray();
        }

        private  ProductBrandDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<ProductBrandDTO>();

            items.AddRange(entities.Select(n => n.Fields).Select(row =>
                {
                    var suppliernameCode =SetFieldValue(row, 4);
                    tblSupplier supplier = HandleDefaultSupplier(suppliernameCode);
                    return new ProductBrandDTO()
                    {
                        Code = SetFieldValue(row, 1),
                        Name = SetFieldValue(row, 2),
                        Description = SetFieldValue(row, 3),
                        SupplierMasterId = supplier.id

                    };
                }));
                return items.ToArray();
            
        }
        private tblSupplier HandleDefaultSupplier(string code)
        {
           tblSupplier supp = null;
                if (!string.IsNullOrEmpty(code))
                {
                    supp =ObjectFactory.GetInstance<CokeDataContext>().tblSupplier.FirstOrDefault(
                        p => p.Code != null && p.Code.ToLower() == code.ToLower()||p.Name !=null && p.Name.ToLower()==code.ToLower());
                }

                if (supp == null)
                {
                    var date = DateTime.Now;
                    supp = ObjectFactory.GetInstance<CokeDataContext>().tblSupplier.FirstOrDefault(
                        p => p.Code != null && p.Code.ToLower() == "default");
                    if(supp==null)
                    {
                        supp = new tblSupplier()
                                   {
                                       id = Guid.NewGuid(),
                                       Code = string.IsNullOrEmpty(code) ? "default" : code,
                                       Name = string.IsNullOrEmpty(code) ? "default" : code,
                                       Description = string.IsNullOrEmpty(code) ? "default" : code,
                                       IM_DateCreated = date,
                                       IM_DateLastUpdated = date,
                                       IM_Status = (int) EntityStatus.Active,
                                   };
                    ObjectFactory.GetInstance<CokeDataContext>().tblSupplier.AddObject(supp);
                    ObjectFactory.GetInstance<CokeDataContext>().SaveChanges();
                    }
                }
                return supp;
            
        }
                                                                    
        private Guid[] SaveItems(IEnumerable<ProductBrand> brands)
        {
            return (from brand in brands
                    let res = ObjectFactory.GetInstance<IProductBrandRepository>().Save(brand, true)
                    select new Guid(res.ToString())
                    {

                    }).ToArray();
            
        }
    }
}
