using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Distributr.MongoDB.Repository.Impl
{
    public class CCAuditRepository : MongoBase, ICCAuditRepository
    {
        private MongoCollection<CCAuditItem> _mongoCollection;

        public CCAuditRepository(string connectionString): base(connectionString) 
        {
            _mongoCollection = CurrentMongoDB.GetCollection<CCAuditItem>("audititem");
        }
        public void Add(CCAuditItem item)
        {
            _mongoCollection.Save(item);
        }

        public List<CCAuditItem> GetByCC(Guid costCentreId, int dayOfYear, int year)
        {
            var dateRange = GetDateRangeFromDOY(dayOfYear, year);
            return _mongoCollection
                .AsQueryable()
                .Where(n => n.CostCentreId == costCentreId 
                    && n.DateInsert > dateRange.Item1  
                    && n.DateInsert <dateRange.Item2).OrderByDescending(s=>s.CostCentreId)
                .ToList();
        }


        public List<CCAuditSummary> GetDailySummary(int dayOfYear, int year)
        {
             var _dateRange = GetDateRangeFromDOY(dayOfYear, year);
            var qry = _mongoCollection
                .AsQueryable()
                .Where(n => n.DateInsert > _dateRange.Item1
                       && n.DateInsert < _dateRange.Item2);
            var mongoQuery = ((MongoQueryable<CCAuditItem>)qry).GetMongoQuery();
            var initial = new BsonDocument { { "NoHits", 0 }}; 
            var reduce = (BsonJavaScript)"function(doc, prev) { prev.NoHits += 1;}";
            var results = _mongoCollection.Group(mongoQuery, "CostCentreId", initial, reduce, null).ToArray();
            var _results =  results.Select(BsonSerializer.Deserialize<CCAuditSummary>);

            return _results.ToList();

        }

        public List<HitSummary> GetHitSummary(int noDays = 30)
        {
            DateTime toDate = DateTime.Now;
            DateTime fromDate = DateTime.Now.AddDays(-noDays);
            var qry = _mongoCollection
               .AsQueryable()
               .Where(n => n.DateInsert > fromDate
                      && n.DateInsert < toDate);
            var mongoQuery = ((MongoQueryable<CCAuditItem>)qry).GetMongoQuery();
            var initial = new BsonDocument { { "Count", 0 } };
            var reduce = (BsonJavaScript)"function(doc, prev) { prev.Count += 1;  }";
            var keyf = (BsonJavaScript)@"function(doc){return {  Year : doc.DateInsert.getYear(), Month: doc.DateInsert.getMonth(), DayOfMonth : doc.DateInsert.getDate() }}";
            var results = _mongoCollection.Group(mongoQuery, keyf, initial, reduce, null).ToArray();
            var _results = results.Select(BsonSerializer.Deserialize<HitSummary>);
            return _results.ToList();
        }
    }
}
