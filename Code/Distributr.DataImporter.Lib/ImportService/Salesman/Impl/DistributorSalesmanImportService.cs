using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportService.Salesman.Impl
{
  public  class DistributorSalesmanImportService:IDistributorSalesmanImportService
  {
      private readonly ICostCentreRepository _costCentreRepository;
      private readonly ICostCentreFactory  _costCentreFactory;
      private readonly IUserRepository _userRepository;
      private readonly RepositoryHelpers _repositoryHelpers;
      private CokeDataContext _ctx;
      Distributor distributor;

      public DistributorSalesmanImportService(ICostCentreRepository costCentreRepository, ICostCentreFactory costCentreFactory, IUserRepository userRepository, CokeDataContext ctx)
      {
          _costCentreRepository = costCentreRepository;
          _costCentreFactory = costCentreFactory;
          _userRepository = userRepository;
          _ctx = ctx;
          _repositoryHelpers=new RepositoryHelpers(ctx);
      }

      public IEnumerable<DistributorSalesmanImport> Import(string path)
      {
          try
          {

             var inputFileDescription = new CsvFileDescription
              {
                  // cool - I can specify my own separator!
                  SeparatorChar = ',',
                  FirstLineHasColumnNames = false,
                  QuoteAllFields = true,
                  EnforceCsvColumnAttribute = true
              };

              CsvContext cc = new CsvContext();
              var importEntities = cc.Read<DistributorSalesmanImport>(path, inputFileDescription);
              return importEntities;
          }
          catch (FileNotFoundException ex)
          {
              throw ex;
          }
          catch (FieldAccessException ex)
          {
              throw ex;
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message, "Importer Error", MessageBoxButton.OK, MessageBoxImage.Error);
              return null;
          }
      }
      
      public IList<ImportValidationResultInfo> Validate(List<DistributorSalesmanImport> entities)
      {
          IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
          int count = 1;
          foreach (var item in ConstructDomainEntities(entities))
          {
              var res = _costCentreRepository.Validate(item);

              var importValidationResult = new ImportValidationResultInfo()
              {
                  Results = res.Results,
                  Description = "Row-" + count,
                  Entity = item,
                  EntityNameOrCode=item.CostCentreCode??item.Name
              };
              results.Add(importValidationResult);

              count++;

          }
          return results;
      }

     
      public void Save(List<User> entities)
      {
          foreach (var item in entities)
          {
              _userRepository.Save(item);
          }
      }

     
      public IList<ImportValidationResultInfo> ValidateUsers(List<DistributorSalesmanImport> entities)
      {
          IList<ImportValidationResultInfo> results = new List<ImportValidationResultInfo>();
          int count = 1;
          foreach (var user in ConstructSalesmanUsers(entities))
          {
              var res = _userRepository.Validate(user);

              var importValidationResult = new ImportValidationResultInfo()
              {
                  Results = res.Results,
                  Description = "Row-" + count,
                  Entity = user
              };
              results.Add(importValidationResult);

              count++;

          }
          return results;
      }

    
      public void Save(List<DistributorSalesman> entities)
      {
          foreach(var item in entities)
          {
            _costCentreRepository.Save(item);
                     
          }
         
      }
      
      private IEnumerable<User> ConstructSalesmanUsers(IEnumerable<DistributorSalesmanImport> salesmen)
      {
          var exisitingDistributorSalesmenUsers = _userRepository.GetByUserType(UserType.DistributorSalesman).ToList();
          var newSalesmenUsers = new List<User>();
          const string fclDefaultMobileNo = "0722205698";
         
          foreach (var importentity in salesmen)
          {
             var domainEntity = exisitingDistributorSalesmenUsers.FirstOrDefault(p => p.Username.Equals(importentity.Name,StringComparison.CurrentCultureIgnoreCase)) ??
                                 new User(Guid.NewGuid());
              var centre = _costCentreRepository.GetByCode(importentity.SalesmanCode, CostCentreType.DistributorSalesman,
                                                           true);
              if (centre != null)
                  domainEntity.CostCentre =centre.Id;
              domainEntity.Username = importentity.Name;
              domainEntity.UserType=UserType.DistributorSalesman;
              domainEntity.Mobile = fclDefaultMobileNo;
              domainEntity.Password = EncryptorMD5.GetMd5Hash("12345678");
              domainEntity.Code = importentity.SalesmanCode;
              newSalesmenUsers.Add(domainEntity);

          }
          return newSalesmenUsers.ToList(); 
      }

      private IEnumerable<DistributorSalesman> ConstructDomainEntities(IEnumerable<DistributorSalesmanImport> entities)
      {
          
          var newSalesmen = new List<DistributorSalesman>();

          var defaultDistributr =
              _costCentreRepository.GetByCode("87878", CostCentreType.Distributor) as Distributor;
                               
          foreach (var importentity in entities)
          {
              var domainEntity = _costCentreRepository.GetByCode(importentity.SalesmanCode,CostCentreType.DistributorSalesman, true) as DistributorSalesman;
              distributor = defaultDistributr;
              if(!string.IsNullOrEmpty(importentity.DistributorCode))
                  distributor = _costCentreRepository.GetByCode(importentity.DistributorCode, CostCentreType.Distributor, true) as Distributor ?? defaultDistributr;
              if (distributor == null) throw new ArgumentNullException("distributor");
              if (domainEntity == null)
              {
                  domainEntity = _costCentreFactory.CreateCostCentre(Guid.NewGuid(),
                                                                     CostCentreType.DistributorSalesman, distributor) as
                                 DistributorSalesman;
              }
              domainEntity.Name = importentity.Name;
              domainEntity.CostCentreCode = importentity.SalesmanCode;
              domainEntity.CostCentreType = CostCentreType.DistributorSalesman;
              newSalesmen.Add(domainEntity);
          }
          return newSalesmen.ToList();
      }

      public Task<IList<ImportValidationResultInfo>> ValidateAsync(List<DistributorSalesmanImport> entities)
      {
          throw new NotImplementedException();
      }

     
  }
}
