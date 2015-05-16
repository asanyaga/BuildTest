using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.Util;

namespace Distributr.Core.Data.Repository.MasterData.Util
{
    public class PagenatedList<T> : List<T>, IPagenatedList<T>
    {
        public PagenatedList(IQueryable<T> collection, int skip, int take, int totalCount)
        {
            PageIndex = skip;
            PageSize = take;
            TotalItemCount = totalCount;
            PageCount = TotalItemCount > 0 ? (int) Math.Ceiling(TotalItemCount/(double) PageSize) : 0;
            HasPrevPage = PageIndex > 0;
            HasNextPage = (PageIndex <= (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));
        }

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageNumber { get { return PageIndex + 1; } }
        public int PageSize { get; private set; }
        public bool HasPrevPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
    }
}
