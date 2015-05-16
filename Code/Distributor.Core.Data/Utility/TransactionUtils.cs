using System;
using System.Transactions;

namespace Distributr.Core.Data.Utility
{
   public class TransactionUtils
    {
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
        /// <summary>
        /// cn: untested
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(IsolationLevel isolationLevel, TimeSpan timeOut)
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = isolationLevel;
            transactionOptions.Timeout = timeOut;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}
