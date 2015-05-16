using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Impl.Model.Transactional;
using Distributr.WPF.Lib.Impl.Repository.Utility;

namespace Distributr.WPF.Lib.Data.Repository.Utility
{
    public class AppTempTransactionRepository:IAppTempTransactionRepository
    {
        private DistributrLocalContext _context;

        public AppTempTransactionRepository(DistributrLocalContext context)
        {
            _context = context;
        }

        public void Save(AppTempTransaction appTempTransaction)
        {
            var temp = _context.AppTempTransactions.FirstOrDefault(s => s.Id == appTempTransaction.Id);
            if(temp==null)
            {
                temp= new AppTempTransaction();
                temp.Id = appTempTransaction.Id;
                _context.AppTempTransactions.Add(temp);
            }
            temp.DateInserted = DateTime.Now;
            temp.Json = appTempTransaction.Json;
            temp.TransactionStatus = appTempTransaction.TransactionStatus;
            temp.TransactionType = appTempTransaction.TransactionType;
            _context.SaveChanges();
        }

        public AppTempTransaction GetById(Guid id)
        {
            return _context.AppTempTransactions.FirstOrDefault(s => s.Id == id);
        }

        public List<AppTempTransaction> Query(DateTime startdate, DateTime endDate, OrderType orderType)
        {
            string _orderType = orderType.ToString();
            return _context.AppTempTransactions.Where(s => s.TransactionType ==_orderType && s.TransactionStatus==false).ToList();
        }

        public void MarkAsConfirmed(Guid orderId)
        {
            var temp = _context.AppTempTransactions.FirstOrDefault(s => s.Id == orderId);
            if (temp != null)
            {
                temp.TransactionStatus = true;
                _context.SaveChanges();
            }
            
           
        }
    }
}