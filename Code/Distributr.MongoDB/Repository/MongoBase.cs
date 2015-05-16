using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Distributr.MongoDB.Repository
{
    public abstract class MongoBase
    {
        private MongoDatabase _mongoDatabase;
        public MongoBase(string connectionString)
        {
            var _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var server = MongoServer.Create(connectionString);
            _mongoDatabase = server.GetDatabase(_databaseName);
           
            
        }

        protected MongoDatabase CurrentMongoDB
        {
            get { return _mongoDatabase; }
        }
        protected bool TestConnection()
        {
            try
            {
              
             var cNames=    _mongoDatabase.GetCollectionNames();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        protected Tuple<DateTime, DateTime> GetDateRangeFromDOY(int dayOfYear, int year)
        {
            var dt = new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
            var dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
            return new Tuple<DateTime, DateTime>(dt, dt1);
        }
    }
}
