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

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders.Impl
{
   public class BankBranchViewModelBuilder:IBankBranchViewModelBuilder 
    {
       IBankBranchRepository _bankBranchRepository;
       IBankRepository _bankRepository;
       private IMasterDataUsage _masterDataUsage;

       public BankBranchViewModelBuilder(IBankBranchRepository bankBranchRepository,IBankRepository bankRepository, IMasterDataUsage masterDataUsage)
      {
          _bankBranchRepository = bankBranchRepository;
          _bankRepository = bankRepository;
           _masterDataUsage = masterDataUsage;
      }
      public BankBranchViewModel Map(BankBranch bankBranch)
      {
          return new BankBranchViewModel
          {
            Id=bankBranch.Id,
            Name=bankBranch.Name,
            BankId=_bankRepository.GetById(bankBranch.Bank.Id).Id ,
            BankName=_bankRepository.GetById(bankBranch.Bank.Id ).Name,
            Code=bankBranch.Code,
            Description=bankBranch.Description,
            IsActive = bankBranch._Status == EntityStatus.Active ? true : false
          };
      }
        public IList<BankBranchViewModel> GetAll(bool inactive = false)
        {
            var bank = _bankBranchRepository.GetAll(inactive);
            return bank.Select(n => Map(n)).ToList();
        }

        public List<BankBranchViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _bankBranchRepository.GetAll(inactive ).Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.Code.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public BankBranchViewModel Get(Guid Id)
        {
            BankBranch bankBranch = null;
            bankBranch = _bankBranchRepository.GetById(Id);
            if (bankBranch == null) return null;
            return Map(bankBranch);
        }

        public void Save(BankBranchViewModel bankBranchViewModel)
        {
            BankBranch bankBranch = new BankBranch(bankBranchViewModel.Id)
            {
                Name = bankBranchViewModel.Name,
                Code = bankBranchViewModel.Code,
                Description = bankBranchViewModel.Description,
                Bank=_bankRepository.GetById (bankBranchViewModel.BankId )
            };
            _bankBranchRepository.Save(bankBranch);
        }

        public void SetInactive(Guid id)
        {
            BankBranch bankBranch = _bankBranchRepository.GetById(id);
            _bankBranchRepository.SetInactive(bankBranch);
        }

        public void SetActive(Guid id)
        {
            BankBranch bankBranch = _bankBranchRepository.GetById(id);
            _bankBranchRepository.SetActive(bankBranch);
        }

        public void SetDeleted(Guid id)
        {
            BankBranch bankBranch = _bankBranchRepository.GetById(id);
            _bankBranchRepository.SetAsDeleted(bankBranch);
        }

        public Dictionary<Guid, string> Bank()
        {
            return _bankRepository.GetAll()
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

       public QueryResult<BankBranchViewModel> Query(QueryStandard query)
       {
           var queryResult = _bankBranchRepository.Query(query);
           var result = new QueryResult<BankBranchViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;
       }
    }
}
