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
    public class BankImporterService:BaseImporterService,IBankImporterService
    {
        private IBankRepository _bankRepository;
        private CokeDataContext _context;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public BankImporterService(IBankRepository bankRepository, CokeDataContext context)
        {
            _bankRepository = bankRepository;
            _context = context;
        }


        public ImportResponse Save(List<BankImport> imports)
        {
            List<Bank> banks = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = banks.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                var validationResultsInfo = ValidationResultsInfo(validationResults);
                return new ImportResponse() { Status = false, Info = validationResultsInfo };
            }
            List<Bank> changedBanks = HasChanged(banks);

            foreach (var changedBank in changedBanks)
            {
                _bankRepository.Save(changedBank);
            }

            return new ImportResponse() {Status = true, Info = changedBanks.Count + " Banks Successfully Imported"};

        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var bankId = _context.tblBank.Where(p => p.Code == deletedCode).Select(p => p.Id).FirstOrDefault();

                    var bank = _bankRepository.GetById(bankId);
                    if (bank != null)
                    {
                        _bankRepository.SetAsDeleted(bank);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Bank Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Bank Deleted Succesfully", Status = true };
        }


        private List<Bank> HasChanged(List<Bank> banks)
        {
            var changedBanks = new List<Bank>();

            foreach (var bank in banks)
            {
                var entity = _bankRepository.GetById(bank.Id);
                if(entity==null)
                {
                    changedBanks.Add(bank);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != bank.Name.ToLower() ||
                                  entity.Code.ToLower() != bank.Code.ToLower();

                if(hasChanged)
                {
                    changedBanks.Add(bank);
                }
            }
            return changedBanks;
        }

        private ValidationResultInfo Validate(Bank bank)
        {
            return _bankRepository.Validate(bank);
        }

        private Bank Map(BankImport bankImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblBank, p => p.Code == bankImport.Code);

            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var bank = new Bank(id);
            bank.Name = bankImport.Name;
            bank.Code = bankImport.Code;
            bank.Description = bankImport.Description;

            return bank;
        }
    }
}
