using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.BankViewModels;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders.Impl
{
  public class BankViewModelBuilder:IBankViewModelBuilder 
    {
        IBankRepository _bankRepository;
        private IMasterDataUsage _masterDataUsage;
      public BankViewModelBuilder(IBankRepository bankRepository, IMasterDataUsage masterDataUsage)
      {
          _bankRepository = bankRepository;
          _masterDataUsage = masterDataUsage;
      }
      public BankViewModel Map(Bank bank)
      {
          return new BankViewModel
          {
            Id=bank.Id,
            Name=bank.Name,
            Code=bank.Code,
            Description=bank.Description,
            IsActive = bank._Status == EntityStatus.Active ? true : false,
          };
      }
        public IList<BankViewModel> GetAll(bool inactive = false)
        {
            var bank = _bankRepository.GetAll(inactive);
            return bank.Select(n => Map(n)).ToList();
        }

        public List<BankViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _bankRepository.GetAll(inactive ).Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())) );
            return items.Select(n => Map(n)).ToList();
        }

        public BankViewModel Get(Guid Id)
        {
            Bank bank;
            bank = _bankRepository.GetById(Id);
            if (bank == null) return null;
            return Map(bank);
        }

        public void Save(BankViewModel bankViewModel)
        {
            Bank bank = new Bank(bankViewModel.Id)
            {
                Name=bankViewModel.Name,
                Code=bankViewModel.Code,
                Description=bankViewModel.Description,
            };
            _bankRepository.Save(bank);
        }

        public void SetInactive(Guid id)
        {
            Bank bank = _bankRepository.GetById(id);
            if (_masterDataUsage.CheckBankIsUsed(bank, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate bank. Bank is assigned bank branches. Remove dependencies first to continue");
            }
            _bankRepository.SetInactive(bank);
        }


        public void SetActive(Guid id)
        {
            Bank bank = _bankRepository.GetById(id);
            _bankRepository.SetActive(bank);
        }

        public void SetAsDeleted(Guid id)
        {
            Bank bank = _bankRepository.GetById(id);
            if (_masterDataUsage.CheckBankIsUsed(bank, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete bank. Bank is assigned bank branches. Remove dependencies first to continue");
            }
            _bankRepository.SetAsDeleted(bank);
        }

      public QueryResult<BankViewModel> Query(QueryStandard query)
      {
          var queryResult=_bankRepository.Query(query);

          var result = new QueryResult<BankViewModel>();

          result.Data = queryResult.Data.Select(Map).ToList();
          result.Count = queryResult.Count;
          return result;
      }
    }
}
