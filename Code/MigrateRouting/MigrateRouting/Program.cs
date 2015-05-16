using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigrateRouting.Old;

namespace MigrateRouting
{
  public  class Program
  {
      
        static void Main(string[] args)
        {
            System.Console.WriteLine("Press Enter to continue.........................................");
            System.Console.ReadLine();
            MigrateRouting();
            System.Console.WriteLine("Press Enter to continue..........................................");
            System.Console.ReadLine();
            System.Console.ReadLine();
            System.Console.ReadLine();
        }

      private static void MigrateRouting()
      {
          MigrateCommands();
          MigrateCommandCentre();
          MigrateCommandStatus();
      }

      private static void MigrateCommandStatus()
      {
          cokecommandroutingEntities db = new cokecommandroutingEntities();
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine("Start Migrating  Command Status");
          System.Console.WriteLine("==================================================================");
          string connection = ConfigurationSettings.AppSettings["MongoRoutingConnectionString"];
          CommandRoutingOnRequestMongoRepository _repo = new CommandRoutingOnRequestMongoRepository(connection);
          int count = 0;
          foreach (tblRoutingStatu cmd in db.tblRoutingStatus.OrderBy(o => o.tblDistributrCommand.Id))
          {
              CommandRoutingStatus cmdmongo = new CommandRoutingStatus();
              cmdmongo.DateAdded = cmd.tblDistributrCommand.DateCommandInserted;
              cmdmongo.CommandRouteOnRequestId = cmd.tblDistributrCommand.Id;
              cmdmongo.DateAdded = DateTime.Now;
              cmdmongo.Delivered = true;
              cmdmongo.DateDelivered = DateTime.Now;
              cmdmongo.DestinationCostCentreApplicationId = cmd.DestinationCostCentreApplicationId;
              cmdmongo.CommandRouteOnRequestId = cmd.tblDistributrCommand.Id;
              cmdmongo.CommandId = cmd.tblDistributrCommand.CommandId;
              cmdmongo.Id = Guid.NewGuid();
              try
              {
                  _repo.AddStatus(cmdmongo);
                  count++;
              }catch{}
             

          }
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine(count + " Migrate Command Status migrated");
          System.Console.WriteLine("==================================================================");  
      }

      private static void MigrateCommandCentre()
      {
          cokecommandroutingEntities db = new cokecommandroutingEntities();
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine("Start migrating Migrate Command Centre");
          System.Console.WriteLine("==================================================================");
          string connection = ConfigurationSettings.AppSettings["MongoRoutingConnectionString"];
          CommandRoutingOnRequestMongoRepository _repo = new CommandRoutingOnRequestMongoRepository(connection);
          int count = 0;
          foreach (tblRoutingCentre cmd in db.tblRoutingCentres.OrderBy(o => o.tblDistributrCommand.Id))
          {
              CommandRouteOnRequestCostcentre cmdmongo = new CommandRouteOnRequestCostcentre();
              cmdmongo.CommandType =cmd.tblDistributrCommand.CommandType;
              cmdmongo.DateAdded = cmd.tblDistributrCommand.DateCommandInserted;
              cmdmongo.CommandRouteOnRequestId = cmd.tblDistributrCommand.Id;
              cmdmongo.CostCentreId = cmd.RoutingCostCentreId;
              cmdmongo.IsRetired = cmd.tblDistributrCommand.IsRetired;
              cmdmongo.IsValid = cmd.Valid;
              cmdmongo.Id = Guid.NewGuid();
              
              _repo.AddRoutingCentre(cmdmongo);
              count++;

          }
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine(count + " Migrate Command Centre migrated");
          System.Console.WriteLine("==================================================================");
          

      }
     static CommandType GetCommandType(string commandType)
      {
          CommandType _commandType;
          Enum.TryParse(commandType, out _commandType);
          return _commandType;
      }
      private static void MigrateCommands()
      {
          cokecommandroutingEntities db = new cokecommandroutingEntities();
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine("Start migrating commands");
          System.Console.WriteLine("==================================================================");
          string connection=ConfigurationSettings.AppSettings["MongoRoutingConnectionString"];
          CommandRoutingOnRequestMongoRepository  _repo = new CommandRoutingOnRequestMongoRepository(connection);
          int count = 0;
          foreach(tblDistributrCommand cmd in db.tblDistributrCommands.OrderBy(o=>o.Id))
          {
              CommandRouteOnRequest cmdmongo = new CommandRouteOnRequest();
              cmdmongo.CommandGeneratedByCostCentreApplicationId = cmd.CommandGeneratedByCostCentreApplicationId;
              cmdmongo.CommandGeneratedByUserId = cmd.CommandGeneratedByUserId;
              cmdmongo.CommandId = cmd.CommandId;
              cmdmongo.CommandType = cmd.CommandType;
              cmdmongo.DateAdded = cmd.DateCommandInserted;
              cmdmongo.DateCommandInserted = cmd.DateCommandInserted;
              cmdmongo.DocumentId = cmd.DocumentId;
              cmdmongo.DocumentParentId = cmd.DocumentParentId.HasValue?cmd.DocumentParentId.Value:Guid.NewGuid();
              cmdmongo.Id = cmd.Id;
              cmdmongo.IsRetired = cmd.IsRetired;
              cmdmongo.JsonCommand = cmd.JsonCommand;
              _repo.Add(cmdmongo);
              count++;

          }
          System.Console.WriteLine("==================================================================");
          System.Console.WriteLine(count + " Command migrated");
          System.Console.WriteLine("==================================================================");
          

      }
    }
}
