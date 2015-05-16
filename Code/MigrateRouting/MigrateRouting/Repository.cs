using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MigrateRouting
{
    public class CommandRoutingOnRequestMongoRepository : MongoBase
    {
        private string _commandRouteOnRequestCollectionName = "CommandRouteOnRequest";
        private string _commandRouteOnRequestCostcentreCollectionName = "CommandRouteOnRequestCostcentre";
        private string _commandCommandRoutingStatusCollectionName = "CommandRoutingStatus";
        private MongoCollection<CommandRouteOnRequest> _CommandRouteOnRequestCollection;
        private MongoCollection<CommandRouteOnRequestCostcentre> _CommandRouteOnRequestCostcentreCollection;
        private MongoCollection<CommandRoutingStatus> _commandRoutingStatusCollection;
        private IdCounterHelper _counterHelper;

        public CommandRoutingOnRequestMongoRepository(string connectionStringM1)
            : base(connectionStringM1)
        {
            _CommandRouteOnRequestCollection = CurrentMongoDB.GetCollection<CommandRouteOnRequest>(_commandRouteOnRequestCollectionName);
            _CommandRouteOnRequestCostcentreCollection = CurrentMongoDB.GetCollection<CommandRouteOnRequestCostcentre>(_commandRouteOnRequestCostcentreCollectionName);
            _commandRoutingStatusCollection = CurrentMongoDB.GetCollection<CommandRoutingStatus>(_commandCommandRoutingStatusCollectionName);
            _counterHelper = new IdCounterHelper(connectionStringM1);

            _CommandRouteOnRequestCollection.EnsureIndex(IndexKeys<CommandRouteOnRequest>.Ascending(n => n.CommandId), IndexOptions.SetUnique(true));
            _CommandRouteOnRequestCollection.EnsureIndex(IndexKeys<CommandRouteOnRequest>.Ascending(n => n.DocumentId));
            _CommandRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandRouteOnRequestCostcentre>.Ascending(n => n.CommandRouteOnRequestId).Ascending(n => n.CostCentreId), IndexOptions.SetUnique(true));
            _commandRoutingStatusCollection.EnsureIndex(IndexKeys<CommandRoutingStatus>.Ascending(n => n.CommandRouteOnRequestId).Ascending(n => n.DestinationCostCentreApplicationId), IndexOptions.SetUnique(true));
            _commandRoutingStatusCollection.EnsureIndex(IndexKeys<CommandRoutingStatus>.Ascending(n => n.CommandId).Ascending(n => n.DestinationCostCentreApplicationId), IndexOptions.SetUnique(true));
        }
        public long Add(CommandRouteOnRequest commandRouteItem)
        {
            commandRouteItem.DateAdded = DateTime.Now;
            if (commandRouteItem.Id == 0) //To allow for migration
            {
                long nextId = _counterHelper.GetNextId(_commandRouteOnRequestCollectionName);
                commandRouteItem.Id = nextId;
            }
            _CommandRouteOnRequestCollection.Save(commandRouteItem);
            return commandRouteItem.Id;
        }
        public void AddRoutingCentre(CommandRouteOnRequestCostcentre commandRouteOnRequestCostcentre)
        {
            //commandRouteOnRequestCostcentre.IsValid = true;
            commandRouteOnRequestCostcentre.DateAdded = DateTime.Now;
            _CommandRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
        }
        public void AddStatus(CommandRoutingStatus commandRoutingStatus)
        {
            _commandRoutingStatusCollection.Save(commandRoutingStatus);
        }
    }
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
    }
}
