using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using PzIntegrations.Lib.ImportEntities;
using StructureMap;

namespace PzIntegrations.Lib.MasterDataImports.Salesmen
{
    public class SalesmanImportService : ISalesmanImportService
    {
        private List<DistributorSalesman> _existingSalesmen;
        private List<string> newCodes;
        private List<DistributorSalesman> _newSalesmen;
        public SalesmanImportService()
        {
            _existingSalesmen=new List<DistributorSalesman>();
            newCodes=new List<string>();
            _newSalesmen=new List<DistributorSalesman>();
        }

        public async Task<IEnumerable<DistributorSalesmanImport>> Import(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var imports = new List<DistributorSalesmanImport>();
                var tempFolder = Path.Combine(FileUtility.GetApplicationTempFolder(), Path.GetFileName(path));
                if (File.Exists(tempFolder))
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
            return new DistributorSalesmanImport()
                       {
                           Code = SetFieldValue(dataRow, 1),
                           Name = SetFieldValue(dataRow, 2),
                           Description = SetFieldValue(dataRow, 3),
                       };

        }

        string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length - 1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }

        public  IList<ImportValidationResultInfo> ValidateAndSave(List<DistributorSalesmanImport> entities)
        {
          return  Task.Factory.StartNew(() =>
                                      {
                                          var result = new List<ImportValidationResultInfo>();
                                          //1.create  and validate costcentre
                                          var costcentrevalidations = ValidateSalesmen(ConstructSalesmen(entities));

                                          var validcostcentre =
                                              costcentrevalidations.Where(p => p.IsValid).Select(
                                                  p => (DistributorSalesman) p.Entity).ToList();

                                          if (validcostcentre.Any())
                                          {
                                              try
                                              {
                                                  SaveSalemen(validcostcentre);
                                              }
                                              catch (Exception ex)
                                              {
                                              }

                                              //2. create and validate users
                                              var userValidations = ValidateUsers(ConstructUsers(entities));
                                              var validUsers =
                                                  userValidations.Where(p => p.IsValid).Select(p => (User) p.Entity);
                                              if (validUsers.Any())
                                              {
                                                  try
                                                  {
                                                      SaveUsers(validUsers);
                                                  }
                                                  catch (Exception ex)
                                                  {
                                                  }

                                              }
                                              result.AddRange(userValidations.Where(p => !p.IsValid));
                                          }
                                          result.AddRange(costcentrevalidations.Where(p => !p.IsValid).ToList());
                                          return result;
                                      }).Result;
        }

        private Guid[] SaveSalemen(IEnumerable<DistributorSalesman> salesmen)
        {
            return (from salesman in salesmen
                    let res = ObjectFactory.Container.GetNestedContainer().GetInstance<ICostCentreRepository>().Save(
                        salesman, true)
                    select new Guid(res.ToString())
                               {

                               }).ToArray();
        }

        private Guid[] SaveUsers(IEnumerable<User> salesmen)
        {
            return (from salesman in salesmen
                    let res = ObjectFactory.Container.GetNestedContainer().GetInstance<IUserRepository>().Save(
                        salesman, true)
                    select new Guid(res.ToString())
                               {

                               }).ToArray();
        }

        private ImportValidationResultInfo[] ValidateSalesmen(IEnumerable<DistributorSalesman> salesmen)
        {
            return (from salesman in salesmen
                    let res =
                        ObjectFactory.Container.GetNestedContainer().GetInstance<ICostCentreRepository>().Validate(
                            salesman)
                    select new ImportValidationResultInfo()
                               {
                                   Results = res.Results,
                                   Entity = salesman
                               }).ToArray();
        }

        private ImportValidationResultInfo[] ValidateUsers(IEnumerable<User> salesmen)
        {
            return (from salesman in salesmen
                    let res =
                        ObjectFactory.Container.GetNestedContainer().GetInstance<IUserRepository>().Validate(
                            salesman)
                    select new ImportValidationResultInfo()
                               {
                                   Results = res.Results,
                                   Entity = salesman
                               }).ToArray();
        }


        private DistributorSalesman[] ConstructSalesmen(IEnumerable<DistributorSalesmanImport> entities)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                
                var defaultDistributr = context.GetInstance<ICostCentreRepository>()
                     .GetByCode("PZ Cussons EA", CostCentreType.Distributor);
                if(defaultDistributr !=null)
                {
                    _existingSalesmen =
                        context.GetInstance<IDistributorSalesmanRepository>().GetByDistributor(defaultDistributr.Id);


                }
                else
                {
                    _existingSalesmen = context.GetInstance<IDistributorSalesmanRepository>()
                        .GetAll(true).OfType<DistributorSalesman>().ToList();
                }

                foreach (var entity in entities)
                {
                    var name = string.Concat(entity.Name.Trim(), "_", entity.Code.Trim());
                    var domainEntity = _existingSalesmen.FirstOrDefault(
                        p => p.Name != null && p.Name.Trim() == name);

                    if (defaultDistributr == null)
                        throw new ArgumentNullException("defaultDistributr");
                    bool isNew = false;
                    if (domainEntity == null)
                    {
                        domainEntity =context.GetInstance<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(),
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
                        isNew = true;

                    }

                    if (domainEntity != null)
                    {
                        domainEntity.Name = name;
                        domainEntity.CostCentreType = CostCentreType.DistributorSalesman;
                        domainEntity.ParentCostCentre = new CostCentreRef() {Id = defaultDistributr.Id};
                        if(isNew || HasChanged(domainEntity))
                        _newSalesmen.Add(domainEntity);
                    }

                }
            }
            return _newSalesmen.ToArray();
        }

        private bool HasChanged(DistributorSalesman import)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var salesman = context.GetInstance<ICostCentreRepository>().GetByCode(import.CostCentreCode, CostCentreType.DistributorSalesman, true) as DistributorSalesman;

                return salesman == null ||
                       salesman.CostCentreCode.Trim().ToLower() == import.CostCentreCode.Trim().ToLower()
                       && (salesman.Name.Trim().ToLower() !=
                           import.Name.Trim().ToLower());
            }


        }

        private string GenerateSalesmanCode(string name = "", bool incrementCounter = false)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                string code;
                if (incrementCounter)
                {
                    var count =
                        context.GetInstance<IDistributorSalesmanRepository>().GetAll(true).OfType<DistributorSalesman>()
                            .Count();
                    code = count < 99 ? (count + 1).ToString("00") : GenerateSalesmanCode(name);
                }
                else
                {
                    var existingcodes = _existingSalesmen.Select(p => p.CostCentreCode.Trim()).ToList();
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
                                    "There is problem generating code for salesman {0} we suggest you add manually",
                                    name));
                            break;
                        }
                    }



                }
                return code;
            }
        }

        private User[] ConstructUsers(IEnumerable<DistributorSalesmanImport> salesmen)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {

                var defaultDistributr = context.GetInstance<ICostCentreRepository>()
                    .GetByCode("PZ Cussons EA", CostCentreType.Distributor);
                var newSalesmenUsers = new List<User>();

                foreach (var importentity in salesmen)
                {
                    var costcentreName = string.Concat(importentity.Name.Trim(), "_", importentity.Code.Trim());
                    var domainEntity = context.GetInstance<IUserRepository>()
                        .GetAll(true).FirstOrDefault(
                            p => p.Username != null &&
                                 p.Username.Trim() == importentity.Name.Trim());
                    bool isNew = false;
                    if (domainEntity == null)
                    {
                        domainEntity = new User(Guid.NewGuid());
                        isNew = true;
                    }

                    if (defaultDistributr == null)
                        throw new ArgumentNullException("distributr");
                    string defaultMobileNo = "";
                    var contact =
                        defaultDistributr.Contact.FirstOrDefault(p => p.ContactOwnerType == ContactOwnerType.Distributor);
                    defaultMobileNo = contact != null ? contact.MobilePhone : "0700000000";

                    var centre =
                        _existingSalesmen.FirstOrDefault(
                            p => p.Name.Trim() == costcentreName.Trim());
                    if (centre != null)
                        domainEntity.CostCentre = centre.Id;
                    domainEntity.Username = importentity.Name.Trim();
                    domainEntity.UserType = UserType.DistributorSalesman;
                    domainEntity.Mobile = defaultMobileNo;
                    domainEntity.Password = EncryptorMD5.GetMd5Hash("12345678");
                    domainEntity.Mobile = defaultMobileNo;
                    domainEntity._Status = EntityStatus.Active;
                    domainEntity.FirstName = importentity.Name.Trim();
                    domainEntity.LastName = importentity.Name.Trim();
                    domainEntity.Code = importentity.Code.Trim();

                    if (isNew || HasSalesmanChanged(domainEntity))
                        newSalesmenUsers.Add(domainEntity);

                }
                return newSalesmenUsers.ToArray();
            }
           
        }

        private bool HasSalesmanChanged(User domainEntity)
        {
            using (var context = ObjectFactory.Container.GetNestedContainer())
            {
                var user = context.GetInstance<IUserRepository>().GetByCode(domainEntity.Code,true);

                return user == null ||
                       (user.CostCentre != domainEntity.CostCentre ||
                        user.Username.Trim().ToLower() != domainEntity.Username.Trim().ToLower());
            }
        }
    }
}
