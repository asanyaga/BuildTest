using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
   public abstract class SourcingDocument:  TransactionalEntity
    {
       protected SourcingDocument(Guid id,DocumentType documentType) : base(id)
       {
           DocumentType = documentType;
           VehicleArrivalTime = null;
           VehicleDepartureTime = null;
       }
       public string DocumentReference { get; set; }
       [Required(ErrorMessage = "Document issuer cost centre is required")]
       public CostCentre DocumentIssuerCostCentre { get;  set; }
       public Guid DocumentIssuerCostCentreApplicationId { get; set; }
       public Guid DocumentParentId { get; set; }
       public User DocumentIssuerUser { get;  set; }
       public DateTime DocumentDateIssued { get;  set; }
       public DateTime DocumentDate { get;  set; }

       public DateTime? VehicleArrivalTime { get; set; }
       public DateTime? VehicleDepartureTime { get; set; }

       public decimal? VehicleArrivalMileage { get; set; }
       public decimal? VehicleDepartureMileage { get; set; }

       public DocumentSourcingStatus Status { get; internal set; }
       [Required(ErrorMessage = "Document Recipient CostCentre is required")]
       public CostCentre DocumentRecipientCostCentre { get;  set; }
       public DocumentType DocumentType { get; private set; }
       public string Description { get; set; }
       public string Note { get; set; }
       public DateTime SendDateTime { get; set; }
       public abstract void Confirm();
       public abstract void Approve();
       public abstract void Close();

       private List<DocumentCommand> _pcommandsToExecute = new List<DocumentCommand>();


       protected void _AddCommand(DocumentCommand command)
       {
           if (_CanAddCommands)
               _pcommandsToExecute.Add(command);
       }

       protected void _ClearCommands()
       {
           _pcommandsToExecute.Clear();
       }

       protected abstract void _AddCreateCommandToExecute(bool isHybrid = false);
       protected abstract void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false) where T : TransactionalEntity;
       protected abstract void _AddConfirmCommandToExecute(bool isHybrid = false);
       protected bool _CanAddCommands { get; set; }
    }
   public static class DocumentExtensions
   {
       public static List<DocumentCommand> GetDocumentCommandsToExecute(this SourcingDocument document, bool clearCommands = true)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           FieldInfo pinfo = type.BaseType.GetField("_pcommandsToExecute", bindingFlags);
           List<DocumentCommand> commands = pinfo.GetValue(document) as List<DocumentCommand>;
           return commands;
       }


       public static void CallClearCommands(this SourcingDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           MethodInfo minfo = type.BaseType.GetMethod("_ClearCommands", bindingFlags);
           minfo.Invoke(document, null);
       }


       public static void CallAddCreateCommandToExecute(this SourcingDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           MethodInfo minfo = type.GetMethod("_AddCreateCommandToExecute", bindingFlags);
           minfo.Invoke(document, null);

       }

       public static void EnableAddCommands(this SourcingDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
           pi.SetValue(document, true);
       }
       public static void DisableAddCommands(this SourcingDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
           pi.SetValue(document, false);
       }
   }
}
