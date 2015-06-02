using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.Recollections;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public abstract class Document : TransactionalEntity
    {
        public Document(Guid id)
            : base(id)
        {
        }

        public Document(Guid id,
            string documentReference,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status
            )
            : base(id)
        {
            DocumentReference = documentReference;
            DocumentIssuerCostCentre = documentIssuerCostCentre;
            DocumentIssuerCostCentreApplicationId = documentIssuerCostCentreApplicationId;
            DocumentIssuerUser = documentIssuerUser;
            DocumentDateIssued = documentDateIssued;
            DocumentRecipientCostCentre = documentRecipientCostCentre;
            Status = status;
        }

        public string DocumentReference { get; set; }

        [Required(ErrorMessage = "Document issuer cost centre is required")]
        public CostCentre DocumentIssuerCostCentre { get;  set; }
        
        public Guid DocumentIssuerCostCentreApplicationId { get; set; }
        public Guid DocumentParentId { get; set; }
        public User DocumentIssuerUser { get;  set; }
        public DateTime DocumentDateIssued { get;  set; }
        public  DocumentStatus Status { get; internal set; }
        public CostCentre DocumentRecipientCostCentre { get;  set; }

        public DocumentType DocumentType { get; internal set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime SendDateTime { get; set; }

        public abstract void Confirm();
        
        DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate.Date; }
            set { _startDate = value; }
        }

        DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 3)); }
            set { _endDate = value; }
        }

        public bool IsWithinDateRange(DateTime date)
        {
            return date > StartDate && date < EndDate;
        }

        private List<DocumentCommand> _pcommandsToExecute = new List<DocumentCommand>();
       

        protected void _AddCommand(DocumentCommand command)
        {
            if(_CanAddCommands)
                _pcommandsToExecute.Add(command);
        }

        protected void _ClearCommands()
        {
            _pcommandsToExecute.Clear();
        }

        protected abstract void _AddCreateCommandToExecute(bool isHybrid=false);
        protected abstract void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid=false) where T : DocumentLineItem;
        protected abstract void _AddConfirmCommandToExecute(bool isHybrid=false);
        protected bool _CanAddCommands { get; set; }
    }

    public static class DocumentExtensions
    {
        public static List<DocumentCommand> GetDocumentCommandsToExecute(this Document document, bool clearCommands=true)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo pinfo = type.BaseType.GetField("_pcommandsToExecute", bindingFlags);
            List<DocumentCommand> commands = pinfo.GetValue(document) as List<DocumentCommand>;
            return commands;
        }
        public static List<DocumentCommand> GetRecollectionCommandsToExecute(this ReCollection document, bool clearCommands = true)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo pinfo = type.GetField("_pcommandsToExecute", bindingFlags);
            List<DocumentCommand> commands = pinfo.GetValue(document) as List<DocumentCommand>;
            return commands;
        }
        public static List<DocumentCommand> GetSubOrderCommandsToExecute(this MainOrder mainOrder, bool clearCommands = true)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type mainordertype = mainOrder.GetType();
            PropertyInfo pi = mainordertype.GetProperty("SubOrders", bindingFlags);
            List<SubOrder> subOrders = pi.GetValue(mainOrder) as List<SubOrder>;
            List<DocumentCommand> commands = new List<DocumentCommand>();
            foreach (var document in subOrders)
            {
                Type type = document.GetType();

                FieldInfo pinfo = type.BaseType.BaseType.GetField("_pcommandsToExecute", bindingFlags);
                commands.AddRange(pinfo.GetValue(document) as List<DocumentCommand>);
            }
            return commands;
        }

        public static void CallClearCommands(this Document document)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo minfo = type.BaseType.GetMethod("_ClearCommands", bindingFlags);
            minfo.Invoke(document, null);
        }


        public static void CallAddCreateCommandToExecute(this Document document)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo minfo = type.GetMethod("_AddCreateCommandToExecute", bindingFlags);
            minfo.Invoke(document, null);

        }

        public static void EnableAddCommands(this Document document)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
            pi.SetValue(document,true);
        }
        public static void DisableAddCommands(this Document document)
        {
            Type type = document.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
            pi.SetValue(document, false);
        }
    }
}
