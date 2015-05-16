using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Validation;
using Distributr.Import.Entities;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class BankBranchImportService:BaseImporterService,IBankBranchImporterService
    {
        private IBankBranchRepository _bankBranchRepository;
        private IBankRepository _bankRepository;


        private readonly CokeDataContext _context;


        public BankBranchImportService(IBankBranchRepository bankBranchRepository, IBankRepository bankRepository, CokeDataContext context)
        {
            _bankBranchRepository = bankBranchRepository;
            _bankRepository = bankRepository;
            _context = context;
        }

        public ImportResponse Save(List<BankBranchImport> imports)
        {
            List<BankBranch> bankBranches = imports.Select(Map).ToList();

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


        private BankBranch Map(BankBranchImport bankBranchImport)
        {
            var exists = _context.tblBankBranch.FirstOrDefault(p => p.Code == bankBranchImport.Code);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var bank = _context.tblBank.FirstOrDefault(p => p.Code == bankBranchImport.Bank);
            var bankId = bank != null ? bank.Id : Guid.Empty;

            var bankEntity = _bankRepository.GetById(bankId);

            var bankBranch = new BankBranch(id);
            bankBranch.Bank = bankEntity;
            bankBranch.Name = bankBranchImport.Name;
            bankBranch.Description = bankBranchImport.Description;

            return bankBranch;

        }
    }
}
