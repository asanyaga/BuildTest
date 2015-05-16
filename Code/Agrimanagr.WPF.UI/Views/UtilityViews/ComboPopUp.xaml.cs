
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;

using Distributr.WPF.Lib.ViewModels.Utils;

namespace Agrimanagr.WPF.UI.Views.UtilityViews
{
    public partial class ComboPopUp : Window,IAgrimanagrComboPopUp
    {
        private ComboPopUpViewModel _vm;
        bool isInitialized = false;
       ComboBox _sender = null;
        private IEnumerable<object> enumerable;
        private Type objType = null;

        public ComboPopUp()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            _vm = this.DataContext as ComboPopUpViewModel;
            _vm.ClearAndSetUp();
            this.Closing += ComboPopUp_Closing;
        }
        public object ShowDlg(object sender)
        {
          object  _sender = (ComboBox)sender;
            _sender = sender;
            if (_sender != null)
            {
               if (_sender.GetType() == typeof(ComboBox))
                {
                  
                    ((ComboBox)_sender).IsDropDownOpen = false;
                   
                }
                Helper((ComboBox)_sender);
                return Return(objType, enumerable.ToArray());
            }
            return null;
        }

        private void Helper(ComboBox comboBox)
        {
            var items = (comboBox).Items.Cast<object>();
            enumerable = items as object[] ?? items.ToArray();
            if (enumerable.Any())
            {
                List<ComboPopUpItem> comboItems = null;
                objType = enumerable.FirstOrDefault().GetType();
                if (objType == typeof (User))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((User) n).Id,
                                                                Code = "",
                                                                Name = ((User) n).Username
                                                            }).ToList();
                }
                else if (objType == typeof (Route))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Route) n).Id,
                                                                Code = ((Route) n).Code,
                                                                Name = ((Route) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (Outlet))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Outlet) n).Id,
                                                                Code = ((Outlet) n).CostCentreCode,
                                                                Name = ((Outlet) n).Name
                                                            }).ToList();
                }
                
                else if (objType == typeof (IANLineItemProductLookupItem))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((IANLineItemProductLookupItem) n).ProductId,
                                                                Code = ((IANLineItemProductLookupItem) n).ProductCode,
                                                                Name = ((IANLineItemProductLookupItem) n).ProductDesc
                                                            }).ToList();
                }
                else if (objType == typeof (ItnLineItemProductLookupItem))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((ItnLineItemProductLookupItem) n).ProductId,
                                                                Code = ((ItnLineItemProductLookupItem) n).ProductCode,
                                                                Name = ((ItnLineItemProductLookupItem) n).ProductDesc
                                                            }).ToList();
                }
                else if (objType == typeof (ProductLookupItem))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((ProductLookupItem) n).Id,
                                                                Code = ((ProductLookupItem) n).Code,
                                                                Name = ((ProductLookupItem) n).Description
                                                            }).ToList();
                }
                else if (objType == typeof (GRNLineItemProductLookupItem))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((GRNLineItemProductLookupItem) n).ProductId,
                                                                Code = ((GRNLineItemProductLookupItem) n).ProductCode,
                                                                Name = ((GRNLineItemProductLookupItem) n).ProductDesc
                                                            }).ToList();
                }
                else if (objType == typeof (Commodity))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Commodity) n).Id,
                                                                Code = ((Commodity) n).Code,
                                                                Name = ((Commodity) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (Bank))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Bank) n).Id,
                                                                Code = ((Bank) n).Code,
                                                                Name = ((Bank) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (BankBranch))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((BankBranch) n).Id,
                                                                Code = ((BankBranch) n).Code,
                                                                Name = ((BankBranch) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (Centre))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Centre) n).Id,
                                                                Code = ((Centre) n).Code,
                                                                Name = ((Centre) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (CommodityProducer))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((CommodityProducer) n).Id,
                                                                Code = ((CommodityProducer) n).Code,
                                                                Name = ((CommodityProducer) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (CommodityGrade))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((CommodityGrade) n).Id,
                                                                Code = ((CommodityGrade) n).Code,
                                                                Name = ((CommodityGrade) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (ContainerType) || objType.BaseType == typeof (ContainerType))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((ContainerType) n).Id,
                                                                Code = ((ContainerType) n).Code,
                                                                Name = ((ContainerType) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (Equipment) || objType.BaseType == typeof (Equipment))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Equipment) n).Id,
                                                                Code = ((Equipment) n).Code,
                                                                Name = ((Equipment) n).Name
                                                            }).ToList();
                }
                else if (objType == typeof (Store))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = ((Store) n).Id,
                                                                Code = ((Store) n).CostCentreCode,
                                                                Name = ((Store) n).Name
                                                            }).ToList();
                }
                    //TODO:Temporary objects
                else if (objType == typeof (LocalEquipmentConfig))
                {
                    comboItems = enumerable.Select(n => new ComboPopUpItem
                                                            {
                                                                Id = Guid.NewGuid(),
                                                                Code = ((LocalEquipmentConfig) n).Code,
                                                                Name = ((LocalEquipmentConfig) n).Name
                                                            }).ToList();
                }

                if (comboItems != null)
                {
                    comboItems = comboItems.Where(n => n.Id != Guid.Empty).ToList();
                    foreach (var item in comboItems.Where(n => n.Code == null || n.Name == null))
                    {
                        if (item.Name == null)
                            item.Name = "";
                        if (item.Code == null)
                            item.Code = "";
                    }
                    _vm.ComboPopUpItemList = comboItems;
                    _vm.CodeVisible = comboItems.Any(n => !string.IsNullOrEmpty(n.Code));
                    _vm.Search();

                    HideOrUnhide();

                    ShowDialog();
                }
            }
        }

        public bool ShowDlg(object sender, out object selected)
        {
            _sender = (ComboBox)sender;
            if (_sender != null)
            {
                if (_sender.GetType() == typeof(ComboBox))
                {
                    //(_sender).IsEnabled = false;
                    (_sender).IsDropDownOpen = false;
                }
                Helper(_sender);
                selected = Return(objType, enumerable.ToArray());
                return (bool)this.DialogResult;
            }
            ComboPopUp_Closing(this, null);
            try
            {
                selected = _sender.Items.GetItemAt(0);
                return false;
            }
            catch
            {
                selected = null;
                return false;
            }
        }

        object Return(Type objType, object[] enumerable)
        {
            if(this.DialogResult == false || _vm.SelectedItem==null)
            {
                try
                {
                    return _sender.Items.GetItemAt(0);
                }
                catch
                {
                    return null;
                }
            }
            if(objType.BaseType == typeof(MasterEntity) || objType.BaseType == typeof(Equipment))
            {
                return enumerable.FirstOrDefault(n => ((MasterEntity)n).Id == _vm.SelectedItem.Id);
            }
            if (objType == typeof(User))
            {
                return enumerable.FirstOrDefault(n => ((User)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(Route))
            {
                return enumerable.FirstOrDefault(n => ((Route)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(Outlet))
            {
                return enumerable.FirstOrDefault(n => ((Outlet)n).Id == _vm.SelectedItem.Id);
            }
            
            else if (objType == typeof(IANLineItemProductLookupItem))
            {
                return enumerable.FirstOrDefault(n => ((IANLineItemProductLookupItem) n).ProductId == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(ItnLineItemProductLookupItem))
            {
                return enumerable.FirstOrDefault(n => ((ItnLineItemProductLookupItem)n).ProductId == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(ProductLookupItem))
            {
                return enumerable.FirstOrDefault(n => ((ProductLookupItem)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(GRNLineItemProductLookupItem))
            {
                return enumerable.FirstOrDefault(n => ((GRNLineItemProductLookupItem)n).ProductId == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(Store))
            {
                return enumerable.FirstOrDefault(n => ((Store)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(Bank))
            {
                return enumerable.FirstOrDefault(n => ((Bank)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(BankBranch))
            {
                return enumerable.FirstOrDefault(n => ((BankBranch)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(LocalEquipmentConfig))
            {
                return enumerable.FirstOrDefault(n => ((LocalEquipmentConfig)n).Code == _vm.SelectedItem.Code);
            }
            else if (objType == typeof(ContainerType))
            {
                return enumerable.FirstOrDefault(n => ((ContainerType)n).Id == _vm.SelectedItem.Id);
            }
            else
            {
                throw new Exception("Combopopup return type " + objType.ToString() + " not mapped.");
            }
        }

        void ComboPopUp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_sender != null)
                if (_sender.GetType() == typeof(ComboBox))
                    ((ComboBox)_sender).IsEnabled = true;
        }

        void HideOrUnhide()
        {
            foreach (DataGridColumn dgColumn in dgItems.Columns)
            {
                if (dgColumn.Header.ToString() == "Code")
                {
                    if (_vm.CodeVisible)
                        dgColumn.Visibility = System.Windows.Visibility.Visible;
                    else
                        dgColumn.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm.SearchText = ((TextBox)sender).Text.Trim();
            _vm.Search();
        }

        private void dgItems_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OKButton_Click(sender, e);
        }

        private void OKButton_Click(object sender, MouseButtonEventArgs e)
        {

        }

       
    }
}

