using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    public interface IPageControlContract
    {
        uint GetTotalCount();
        ICollection<object> GetRecordsBy(uint StartingIndex, uint NumberOfRecords, object FilterTag);
    }
}
