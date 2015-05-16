using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;

namespace Distributr.WSAPI.Lib.Services.Bus.EasyNetQ
{
    public class EasyNetQNullLogger : IEasyNetQLogger
    {
        public void DebugWrite(string format, params object[] args)
        {

        }

        public void InfoWrite(string format, params object[] args)
        {

        }

        public void ErrorWrite(string format, params object[] args)
        {

        }

        public void ErrorWrite(Exception exception)
        {

        }
    }
}
