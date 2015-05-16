using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr_Middleware.WPF.Lib.Utils;
using PzIntegrations.Lib.ImportEntities;
using StructureMap;

namespace PzIntegrations.Lib.MasterDataImports.Shipping
{
    internal class ShipToAddressImportService : IShipToAddressImportService
    {
         private IList<ImportValidationResultInfo> validations;
        public ShipToAddressImportService()
        {
            validations=new List<ImportValidationResultInfo>();
        }

        public async Task<IEnumerable<ShipToAddressImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var imports = new List<ShipToAddressImport>();
                var tempFolder = Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                if(File.Exists(tempFolder))File.Delete(tempFolder);
                   File.Delete(tempFolder);
                File.Copy(path, tempFolder);
                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                {
                    parser.SetDelimiters("\t");
                    string[] currentRow = null;

                    while (!parser.EndOfData)
                    {
                        currentRow = parser.ReadFields();
                        if (currentRow != null && currentRow.Length > 0)
                        {
                            imports.Add(MapShipToAddressImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return imports;
            });
        }
        ShipToAddressImport MapShipToAddressImport(string[] dataRow)
        {
            var item = new ShipToAddressImport()
            {
                OutletCode = SetFieldValue(dataRow, 1),
                OutletName = SetFieldValue(dataRow, 2),
                ShipToCode = SetFieldValue(dataRow, 3),
                ShipToName = SetFieldValue(dataRow, 4),
                PostalAddress = SetFieldValue(dataRow, 5),
            };
            return item;
        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }

        public IList<ImportValidationResultInfo> ValidateAndSave(List<ShipToAddressImport> entities)
        {
            int batchSize = Convert.ToInt32(0.2 * entities.Count);
            var addressImports = entities.OrderBy(p => p.OutletCode).Batch(batchSize).Select(x => x.ToList()).ToList();

            #region Contruct outlets
            var taskArray = new Task<bool>[addressImports.Count];
            var results = new List<bool>();
            try
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    var current = addressImports.FirstOrDefault();
                    if (current != null && current.Any())
                    {
                        taskArray[i] = Task<bool>.Factory.StartNew(() => ConstructValidateAndUpdate(current));
                        addressImports.Remove(current);
                    }
                }

                results.AddRange(taskArray.Select(n => n.Result).ToList());
            }
            catch (AggregateException ex)
            {
            }
            catch(Exception exception)
            {
            }

            #endregion

            return validations;
        }

        private bool ConstructValidateAndUpdate(IEnumerable<ShipToAddressImport> entities)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                foreach (var entity in entities)
                {
                    var outlet = context.GetInstance<CokeDataContext>().tblCostCentre.FirstOrDefault(p => p
                         .Cost_Centre_Code.Trim().ToLower() ==
                                entity.OutletCode.Trim().ToLower()
                                && p.CostCentreType == (int)CostCentreType.Outlet);
                   
                   
                   if (outlet == null)
                    {
                        var result = new List<ValidationResult>
                                         {
                                             new ValidationResult(
                                                 string.Format("outlet code=>{0} not found",
                                                               entity.OutletCode))
                                         };

                        validations.Add(new ImportValidationResultInfo()
                                            {
                                                Description = entity.OutletName,
                                                Entity = new ShipToAddress(Guid.NewGuid()),
                                                Results = result
                                            });
                    }
                    else
                    {

                        var address = context.GetInstance<CokeDataContext>().tblShipToAddress.FirstOrDefault(
                            n =>
                            n.CostCentreId == outlet.Id && n.Code != null &&
                            n.Code.ToLower() == entity.ShipToCode.ToLower() && n.Name != null &&
                            (n.Name.ToLower() == entity.ShipToName.ToLower()) &&
                            (n.PostalAddress != null &&
                             (n.PostalAddress.ToLower() == entity.PostalAddress.ToLower())));
                        var dt = DateTime.Now;
                        bool isNew = false;
                        if (address == null)
                        {
                            address = new tblShipToAddress
                                          {
                                              IM_DateCreated = dt,
                                              IM_Status = (int) EntityStatus.Active
                                              ,
                                              Id = Guid.NewGuid()
                                          };
                            context.GetInstance<CokeDataContext>().tblShipToAddress.AddObject(address);
                            isNew = true;
                        }

                        if (isNew || HasChanged(entity, address, outlet.Id))
                        {
                            address.CostCentreId = outlet.Id;
                            address.Name = entity.ShipToName ?? outlet.Name;
                            address.PhysicalAddress = entity.PostalAddress;
                            address.PostalAddress = entity.PostalAddress;
                            address.Description = entity.ShipToCode ?? string.Empty;
                            address.Code = entity.ShipToCode ?? string.Empty;
                            address.Latitude = 0;
                            address.Longitude = 0;
                            address.IM_DateLastUpdated = dt;
                            try
                            {
                                context.GetInstance<CokeDataContext>().SaveChanges();
                            }
                            catch (DbEntityValidationException ex)
                            {
                                foreach (var error in ex.EntityValidationErrors)
                                {
                                    Console.WriteLine("====================");

                                    FileUtility.LogError(string.Format("Entity {0} in state {1} has validation errors:",
                                                                       error.Entry.Entity.GetType().Name,
                                                                       error.Entry.State));
                                    foreach (var ve in error.ValidationErrors)
                                    {
                                        Console.WriteLine("\tProperty: {0}, Error: {1}",
                                                          ve.PropertyName, ve.ErrorMessage);
                                    }
                                    Console.WriteLine();
                                }
                                throw;
                            }

                        }
                    }
                }
            }
            return true;

        }

        private bool HasChanged(ShipToAddressImport import, tblShipToAddress existing,Guid outletId)
        {
            var changed = existing.CostCentreId == outletId &&
                          ((existing.Code != null &&
                            existing.Code.Trim().ToLower() != import.ShipToCode.Trim().ToLower()));

            return changed;
        }
    }
}
