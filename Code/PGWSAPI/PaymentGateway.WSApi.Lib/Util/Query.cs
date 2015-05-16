using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.MasterData;

namespace PaymentGateway.WSApi.Lib.Util
{
    public abstract class PGQueryBase
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

    }
    public class PGQueryMasterData : PGQueryBase
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
       
    }
    public class PGQuery : PGQueryMasterData
    {
        public bool ShowInactive { get; set; }
    }
    public class PGQueryClientMember : PGQueryMasterData
    {
        public int? ClientId { get; set; }
    }
    public class PGQueryResult
    {
        public PGQueryResult()
        {
            Result = new List<MasterEntity>();
        }

        public List<MasterEntity> Result { get; set; }
        public int Count { get; set; }

    }

    public class QueryResult<T> where T : class
    {
        public QueryResult()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; set; }
        public int Count { get; set; }

    }
    public abstract class QueryBase
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

    }

    public class QueryStandard : QueryBase
    {
        public string Name { get; set; }
        public bool ShowInactive { get; set; }
    }
}
