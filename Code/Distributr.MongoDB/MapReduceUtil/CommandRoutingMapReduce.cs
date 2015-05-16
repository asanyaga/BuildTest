using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.MongoDB.CommandRoutingMapReduce;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Distributr.MongoDB.MapReduceUtil
{
    //public class CommandRoutingMapReduce : MongoBase, ICommandRoutingMapReduce
    //{
    //    public CommandRoutingMapReduce(string connectionString) : base(connectionString)
    //    {
            
    //    }
    //    //public MapReduceResult CommandProcessingAuditMapReduce(string collectionName, string keyFunction, string reduceFunction, string finalizeFunction = "", DateTime? fromDate = null)
    //    //{
    //    //    var options = new MapReduceOptionsBuilder();
    //    //    options.SetOutput(MapReduceOutput.Inline);
    //    //    if (!string.IsNullOrEmpty(finalizeFunction))
    //    //        options.SetFinalize(finalizeFunction);
    //    //    var collection = CurrentMongoDB.GetCollection(collectionName);
    //    //    if (fromDate.HasValue)
    //    //    {
    //    //      var qry =  collection.AsQueryable<CommandProcessingAudit>().Where(n => n.DateInserted > fromDate.Value);
    //    //      var mongoQuery = ((MongoQueryable<CommandProcessingAudit>)qry).GetMongoQuery();
    //    //        return collection.MapReduce(mongoQuery, keyFunction, reduceFunction, options);
    //    //    }

    //    //    var result = collection.MapReduce(keyFunction, reduceFunction, options);
    //    //    return result;
    //    //}

    //    public MapReduceResult CommandRouteOnMapReduce(string collectionName, string keyFunction, string reduceFunction,
    //                                                    string finalizeFunction = "", DateTime? fromDate = null)
    //    {
    //        var options = new MapReduceOptionsBuilder();
    //        options.SetOutput(MapReduceOutput.Inline);
    //        if (!string.IsNullOrEmpty(finalizeFunction))
    //            options.SetFinalize(finalizeFunction);
    //        var collection = CurrentMongoDB.GetCollection(collectionName);
    //        if (fromDate.HasValue)
    //        {
    //            var qry = collection
    //                .AsQueryable<CommandRouteOnRequestCostcentre>()
    //                .Where(n => n.DateAdded > fromDate.Value);
    //            var mongoQuery = ((MongoQueryable<CommandRouteOnRequestCostcentre>)qry).GetMongoQuery();
    //            return collection.MapReduce(mongoQuery, keyFunction, reduceFunction, options);
    //        }

    //        var result = collection.MapReduce(keyFunction, reduceFunction, options);
    //        return result;
    //    }
    //    public MapReduceResult CommandRouteStatusMapReduce(string collectionName, string keyFunction, string reduceFunction,
    //                                                    string finalizeFunction = "", DateTime? fromDate = null)
    //    {
    //        var options = new MapReduceOptionsBuilder();
    //        options.SetOutput(MapReduceOutput.Inline);
    //        if (!string.IsNullOrEmpty(finalizeFunction))
    //            options.SetFinalize(finalizeFunction);
    //        var collection = CurrentMongoDB.GetCollection(collectionName);
    //        if (fromDate.HasValue)
    //        {
    //            var qry = collection
    //                .AsQueryable<CommandRoutingStatus>()
    //                .Where(n => n.DateAdded > fromDate.Value);
    //            var mongoQuery = ((MongoQueryable<CommandRoutingStatus>)qry).GetMongoQuery();
    //            return collection.MapReduce(mongoQuery, keyFunction, reduceFunction, options);
    //        }

    //        var result = collection.MapReduce(keyFunction, reduceFunction, options);
    //        return result;
    //    }
    //}
}
