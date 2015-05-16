using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Bus;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using log4net;


namespace Distributr.Azure.Lib.Bus
{
    public class AzureBusPublisher : IBusPublisher
    {
        private ILog _log = LogManager.GetLogger("AzureBusPublisher");
        private string messageQueueName = "inboundqueue";
        public AzureBusPublisher()
        {
            
        }

        public void Publish(BusMessage busMessage)
        {
            byte[] _busMessage = busMessage.ToBinary();
            CloudQueue queue = GetQueue();
            CloudQueueMessage message = new CloudQueueMessage(_busMessage);
            queue.AddMessage(message);
        }

        public void SignalComplete(ICommand command)
        {
            
        }

        public void Publish(EnvelopeBusMessage busMessage)
        {
            throw new NotImplementedException();
        }

        public void WrapAndPublish(ICommand command, CommandType commandType)
        {
           
        }

        private CloudQueue GetQueue()
        {
            string connectionstring = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);
            
            if(storageAccount == null)
                throw new ArgumentOutOfRangeException("StorageConnectionString","Invalid connectionstring");
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = cloudQueueClient.GetQueueReference(messageQueueName);
            queue.CreateIfNotExists();
            return queue;
        }

    }
}
