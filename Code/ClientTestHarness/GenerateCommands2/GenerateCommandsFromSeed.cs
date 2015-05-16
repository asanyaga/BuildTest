using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Newtonsoft.Json;

namespace ClientTestHarness.GenerateCommands2
{
    public class GenerateCommandsFromSeed
    {
        public GenerateCommandsFromSeed()
        {
            _filesDirectory = Environment.CurrentDirectory + @"\files\";
        }
        private string _filesDirectory = "";
        private Guid _issuedOnBehalfOfCostCentreId = new Guid("ce0bd3f2-7e5e-4193-a822-7822626f02cc");
        private Guid _documentRecipientCostCentreId = new Guid("d2d6e532-f5fe-4a3f-aa0f-7c5f011140d9");
        private Guid _documentIssuedCostCentreId = new Guid("19d72b83-969e-42cb-b084-249be9744bab");
        private Guid _documentGeneratedByCostCentreApplicationId = new Guid("415979c1-39a7-4703-8632-d1dc5f6adda6");
        private Guid _documentGeneratedByCostCentreId = new Guid("19d72b83-969e-42cb-b084-249be9744bab");

        public Guid IssuedOnBehalfOfCostCentreId
        {
            get { return _issuedOnBehalfOfCostCentreId; }
            set { _issuedOnBehalfOfCostCentreId = value; }
        }

        public Guid DocumentRecipientCostCentreId
        {
            get { return _documentRecipientCostCentreId; }
            set { _documentRecipientCostCentreId = value; }
        }

        public Guid DocumentIssuedCostCentreId
        {
            get { return _documentIssuedCostCentreId; }
            set { _documentIssuedCostCentreId = value; }
        }

        public Guid DocumentGeneratedByCostCentreApplicationId
        {
            get { return _documentGeneratedByCostCentreApplicationId; }
            set { _documentGeneratedByCostCentreApplicationId = value; }
        }

        public Guid DocumentGeneratedByCostCentreId
        {
            get { return _documentGeneratedByCostCentreId; }
            set { _documentGeneratedByCostCentreId = value; }
        }


        public List<Tuple<int,DocumentCommand>> BuildMainOrderCommandsFromSeed()
        {
            var commands = new List<Tuple<int, DocumentCommand>>();
            string[] fileNames = new string[]
                {
                    "CreateMainOrder_253",
                    "AddMainOrderLineItem_254",
                    "AddMainOrderLineItem_255",
                    "AddMainOrderLineItem_256",
                    "OrderPaymentInfo_257",
                    "ConfirmMainOrder_258"
                };
            Guid documentId = Guid.NewGuid();
            foreach (var filename in fileNames)
            {
                int counter = GetCounter();
                string createCommandFile = _filesDirectory + filename;
                string createCommandJson = File.ReadAllText(createCommandFile);
                DocumentCommand command = JsonConvert.DeserializeObject<DocumentCommand>(createCommandJson);
                command.DocumentId = documentId;
                command.PDCommandId = documentId;
                command.CommandId = Guid.NewGuid();
                command.CommandGeneratedByCostCentreId = _documentGeneratedByCostCentreId;
                command.CommandSequence = counter;
                command.CommandGeneratedByCostCentreApplicationId = _documentGeneratedByCostCentreApplicationId;
                command.SendDateTime = DateTime.Now;
                command.CommandCreatedDateTime = DateTime.Now;
                CreateMainOrderCommand createMainOrderCommand = command as CreateMainOrderCommand;
                if (createMainOrderCommand != null)
                {
                    createMainOrderCommand.DocumentDateIssued = DateTime.Now;
                    createMainOrderCommand.DateOrderRequired = DateTime.Now;
                    createMainOrderCommand.IssuedOnBehalfOfCostCentreId = _issuedOnBehalfOfCostCentreId;
                    createMainOrderCommand.DocumentRecipientCostCentreId = _documentRecipientCostCentreId;
                    createMainOrderCommand.DocumentIssuerCostCentreId = _documentIssuedCostCentreId;
                }
                commands.Add(Tuple.Create(counter,command));
            }
            
            return commands;
        }

        private int GetCounter()
        {
            string counterFile = _filesDirectory + "commandseqcounter.txt";
            string txt = "281";
            bool exists = File.Exists(counterFile);
            if(exists)
                 txt = File.ReadAllText(counterFile);
            int val = int.Parse(txt);
            if(exists)
                File.Delete(counterFile);
            val++;
            File.WriteAllText(counterFile,val.ToString());
            return val;
        }



    }
}
