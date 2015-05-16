using Distributr.Core.Data.EF;
using Distributr.MongoDB.CommandRouting;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoEnvelopeMigrate
{
    public class AppSettings
    {
        private static readonly ILog log = LogManager.GetLogger
                                    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AppSettings()
        {
            MongoConnectionString = ConfigurationManager.AppSettings["mongoconnectionstring"];
            log.InfoFormat("Mongoconnection string {0}", MongoConnectionString);
            MongoDatabase = ConfigurationManager.AppSettings["database"];
            log.InfoFormat("Mongo database name {0}", MongoDatabase);
            LocalCache = ConfigurationManager.AppSettings["localcache"];
            log.InfoFormat("Local cache location {0}", LocalCache);
            SqlConnectionString = ConfigurationManager.AppSettings["sqlconnectionstring"];
            log.InfoFormat("Sql connection string {0}", SqlConnectionString);
            SetupMongo();
        }

        void SetupMongo()
        {
            var client = new MongoClient(MongoConnectionString);
            var server = client.GetServer();
            MongoDatabase database = server.GetDatabase(MongoDatabase);
            MongoCollection_Command_CommandProcessingAudit = database.GetCollection<CommandProcessingAudit>("CommandProcessingAudit");
            MongoCollection_Command_CommandRouteOnRequest = database.GetCollection<CommandRouteOnRequest>("CommandRouteOnRequest");
            MongoCollection_Command_CommandRouteOnRequestCostCentre = database.GetCollection<CommandRouteOnRequestCostcentre>("CommandRouteOnRequestCostcentre");
            MongoCollection_Command_CommandRoutingStatus = database.GetCollection<CommandRoutingStatus>("CommandRoutingStatus");
            Uri baseUri = new Uri(MongoConnectionString);
            Uri myUri = new Uri(baseUri, MongoDatabase);
            MongoCommandEnvelopeProccessingAuditRepository = new CommandEnvelopeRouteOnRequestCostcentreRepository(myUri.ToString());
            MongoCommandEnvelopeRORCostCentreRepository = new CommandEnvelopeRouteOnRequestCostcentreRepository(myUri.ToString());
        }

        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
        public string LocalCache { get; set; }
        public string SqlConnectionString { get; set; }

        public CokeDataContext GetContext()
        {
            return new CokeDataContext(SqlConnectionString);
        }

        public MongoCollection<CommandProcessingAudit> MongoCollection_Command_CommandProcessingAudit { get; set; }
        public MongoCollection<CommandRouteOnRequest> MongoCollection_Command_CommandRouteOnRequest { get; set; }
        public MongoCollection<CommandRouteOnRequestCostcentre> MongoCollection_Command_CommandRouteOnRequestCostCentre { get; set; }
        public MongoCollection<CommandRoutingStatus> MongoCollection_Command_CommandRoutingStatus { get; set; }

        public ICommandEnvelopeProcessingAuditRepository MongoCommandEnvelopeProccessingAuditRepository { get; set; }
        public ICommandEnvelopeRouteOnRequestCostcentreRepository MongoCommandEnvelopeRORCostCentreRepository {get;set;}
        //public ICommandEnvelopeProcessingAuditRepository MongoCommandEnvelopee

    }
}
