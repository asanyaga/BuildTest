using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using StructureMap;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class CommodityOwnerImportService : MasterDataImportServiceBase, ICommodityOwnerImportService
    {
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
                var commodityOwnerImports =
                    importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                        x => x.ToList()).ToList();
                validationResultInfos.Clear();

                #region Construct Items

                var taskArray = new Task<CommodityOwnerDTO[]>[commodityOwnerImports.Count];
                var results = new List<CommodityOwnerDTO>();
                try
                {
                    for (int i = 0; i < taskArray.Length; i++)
                    {
                        var current = commodityOwnerImports.FirstOrDefault();
                        if (current != null && current.Any())
                        {
                            taskArray[i] =
                                Task<CommodityOwnerDTO[]>.Factory.StartNew(() => ConstructDtOs(current));
                            commodityOwnerImports.Remove(current);
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
                    var commodityowners =
                        results.OrderBy(p => p.FirstName).Distinct().Batch(batchSize).Select(x => x.ToList()).
                            ToList();
                    var validationTaskArray = new Task<ImportValidationResultInfo[]>[commodityowners.Count];


                    try
                    {
                        for (int i = 0; i < validationTaskArray.Length; i++)
                        {
                            var current = commodityowners.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                validationTaskArray[i] =
                                    Task<ImportValidationResultInfo[]>.Factory.StartNew(
                                        () => MapAndValidate(current));
                                commodityowners.Remove(current);
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
                    validationResults.Where(n => n.IsValid).Select(n => (CommodityOwner)n.Entity).
                        ToList();
                if (validatedCommodities.Any())
                {
                    batchSize = Convert.ToInt32(0.2 * validatedCommodities.Count);
                    var commodityowners =
                        validatedCommodities.OrderBy(p => p.FirstName).Batch(batchSize).Select(
                            x => x.ToList()).ToList();

                    var saveTasksArray = new Task[commodityowners.Count];
                    try
                    {
                        for (int i = 0; i < saveTasksArray.Length; i++)
                        {
                            var current = commodityowners.FirstOrDefault();
                            if (current != null && current.Any())
                            {
                                saveTasksArray[i] =
                                    Task.Factory.StartNew(() => SaveItems(current));
                                commodityowners.Remove(current);
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
                    result.EntityItem = "CommodityOwner";
                }
                return response;
            });
        }

        private ImportValidationResultInfo[] MapAndValidate(IEnumerable<CommodityOwnerDTO> dtos)
        {
            var result = new List<ImportValidationResultInfo>();
            int index = 0;

            Parallel.ForEach(dtos, dto =>
            {
                var entity = ObjectFactory.GetInstance<IDTOToEntityMapping>().Map(dto);
                var exist = ObjectFactory.GetInstance
                    <CokeDataContext>().tblCommodityOwner.FirstOrDefault(
                        p =>
                        p.IdNo.ToLower() == dto.IdNo.ToLower());

                entity.Id = exist == null ? Guid.NewGuid() : exist.Id;
               var res =
                    ObjectFactory.GetInstance
                        <ICommodityOwnerRepository>().Validate(entity);
                var vResult = new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Description =
                        string.Format("Row-{0} name or code=>{1}", index,
                                      entity.FirstName ?? entity.Code),
                    Entity = entity
                };
                result.Add(vResult);
            });
            return result.ToArray();
        }

        private void SaveItems(IEnumerable<CommodityOwner> commodityowners)
        {
            Parallel.ForEach(commodityowners,
                             owner =>
                                 {
                                     owner._SetStatus(EntityStatus.Active);
                                     ObjectFactory.GetInstance<ICommodityOwnerRepository>().Save(owner, true);
                                 });


        }

        private CommodityOwnerDTO[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<CommodityOwnerDTO>();
            var vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                string firstName = SetFieldValue(row, 1);
                string idNo = SetFieldValue(row, 2);
                string pinNo = SetFieldValue(row, 3);
                string gnder = SetFieldValue(row, 4);
                Gender gender;
                if (!Enum.TryParse(gnder, true, out gender))
                    gender = Gender.Unknown;

                string commodityOwnerTypeName = SetFieldValue(row, 5);
                string commoditySupplierName = SetFieldValue(row, 6);

                DateTime dob = GetDatetime(SetFieldValue(row, 7));
                if (dob.ToShortDateString() == DateTime.Now.ToShortDateString())
                    dob = DateTime.Today.AddYears(-50);

                string fone = SetFieldValue(row, 8);
                if (string.IsNullOrEmpty(fone))
                    fone = "0722000000";


              
                if (string.IsNullOrEmpty(firstName))
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity Owner Name cannot be empty"))
                                  };
                    vris.Add(new ImportValidationResultInfo
                                                  ()
                    {
                        Results = res
                    });
                    return null;
                }
                if (string.IsNullOrEmpty(idNo))
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity Owner ID No. cannot be empty"))
                                  };
                    vris.Add(new ImportValidationResultInfo
                                                  ()
                    {
                        Results = res
                    });
                    return null;
                }
                if (string.IsNullOrEmpty(pinNo))
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity Owner PIN No. cannot be empty"))
                                  };
                    vris.Add(new ImportValidationResultInfo
                                                  ()
                    {
                        Results = res
                    });
                    return null;
                }


                if (string.IsNullOrEmpty(commodityOwnerTypeName))
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity Owner type cannot be empty"))
                                  };
                    validationResultInfos.Add(new ImportValidationResultInfo
                                                  ()
                    {
                        Results = res
                    });
                    return null;
                }
                if (string.IsNullOrEmpty(commoditySupplierName))
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity supplier name cannot be empty"))
                                  };
                    vris.Add(new ImportValidationResultInfo
                                                  ()
                    {
                        Results = res
                    });
                    return null;
                }


                var commodityOwnerType = GetCommodityOwnerType(commodityOwnerTypeName);
                var commoditySupplier = GetCommoditySupplier(commoditySupplierName);

                if (commoditySupplier==null)
                {
                    var res = new List<ValidationResult>
                                  {
                                      new ValidationResult(
                                          string.Format("Commodity supplier {0} is not found..import supplier first",commoditySupplierName))
                                  };
                    validationResultInfos.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
                var office = SetFieldValue(row, 16);
              

                return new CommodityOwnerDTO
                           {
                               FirstName = firstName,
                               IdNo = idNo,
                               PinNo = pinNo,
                               GenderId = (int)gender,
                               CommodityOwnerTypeId = commodityOwnerType.Id,
                               CommoditySupplierId = commoditySupplier.Id,
                               DateOfBirth = dob,
                               PhoneNumber = fone,

                               Code = SetFieldValue(row, 9),
                               Surname = SetFieldValue(row, 10),
                               LastName = SetFieldValue(row, 11),
                               Description = SetFieldValue(row, 12),
                               PhysicalAddress = SetFieldValue(row, 13),
                               PostalAddress = SetFieldValue(row, 14),
                               Email = SetFieldValue(row, 15),
                               BusinessNumber = string.IsNullOrEmpty(office) ? fone : office,
                               FaxNumber =string.IsNullOrEmpty(SetFieldValue(row, 17)) ? fone :  SetFieldValue(row, 17),
                               OfficeNumber = string.IsNullOrEmpty(SetFieldValue(row, 18)) ? fone : SetFieldValue(row, 18),
                           };
            }));
            Object lockMe = new Object();
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
            return items.Where(p=>p !=null).ToArray();

        }

        private tblCostCentre GetCommoditySupplier(string name)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblCostCentre supplier = null;
                if (!string.IsNullOrEmpty(name))
                {
                    supplier = ctx
                        .tblCostCentre.FirstOrDefault(
                            p => p.CostCentreType == (int) CostCentreType.CommoditySupplier &&
                                 p.Name.ToLower() == name.ToLower() ||
                                 p.Cost_Centre_Code != null &&
                                 p.Cost_Centre_Code.ToLower() == name.ToLower());
                }
                return supplier;
            }
        }

        private tblCommodityOwnerType GetCommodityOwnerType(string name)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblCommodityOwnerType ownerType = null;
                if (!string.IsNullOrEmpty(name))
                {
                    ownerType = ctx
                        .tblCommodityOwnerType.FirstOrDefault(
                            p =>
                            p.Name.ToLower() == name.ToLower() ||
                            p.Code != null &&
                            p.Code.ToLower() == name.ToLower());
                }

                if (ownerType == null)
                {
                    ownerType = ctx.tblCommodityOwnerType.FirstOrDefault(p => p.Name.ToLower() == "default");
                    if (ownerType == null)
                    {
                        ownerType = new tblCommodityOwnerType()
                        {
                            Id = Guid.NewGuid(),
                            Name = string.IsNullOrEmpty(name) ? "default" : name,
                            Description = string.IsNullOrEmpty(name) ? "default" : name,
                            Code = string.IsNullOrEmpty(name) ? "default" : name,
                            IM_DateCreated = DateTime.Now,
                            IM_Status = (int)EntityStatus.Active,
                            IM_DateLastUpdated = DateTime.Now
                        };
                        ctx.tblCommodityOwnerType.AddObject(ownerType);
                        ctx.SaveChanges();
                    }
                }
                return ownerType;
            }
        }
    }
}
