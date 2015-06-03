

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile.Common.Core.Data
{
    public interface IDataSource<T>
    {
        List<T> Fetch(int offset, int pageSize);
        Task<List<T>> FetchAsync(int offset, int pageSize);
    }
}