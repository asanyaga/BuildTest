using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;

namespace Distributr.DataImporter.Lib.Experimental.Sync
{
   public abstract class SyncBase
    {
        protected IDbConnection HqConnection
        {
            get
            {
                return GetConnectionString("cokeconnectionstring");
            }
        }
        protected IDbConnection DistributrLocalConnection
        {
            get
            {
                return GetConnectionString("DistributrLocal");
            }
        }


        private SqlConnection GetConnectionString(string connectionKey)
        {
            var ctx = new CokeDataContext(ConfigurationManager.AppSettings[connectionKey]);
            SqlConnection sqlConnection = null;
            var entityConnection = ctx.Connection as EntityConnection;

            if (entityConnection == null)
            {
                throw new ArgumentNullException("entityConnection");

            }
            sqlConnection = entityConnection.StoreConnection as SqlConnection;

            sqlConnection.Open();

            return sqlConnection;
        }
    }
}
