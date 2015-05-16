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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryViewModels;

namespace Distributr.WPF.UI.Views.ReconcileGenerics
{
    public partial class ReconcileGenerics : Page
    {
        ListInventoryViewModel vm;
        public ReconcileGenerics()
        {
            InitializeComponent();
            vm = DataContext as ListInventoryViewModel;

        }

        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbReturnables.SelectedItem != null)
                {
                    if (isValid(vm.ReturnableQuantity.ToString()))
                    {
                        if (vm.ReturnableQuantity <=vm.GenericQuantity)
                        {
                            var x = (Product)cmbReturnables.SelectedItem;
                            vm.AddLineItem(x.Id, x.Description,vm.ReturnableQuantity);
                            vm.GenericQuantity -= vm.ReturnableQuantity;
                            vm.SelectedProduct = null;
                        }
                        else
                            MessageBox.Show("Returnable quantity cannot be greater than the Generic quantity");
                    }
                    else
                        MessageBox.Show("Returnable quantity should be numeric");
                }
                else
                    MessageBox.Show("Please select a returnable product");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vm.LineItems.Count <= 0)
                {
                    MessageBox.Show("Add returnables to reconcile");
                    return;
                }
                vm.ConfirmCommand.Execute(null);
                MessageBox.Show("Reconcile Generics Successful!");
                NavigationService.Navigate(new Uri("/views/ReconcileGenerics/ListGenericInventory.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.CleanUpCommand.Execute(null);
                NavigationService.Navigate(new Uri("/views/ReconcileGenerics/ListGenericInventory.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool isValid(string str)
        {
            try
            {
                int.Parse(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void hlView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.RemoveLineItem(Guid.Parse(((Hyperlink)sender).Tag.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
