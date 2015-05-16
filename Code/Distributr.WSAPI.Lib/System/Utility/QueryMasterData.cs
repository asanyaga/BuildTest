using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WSAPI.Lib.System.Utility
{
    public abstract class QueryBase
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

    }
    public class QueryMasterData : QueryBase
    {
        public QueryMasterData()
        {
            IsFirstSync = false;
        }
        public DateTime From { get; set; }
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFirstSync { get; set; }

    }

}
