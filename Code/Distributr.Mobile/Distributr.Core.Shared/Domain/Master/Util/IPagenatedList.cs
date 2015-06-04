using System.Collections.Generic;

namespace Distributr.Core.Domain.Master.Util
{
    public interface IPagenatedList<T>:IList<T>, IEnumerable<T>
    {
        int PageCount { get;  }
        int TotalItemCount { get; }
        int PageIndex { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPrevPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        IEnumerable<T> UnderlyingList { get; }
    }
}
