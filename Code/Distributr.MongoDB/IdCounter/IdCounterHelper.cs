using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.MongoDB.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Distributr.MongoDB.IdCounter
{
    public class IdCounterHelper : MongoBase
    {
        private string idCounterCollectionName = "idcounter";
        private MongoCollection<BsonDocument> _counterCollection;
        public IdCounterHelper(string connectionString): base(connectionString)
        {
            _counterCollection = CurrentMongoDB[idCounterCollectionName];
        }

        public int GetNextId(string collectionName)
        {
           if( _counterCollection.FindOne(Query.EQ("_id",collectionName)) == null)
                _counterCollection.Insert(new BsonDocument { { "_id", collectionName }, { "c", 0 }});
            var query = Query.EQ("_id", collectionName);
            var sortBy = SortBy.Descending("_id");
            var update = Update.Inc("c", 1);
            var result = _counterCollection.FindAndModify(query, sortBy, update, true);
            return result.ModifiedDocument["c"].AsInt32;
        }

    }

    public class Counter
    {

    }
}
