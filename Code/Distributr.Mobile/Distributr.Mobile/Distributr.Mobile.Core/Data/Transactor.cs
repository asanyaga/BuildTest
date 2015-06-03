using System;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Data
{
    public class Transactor
    {
        private readonly Database database;

        public Transactor(Database database)
        {
            this.database = database;
        }

        // Pass in an Action that represents a unit of work that should be transactional. 
        public Result<object> Transact(Action unitOfWork)
        {
            try
            {
                database.BeginTransaction();

                unitOfWork();

                database.Commit();

                return Result<object>.Success(default(object));
            }
            catch (Exception e)
            {
                database.Rollback();
                return Result<object>.Failure(e, "Error when processing transaction");
            }  
        }
    }
}
