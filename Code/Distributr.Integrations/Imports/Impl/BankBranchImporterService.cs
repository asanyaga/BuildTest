using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class BankBranchImporterService:BaseImporterService,IBankBranchImporterService
    {
        private IBankBranchRepository _bankBranchRepository;
        private IBankRepository _bankRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly CokeDataContext _context;


        public BankBranchImporterService(IBankBranchRepository bankBranchRepository, IBankRepository bankRepository, CokeDataContext context)
        {
            _bankBranchRepository = bankBranchRepository;
            _bankRepository = bankRepository;
            _context = context;
        }

        public ImportResponse Save(List<BankBranchImport> imports)
        {
            var mappingValidationList = new List<string>();
            List<BankBranch> bankBranches = imports.Select(s => Map(s, mappingValidationList)).ToList();
            if (mappingValidationList.Any())
            {
                return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

            }

            List<ValidationResultInfo> validationResults = bankBranches.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };
            }

            List<BankBranch> changedBankBranches = HasChanged(bankBranches);

            foreach (var changedBankBranch in changedBankBranches)
            {
                _bankBranchRepository.Save(changedBankBranch);
            }
            return new ImportResponse() { Status = true, Info = changedBankBranches.Count + " Bank Branches Successfully Imported" };
        }


        private List<BankBranch> HasChanged(List<BankBranch> bankBranches)
        {
            var changedBankBranches = new List<BankBranch>();
            foreach (var bankBranch in bankBranches)
            {
                var entity = _bankBranchRepository.GetById(bankBranch.Id);
                if (entity == null)
                {
                    changedBankBranches.Add(bankBranch);
                    continue;
                }

                bool hasChanged = entity.Name.ToLower() != bankBranch.Name.ToLower() ||
                                  entity.Description.ToLower() != bankBranch.Description.ToLower() ||
                                  entity.Bank != bankBranch.Bank;

                if (hasChanged)
                {
                    changedBankBranches.Add(bankBranch);
                }
            }
            return changedBankBranches;
        }

        private ValidationResultInfo Validate(BankBranch bankBranch)
        {
            return _bankBranchRepository.Validate(bankBranch);
        }


        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var bankId = _context.tblBankBranch.Where(p => p.Code == deletedCode).Select(p => p.Id).FirstOrDefault();

                    var bank = _bankBranchRepository.GetById(bankId);
                    if (bank != null)
                    {
                        _bankBranchRepository.SetAsDeleted(bank);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("BankBranch Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "BankBranch Deleted Succesfully", Status = true };
        }


        private BankBranch Map(BankBranchImport bankBranchImport, List<string> mappingvalidationList)
        {
            var exists = Queryable.FirstOrDefault(_context.tblBankBranch, p => p.Code == bankBranchImport.Code);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var bank = Queryable.FirstOrDefault(_context.tblBank, p => p.Code == bankBranchImport.BankCode);

            if (bank == null) { mappingvalidationList.Add(string.Format((string) "Invalid Bank Code {0}", (object) bankBranchImport.BankCode)); }
            var bankId = bank != null ? bank.Id : Guid.Empty;

            var bankEntity = _bankRepository.GetById(bankId);

            var bankBranch = new BankBranch(id);
            bankBranch.Bank = bankEntity;
            bankBranch.Code = bankBranchImport.Code;
            bankBranch.Name = bankBranchImport.Name;
            bankBranch.Description = bankBranchImport.Description;

            return bankBranch;

        }
    }
}
