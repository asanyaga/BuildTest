using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.Util;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    public class PagerControlData
    {
        public PagerControlData()
        {
        }

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPrevPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
    }
}
