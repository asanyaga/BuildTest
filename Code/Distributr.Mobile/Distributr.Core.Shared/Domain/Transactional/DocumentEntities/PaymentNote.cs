using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{       public enum PaymentNoteType{Availabe=1,Unavailable=2, Returns=3}
        public class PaymentNote : Document
        {
            public PaymentNote(Guid id)
                : base(id)
            {
                _lineItem = new List<PaymentNoteLineItem>();
            }

            public PaymentNote(Guid id, string documentReference,
                CostCentre documentIssuerCostCentre,
                Guid documentIssueCostCentreApplicationId,
                User documentIssuerUser,
                DateTime documentDateIssued,
               CostCentre documentRecipientCostCentre,
                DocumentStatus status,
                List<PaymentNoteLineItem> lineItems
                )
                : base(
                id,
                documentReference,
                documentIssuerCostCentre,
                documentIssueCostCentreApplicationId,
                documentIssuerUser, documentDateIssued,
                documentRecipientCostCentre,
                status)
            {
                _lineItem = lineItems;
                this.DocumentType = DocumentType.PaymentNote;
            }

            public void AddLineItem(PaymentNoteLineItem lossLineItem)
            {
                if (Status != DocumentStatus.New)
                    throw new InvalidDocumentOperationException("Cannot add line items to a Payment note that is not new");
                lossLineItem.IsNew = true;
                _lineItem.Add(lossLineItem);
                _AddAddLineItemCommandToExecute(lossLineItem);
            }

            public List<PaymentNoteLineItem> LineItems
            {
                get { return _lineItem; }

            }
            private List<PaymentNoteLineItem> _lineItem { get; set; }

            public void _SetLineItems(List<PaymentNoteLineItem> items)
            {
                _lineItem = items;
            }

            public PaymentNoteType PaymentNoteType { get; set; }

            public override void Confirm()
            {
                if (Status != DocumentStatus.New)
                    throw new InvalidDocumentOperationException("Cannot confirm a Payment note that is not new");
                Status = DocumentStatus.Confirmed;
                _AddCreateCommandToExecute();
                _AddConfirmCommandToExecute();
            }









            protected override void _AddCreateCommandToExecute(bool isHybrid = false)
            {
                var coc = new CreatePaymentNoteCommand();
                coc.CommandCreatedDateTime = DateTime.Now;
                coc.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
                coc.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
                coc.CommandGeneratedByUserId = DocumentIssuerUser.Id;
                coc.CommandId = Guid.NewGuid();
                coc.CommandSequence = 0;
                coc.CostCentreApplicationCommandSequenceId = 0;
                coc.Description = "";
                coc.DocumentDateIssued = DocumentDateIssued;
                coc.DocIssuerUserId = DocumentIssuerUser.Id;
                coc.DocumentDateIssued = DocumentDateIssued;
                coc.DocumentId = Id;
                coc.DocumentIssuerCostCentreId = DocumentIssuerCostCentre.Id;
                coc.PaymentNoteRecipientCostCentreId = DocumentRecipientCostCentre.Id;
                coc.DocumentReference = DocumentReference;
                coc.ExtDocumentReference = "";
                coc.SendDateTime = SendDateTime;
                coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                _AddCommand(coc);
            }

            protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
            {
                var item = lineItem as PaymentNoteLineItem;

                var lic = new AddPaymentNoteLineItemCommand();
                if (item != null)
                {
                    lic.Amount = item.Amount;
                    lic.CommandCreatedDateTime = DateTime.Now;
                    lic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
                    lic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
                    lic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
                    lic.CommandId = Guid.NewGuid();
                    lic.CommandSequence = 0;
                    lic.CostCentreApplicationCommandSequenceId = 0;
                    lic.Description = item.Description;
                    lic.DocumentId = Id;
                    lic.SendDateTime = SendDateTime;
                    _AddCommand(lic);
                }
            }


            protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
            {
                var co = new ConfirmPaymentNoteCommand(
                   Guid.NewGuid(),
                   Id,
                   DocumentIssuerUser.Id,
                   DocumentIssuerCostCentre.Id,
                   0,
                   DocumentIssuerCostCentreApplicationId);
                _AddCommand(co);
            }
        }
    
}
