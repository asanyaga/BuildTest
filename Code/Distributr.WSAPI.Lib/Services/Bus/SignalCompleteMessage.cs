using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    [Obsolete("Can be removed")]
    public class SignalCompleteMessage
    {
        public string CommandId { get; set; }
        public string DocumentId { get; set; }
    }
}
