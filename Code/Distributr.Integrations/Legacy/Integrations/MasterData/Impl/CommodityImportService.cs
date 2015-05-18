using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
{
    public class CommodityImportService : MasterDataImportServiceBase, ICommodityImportService
    {
        public CommodityImportService()
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
                var commodityImports =
                    Enumerable.Select<IEnumerable<ImportEntity>, List<ImportEntity>>(importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize), x => Enumerable.ToList<ImportEntity>(x)).ToList();
                validationResultInfos.Clear();

                #region Construct Items

                var taskArray = new Task<CommodityDTO[]>[commodityImports.Count];
                var results = new List<CommodityDTO>();
                try
                {
                    for (int i = 0; i < taskArray.Length; i++)
                    {
                        var current = commodityImports.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            taskArray[i] =
                                Task<CommodityDTO[]>.Factory.StartNew(() => ConstructDtOs(current));
                            commodityImports.Remove(current);
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
                    var commodities =
                        results.OrderBy(p => p.Name).Distinct().Batch(batchSize).Select(x => x.ToList()).
                            ToList();
                    var validationTaskArray = new Task<ImportValidationResultInfo[]>[commodities.Count];


                    try
                    {
                        for (int i = 0; i < validationTaskArray.Length; i++)
                        {
                            var current = commodities.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                validationTaskArray[i] =
                                    Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                        () => MapAndValidate(current));
                                commodities.Remove(current);
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

                var validatedCommodities =
                    validationResults.Where(n => n.IsValid).Select(n => (Commodity)n.Entity).
                        ToList();
                if (validatedCommodities.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * validatedCommodities.Count);
                    var commodities =
                        validatedCommodities.OrderBy(p => p.Name).Batch(batchSize).Select(
                            x => x.ToList()).ToList();

                    var saveTasksArray = new Task[commodities.Count];
                    try
                    {
                        for (int i = 0; i < saveTasksArray.Length; i++)
                        {
                            var current = commodities.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                saveTasksArray[i] =
                                    Task.Factory.StartNew(() => SaveItems(current));
                                commodities.Remove(current);
                            }
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
                    result.EntityItem = "Commodity";
                }
                return response;
            });
        }

        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<CommodityDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;

            Parallel.ForEach(dtos, dto =>
                                       {
                                           var entity = ObjectFactory.GetInstance<IDTOToEntityMapping>().Map(dto);
                                           var exist = ObjectFactory.GetInstance
                                               <CokeDataContext>().tblCommodityType.FirstOrDefault(
                                                   p =>
                                                   p.Name.ToLower() == dto.Name.ToLower() ||
                                                   p.Code != null &&
                                                   p.Code.Equals(dto.Code, StringComparison.CurrentCultureIgnoreCase));

                                           entity.Id = exist == null ? Guid.NewGuid() : exist.Id;
                                           entity._SetStatus(EntityStatus.Active);
                                           var res =
                                               ObjectFactory.GetInstance
                                                   <ICommodityRepository>().Validate(entity);
                                           var vResult = new ImportValidationResultInfo()
                                           {
                                               Results = res.Results,
                                               Description =
                                                   string.Format("Row-{0} name or code=>{1}", index,
                                                                 entity.Name ?? entity.Code),
                                               Entity = entity
                                           };
                                           result.Add(vResult);
                                       });
            return result.ToArray();
        }

        private void SaveItems(IEnumerable<Commodity> commodities)
        {
            Parallel.ForEach(commodities,
                             commodity =>
                                 {
                                     commodity._SetStatus(EntityStatus.Active);
                                     ObjectFactory.GetInstance<ICommodityRepository>().Save(commodity, true);
                                 });


        }
        private CommodityDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CommodityDTO>();
          var  vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
                                                                     {
                                                                         string pCode = SetFieldValue(row, 1);
                                                                         string pName = SetFieldValue(row, 2);
                                                                         string desc = SetFieldValue(row, 3);

                                                                         if (string.IsNullOrEmpty(pName))
                                                                         {
                                                                             var res = new List<ValidationResult>
                                                                                           {
                                                                                               new ValidationResult(
                                                                                                   string.Format("Commodity Name cannot be empty"))
                                                                                           };
                                                                             vris.Add(new ImportValidationResultInfo
                                                                                                           ()
                                                                                                           {
                                                                                                               Results =
                                                                                                                   res
                                                                                                           });
                                                                             return null;
                                                                         }
                                                                         var commodityTypeName = SetFieldValue(row, 4);


                                                                         var commodityType =GetCommodityType(commodityTypeName);
                                                                         return new CommodityDTO()
                                                                                    {
                                                                                        Code = pCode,
                                                                                        Description = desc,
                                                                                        Name = pName,
                                                                                        CommodityTypeId =
                                                                                            commodityType.Id
                                                                                    };
                                                                     }));
           Object lockMe = new Object();
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
            return items.ToArray();

        }

        private tblCommodityType GetCommodityType(string name)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblCommodityType productType = null;
                if (!string.IsNullOrEmpty(name))
                {
                    productType = ctx
                        .tblCommodityType.FirstOrDefault(
                            p =>
                            p.Name.ToLower() == name.ToLower() ||
                            p.Code != null &&
                            p.Code.ToLower() == name.ToLower());
                }

                if (productType == null)
                {
                    productType = ctx.tblCommodityType.FirstOrDefault(p => p.Name.ToLower() == "default");
                    if (productType == null)
                    {
                        productType = new tblCommodityType()
                                          {
                                              Id = Guid.NewGuid(),
                                              Name = string.IsNullOrEmpty(name) ? "default" : name,
                                              Description = string.IsNullOrEmpty(name) ? "default" : name,
                                              Code = string.IsNullOrEmpty(name) ? "default" : name,
                                              IM_DateCreated = DateTime.Now,
                                              IM_Status = (int) EntityStatus.Active,
                                              IM_DateLastUpdated = DateTime.Now
                                          };
                        ctx.tblCommodityType.AddObject(productType);
                        ctx.SaveChanges();
                    }
                }
                return productType;
            }
        }

    }
}
