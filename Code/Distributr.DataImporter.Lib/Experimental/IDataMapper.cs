using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

namespace Distributr.DataImporter.Lib.Experimental
{
    public interface IDataMapper<T> where T :MasterEntity
    {
        /// <summary>
        /// Finds an entity by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T FindById(Guid id);

        /// <summary>
        /// Finds all entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        void Insert(T item);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        void Update(T item);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void Delete(Guid id);
    }
}
