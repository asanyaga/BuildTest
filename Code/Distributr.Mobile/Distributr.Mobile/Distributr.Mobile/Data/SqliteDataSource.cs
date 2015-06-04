using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Mobile.Core.Exceptions;
using Mobile.Common.Core.Data;

namespace Distributr.Mobile.Data
{
    public class SqliteDataSource<T> : IDataSource<T> where T : new()
    {
        private readonly Database database;
        private readonly string baseQuery;
        private readonly object[] baseParams;

        public SqliteDataSource(Database database, string baseQuery, params object [] baseParams)
        {
            CheckOrderByClause(baseQuery);
            this.database = database;
            this.baseQuery = baseQuery + " limit ?, ?";
            this.baseParams = baseParams;
        }

        public List<T> Fetch(int offset, int pageSize)
        {
            return database.Query<T>(baseQuery, CreateParams(offset, pageSize));
        }

        public async Task<List<T>> FetchAsync(int offset, int pageSize)
        {
            return await Task.Run(() => Fetch(offset, pageSize));
        }

        private object[] CreateParams(int offset, int pageSize)
        {
            return new List<object>(baseParams)
            {
                offset,
                pageSize

            }.ToArray();
        }

        private static void CheckOrderByClause(string sql)
        {
            if (!sql.ToLower().Contains("order by"))
            {
                throw new Bug("You must always specify an order by clause to use this Data Source: \n" + sql);
            } 
        }
    }
}