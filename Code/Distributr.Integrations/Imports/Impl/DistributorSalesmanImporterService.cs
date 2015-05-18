using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class DistributorSalesmanImporterService:BaseImporterService,IDistributorSalesmanImporterService
    {
        private ICostCentreRepository _salesmanRepository;
        private IUserRepository _userRepository;
        private CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);  



        public DistributorSalesmanImporterService(ICostCentreRepository salemanRepository, CokeDataContext context, IUserRepository userRepository)
        {
            _salesmanRepository = salemanRepository;
            _context = context;
            _userRepository = userRepository;
        }

        public ImportResponse Save(List<DistributorSalesmanImport> imports)
        {
            #region Construct & Save DistributorSalesman CostCentre
            var mappingValidationList = new List<string>();
            List<DistributorSalesman> distributorSalesmen = imports.Select(s=>Map(s,mappingValidationList)).ToList();
            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }

            List<ValidationResultInfo> validationResults = distributorSalesmen.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<DistributorSalesman> changedDistributorSalesmen = HasChanged(distributorSalesmen);

            foreach (var changedDistributorSalesman in changedDistributorSalesmen)
            {
                _salesmanRepository.Save(changedDistributorSalesman);
            }
            #endregion

            #region Construct & Save DistributorSalesman Users
            var mappingUserValidationList = new List<string>();
            List<User> distributorSalesmenUsers = imports.Select(s=>MapUser(s,mappingUserValidationList)).ToList();

            if (mappingUserValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }
            List<ValidationResultInfo> userValidationResults = distributorSalesmenUsers.Select(ValidateUser).ToList();

            if (userValidationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(userValidationResults) };

            }
            List<User> changedDistributorSalesmenUsers = HasUserChanged(distributorSalesmenUsers);

            foreach (var changedDistributorSalesmanUser in changedDistributorSalesmenUsers)
            {
                _userRepository.Save(changedDistributorSalesmanUser);
            }
            #endregion


            return new ImportResponse() { Status = true, Info = changedDistributorSalesmen.Count + " Distributor Salesmen Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var distributrSalesmanId = _context.tblCostCentre.Where(p => p.Name == deletedCode && p.CostCentreType == (int)CostCentreType.DistributorSalesman).Select(p => p.Id).FirstOrDefault();

                    var distributrSalesman = _salesmanRepository.GetById(distributrSalesmanId);
                    if (distributrSalesman != null)
                    {
                        _salesmanRepository.SetAsDeleted(distributrSalesman);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("DistributrSalesman Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "DistributrSalesman Deleted Succesfully", Status = true };
        }


        private ValidationResultInfo ValidateUser(User user)
        {
            return _userRepository.Validate(user);
        }

        private User MapUser(DistributorSalesmanImport distributorSalesmanImport,List<string> mappingUserValidationList)
        {
            var exists = Queryable.FirstOrDefault(_context.tblUsers, p => p.Code == distributorSalesmanImport.Code);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var costCentre =Queryable.FirstOrDefault(_context.tblCostCentre, p => p.Cost_Centre_Code == distributorSalesmanImport.Code);
            if (costCentre == null) { mappingUserValidationList.Add(string.Format((string) "Invalid DistributorSalesman Code {0}", (object) distributorSalesmanImport.Code)); }

            var distributorSalesmanUser = new User(id);
            distributorSalesmanUser.CostCentre =costCentre!=null?costCentre.Id:Guid.Empty;
            distributorSalesmanUser.Username = distributorSalesmanImport.Name;
            distributorSalesmanUser.FirstName = distributorSalesmanImport.Name;
            distributorSalesmanUser.Code = distributorSalesmanImport.Code;
            distributorSalesmanUser.UserType = UserType.DistributorSalesman;
            distributorSalesmanUser.Password = EncryptorMD5.GetMd5Hash("12345678");
            distributorSalesmanUser.Mobile = distributorSalesmanImport.MobileNumber;

            return distributorSalesmanUser;
        }

        private List<DistributorSalesman> HasChanged(List<DistributorSalesman> distributorSalesmen)
        {
            var changedDistributorSalesmen = new List<DistributorSalesman>();
            foreach (var distributorSalesman in distributorSalesmen)
            {
                var entity = _salesmanRepository.GetById(distributorSalesman.Id);
                if (entity == null)
                {
                    changedDistributorSalesmen.Add(distributorSalesman);
                    continue;
                }
                bool hasChanged = false;
                if(entity.Name.ToLower() != distributorSalesman.Name.ToLower() || entity.CostCentreCode.ToLower() != distributorSalesman.CostCentreCode.ToLower())
                {
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    changedDistributorSalesmen.Add(distributorSalesman);
                }
            }
            return changedDistributorSalesmen;
        }


        private List<User> HasUserChanged(List<User> distributorSalesmenUsers)
        {
            var changedDistributorSalesmenUser = new List<User>();
            foreach (var distributorSalesmanUser in distributorSalesmenUsers)
            {
                var entity = _userRepository.GetById(distributorSalesmanUser.Id);
                if (entity == null)
                {
                    changedDistributorSalesmenUser.Add(distributorSalesmanUser);
                    continue;
                }
                bool hasChanged = false;

                if (entity.Username.ToLower() != distributorSalesmanUser.Username.ToLower() || entity.Code.ToLower() != distributorSalesmanUser.Code.ToLower())
                {
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    changedDistributorSalesmenUser.Add(distributorSalesmanUser);
                }
            }
            return changedDistributorSalesmenUser;
        }


        protected ValidationResultInfo Validate(DistributorSalesman distributorSalesman)
        {
            return _salesmanRepository.Validate(distributorSalesman);
        }

        protected DistributorSalesman Map(DistributorSalesmanImport distributorSalesmanImport, List<string> mappingvalidationList)
        {
            var exists = Queryable.FirstOrDefault(_context.tblCostCentre, p => p.Cost_Centre_Code == distributorSalesmanImport.Code && p.CostCentreType == (int)CostCentreType.DistributorSalesman);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var distributor =
                Queryable.FirstOrDefault(_context.tblCostCentre, p =>
                    p.Cost_Centre_Code == distributorSalesmanImport.DistributorCode &&
                    p.CostCentreType == (int) CostCentreType.Distributor) ;
            if (distributor == null) { mappingvalidationList.Add(string.Format((string) "Invalid Distributor Code {0}", (object) distributorSalesmanImport.DistributorCode)); }
            var distributorId = distributor != null ? distributor.Id : Guid.Empty;

            var distributorSalesman = new DistributorSalesman(id);
            distributorSalesman.Name = distributorSalesmanImport.Name;
            distributorSalesman.CostCentreCode = distributorSalesmanImport.Code;
            distributorSalesman.CostCentreType = CostCentreType.DistributorSalesman;
            distributorSalesman.ParentCostCentre=new CostCentreRef(){Id = distributorId};
            return distributorSalesman;

        }
    }
}
