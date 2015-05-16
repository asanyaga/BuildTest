using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Distributr.Core.Tests.Commands
{
    [TestFixture]
    public class CommandEnvelopeSerializationTest
    {
        [Test]
        public void Test()
        {
            Fixture f = new Fixture();
            var createCommand = f.Create<CreateMainOrderCommand>();
            var addLineItemCommand = f.Create<AddMainOrderLineItemCommand>();
            var confirmCommand = f.Create<ConfirmMainOrderCommand>();
            var env = f.Build<CommandEnvelope>().Without(n => n.CommandsList).Create();
            long t = DateTime.Now.Ticks;
            env.EnvelopeGeneratedTick = t;
            env.CommandsList.Add(new CommandEnvelopeItem { Order = 1, Command = createCommand });
            env.CommandsList.Add(new CommandEnvelopeItem { Order = 2, Command = addLineItemCommand });
            env.CommandsList.Add(new CommandEnvelopeItem { Order = 3, Command = confirmCommand });
            string json = JsonConvert.SerializeObject(env);
            var dEnv = JsonConvert.DeserializeObject<CommandEnvelope>(json);
            Assert.AreEqual(3,dEnv.CommandsList.Count);
            Assert.AreEqual(t, dEnv.EnvelopeGeneratedTick);
        }
    }
}
