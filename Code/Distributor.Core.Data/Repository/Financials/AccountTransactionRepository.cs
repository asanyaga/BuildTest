using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility.Validation;
using log4net;


namespace Distributr.Core.Data.Repository.Financials
{
    internal class AccountTransactionRepository: IAccountTransactionRepository
    {
       CokeDataContext _ctx;       
       IAccountRepository _accountRepository;
       protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AccountTransactionRepository(CokeDataContext ctx,IAccountRepository accountRepository){
           
            _ctx = ctx;                 
            _accountRepository = accountRepository;
            
        }
        public AccountTransaction Map(tblAccountTransaction tblAccTr){
        
            AccountTransaction accTr = new AccountTransaction(tblAccTr.Id){
            
                Account  = _accountRepository.GetById(tblAccTr.AccountId),
                Amount = (decimal)tblAccTr.Amount,
                DocumentType = (DocumentType)tblAccTr.DocumentType,
                DocumentId = tblAccTr.DocumentId,
                DateInserted = tblAccTr.DateInserted
            };
        
          
            return accTr;
        
        }

        public Guid Add(AccountTransaction accountTransaction)
        {
                    
            ValidationResultInfo vri = Validate(accountTransaction);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed To Validate AccountTransaction");
            }
            tblAccountTransaction tblAccTr  =_ctx.tblAccountTransaction.FirstOrDefault(n => n.Id == accountTransaction.Id);
            DateTime dt = DateTime.Now;
            if (tblAccTr == null)
            {
                tblAccTr = new tblAccountTransaction();
                tblAccTr.Id = accountTransaction.Id;
                _ctx.tblAccountTransaction.AddObject(tblAccTr);
                _log.DebugFormat("accountTransaction.Id == 0");
            }
           


            tblAccTr.AccountId = accountTransaction.Account.Id;
            tblAccTr.Amount = accountTransaction.Amount;
            tblAccTr.DocumentType = (int)accountTransaction.DocumentType;
            tblAccTr.DocumentId = accountTransaction.DocumentId;
            tblAccTr.DateInserted = accountTransaction.DateInserted;
            
            _log.DebugFormat("Saving/Updating AccountTransaction");

           _ctx.SaveChanges();
            return tblAccTr.Id;
        }
        public AccountTransaction GetById(Guid id)
        {
          
            tblAccountTransaction tblAccTr = _ctx.tblAccountTransaction.FirstOrDefault(n => n.Id == id);
            if (tblAccTr == null)
            {
                return null;
            }
            AccountTransaction aCC = Map(tblAccTr);
            return aCC;
        }

        public List<AccountTransaction> GetByCostCentre(Guid costCentreId)
        {

            List<AccountTransaction> qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.CostCenterId == costCentreId).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        public List<AccountTransaction> GetByCostCentre(Guid costCentreId, Account account, DocumentType? documentType)
        {

            List<AccountTransaction> qry = null;
            if (documentType == null)
            {
                qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.CostCenterId == costCentreId && n.tblAccount.id == account.Id
                                            ).ToList().Select(n => Map(n)).ToList();
            }
            else
            {
                int docType = (int)documentType;
                qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.CostCenterId == costCentreId && n.tblAccount.id == account.Id)
                                            .ToList().Select(n => Map(n)).ToList();
            }

            return qry;
        }

        public List<AccountTransaction> GetByDate(DateTime startDate, DateTime endDate)
        {
           
            List<AccountTransaction> qry = _ctx.tblAccountTransaction.Where(n =>  n.DateInserted >= startDate && n.DateInserted <= endDate).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        public List<AccountTransaction> GetByCostCentre(Guid costCentreId, Account account, DocumentType? documentType, DateTime startDate, DateTime endDate)
        {
            List<AccountTransaction> qry = null;
            if(documentType==null){
                qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.CostCenterId == costCentreId&& n.tblAccount.id == account.Id
                                            && n.DateInserted >= startDate && n.DateInserted <= endDate).ToList().Select(n => Map(n)).ToList();
            }else{
                int docType = (int)documentType;
                qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.CostCenterId == costCentreId && n.tblAccount.id == account.Id
                                            && n.DateInserted >= startDate && n.DateInserted <= endDate && n.DocumentType==docType).ToList().Select(n => Map(n)).ToList();
            }
                          
            return qry;
        }
       
        public ValidationResultInfo Validate(AccountTransaction itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }


        public List<AccountTransaction> GetByAccount(Guid accountId)
        {
            List<AccountTransaction> qry = _ctx.tblAccountTransaction.Where(n => n.tblAccount.id == accountId).ToList().Select(n => Map(n)).ToList();
            return qry;
        }
    }
}
