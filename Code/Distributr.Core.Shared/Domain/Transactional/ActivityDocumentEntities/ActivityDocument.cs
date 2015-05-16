using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Domain.Transactional.ActivityDocumentEntities
{

    public  class ActivityDocument:  TransactionalEntity
    {
       private ActivityDocument(Guid id)
           : base(id)
       {
          _activityInputItem=  new List<ActivityInputItem>();
           _activityInfectionItems= new List<ActivityInfectionItem>();
           _activityProduceItems= new List<ActivityProduceItem>();
           _activityServiceItems= new List<ActivityServiceItem>();
       }
       public string DocumentReference { get; set; }
       public DocumentType DocumentType { get; set; }
       public CostCentre Hub { get;  set; }
       public CostCentre FieldClerk { get; set; }
       public CostCentre Supplier { get; set; }
       public CommodityProducer Producer { get; set; }
       public Route Route { get; set; }
       public Centre Centre { get; set; }
       public ActivityType ActivityType { get; set; }
       public DateTime ActivityDate { get; set; }
       public Guid DocumentIssuerCostCentreApplicationId { get; set; }
       public Season Season { get; set; }
       public DateTime DocumentDateIssued { get;  set; }
       public DateTime DocumentDate { get;  set; }
       public string Description { get; set; }
       public string ActivityReference { get; set; }
       public DateTime SendDateTime { get; set; }

       private List<ActivityInfectionItem> _activityInfectionItems;
       public List<ActivityInfectionItem> InfectionLineItems
       {
           get { return _activityInfectionItems; }

       }
       private List<ActivityProduceItem> _activityProduceItems;
       public List<ActivityProduceItem> ProduceItems
       {
           get { return _activityProduceItems; }

       }

       private List<ActivityServiceItem> _activityServiceItems;
       public List<ActivityServiceItem> ServiceItems
       {
           get { return _activityServiceItems; }

       }

        private List<ActivityInputItem> _activityInputItem;
       public List<ActivityInputItem> InputLineItems
       {
           get { return _activityInputItem; }

       }
       public  void Confirm()
       {
           AddCreateCommandToExecute();
           AddConfirmCommandToExecute();
          
       }

        private void AddConfirmCommandToExecute()
        {
            var cmd = new ConfirmActivityCommand();
            
            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();
            cmd.Description = Description;
            cmd.DocumentId = Id;
            _AddCommand(cmd);
        }

        private void AddCreateCommandToExecute()
        {
            var cmd =new CreateActivityNoteCommand();
            cmd.ActivityDate = ActivityDate;
            cmd.ActivityTypeId = ActivityType.Id;
            cmd.CentreId = Centre.Id;
            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();
            cmd.CommodityProducerId = Producer.Id;
            cmd.CommoditySupplierId = Supplier.Id;
            cmd.Description = Description;
            cmd.DocumentDateIssued = DateTime.Now;
            cmd.DocumentId = Id;
            cmd.DocumentIssuerCostCentreId = FieldClerk.Id;
            cmd.DocumentReference = DocumentReference;
            cmd.HubId = Hub.Id;
            cmd.RouteId = Route.Id;
            cmd.SeasonId = Season.Id;
            cmd.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(cmd);
        }

        public  void Add(ActivityInputItem item)
       {
           _activityInputItem.Add(item);
           AddAddInputLineItemCommandToExecute(item);
       }

     
        public void Add(ActivityServiceItem item)
        {
            _activityServiceItems.Add(item);
            AddAddServiceLineItemCommandToExecute(item);
        }

       
        public void Add(ActivityInfectionItem item)
        {
            _activityInfectionItems.Add(item);

            AddAddInfectionLineItemCommandToExecute(item);
        }

       
        public void Add(ActivityProduceItem item)
        {
            _activityProduceItems.Add(item);
            AddAddProduceLineItemCommandToExecute(item);
        }
        private void AddAddInputLineItemCommandToExecute(ActivityInputItem lineItem)
        {
            if (lineItem == null) return;
            if (!_CanAddCommands) return;
            var cmd = new AddActivityInputLineItemCommand();

            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();

            cmd.DocumentId = Id;
            cmd.ExpiryDate = lineItem.ExpiryDate;
            cmd.ManufacturedDate = lineItem.ManufacturedDate;
            cmd.ProductId = lineItem.Product.Id;
            cmd.Quantity = lineItem.Quantity;
            cmd.SerialNo = lineItem.SerialNo;
            cmd.LineItemId = lineItem.Id;
            cmd.Description = lineItem.Description;
            _AddCommand(cmd);

        }

        private void AddAddServiceLineItemCommandToExecute(ActivityServiceItem item)
        {
            if (item == null) return;

            var cmd = new AddActivityServiceLineItemCommand();

            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();

            cmd.DocumentId = Id;
            cmd.ServiceId = item.Service.Id;
            cmd.ServiceProviderId = item.ServiceProvider.Id;
            cmd.ShiftId = item.Shift.Id;
            cmd.Description = item.Description;
            cmd.LineItemId = item.Id;
            cmd.Description = item.Description;
            _AddCommand(cmd);
        }

        private void AddAddInfectionLineItemCommandToExecute(ActivityInfectionItem item)
        {
            if (item == null) return;
            var cmd = new AddActivityInfectionLineItemCommand();

            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();
            cmd.DocumentId = Id;
            cmd.InfectionId = item.Infection.Id;
            cmd.Description = item.Description;
            cmd.LineItemId = item.Id;
            cmd.Description = item.Description;
            _AddCommand(cmd);
        }

        private void AddAddProduceLineItemCommandToExecute(ActivityProduceItem item)
        {
            if (item == null) return;

            var cmd = new AddActivityProduceLineItemCommand();

            cmd.CommandCreatedDateTime = DateTime.Now;
            cmd.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            cmd.CommandGeneratedByCostCentreId = FieldClerk.Id;
            cmd.CommandId = Guid.NewGuid();
            cmd.DocumentId = Id;
            cmd.CommodityId = item.Commodity.Id;
            cmd.GradeId = item.Grade.Id;
            cmd.Weight = item.Weight;
            cmd.ServiceProviderId = item.ServiceProvider.Id;
            cmd.Description = item.Description;
            cmd.LineItemId = item.Id;
            cmd.Description = item.Description;
            _AddCommand(cmd);
        }


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

    
       protected bool _CanAddCommands { get; set; }
    }
   public static class DocumentExtensions
   {
       public static List<DocumentCommand> GetDocumentCommandsToExecute(this ActivityDocument document, bool clearCommands = true)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           FieldInfo pinfo = type.GetField("_pcommandsToExecute", bindingFlags);
           List<DocumentCommand> commands = pinfo.GetValue(document) as List<DocumentCommand>;
           return commands;
       }


       public static void CallClearCommands(this ActivityDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           MethodInfo minfo = type.GetMethod("_ClearCommands", bindingFlags);
           minfo.Invoke(document, null);
       }


       public static void CallAddCreateCommandToExecute(this ActivityDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           MethodInfo minfo = type.GetMethod("_AddCreateCommandToExecute", bindingFlags);
           minfo.Invoke(document, null);

       }

       public static void EnableAddCommands(this ActivityDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
           pi.SetValue(document, true);
       }
       public static void DisableAddCommands(this ActivityDocument document)
       {
           Type type = document.GetType();
           BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
           PropertyInfo pi = type.GetProperty("_CanAddCommands", bindingFlags);
           pi.SetValue(document, false);
       }
   }
}
