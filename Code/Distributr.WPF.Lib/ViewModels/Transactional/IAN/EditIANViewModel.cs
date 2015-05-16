using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using System.Collections.Generic;
using Distributr.Core.ClientApp;

namespace Distributr.WPF.Lib.ViewModels.Transactional.IAN
{

    public class EditIANViewModel : DistributrViewModelBase
    {

        public ObservableCollection<EditIANViewModelInventoryAdustment> InventoryAdjustmentProducts { get; set; }
        private InventoryAdjustmentNote note = null;
        public bool AdjustInventory = true;

        public EditIANViewModel()
        {
            LineItems = new ObservableCollection<EditIANViewModelInventoryAdustment>();
            _LineItems = new ObservableCollection<EditIANViewModelInventoryAdustment>();
            CancelCommand = new RelayCommand(CancelIAN);
            SaveCommand = new RelayCommand(SaveIAN);
            ConfirmCommand = new RelayCommand(RunConfirmCommand);
        }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public ObservableCollection<EditIANViewModelInventoryAdustment> _LineItems { set; get; }
        public ObservableCollection<EditIANViewModelInventoryAdustment> LineItems { set; get; }

       

        public void CancelIAN() { ClearViewModel(); }


        public bool CanAdjustInventory()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                if (user != null && user.Group!=null)
                { bool canAccess=Using<IUserGroupRolesRepository>(c)
                            .GetByGroup(user.Group.Id)
                            .Any(s => s.UserRole == (int)UserRole.RoleAdjustInventory && s.CanAccess);
                    return canAccess;
                }
            }
            return false;
        }

        private void SaveIAN()
        {
            
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);

                CostCentre cc = Using<ICostCentreRepository>(c) .GetById(Using<IConfigService>(c).Load().CostCentreId);
                Guid ccAppId = Using<IConfigService>(c).Load().CostCentreApplicationId;
                InventoryAdjustmentNoteType t = AdjustInventory
                                                       ? InventoryAdjustmentNoteType.AdjustOnly
                                                       : InventoryAdjustmentNoteType.StockTake;
                note = Using<IInventoryAdjustmentNoteFactory>(c).Create(cc, ccAppId, cc, user, "", t, Guid.Empty);


                var itemsToAdd = LineItems.Select(n => Using<IInventoryAdjustmentNoteFactory>(c)
                                                           .CreateLineItem(n.ActualQty, n.Id, n.ExpectedQty, 0, n.Reason))
                                          .ToList();
                foreach (var i in itemsToAdd) { 
                    note.AddLineItem(i);
                }

                

               // note._SetLineItems(itemsToAdd);
              //Using<IMessageBoxWrapper>(c).Show("Successfully Saved", "Information", MessageBoxButton.OK);
            }
        }

        void RunConfirmCommand()
        {
            try
            {
                if (LineItems.Count < 1)
                {
                    MessageBox.Show("Please Add product to adjust inventory");
                    return;
                }
                Confirm();
                using (StructureMap.IContainer c = NestedContainer)
                {
                    MessageBoxResult result = Using<IMessageBoxWrapper>(c).Show(
                        "Successfully Confirmed. Click OK to view the Summary and Cancel to Continue with" +
                        (AdjustInventory ? " inventory adjustment" : " stock take"),
                        (AdjustInventory ? "Inventory Adjustment Information" : "Stock Take Information"),
                        MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        ViewInventoryLevels();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in the inputs", MessageBoxButton.OK);
            }
        }

        void Confirm()
        {
            DateTime start = DateTime.Now;
            
            SaveIAN();

            if (note != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    BasicConfig config = c.GetInstance<IConfigService>().Load();
                    note.Confirm();//= DocumentStatus.Confirmed;
                    Using<IInventoryAdjustmentNoteWfManager>(c).SubmitChanges(note,config);
                    ClearViewModel();
                }
            }
            else
            {
                throw new Exception("Save first ");
            }
            SavedDocumentId = note.Id;
        }
        public Guid SavedDocumentId { get; set; }
        public void ViewInventoryLevels()
        {
            ConfirmNavigatingAway = false;
            if (AdjustInventory)
                SendNavigationRequestMessage(new Uri("/Views/Reports/InventoryAdjustmentsReport.xaml?" + note.Id, UriKind.RelativeOrAbsolute));
            else
                SendNavigationRequestMessage(new Uri("/Views/Reports/StockTakeReport.xaml?" + note.Id, UriKind.RelativeOrAbsolute));
        }

        private void ClearViewModel()
        {
            LineItems.Clear();
            _LineItems.Clear();
            _productPackagingSummaryService.ClearBuffer();
        }

        public void RemoveProduct(Guid proId)
        {
            LineItems.Clear();
            _productPackagingSummaryService.RemoveProduct(proId);
            List<PackagingSummary> summaryList = _productPackagingSummaryService.GetProductSummary();
            RefreshList(summaryList);
        }

        public void sAddLineItem(Guid productId, string productdescription, decimal expected, decimal actual,
                                 string reason, bool isEdit, LineItemType lineItemType)
        {
            LineItems.Clear();
            bool isBulk = lineItemType == LineItemType.Bulk;
            using (var container = NestedContainer)
            {
                var bulkQuantity = Using<IProductPackagingSummaryService>(container).GetProductQuantityInBulk(productId);
                actual = isBulk ? actual * bulkQuantity : actual;
            }
            _productPackagingSummaryService.AddProduct(productId, actual, true, isEdit, false);
            List<PackagingSummary> summaryList = _productPackagingSummaryService.GetProductSummary();
            // List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummaryByProduct(productId, actual,false);
            //int sd = _productPackagingSummaryService.GetProductQuantityInBulk(productId);
            RefreshList(summaryList);
        }

        private void RefreshList(List<PackagingSummary> summaryList)
        {
            foreach (PackagingSummary summary in summaryList)
            {
                EditIANViewModelInventoryAdustment pr = null;

                Guid productScopeId = summaryList.FirstOrDefault(p => p.Product.Id == summary.ParentProductId) != null ? summaryList.First(p => p.Product.Id == summary.ParentProductId).Product.Id : Guid.Empty;
                pr = _LineItems.FirstOrDefault(p => p.Id == productScopeId);


                LineItems.Add(new EditIANViewModelInventoryAdustment
                                  {
                                      ActualQty = summary.Quantity,
                                      ExpectedQty = pr != null ? pr.ExpectedQty : 0,
                                      Id = summary.Product.Id,
                                      ProductName = summary.Product.Description,
                                      Reason = pr != null ? pr.Reason : "",
                                      Variance = pr != null ? pr.Variance : 0,
                                      IsEditable = summary.IsEditable,
                                    LineItemType=pr!=null?pr.LineItemType:LineItemType.Unit
                                  });
            }
            CalculateVariance();
        }

        private void CalculateVariance()
        {
            foreach (EditIANViewModelInventoryAdustment line in LineItems)
            {
                line.Variance = LineItems.Where(p => p.Id == line.Id).Sum(s => (s.ActualQty - s.ExpectedQty));
            }
        }

        public void AddLineItem(Guid productId, string productdescription, decimal expected, decimal actual, string reason,LineItemType lineItemType)
        {
            bool isBulk = lineItemType == LineItemType.Bulk;
            using (var container = NestedContainer)
            {
                var bulkQuantity =Using<IProductPackagingSummaryService>(container).GetProductQuantityInBulk(productId);
                actual=isBulk? actual*bulkQuantity:actual;
            }
            if (_LineItems.Any(p => p.Id == productId))
            {
                var liupdate = _LineItems.First(p => p.Id == productId);
                liupdate.ExpectedQty = expected;
                liupdate.ActualQty = actual;
                liupdate.Reason = reason;
                liupdate.Variance = liupdate.ActualQty - liupdate.ExpectedQty;
                liupdate.LineItemType = lineItemType;
            }
            else
            {
                EditIANViewModelInventoryAdustment li = new EditIANViewModelInventoryAdustment
                {
                    ActualQty = actual,
                    ExpectedQty = expected,
                    Id = productId,
                    ProductName = productdescription,
                    Reason = reason,
                    LineItemType = lineItemType

                };
                li.Variance = li.ActualQty - li.ExpectedQty;
                _LineItems.Add(li);
            }
        }

        #region Helper Classes
        public class EditIANViewModelInventoryAdustment : ViewModelBase
        {

            public const string IdPropertyName = "Id";
            private Guid _id = Guid.Empty;
            public Guid Id
            {
                get
                {
                    return _id;
                }

                set
                {
                    if (_id == value)
                    {
                        return;
                    }

                    var oldValue = _id;
                    _id = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);


                }
            }

            public const string ProductNamePropertyName = "ProductName";
            private string _ProductName = "";
            public string ProductName
            {
                get
                {
                    return _ProductName;
                }

                set
                {
                    if (_ProductName == value)
                    {
                        return;
                    }

                    var oldValue = _ProductName;
                    _ProductName = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(ProductNamePropertyName);


                }
            }

            public const string ExpectedQtyPropertyName = "ExpectedQty";
            private decimal _ExpectedQty = 0;
            public decimal ExpectedQty
            {
                get
                {
                    return _ExpectedQty;
                }

                set
                {
                    if (_ExpectedQty == value)
                    {
                        return;
                    }

                    var oldValue = _ExpectedQty;
                    _ExpectedQty = value;


                    // Update bindings, no broadcast
                    RaisePropertyChanged(ExpectedQtyPropertyName);


                }
            }

            public const string ActualQtyPropertyName = "ActualQty";
            private decimal _ActualQty = 0;
            public decimal ActualQty
            {
                get
                {
                    return _ActualQty;
                }

                set
                {
                    if (_ActualQty == value)
                    {
                        return;
                    }

                    var oldValue = _ActualQty;
                    _ActualQty = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(ActualQtyPropertyName);


                }
            }
            public const string ReasonPropertyName = "Reason";
            private string _reason = "";
            public string Reason
            {
                get
                {
                    return _reason;
                }

                set
                {
                    if (_reason == value)
                    {
                        return;
                    }

                    var oldValue = _reason;
                    _reason = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ReasonPropertyName);

                }
            }


            public const string VariancePropertyName = "Variance";
            private decimal _variance = 0;
            public decimal Variance
            {
                get
                {
                    return _variance;
                }

                set
                {
                    if (_variance == value)
                    {
                        return;
                    }

                    var oldValue = _variance;
                    _variance = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(VariancePropertyName);


                }
            }


            public const string IsEditablePropertyName = "IsEditable";
            private bool _IsEditable = false;
            public bool IsEditable
            {
                get
                {
                    return _IsEditable;
                }

                set
                {
                    if (_IsEditable == value)
                    {
                        return;
                    }

                    var oldValue = _IsEditable;
                    _IsEditable = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(IsEditablePropertyName);


                }
            }


            /// <summary>
            /// The <see cref="ParentProductId" /> property's name.
            /// </summary>
            public const string ParentProductIdPropertyName = "ParentProductId";

            private int _parentProductId = 0;

            /// <summary>
            /// Gets the ParentProductId property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public int ParentProductId
            {
                get
                {
                    return _parentProductId;
                }

                set
                {
                    if (_parentProductId == value)
                    {
                        return;
                    }

                    var oldValue = _parentProductId;
                    _parentProductId = value;



                    // Update bindings, no broadcast
                    RaisePropertyChanged(ParentProductIdPropertyName);


                }
            }


            public const string LineItemTypePropertyName = "LineItemType";
            private LineItemType _type = LineItemType.Unit;
            public LineItemType LineItemType
            {
                get
                {
                    return _type;
                }

                set
                {
                    if (_type == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(LineItemTypePropertyName);
                    _type = value;
                    RaisePropertyChanged(LineItemTypePropertyName);
                }
            }

        }
        #endregion


    }
}