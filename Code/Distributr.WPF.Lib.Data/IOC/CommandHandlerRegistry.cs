using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire;
using Distributr.Core.Data.IOC;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.IncomingCommandHandlers;
using Distributr.WPF.Lib.Services.CommandHandler;
using StructureMap.Configuration.DSL;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class CommandHandlerRegistry : Registry
    {
        public CommandHandlerRegistry()
        {
            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }
        }

        public List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
               {
                   Tuple.Create(typeof (IIncomingCommandHandler), typeof (IncomingCommandHander)),
                   Tuple.Create(typeof (IRetireDocumentCommandHandler), typeof (RetireDocumentWPFCommandHandler))
               };
            var chDefaultList = DefaultCommandHandlers.ServiceList().Union(serviceList);
            return chDefaultList.ToList();
        }
    }
}
