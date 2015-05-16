using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Distributr.Core.Data.EF;

namespace Distributr.DataImporter.Lib.Experimental
{
     /// <summary>
        /// The abstract data mapper.
        /// </summary>
        /// <typeparam name="T">
        /// The data entity.
        /// </typeparam>
        public abstract class AbstractDataMapper<T>
        {
            /// <summary>
            /// Gets the table name.
            /// </summary>
            protected abstract string TableName { get; }

            /// <summary>
            /// Gets the connection.
            /// </summary>
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
            public tblProduct GetProduct(string code)
            {
                using (IDbConnection cn = HqConnection)
                {
                    return cn.Query<tblProduct>("SELECT  * FROM tblProduct").FirstOrDefault(p=>p.ProductCode.Contains(code));
                    
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
                //return new SqlConnection("Data Source=(local);initial catalog=fcl;persist security info=True;user id=sa;password=P@ssw0rd; multipleactiveresultsets=True;");
            }

            /// <summary>
            /// Find a single record.
            /// </summary>
            /// <param name="query">
            /// The query.
            /// </param>
            /// <param name="param">
            /// The parameters.
            /// </param>
            /// <returns>
            /// The <see cref="T"/>.
            /// </returns>
            public virtual T FindSingle(string query, dynamic param)
            {
                dynamic item = null;

                using (IDbConnection cn = HqConnection)
                {
                  var result = cn.Query(query, (object)param).SingleOrDefault();

                    if (result != null)
                    {
                        item = Map(result);
                    }
                }

                return item;
            }

            /// <summary>
            /// Find all.
            /// </summary>
            /// <returns>
            /// The <see cref="IEnumerable"/>.
            /// </returns>
            public virtual IEnumerable<T> FindAll()
            {
                var items = new List<T>();

                using (IDbConnection cn = HqConnection)
                {
                   
                    var results = cn.Query("SELECT * FROM " + TableName).ToList();

                    for (int i = 0; i < results.Count(); i++)
                    {
                        items.Add(Map(results.ElementAt(i)));
                    }
                }

                return items;
            }

            /// <summary>
            /// The delete.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            public virtual void Delete(Guid id)
            {
                using (IDbConnection cn = HqConnection)
                {
                   cn.Execute("DELETE FROM " + TableName + " WHERE ID=@ID", new { ID = id });
                }
            }

            /// <summary>
            /// The map.
            /// </summary>
            /// <param name="result">
            /// The result.
            /// </param>
            /// <returns>
            /// The <see cref="dynamic"/>.
            /// </returns>
            public abstract T Map(dynamic result);
        }
    }

