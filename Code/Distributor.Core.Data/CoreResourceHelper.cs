using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.Core.Data
{
    public static class CoreResourceHelper
    {
        public static string GetText(string code)
        {
            IMessageSourceAccessor msa = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            if (msa != null)
                return msa.GetText(code);
            else
                return code;
        }
    }
}
