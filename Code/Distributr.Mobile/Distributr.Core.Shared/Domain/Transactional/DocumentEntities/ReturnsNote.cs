using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
        public enum ReturnsNoteType{SalesmanToDistributor = 1, DistributorToHQ = 2}
    public class ReturnsNote : Document
    {
        public List<ReturnsNoteLineItem> _lineItems;

        public ReturnsNote(Guid id)
            : base(id)
        {
            _lineItems = new List<ReturnsNoteLineItem>();
        }

        public ReturnsNote(Guid id,
            string documentReference,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            ReturnsNoteType returnsNoteType,
            //Returns Note
            List<ReturnsNoteLineItem> lineItems
            ): base(id, documentReference, 
            documentIssuerCostCentre,
            documentIssuerCostCentreApplicationId, 
            documentIssuerUser, 
            documentDateIssued,
            documentRecipientCostCentre, 
            status
            )
        {
            ReturnsNoteType = returnsNoteType;
            _lineItems = new List<ReturnsNoteLineItem>();
            _lineItems = lineItems;
            this.DocumentType = DocumentType.ReturnsNote;
        }

        public ReturnsNoteType ReturnsNoteType { get; set; }

        public void Add(ReturnsNoteLineItem returnsNoteLineItem)
        {
            if (Status == DocumentStatus.New)
            {
                _lineItems.Add(returnsNoteLineItem);
                _AddAddLineItemCommandToExecute(returnsNoteLineItem);
            }
        }

        public void _SetLineItems(List<ReturnsNoteLineItem> lineItems)
        {
            _lineItems = lineItems;
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a returns note that is not new");
            Status = DocumentStatus.Confirmed;
          
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        

        

        public  void Close()
        {
            //if (Status != DocumentStatus.Confirmed)
              //  throw new InvalidDocumentOperationException("Returns note needs to be confirmed");
            Status = DocumentStatus.Closed;
            _AddCloseCommandToExecute();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateReturnsNoteCommand();
            coc.CommandCreatedDateTime = DateTime.Now;
            coc.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            coc.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
            coc.CommandGeneratedByUserId = DocumentIssuerUser.Id;
            coc.CommandId = Guid.NewGuid();
            coc.CommandSequence = 0;
            coc.CostCentreApplicationCommandSequenceId = 0;
            coc.Description = "";
            coc.DateReturnsNoteCreated = DocumentDateIssued;
            coc.DocIssuerUserId = DocumentIssuerUser.Id;
            coc.DocumentDateIssued = DocumentDateIssued;
            coc.DocumentId = Id;
            coc.DocumentIssuerCostCentreId = DocumentIssuerCostCentre.Id;
            coc.DocumentRecipientCostCentreId = DocumentRecipientCostCentre.Id;
            coc.DocumentReference = DocumentReference;
            coc.ExtDocumentReference = "";
            coc.ReturnsNoteTypeId = (int) ReturnsNoteType;
            
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as ReturnsNoteLineItem;

            var lic = new AddReturnsNoteLineItemCommand();
            if (item != null)
            {
                lic.Actual = item.Actual;
                lic.CommandCreatedDateTime = DateTime.Now;
                lic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
                lic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
                lic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
                lic.CommandId = Guid.NewGuid();
                lic.CommandSequence = 0;
                lic.CostCentreApplicationCommandSequenceId = 0;
                lic.Description = item.Description;
                lic.DocumentId = Id;
                lic.Expected = item.Qty;
                lic.LossType = (int) item.LossType;
                lic.LineItemId = item.Id;
                lic.ProductId = item.Product != null ? item.Product.Id:Guid.Empty;
                lic.ReturnTypeId = (int)item.ReturnType;
                lic.Value = item.Value;
                lic.Reason = item.Reason;
                _AddCommand(lic);
            }
           
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmReturnsNoteCommand(
                  Guid.NewGuid(),
                  Id,
                  DocumentIssuerUser.Id,
                  DocumentIssuerCostCentre.Id,
                  0,
                  DocumentIssuerCostCentreApplicationId);
            _AddCommand(co);
        }
        protected  void _AddCloseCommandToExecute(bool isHybrid = false)
        {
            var clo = new CloseReturnsNoteCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentParentId
               );
            _AddCommand(clo);
        }
    }
}
