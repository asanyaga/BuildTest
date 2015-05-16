using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Services.Bus;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Distributr.Azure.Lib.Bus
{
    public static class AzureBusMessageExtensions
    {
        public static byte[] ToBinary(this BusMessage busMessage)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] output = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                bf.Serialize(ms, busMessage);
                output = ms.GetBuffer();
            }
            return output;
        }

        public static BusMessage FromMessage(this CloudQueueMessage message)
        {
            BusMessage busMessage = null;
            byte[] buffer = message.AsBytes;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                BinaryFormatter bf = new BinaryFormatter();
                busMessage = (BusMessage) bf.Deserialize(ms);
            }
            return busMessage;
        }

    }
}
