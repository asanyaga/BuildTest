using System;
using Distributr.Core.Commands;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Utility.Command
{
    public interface IResolveCommand
    {
        //CommandType ResolveCommandType(ICommand command);
        ResolveCommandItem Get(string commandName);
        ResolveCommandItem Get(CommandType commandType);
        ResolveCommandItem Get(ICommand commandType);
    }
    public class ResolveCommandItem
    {
        public string CommandName { get; set; }
        public CommandType CommandType { get; set; }
        public Type Command { get; set; }
        public Type CommandHandlerContract { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
