using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.BusSubscriber
{
    public interface IDistributrSubscriberService
    {
        void Start();
        void Stop();
    }
}
