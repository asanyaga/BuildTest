using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts
{
    public class DispatchProductsViewModel : DistributrViewModelBase
    {

        public DispatchProductsViewModel()
        {
            LineItems = new ObservableCollection<DispatchProductLineItem>();
        }

        public ObservableCollection<DispatchProductLineItem> LineItems { get; set; }



        public void AddorUpdateLineItem(Guid productId, string productDescription, string reason, string otherReason, decimal qty, bool isEdited, int sequenceId = 0)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (isEdited)
                {
                    UpdateLineItem(sequenceId, qty, reason, otherReason);
                    return;
                }

                //get a product and its returnable
                ReturnableProduct returnable = null;
                Product product = Using<IProductRepository>(c).GetById(productId);
                if (product is SaleProduct)
                {
                    returnable = (product as SaleProduct).ReturnableProduct;
                }

                InsertLineItem(product, reason, otherReason, qty, product.Id == productId);

                if (returnable != null)
                {
                    InsertLineItem(returnable, reason, otherReason, qty, false);
                }
            }
        }

        private void InsertLineItem(Product product, string reason, string otherReason, decimal qty, bool isEditable)
        {
            var existing = LineItems.FirstOrDefault(n => n.ProductId == product.Id && n.Reason == reason && n.OtherReason == otherReason && n.IsEditable == isEditable);
            if (existing == null)
            {
                LineItems.Add(new DispatchProductLineItem
                {
                    SequenceId = LineItems.Count() + 1,
                    ProductId = product.Id,
                    ProductDesc = product.Description,
                    ProductType = product.GetType().ToString().Split('.').Last(),
                    Reason = reason,
                    OtherReason = otherReason,
                    Qty = qty,
                    IsEditable = isEditable
                });
            }
            else
            {
                existing.Qty += qty;
            }
        }

        private void UpdateLineItem(int sequenceId, decimal qty, string reason, string otherReason)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var existing = LineItems.FirstOrDefault(n => n.SequenceId == sequenceId);
                if (existing != null)
                {
                    Product product = Using<IProductRepository>(c).GetById(existing.ProductId);
                    ReturnableProduct returnable = null;
                    if (product is SaleProduct)
                    {
                        if ((product as SaleProduct).ReturnableProduct != null)
                        {
                            returnable = (product as SaleProduct).ReturnableProduct;
                            var myRet =
                                LineItems.FirstOrDefault(
                                    n =>
                                    n.ProductId == returnable.Id && !n.IsEditable && n.Reason == existing.Reason &&
                                    n.OtherReason == existing.OtherReason);

                            myRet.OtherReason = otherReason;
                            myRet.Reason = reason;
                            myRet.Qty = qty;
                        }
                    }

                    existing.OtherReason = otherReason;
                    existing.Reason = reason;
                    existing.Qty = qty;
                }
            }
        }

        public void RemoveProduct(int sequenceId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var toRemove = new List<DispatchProductLineItem>();

                var target = LineItems.FirstOrDefault(n => n.SequenceId == sequenceId);
                Product product = null;
                ReturnableProduct returnable = null;
                if (target != null)
                {
                    product = Using<IProductRepository>(c).GetById(target.ProductId);

                    if (product is SaleProduct)
                    {
                        if (((SaleProduct)product).ReturnableProduct != null)
                        {
                            returnable = (product as SaleProduct).ReturnableProduct;
                        }
                    }

                    if (returnable != null)
                    {
                        var retLi =
                            LineItems.FirstOrDefault(
                                n =>
                                n.ProductId == returnable.Id && n.Reason == target.Reason &&
                                n.OtherReason == target.OtherReason && !n.IsEditable);
                        if (retLi != null)
                        {
                            if (retLi.Qty == target.Qty)
                                toRemove.Add(retLi);
                            else
                            {
                                retLi.Qty -= target.Qty;
                            }
                        }
                    }

                    toRemove.Add(target);
                }

                //who knew it'd be this complicated??
                string prods = toRemove.Aggregate("Are you sure you want to remove the following products;\n",
                                                  (current, prod) =>
                                                  current + ("\t- " + prod.ProductDesc + ":  Qty = " + prod.Qty + "\n"));
                if (MessageBox.Show(prods, "Distributr: Remove Items to Dispatch", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.OK)
                {
                    toRemove.ForEach(n => LineItems.Remove(n));
                }

            }
        }

        public void ClearAndSetUp()
        {
            Clear();
        }

        private void Clear()
        {
            LineItems.Clear();
            _productPackagingSummaryService.ClearBuffer();
        }

        public void Confirm()
        {
            ReturnsNote rn = CreateReturnsNote();
            using (StructureMap.IContainer c = NestedContainer)
            {
                BasicConfig config = c.GetInstance<IConfigService>().Load();
                Using<IConfirmReturnsNoteWFManager>(c).SubmitChanges(rn,config);
                Using<IInventoryAdjustmentNoteWfManager>(c).SubmitChanges(CreateDistributrIAN(rn),config);
            }
        }

        ReturnsNote CreateReturnsNote()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var thisCostCentre = Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId);
                var thisUser = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                ReturnsNote rn = new ReturnsNote(Guid.NewGuid())
                    {
                        DocumentDateIssued = DateTime.Now,
                        DocumentIssuerCostCentre = thisCostCentre,
                        DocumentIssuerCostCentreApplicationId = Using<IConfigService>(c).Load().CostCentreApplicationId,
                        DocumentIssuerUser = thisUser,
                        DocumentRecipientCostCentre = thisCostCentre,
                        DocumentReference =
                        Using<IGetDocumentReference>(c).GetDocReference("RN", thisUser.Id, thisCostCentre.Id),
                        DocumentType = DocumentType.ReturnsNote,
                        Status = DocumentStatus.New,
                        ReturnsNoteType = ReturnsNoteType.DistributorToHQ
                    };
                foreach (DispatchProductLineItem item in LineItems)
                {
                    var li = new ReturnsNoteLineItem(Guid.NewGuid())
                        {
                            Actual = item.Qty,
                            Qty = item.Qty,
                            Description = item.ProductDesc,
                            Product = Using<IProductRepository>(c).GetById(item.ProductId),
                            ReturnType = ReturnsType.Inventory,
                            LineItemSequenceNo = rn._lineItems.Count + 1,
                            IsNew = true,
                            Reason = item.Reason,
                            Other = item.OtherReason
                        };
                    rn.Add(li);
                }

                return rn;
            }
        }
        private InventoryAdjustmentNote CreateDistributrIAN(ReturnsNote returnnote)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Guid appId = Using<IConfigService>(c).Load().CostCentreApplicationId;
                InventoryAdjustmentNote note =
                    Using<IInventoryAdjustmentNoteFactory>(c)
                        .Create(returnnote.DocumentIssuerCostCentre, appId, returnnote.DocumentRecipientCostCentre,
                                returnnote.DocumentIssuerUser, "", InventoryAdjustmentNoteType.Available, Guid.Empty);
                var ListianLineitem = new List<InventoryAdjustmentNoteLineItem>();
                foreach (var item in returnnote._lineItems)
                {
                    if (item.ReturnType != ReturnsType.Inventory && item.Product == null) continue;
                    var inventory = Using<IInventoryRepository>(c).GetByProductIdAndWarehouseId(item.Product.Id,
                                                                     returnnote.DocumentRecipientCostCentre.Id);
                    decimal expected = inventory != null ? inventory.Balance : 0;
                    var ianLineitem = Using<IInventoryAdjustmentNoteFactory>(c)
                        .CreateLineItem(expected, item.Product.Id, expected + item.Qty, 0, item.Reason);
                    ListianLineitem.Add(ianLineitem);
                }
                foreach (var i in ListianLineitem)
                {
                    note.AddLineItem(i);
                }
                //note._SetLineItems(ListianLineitem);
                note.Confirm();
                return note;
            }
        }
        //void AdjustInventory(ReturnsNote rn)
        //{
        //    using (StructureMap.IContainer c = NestedContainer)
        //    {
        //        foreach (DispatchProductLineItem item in LineItems)
        //        {
        //     Using<IInventoryWorkflow>(c)  .InventoryAdjust(rn.DocumentIssuerCostCentre.Id, item.ProductId, -item.Qty,
        //                                               DocumentType.ReturnsNote, rn.Id, DateTime.Now,
        //                                               InventoryAdjustmentNoteType.Available);
        //        }
        //    }
        //}
    }

    public class DispatchProductLineItem
    {
        public int SequenceId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductDesc { get; set; }
        public string Reason { get; set; }
        public string OtherReason { get; set; }
        public string ProductType { get; set; }
        public decimal Qty { get; set; }
        public bool IsEditable { get; set; }
    }
}
