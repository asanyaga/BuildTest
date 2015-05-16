using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Resources.Util
{
    public enum PlatformType { win, web, service }
    public interface IMessageSourceAccessor
    {
         string GetText(string code);
       
    }
}
