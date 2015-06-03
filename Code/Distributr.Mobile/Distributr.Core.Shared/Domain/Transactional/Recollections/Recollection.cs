using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.Recollections;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;

namespace Distributr.Core.Domain.Transactional.Recollections
{
    public enum ReCollectionType 
    {
        Cash=1,
        Cheque=2,
        Mpesa=3
    }

    public class ReCollection : MasterEntity
    {

        public ReCollection(Guid id)
            : base(id)
        {
        }

        public Guid CostCentreApplicationId { get; set; }
        public DocumentType DocumentType { get; set; }
        public Guid RecepientCostCentreId { get; set; }
        
        public Guid CostCentreId { get; set; }

        public void ConfirmLineItem(UnderBankingItem item)
        {
            _AddConfirmCommandLineItemCommandToExecute(item);
        }

        public void AddLineItem(UnderBankingItem item)
        {

            _AddCommandLineItemCommandToExecute(item);
        }

        public void Confirm()
        {

        }

        public List<UnderBankingItem> LineItems;
        protected void _AddConfirmCommandLineItemCommandToExecute(UnderBankingItem lineItem)
        {
            if (lineItem == null)
                return;
            var li = new ReCollectionCommand();
            li.IsConfirm = true;
            li.Amount = lineItem.Amount;
            li.CommandCreatedDateTime = DateTime.Now;
            li.CommandGeneratedByCostCentreApplicationId = CostCentreApplicationId;
            li.CommandGeneratedByCostCentreId = CostCentreId;
            li.CommandId = Guid.NewGuid();
            li.DocumentId =Id;
            li.ItemId = lineItem.Id;
            li.Description = lineItem.Description;
            li.DocumentRecipientCostCentreId = RecepientCostCentreId;
            li.FromCostCentreId = lineItem.FromCostCentreId;
            li.PDCommandId = Id;
            _AddCommand(li);
        }
        protected void _AddCommandLineItemCommandToExecute(UnderBankingItem lineItem)

        {
            if (lineItem == null)
                return;
            var li = new ReCollectionCommand();
            li.Amount = lineItem.Amount;
            li.CommandCreatedDateTime = DateTime.Now;
            li.CommandGeneratedByCostCentreApplicationId = CostCentreApplicationId;
            li.CommandGeneratedByCostCentreId = CostCentreId;
            li.CommandId = Guid.NewGuid();
            li.DocumentId =lineItem.Id;
            li.Description = lineItem.Description;
            li.DocumentRecipientCostCentreId = RecepientCostCentreId;
            li.FromCostCentreId = lineItem.FromCostCentreId;
            li.PDCommandId = Id;
            _AddCommand(li);
        }
        private List<DocumentCommand> _pcommandsToExecute = new List<DocumentCommand>();
        protected void _AddCommand(DocumentCommand command)
        {
            _pcommandsToExecute.Add(command);
        }

    }

    public class UnderBankingItem : MasterEntity
    {
        public Guid FromCostCentreId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public string Description { get; set; }
        public ReCollectionType Type { get; set; }
        public UnderBankingItem(Guid id)
            : base(id)
        {
        }

        public UnderBankingItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status)
            : base(id, dateCreated, dateLastUpdated, status)
        {
        }
    }
    public class UnderBankingItemSummary :MasterEntity
    {
        public Guid CostCentreId { get; set; }
        public Guid SalesmanId { get; set; }
        public string CostCentreName { get; set; }
        public string SalesmanName { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public string Description { get; set; }
        public string CostCentreType { get; set; }

        public UnderBankingItemSummary(Guid id)
            : base(id)
        {
        }

       
    }
    public class UnderBankingItemReceived : MasterEntity
    {
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
        public ReCollectionType Type { get; set; }
        public bool IsConfirmed { get; set; }
        public UnderBankingItemReceived(Guid id)
            : base(id)
        {
        }


    }
}
