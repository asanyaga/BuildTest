using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    [Serializable]
    public class BusMessage
    {
        public Guid MessageId { get; set; }
        public string CommandType { get; set; }
        public string BodyJson { get; set; }
        public string SendDateTime { get; set; }
        public bool IsSystemMessage { get; set; }
    }
    [Serializable]
    public class EnvelopeBusMessage
    {
        public Guid MessageId { get; set; }
        public int DocumentTypeId { get; set; }
        public string BodyJson { get; set; }
        public string SendDateTime { get; set; }
        public bool IsSystemMessage { get; set; }
    }
}
