using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.Utils;
using LINQtoCSV;

namespace Integration.Cussons.WPF.Lib.ImportService.DistributrSalesmen
{
    internal class DistributorSalesmanImportService : IDistributorSalesmanImportService
    {
        private readonly ICostCentreRepository _costCentreRepository;
        private readonly ICostCentreFactory _costCentreFactory;
        private readonly IUserRepository _userRepository;
        private  List<DistributorSalesman> _exisitingDistributorSalesmen;
        private List<DistributorSalesman> _newSalesmen;
        private List<string> newCodes; 

        public DistributorSalesmanImportService(ICostCentreRepository costCentreRepository, ICostCentreFactory costCentreFactory, IUserRepository userRepository, List<DistributorSalesman> exisitingDistributorSalesmen)
        {
            _costCentreRepository = costCentreRepository;
            _costCentreFactory = costCentreFactory;
            _userRepository = userRepository;
            this._exisitingDistributorSalesmen = exisitingDistributorSalesmen;
            this._newSalesmen=new List<DistributorSalesman>();
            newCodes=new List<string>();
        }

        public async Task<IEnumerable<DistributorSalesmanImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var imports = new List<DistributorSalesmanImport>();
                var tempFolder =Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
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
                            imports.Add(MapSalesmanImport(currentRow));
                        }
                    }

                }
                File.Delete(tempFolder);
                return imports;
            });
        }

        DistributorSalesmanImport MapSalesmanImport(string[] dataRow)
        {
            var item = new DistributorSalesmanImport()
                           {
                               Code = SetFieldValue(dataRow,1),
                               Name = SetFieldValue(dataRow, 2),
                               Description = SetFieldValue(dataRow, 3),
                           };
            return item;
        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length > index && string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }
        public async Task<IEnumerable<DistributorSalesmanImport>> Import(string path,bool overload=true)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEnumerable<DistributorSalesmanImport> imports;
                try
                {

                    var inputFileDescription = new CsvFileDescription
                    {
                        // cool - I can specify my own separator!
                        SeparatorChar = '\t',
                        //tab delimited
                        FirstLineHasColumnNames =
                            false,
                        QuoteAllFields = true,
                        EnforceCsvColumnAttribute
                            =
                            true
                    };
                    var cc = new CsvContext();

                    imports = cc.Read<DistributorSalesmanImport>(path,
                                                           inputFileDescription);

                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show("File not found on specified path:\n" + path);
                    return null;
                }
                catch (FieldAccessException ex)
                {
                    MessageBox.Show(
                        "File cannot be accessed,is it in use by another application?",
                        "Importer Error", MessageBoxButton.OK,
                        MessageBoxImage.Stop);
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unknown Error:Details\n" + ex.Message,
                                    "Importer Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return imports;
            });
        }

        public async Task<IList<ImportValidationResultInfo>> ValidateAsync(List<DistributorSalesmanImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var salesmen = await ConstructDomainEntities(entities);
                int count = 0;
                foreach (var user in salesmen)
                {
                    var res = await ValidateEntityAsync(user);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description = string.Format("Row-{0}=>{1}", count, user.CostCentreCode),
                        Entity = user
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
        }

        public async Task<bool> SaveAsync(IEnumerable<DistributorSalesman> salesmen)
        {
            return await Task.Run(() =>
            {
                foreach (var salesman in salesmen)
                {
                    _costCentreRepository.Save(salesman);
                }
                return true;

            });
        }

        public async Task<bool> SaveAsync(List<User> entities)
        {
            return await Task.Run(() =>
                                      {
                                          foreach (var user in entities)
                                          {
                                              _userRepository.Save(user);
                                          }
                                          return true;

                                      });
        }

        public async Task<IList<ImportValidationResultInfo>> ValidateUsers(List<DistributorSalesmanImport> entities)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ImportValidationResultInfo>();
                var users = await ConstructSalesmanUsers(entities);
                int count = 0;
                foreach (var user in users)
                {
                    var res = await ValidateEntityAsync(user);
                    var importValidationResult = new ImportValidationResultInfo()
                    {
                        Results = res.Results,
                        Description =string.Format("Row-{0}=>{1}",count,user.Code),
                        Entity = user
                    };
                    results.Add(importValidationResult);
                    count++;
                }
                return results;
            });
        }

        private async Task<ImportValidationResultInfo> ValidateEntityAsync(User user)
        {
            return await Task.Run(() =>
            {
                var res = _userRepository.Validate(user);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Entity = user,
                    Description = user.Code
                };
            });
        }
        private async Task<ImportValidationResultInfo> ValidateEntityAsync(DistributorSalesman user)
        {
            return await Task.Run(() =>
            {
                var res = _costCentreRepository.Validate(user);
                return new ImportValidationResultInfo()
                {
                    Results = res.Results,
                    Entity = user,
                    Description = user.CostCentreCode

                };
            });
        }

       
        private async Task<IEnumerable<DistributorSalesman>> ConstructDomainEntities(IEnumerable<DistributorSalesmanImport> entities)
        {
            return await Task.Run(() =>
                                      {
                                          
                                          _exisitingDistributorSalesmen=
                                              _costCentreRepository.GetAll(true).OfType<DistributorSalesman>().ToList();
                                          var defaultDistributr =
                                              _costCentreRepository.GetAll().OfType<Distributor>().FirstOrDefault(
                                                  p =>
                                                  p.CostCentreCode.Equals("PZ Cussons EA",
                                                                          StringComparison.CurrentCultureIgnoreCase));
                                         
                                          foreach (var entity in entities)
                                          {
                                              var name = string.Concat(entity.Name.Trim(),"_",entity.Code.Trim());
                                              var domainEntity=  _exisitingDistributorSalesmen.FirstOrDefault(
                                                     p => p.Name != null && p.Name.Trim()==name);
                                             
                                              if (defaultDistributr == null)
                                                  throw new ArgumentNullException("defaultDistributr");
                                              if (domainEntity == null)
                                              {
                                                  domainEntity = _costCentreFactory.CreateCostCentre(Guid.NewGuid(),
                                                                                                     CostCentreType.
                                                                                                         DistributorSalesman,
                                                                                                     defaultDistributr)
                                                                 as
                                                                 DistributorSalesman;
                                                  if (domainEntity != null)
                                                  {
                                                      domainEntity.CostCentreCode = GenerateSalesmanCode(entity.Name);
                                                      newCodes.Add(domainEntity.CostCentreCode);
                                                  }
                                                  

                                                  
                                              }
                                             
                                              if (domainEntity != null)
                                              {
                                                  domainEntity.Name = name;
                                                  domainEntity.CostCentreType = CostCentreType.DistributorSalesman;
                                                  domainEntity.ParentCostCentre = new CostCentreRef() { Id = defaultDistributr.Id };
                                                  _newSalesmen.Add(domainEntity);
                                              }
                                             
                                          }
                                          return _newSalesmen.ToList();
                                      });
        }

        private async Task<IEnumerable<User>> ConstructSalesmanUsers(IEnumerable<DistributorSalesmanImport> salesmen)
        {
            return await Task.Run(() =>
                                      {
                                          var newSalesmenUsers = new List<User>();
                                          var defaultDistributr =
                                              _costCentreRepository.GetAll().OfType<Distributor>().FirstOrDefault(p => p.CostCentreCode == "PZ Cussons EA");
                                         
                                          foreach (var importentity in salesmen)
                                          {
                                              var costcentreName = string.Concat(importentity.Name.Trim(), "_", importentity.Code.Trim());
                                              var domainEntity =
                                                  _userRepository.GetAll(true).FirstOrDefault(
                                                      p => p.Username != null &&
                                                           p.Username.Trim()==importentity.Name.Trim()) ??

                                                  new User(Guid.NewGuid());

                                              
                                              if (defaultDistributr == null)
                                                  throw new ArgumentNullException("distributr");
                                              string defaultMobileNo = "";
                                              var contact  = defaultDistributr.Contact.FirstOrDefault(p => p.ContactOwnerType == ContactOwnerType.Distributor);
                                              defaultMobileNo = contact != null ? contact.MobilePhone : "0700000000";

                                              var centre =
                                                  _exisitingDistributorSalesmen.FirstOrDefault(
                                                      p => p.Name.Trim() == costcentreName.Trim());
                                              if (centre != null)
                                                  domainEntity.CostCentre = centre.Id;
                                              domainEntity.Username = importentity.Name.Trim();
                                              domainEntity.UserType = UserType.DistributorSalesman;
                                              domainEntity.Mobile = defaultMobileNo;
                                              domainEntity.Password = EncryptorMD5.GetMd5Hash("12345678");
                                              domainEntity.Mobile = defaultMobileNo;
                                              domainEntity._Status=EntityStatus.New;
                                              domainEntity.FirstName = importentity.Name.Trim();
                                              domainEntity.LastName = importentity.Name.Trim();
                                              domainEntity.Code = importentity.Code.Trim();
                                              newSalesmenUsers.Add(domainEntity);

                                          }
                                          return newSalesmenUsers.ToList();
                                      });

           
        }

        private string GenerateSalesmanCode(string name = "", bool incrementCounter = false)
        {
            string code;
            if (incrementCounter)
            {
                var count= _costCentreRepository.GetAll(true).OfType<DistributorSalesman>().Count();
                code = count<99 ? (count + 1).ToString("00") : GenerateSalesmanCode(name);
            }
            else
            {
                var existingcodes = _exisitingDistributorSalesmen.Select(p => p.CostCentreCode.Trim()).ToList();
                existingcodes.AddRange(newCodes);
                int attempt = 0;
                code = StringUtils.GenerateRandomString(2, 2, new Random(Environment.TickCount));
                while (existingcodes.Any(p => p.Contains(code)))
                {
                    var random = new Random(Environment.TickCount);
                    code = StringUtils.GenerateRandomString(2, 2, random);
                    attempt++;
                    if (attempt > 100000000 && existingcodes.Contains(code))
                    {
                        MessageBox.Show(
                            string.Format(
                                "There is problem generating code for salesman {0} we suggest you add manually", name));
                        break;
                    }
                }

                
            }
            return code;
        }
    }
}
