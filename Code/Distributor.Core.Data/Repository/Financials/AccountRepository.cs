using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.Financials
{
    internal class AccountRepository : IAccountRepository
    {
        CokeDataContext _ctx;


        ICostCentreRepository _costCentreRepository;
        protected static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AccountRepository(CokeDataContext ctx, ICostCentreRepository costCenterRepository)
        {
            _ctx = ctx;
            _costCentreRepository = costCenterRepository;
        }
        public Account GetById(Guid id)
        {
            _log.DebugFormat("Getting By Id:{0}", id);
            tblAccount tblAcc = _ctx.tblAccount.FirstOrDefault(n => n.id == id);
            if (tblAcc == null)
            {
                return null;
            }
            Account aCC = Map(tblAcc);
            return aCC;
        }

        public List<Account> GetAll()
        {
            List<Account> qry = _ctx.tblAccount.ToList().Select(n => Map(n)).ToList();
            return qry;

        }

        public List<Account> GetByCostCentreId(Guid id)
        {
            _log.DebugFormat("Getting by Cost Center Id:{0}", id);
            List<Account> qry = _ctx.tblAccount.Where(n => n.CostCenterId == id).ToList().Select(n => Map(n)).ToList();
            return qry;
        }



        public Guid Add(Account account)
        {
              tblAccount tblAcc = _ctx.tblAccount.FirstOrDefault(p=>p.id==account.Id);
            if (tblAcc==null)
            {
                tblAcc = new tblAccount();
                tblAcc.id = account.Id;
            }
               
            ValidationResultInfo vri = Validate(account);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed To Validate Account");
            }
          
            DateTime dt = DateTime.Now;
          
            tblAcc.CostCenterId = account.CostcentreId;
            tblAcc.AccountType = (int)account.AccountType;
            tblAcc.Balance = account.Balance;
            
            _ctx.tblAccount.AddObject(tblAcc);
            _ctx.SaveChanges();
            return tblAcc.id;
        }

        public DateTime GetLastTimeItemUpdated()
        {
            throw new NotImplementedException();
        }

        public Account Map(tblAccount tblAcc)
        {
            Account acc = new Account
            {
                Id = tblAcc.id,
                CostcentreId = tblAcc.CostCenterId,
                Balance = (decimal)tblAcc.Balance,
                AccountType = (AccountType)tblAcc.AccountType,
                
            };
            return acc;

        }
        public ValidationResultInfo Validate(Account itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }



        public Account GetAccount(Guid costCentreId, AccountType accountType)
        {
            tblAccount tblAcc =
                _ctx.tblAccount.FirstOrDefault(n => n.CostCenterId == costCentreId && n.AccountType == (int)accountType);
            if (tblAcc == null) return null;

            return Map(tblAcc);
        }


        public void AdjustAccountBalance(Guid costCentreId, AccountType accountType, decimal amount)
        {
            tblAccount tblAcc =
                _ctx.tblAccount.FirstOrDefault(n => n.CostCenterId == costCentreId && n.AccountType == (int)accountType);
            if (tblAcc == null) return ;
            tblAcc.Balance += amount;
            _ctx.SaveChanges();
        }

        public List<Account> GetByAccount(Guid accountId)
        {
            List<Account> qry = _ctx.tblAccount.Where(n => n.id == accountId).ToList().Select(n => Map(n)).ToList();
            return qry;
        }
    }
}
