using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Utils;


namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for ComboPopUp.xaml
    /// </summary>
    public partial class ComboPopUp : Window, IComboPopUp
    {
        private ComboPopUpViewModel _vm;
        bool isInitialized = false;
        object _sender = null;

        public ComboPopUp()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            _vm = this.DataContext as ComboPopUpViewModel;
            _vm.ClearAndSetUp();
            _vm.RequestClose += (s, e) => this.Close();
            this.Closing += ComboPopUp_Closing;
        }
       
         public object ShowDlg(IEnumerable<object> objects )
         {
             var enumerable = objects as object[] ?? objects.ToArray();
            
             var firstItem  = enumerable.FirstOrDefault();
             if (firstItem == null)
                 return null;
             Type objType = firstItem.GetType();
             var comboObjects = GetComboPopUpItems(objType, enumerable);
             if (comboObjects != null)
             {
                 _vm.ComboPopUpItemList = comboObjects.Any(p => p.Name.Equals("--Select Payment Instrument--"))
                                              ? comboObjects.Where(p => !p.Name.Equals("--Select Payment Instrument--"))
                                                    .ToList()
                                              : comboObjects.Where(n => n.Id != Guid.Empty).ToList();

                 _vm.CodeVisible = comboObjects.Any(n => !string.IsNullOrEmpty(n.Code));
                 _vm.Search();

                 HideOrUnhide();
                 this.Topmost = true;
                 ShowDialog();

                 return Return(objType, enumerable);
             }
             return null;
         }

        public object ShowDlg(object sender)
        {
            _sender = sender;
            if (_sender != null)
            {
                if (_sender.GetType() == typeof (ComboBox))
                {
                    ComboBox selectedcombo = (ComboBox) _sender;
                    //this = selectedcombo.Parent;
                    ((ComboBox) _sender).IsDropDownOpen = false;
                  //  ((ComboBox) _sender).IsEnabled = false;
                }
            }

            var items = ((ComboBox) sender).Items.Cast<object>();
            var enumerable = items as object[] ?? items.ToArray();
            List<ComboPopUpItem> comboItems = new List<ComboPopUpItem>();
            if (enumerable.Any())
            {
               
                Type objType = enumerable.FirstOrDefault().GetType();
                comboItems = GetComboPopUpItems(objType, enumerable);

                             
                if (comboItems != null)
                {
                    _vm.ComboPopUpItemList = comboItems.Any(p => p.Name.Equals("--Select Payment Instrument--"))
                                                 ? comboItems.Where(p => !p.Name.Equals("--Select Payment Instrument--"))
                                                       .ToList()
                                                 : comboItems.Where(n => n.Id != Guid.Empty).ToList();

                    _vm.CodeVisible = comboItems.Any(n => !string.IsNullOrEmpty(n.Code));
                    _vm.Search();

                    HideOrUnhide();
                    this.Topmost=true;
                    ShowDialog();

                    return Return(objType, enumerable);
                }
            }
            return null;
        }

        private static List<ComboPopUpItem> GetComboPopUpItems(Type objType, object[] enumerable)
        {
            var comboItems=new List<ComboPopUpItem>();
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
            
          
            else if (objType == typeof (PaymentInstrumentLookup))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            //Id =Guid.Parse(((PaymentInstrumentLookup)n).AccountId),
                                                            Code = ((PaymentInstrumentLookup) n).Type,
                                                            Name = ((PaymentInstrumentLookup) n).Name
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
            else if (objType == typeof (Bank))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((Bank) n).Id,
                                                            Code = ((Bank) n).Code,
                                                            Name = ((Bank) n).Description
                                                        }).ToList();
            }
            else if (objType == typeof (BankBranch))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((BankBranch) n).Id,
                                                            Code = ((BankBranch) n).Code,
                                                            Name = ((BankBranch) n).Description
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
            else if (objType == typeof (Equipment))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((Equipment) n).Id,
                                                            Code = ((Equipment) n).Code,
                                                            Name = ((Equipment) n).Description
                                                        }).ToList();
            }
            else if (objType == typeof (ShipToAddress))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((ShipToAddress) n).Id,
                                                            Code = ((ShipToAddress) n).Description,
                                                            Name = ((ShipToAddress) n).Name
                                                        }).ToList();
            }
            else if (objType == typeof (DistributorSalesman))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((DistributorSalesman) n).Id,
                                                            Code = ((DistributorSalesman) n).CostCentreCode,
                                                            Name = ((DistributorSalesman) n).Name
                                                        }).ToList();
            }
            else if (objType == typeof (Product) || objType == typeof (SaleProduct))
            {
                comboItems = enumerable.Select(n => new ComboPopUpItem
                                                        {
                                                            Id = ((Product) n).Id,
                                                            Code = ((Product) n).ProductCode,
                                                            Name = ((Product) n).Description
                                                        }).ToList();
            }
            return comboItems;
        }

       
        object Return(Type objType, object[] enumerable)
        {
            if(_vm.SelectedItem==null)
            {
                return null;
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
            else if (objType == typeof(ShipToAddress))
            {
                return enumerable.FirstOrDefault(n => ((ShipToAddress)n).Id == _vm.SelectedItem.Id);
            }
          
            else if (objType == typeof(IANLineItemProductLookupItem))
            {
                return enumerable.FirstOrDefault(n => ((IANLineItemProductLookupItem) n).ProductId == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(PaymentInstrumentLookup))
            {
                return enumerable
                    .FirstOrDefault(n => ((PaymentInstrumentLookup)n)
                        .Name == _vm.SelectedItem.Name);
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
            else if (objType == typeof(DistributorSalesman))
            {
                return enumerable.FirstOrDefault(n => ((DistributorSalesman)n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(Product) || objType == typeof(SaleProduct))
            {
                return enumerable.FirstOrDefault(n => ((Product)n).Id == _vm.SelectedItem.Id);
            }
            else if(objType == typeof(Bank))
            {
                return enumerable.FirstOrDefault(n => ((Bank) n).Id == _vm.SelectedItem.Id);
            }
            else if (objType == typeof(BankBranch))
            {
                return enumerable.FirstOrDefault(n => ((BankBranch)n).Id == _vm.SelectedItem.Id);
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
                        dgColumn.Visibility = Visibility.Visible;
                    else
                        dgColumn.Visibility =Visibility.Collapsed;
                    break;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

       
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm.SearchText = ((TextBox)sender).Text.Trim();
            _vm.Search();
        }

        
    }
}
