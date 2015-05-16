using Akavache;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using log4net;

namespace MongoEnvelopeMigrate
{


    public class SaveMongoCommandsToLocalCache
    {
        private static readonly ILog log = LogManager.GetLogger
                                  (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<int> Go()
        {
            log.Info("Start map reduce to extract and save document ids to local cache==============================");
            var settings = new AppSettings();

            await BlobCache.UserAccount.InvalidateAll();

            int noDocuments = await MapReduceToDocumentid(settings.MongoCollection_Command_CommandProcessingAudit);
            //List<MapReduceResult> qry1 = await SimpleAkavacheQuery();
            return noDocuments;
        }

        public async Task<int> MapReduceToDocumentid(MongoCollection collection)
        {
            string map = @"
                    function() {
                        var auditItem = this;
                        emit(auditItem.DocumentId, { count: 1 });
                    }";
            string reduce = @"        
                    function(key, values) {
                        var result = {count: 0, Key:key };

                        values.forEach(function(value){               
                            result.count += value.count;
                        });

                        return result;
                    }";

            int count = 0;
            try
            {

                var options = new MapReduceOptionsBuilder();
                //options.SetFinalize(finalize);
                options.SetOutput(MapReduceOutput.Inline);
                var results = collection.MapReduce(map, reduce, options);

                var l = new Dictionary<Guid, MapReduceResult>();

                foreach (var result in results.GetResults())
                {

                    BsonValue rVal = result["value"];
                    MapReduceResult d = BsonSerializer.Deserialize<MapReduceResult>(rVal.ToJson());
                    if (await KeyExists(d.Key))
                    {
                        log.InfoFormat("Map reduce duplicate for document id {0} ", d.Key);
                        continue;
                    }
                    count++;
                    Console.WriteLine("Adding No [{0}] Key {1} ", count, d.Key.ToString());
                    l.Add(d.Key, d);
                    var o = await AddToLocalDB(d);
                    if (count % 100 == 0)
                    {
                        log.InfoFormat("{0} document keys processed", count);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.InfoFormat("{0} document keys saved to local cache ");
            return count;
        }
        public async Task<MapReduceResult> AddToLocalDB(MapReduceResult mr)
        {
            var r = await BlobCache.UserAccount.InsertObject(mr.Key.ToString(), mr);
            return mr;
        }

        public async Task<bool> KeyExists(Guid key)
        {
            try
            {
                MapReduceResult o = await BlobCache.UserAccount.GetObject<MapReduceResult>(key.ToString());
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
    }

    public class MapReduceResult
    {
        public Guid Key { get; set; }
        public int count { get; set; }
    }
}
