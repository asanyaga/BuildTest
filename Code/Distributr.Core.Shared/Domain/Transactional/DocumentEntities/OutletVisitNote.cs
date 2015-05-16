using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class OutletVisitNote : Document
    {
        public OutletVisitNote(Guid id)
            : base(id)
        {
         
        }

       

        public string Description { get; set; }

       

        private List<ReceiptLineItem> _lineItems;
        public List<ReceiptLineItem> LineItems
        {
            get { return _lineItems/*.Where(n => n.LineItemType == OrderLineItemType.PostConfirmation).ToList()*/; }
        }

        public decimal Total
        {
            get
            {
                return LineItems.Where(n => n.LineItemType == OrderLineItemType.PostConfirmation).Sum(n => n.Value);
            }
        }

        public Guid InvoiceId { get; set; }

        public override void Confirm()
        {
               Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            
        }





        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var ic = new CreateOutletVisitNoteCommand();
            ic.CommandId = Guid.NewGuid();
            ic.DocumentId = Id;
            ic.DocIssuerUserId = DocumentIssuerUser.Id;
            ic.DocumentIssuerCostCentreId = DocumentIssuerCostCentre.Id;

            ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            ic.DocumentReference = DocumentReference;
            ic.DocumentDateIssued = DocumentDateIssued;
            ic.Note = Description;
            ic.DocumentOnBehalfCostCentreId = DocumentRecipientCostCentre.Id;


            ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(ic);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
          
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
           
        }

       
    }
}
