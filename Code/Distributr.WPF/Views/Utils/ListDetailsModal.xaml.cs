using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for ListDetailsModal.xaml
    /// </summary>
    public partial class ListDetailsModal : Window
    {
        public ObservableCollection<LineItem> ListItems { get; set; }
        public string Msg { get; set; }
        private bool IsInitialised = false;
        public ListDetailsModal()
        {
            InitializeComponent();
            IsInitialised = true;
            this.Loaded += new RoutedEventHandler(ListDetailsModal_Loaded);
            ListItems = new ObservableCollection<LineItem>();

        }

        void ListDetailsModal_Loaded(object sender, RoutedEventArgs e)
        {
            
            dgListDetails.ItemsSource = ListItems;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInitialised) return;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInitialised) return;
            this.DialogResult = false;
        }

        private void hlMoreDetails_Click(object sender, RoutedEventArgs e)
        {
            SOLineItemViewModel vm = ViewModelLocator.SalesmanOrderLineItemViewModelStatic;
            var hl = sender as Hyperlink;
            var prodId = (Guid)hl.Tag;
            Msg = "No more available details about this item";
            string detail = "";
            string header = "Further Details";
            var ls = ListItems.First(n => n.ProductId == prodId);
            if (ls != null)
            {
                header = ls.ProductDesc + ": " + ls.ProductType;
            }

            vm.BindTree(prodId);
            if (vm.ProductTree != null)
            {
                foreach (var item in vm.ProductTree)
                {
                    foreach (var child in item.ChildNodes)
                    {
                        detail += "Product: " + child.Entity.Description + ";  Qty:  " +
                                  child.Entity.QtyPerConsolidatedProduct + "\n";
                    }
                }
                Msg = detail;
            }
            MessageBox.Show(Msg, "Distributr: " + header, MessageBoxButton.OK);
        }
    }
}
