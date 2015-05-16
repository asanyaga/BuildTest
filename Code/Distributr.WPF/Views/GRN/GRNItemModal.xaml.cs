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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.GRN
{
    public partial class GRNItemModal : Window, IGrnPopUp
    {
        private GRNItemModalViewModel _vm;
      
        public GRNItemModal()
        {
           InitializeComponent();
             LocalizeLabel();
            _vm = DataContext as GRNItemModalViewModel;
            _vm.ClearAndSetup.Execute(null);
            _vm.RequestClose += (s, e) => this.Close();
        }

        
        private void LocalizeLabel()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.Title = messageResolver.GetText("sl.inventory.receive.modal.title");
            lblProduct.Content = messageResolver.GetText("sl.inventory.receive.modal.product");
            lblQty.Content = messageResolver.GetText("sl.inventory.receive.modal.quantity");
            lblExpected.Content = messageResolver.GetText("sl.inventory.receive.modal.expectedqty");
            lblUnitPrice.Content = messageResolver.GetText("sl.inventory.receive.modal.unitprice");
            lblTotalCost.Content = messageResolver.GetText("sl.inventory.receive.modal.totalcost");
            labelreason.Content = messageResolver.GetText("sl.inventory.receive.modal.reason");
        }

        

       

        public GRNModalItems AddGrnItems()
        {
            this.Owner = Application.Current.MainWindow;
            txtReason.IsEnabled = false;
            ComboBoxSelectedProduct.IsEnabled = true;
            _vm.IsNew = true;
            _vm.SelectedProduct = null;
            _vm.LineItemId = Guid.NewGuid();
            ShowDialog();
            return _vm.GetModalItems();
        }

        public GRNModalItems EditGrnItems(AddGrnLineItemViewModel lineItem,List<ProductSerialNumbers> productSerials)
        {
            this.Owner = Application.Current.MainWindow;
            txtReason.IsEnabled = true;
            ComboBoxSelectedProduct.IsEnabled = false;
            _vm.LoadForEdit(lineItem, productSerials);
            ShowDialog();
            return _vm.GetModalItems();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _vm.Cleanup();
        }
    }
}
