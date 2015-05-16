using System.Configuration;

namespace DistributrAgrimanagrFeatures.Helpers.DB
{
    public class DefaultDbTestingHelper
    {
        public static DB_TestingHelper GetDefaultDbTestingHelper()
        {
            string serverSqlConnectionString = ConfigurationManager.AppSettings["Server_DistributrConnectionString"];
            string hubSqlConnectionString = ConfigurationManager.AppSettings["Hub_DistributrConnectionString"];
            string hubLocalConnectionString = ConfigurationManager.AppSettings["Hub_RoutingConnectionString"];
            string mongoConnectionstring = ConfigurationManager.AppSettings["MongoRoutingConnectionString"];
            string mongoAuditingConnectionString = ConfigurationManager.AppSettings["MongoAuditingConnectionString"];
            string createtablesscriptlocation = ConfigurationManager.AppSettings["createtablesscriptlocation"];
            var cs = new DB_TestingHelper(hubSqlConnectionString, hubLocalConnectionString, serverSqlConnectionString, mongoConnectionstring, mongoAuditingConnectionString, createtablesscriptlocation);
            return cs;
        }


    }
}