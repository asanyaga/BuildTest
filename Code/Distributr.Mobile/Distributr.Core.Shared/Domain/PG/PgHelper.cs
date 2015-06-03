using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.PG
{
    public class ExportClientMember
    {
        
        public string Name { set; get; }

        public string Code { set; get; }

      
        public int MemberType { set; get; }

      
        public string ExternalId { get; set; }
    }
}
