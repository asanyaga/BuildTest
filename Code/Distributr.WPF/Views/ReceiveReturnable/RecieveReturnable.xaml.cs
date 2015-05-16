 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.DisbursementNotes;

using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.ReceiveReturnable
{
    /// <summary>
    /// Interaction logic for RecieveReturnable.xaml
    /// </summary>
    public partial class RecieveReturnable : Page
    {
        private AddReturnableModal modal;
        private IMessageSourceAccessor messageResolver;
        private RecieveReturnableViewModel _vm;
        public RecieveReturnable()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(RecieveReturnable_Loaded);
        }

        void RecieveReturnable_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as RecieveReturnableViewModel;
            _vm.Setup();
            LocalizeLabels();
        }

        private void LocalizeLabels()
        {
            messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            labeldatereceived.Content = messageResolver.GetText("sl.inventory.receive.returnable.datereceived");
            labeloutlet.Content = messageResolver.GetText("sl.inventory.receive.returnable.outlet");
            colproduct.Header = messageResolver.GetText("sl.inventory.receive.returnable.grid.col.product");
            colquantity.Header = messageResolver.GetText("sl.inventory.receive.returnable.grid.col.quantity");
            colunitprice.Header = messageResolver.GetText("sl.inventory.receive.returnable.grid.col.unitprice");
            coltotalamt.Header = messageResolver.GetText("sl.inventory.receive.returnable.grid.col.totalamount");

        }

        private void btAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.OutletLookups == null || _vm.OutletLookups.OutletId == Guid.Empty)
            {
                MessageBox.Show("Please select an" + messageResolver.GetText("sl.inventory.receive.returnable.outlet") + " form the list.", messageResolver.GetText("sl.inventory.receive.returnable.title"), MessageBoxButton.OK);
                return;
            }

            modal = new AddReturnableModal();
            modal.Closed += new EventHandler(modal_Closed);
            modal.Closing += modal_Closing;
            RecieveReturnableLineItemViewModel vmItem = modal.DataContext as RecieveReturnableLineItemViewModel;
            vmItem.SetUp();
            modal.ShowDialog();

        }

        void modal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RecieveReturnableLineItemViewModel vmItem = modal.DataContext as RecieveReturnableLineItemViewModel;
            if (vmItem.ProductLookups == null && modal.DialogResult != null && modal.DialogResult.Value)
            {
                MessageBox.Show("Select Returnable Product");
                modal.DialogResult = false;
            }
            else if (vmItem.Quantity == 0 && modal.DialogResult.Value)
            {
                MessageBox.Show("Enter Valid Quantity");

            }
        }

        void modal_Closed(object sender, EventArgs e)
        {
            RecieveReturnableLineItemViewModel vmItem = modal.DataContext as RecieveReturnableLineItemViewModel;
            bool result = modal.DialogResult.Value;
            if (result)
            {

                _vm.AddLineItems(vmItem.ProductLookups, vmItem.Quantity, vmItem.IsEditable);
            }
            result = false;
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsValid())
            {
                _vm.SaveCommand.Execute(null);
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult retult = MessageBox.Show("Are you sure you want to cancel receiveing returnables?",
                                                      "Recieve Returnables", MessageBoxButton.OKCancel);
            if (retult == MessageBoxResult.OK)
            {
                _vm.ClearAll();
            }
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            modal = new AddReturnableModal();
            modal.Closed += new EventHandler(modal_Closed);
            modal.Closing += modal_Closing;
            
            RecieveReturnableLineItemViewModel vmItem = modal.DataContext as RecieveReturnableLineItemViewModel;
            vmItem.SetUp();
            Hyperlink hl = sender as Hyperlink;

            Guid product = (Guid)hl.Tag;
            _vm = DataContext as RecieveReturnableViewModel;
            vmItem.EditLoad(_vm.ReturnableItems.First(p => p.ProductId == product));
            modal.ShowDialog();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {

            Hyperlink hl = sender as Hyperlink;
            Guid product = (Guid)hl.Tag;
            _vm = DataContext as RecieveReturnableViewModel;
            MessageBoxResult rs = MessageBox.Show("Are you sure you want to delete returnable entry",
                                                  "Distributr: Recieve returnables", MessageBoxButton.OKCancel);
            if (rs == MessageBoxResult.OK)
            {
                _vm.DeleteReturns(product);
            }
        }
    }
}
