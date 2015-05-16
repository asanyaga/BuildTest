using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories;

namespace Distributr.Core.Data.Repository.Transactional
{
   public class DocumentHelper : IDocumentHelper
    {
       private CokeDataContext _ctx;

       public DocumentHelper(CokeDataContext ctx)
       {
           _ctx = ctx;
       }

       public ICommand GetExternalRef(ICommand command)
       {
           bool canGen = true;
           if(canGen)
           {
               if (command is CreateMainOrderCommand)
               {
                   var orderCommand = (CreateMainOrderCommand) command;
                   var costcentre =
                       _ctx.tblCostCentre.FirstOrDefault(
                           s => (
                                    s.Id == orderCommand.DocumentRecipientCostCentreId ||
                                    s.Id == orderCommand.DocumentIssuerCostCentreId) &&
                                s.CostCentreType == (int) CostCentreType.DistributorSalesman);
                   if (costcentre!=null)
                   {
                       string sc = costcentre.Cost_Centre_Code;
                       /*int count =
                           _ctx.tblDocument.Count(
                               s =>
                               (s.DocumentRecipientCostCentre == costcentre.Id ||
                               s.DocumentIssuerCostCentreId == costcentre.Id) && s.DocumentTypeId == (int)DocumentType.Order);
                       string sq = (count + 1).ToString().PadLeft(6,'0');*/
                       var sql = String.Format(Resources.Others.OtherResource.GetNextDocRef, sc);
                       var code = _ctx.ExecuteStoreQuery<string>(sql);
                       orderCommand.ExtDocumentReference = code.FirstOrDefault();
                       return orderCommand;
                   }
                    
               }
           }
           return command;

       }
    }
}
