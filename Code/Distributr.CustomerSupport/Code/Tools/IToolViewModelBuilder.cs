using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.CustomerSupport.Code.Tools
{
    public interface IToolViewModelBuilder
    {
        void Retire(Guid docParentId);
        void UnRetire(Guid docParentId);
    }
}
