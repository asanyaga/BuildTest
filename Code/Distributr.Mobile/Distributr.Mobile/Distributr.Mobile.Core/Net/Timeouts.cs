using System;

namespace Distributr.Mobile.Core.Net
{
    public static class Timeouts
    {
        //10 Seconds
        private static readonly TimeSpan DEFAULT_HTTP_TIMEOUT = new TimeSpan(0, 0, 10);
        
        public static TimeSpan DefaultHttpTimeout()
        {
            return DEFAULT_HTTP_TIMEOUT;
        }
    }
}