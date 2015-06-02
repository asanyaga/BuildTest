using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
   public class CommodityStorageNote : SourcingDocument
    {
       public CommodityStorageNote(Guid id)
           : base(id, DocumentType.CommodityStorageNote)
       {
           _lineItems = new List<CommodityStorageLineItem>();
       }

       public void AddLineItem(CommodityStorageLineItem lineItem)
       {
           if (Status != DocumentSourcingStatus.New)
               return;
           _lineItems.Add(lineItem);
           _AddAddLineItemCommandToExecute(lineItem);
       }


       public decimal TotalNetWeight
       {
           get { return LineItems.Sum(n => n.Weight); }
       }

       private List<CommodityStorageLineItem> _lineItems;

       public List<CommodityStorageLineItem> LineItems
       {
           get { return _lineItems; }

       }
       public void _SetLineItems(List<CommodityStorageLineItem> items)
       {
           _lineItems = items;
       }
       public override void Confirm()
       {
           if (Status != DocumentSourcingStatus.New)
               throw new InvalidDocumentOperationException("Cannot confirm an commodity storage note that is not new");
           else
               Status = DocumentSourcingStatus.Confirmed;
           _AddCreateCommandToExecute();
           _AddConfirmCommandToExecute();
       }

       public override void Approve()
       {
           throw new NotImplementedException();
       }

       public override void Close()
       {
           throw new NotImplementedException();
       }

       protected override void _AddCreateCommandToExecute(bool isHybrid = false)
       {
           var command =
               new CreateCommodityStorageCommand(
                   Guid.NewGuid(),
                   Id,
                   DocumentIssuerUser.Id,
                   DocumentIssuerCostCentre.Id,
                   0,
                   DocumentIssuerCostCentreApplicationId,
                   DocumentParentId,
                   DocumentRecipientCostCentre.Id,
                   Note,
                   DocumentReference,
                   Description,
                   DocumentDateIssued,
                   DocumentDate,
                   DocumentIssuerCostCentre.Id,
                   DocumentIssuerUser.Id
                   );
           command.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
           _AddCommand(command);
       }

       protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
       {
           var item = lineItem as CommodityStorageLineItem;
           var command =
               new AddCommodityStorageLineItemCommand
                   (Guid.NewGuid(),
                    Id,
                    item.Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    DocumentParentId,
                    item.ParentLineItemId,
                    item.Commodity.Id,
                    item.CommodityGrade.Id,
                    Guid.Empty,
                    item.ContainerNo,
                    item.Weight,
                    item.Description,
                    item.Note
                   );
           _AddCommand(command);
       }

       protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
       {
           var command = new ConfirmCommodityStorageCommand
               (Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId
               );
           _AddCommand(command);
       }

    }
}
