using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
   public class ShiptoAddressesImportService : MasterDataImportServiceBase, IShiptoAddressesImportService
    {
       private List<ImportValidationResultInfo> validationResultInfos;

       public ShiptoAddressesImportService()
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
                                        importEntities.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(
                                            x => x.ToList()).ToList();
                                  validationResultInfos.Clear();

                                    #region Construct Items

                                    var taskArray = new Task<tblShipToAddress[]>[outletImports.Count];
                                    var results = new List<tblShipToAddress>();
                                    try
                                    {
                                        for (int i = 0; i < taskArray.Length; i++)
                                        {
                                            var current = outletImports.FirstOrDefault();
                                            if (current != null && current.Any())
                                            {
                                                taskArray[i] =
                                                    Task<tblShipToAddress[]>.Factory.StartNew(
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
                                    var validationResults = new List<ImportValidationResultInfo>();
                                    if (validationResultInfos.Any())
                                    {
                                        validationResults.AddRange(validationResultInfos);
                                        validationResultInfos.Clear();
                                    }

                                    #endregion

                                    #region Save valid items

                                    var validatedAddresses = results;
                                    if (validatedAddresses.Any())
                                    {
                                        batchSize = Convert.ToInt32(0.2*validatedAddresses.Count);
                                        var outlets =
                                            validatedAddresses.OrderBy(p => p.Name).Batch(batchSize).Select(
                                                x => x.ToList()).ToList();

                                        var saveTasksArray = new Task<int>[outlets.Count];
                                        try
                                        {
                                            for (int i = 0; i < saveTasksArray.Length; i++)
                                            {
                                                var current = outlets.FirstOrDefault();
                                                if (current != null && current.Any())
                                                {
                                                    saveTasksArray[i] =
                                                        Task<int>.Factory.StartNew(() => MapAndInsert(current));
                                                    outlets.Remove(current);
                                                }
                                            }
                                            var savedResults = saveTasksArray.Select(n => n.Result).ToList().ToList();
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

                                    return response;
                                });

        }
        private int MapAndInsert (List<tblShipToAddress> dtos)
        {
            int count = 0;
            foreach (var dto in dtos)
            {
                if (CanInsert(dto))
                {
                    Insert(dto);
                    count++;
                }
                
            }
            return count;
        }

       private tblShipToAddress[] ConstructDtOs(IEnumerable<ImportEntity> entities)
        {
            var items = new List<tblShipToAddress>();
            var vris = new List<ImportValidationResultInfo>();
            items.AddRange(entities.Select(n => n.Fields).Select(row =>
            {
                var outletCode = SetFieldValue(row, 1);
                var shiptoName = SetFieldValue(row, 2);
                var shiptoCode = SetFieldValue(row, 3);
                var physicalAddress = SetFieldValue(row, 4);
                var postalAddress = SetFieldValue(row, 5);
                var lat = SetFieldValue(row, 6);
                var longt = SetFieldValue(row, 7);
                if (string.IsNullOrEmpty(outletCode) || string.IsNullOrEmpty(outletCode))
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Outlet name or code is null"))
                                      };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }

                var outlet = GetOutlet(outletCode);
                if (outlet==null)
                {
                    var res = new List<ValidationResult>
                                      {
                                          new ValidationResult(
                                              string.Format("Outlet name or code is null"))
                                      };
                    vris.Add(new ImportValidationResultInfo()
                    {
                        Results = res
                    });
                    return null;
                }
              return new tblShipToAddress()
                           {
                               Id = Guid.NewGuid(),
                               Latitude = GetDecimal(lat),
                               Longitude = GetDecimal(longt),
                               CostCentreId = outlet.Id,
                               Name = shiptoName,
                               Code = shiptoCode,
                               Description = shiptoName,
                               PhysicalAddress = physicalAddress,
                               PostalAddress = postalAddress,
                               IM_DateLastUpdated = DateTime.Now,
                               IM_DateCreated = DateTime.Now,
                               IM_Status = (int) EntityStatus.Active

                           };
               
            }));
            Object lockMe = new Object();
            lock (lockMe)
            {
                validationResultInfos.AddRange(vris);
            }
            return items.Where(p => p != null).ToArray();

        }

        private tblCostCentre GetOutlet(string outletCode)
        {
            if (string.IsNullOrEmpty(outletCode)) return null;
            using (var ctx=new CokeDataContext(Con))
            {
                return
                    ctx.tblCostCentre.FirstOrDefault(
                        p => (
                                 p.Cost_Centre_Code != null && p.Cost_Centre_Code.ToLower() == outletCode.ToLower() ||
                                 p.Name.ToLower() == outletCode.ToLower()) &&
                             p.CostCentreType == (int) CostCentreType.Outlet);

            }
        }

        private void Insert(tblShipToAddress dto)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                ctx.tblShipToAddress.AddObject(dto);
                ctx.SaveChanges();
            }
        }

       private bool CanInsert(tblShipToAddress dto)
       {
           using (var ctx = new CokeDataContext(Con))
           {
               return
                   ctx.tblShipToAddress.Any(
                       p => (
                                p.CostCentreId == dto.CostCentreId &&
                                (p.Code != null && p.Code.ToLower() == dto.Code.ToLower())
                                || (p.Name != null && p.Name.ToLower() == dto.Name)) &&
                            ((p.PhysicalAddress != null &&
                              p.PhysicalAddress.Equals(dto.PhysicalAddress, StringComparison.InvariantCultureIgnoreCase)) ||
                             (p.PostalAddress != null &&
                              p.PostalAddress.Equals(dto.PostalAddress, StringComparison.CurrentCultureIgnoreCase))));

           }
       }
    }
}
