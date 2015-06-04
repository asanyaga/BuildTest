using System;
using System.Collections.Generic;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Support
{
    public class CommandEnvelopeChecker
    {
        private readonly CommandEnvelopeChecks[] checks;

        public CommandEnvelopeChecker(params CommandEnvelopeChecks [] checks)
        {
            this.checks = checks;
        }

        public bool IsSatisfied(List<CommandEnvelope> envelopes)
        {
            Assert.AreEqual(checks.Length, envelopes.Count, "Expected envelope count versus actual");
            var index = 0;

            foreach (var envelope in envelopes)
            {
                checks[index].IsSatisfied(envelope, index);
                index++;
            }
            return true;
        }
    }

    public class CommandEnvelopeChecks
    {
        public DocumentType TypeOfDocumentToCheck;
        public CommandType[] CommandTypesToCheck;

        public CommandEnvelopeChecks(DocumentType typeOfDocumentToCheck, params CommandType[] commandTypesToCheck)
        {
            TypeOfDocumentToCheck = typeOfDocumentToCheck;
            CommandTypesToCheck = commandTypesToCheck;
        }

        public bool IsSatisfied(CommandEnvelope envelope, int index)
        {
            Assert.AreEqual((int)TypeOfDocumentToCheck, envelope.DocumentTypeId, "Document type at index {0}", index);

            var actualCommands = envelope.CommandsList;
            
            for (var i =0; i < CommandTypesToCheck.Length; i++)
            {
                var expectedCommandType = CommandTypesToCheck[i].ToString();
                var actualType = actualCommands[i].Command.CommandTypeRef;
                Console.WriteLine(actualType);
                Assert.AreEqual(expectedCommandType, actualType, "Command Type");
            }

            return true;
        }
    }
}