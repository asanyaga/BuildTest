using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public class CommandRouteItem
    {
        public long Id { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateCommandInserted { get; set; }
        public Guid CommandDestinationCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        public string CommandType { get; set; }
        public string JsonCommand { get; set; }
        public bool CommandDelivered { get; set; }
        public DateTime? DateCommandDelivered { get; set; }
        public bool CommandExecuted { get; set; }
        public DateTime? DateCommandExecuted { get; set; }
        public int CostCentreApplicationCommandSequenceId { get; set; }

    }

    public class CommandRouteOnRequest
    {
        public CommandRouteOnRequest()
        {
            CommandRouteCentre = new List<CommandRouteCentre>();
        }

        public long Id { get; set; }
        public Guid CommandId { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateCommandInserted { get; set; }
        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }
        public Guid CommandGeneratedByUserId { get; set; }
        public string CommandType { get; set; }
        public string JsonCommand { get; set; }
        public List<CommandRouteCentre> CommandRouteCentre { get; set; }
        public Guid DocumentParentId { get; set; }
       

    }

    public class CommandRouteCentre
    {
        public Guid CostCentreId { get; set; }
    }
}
