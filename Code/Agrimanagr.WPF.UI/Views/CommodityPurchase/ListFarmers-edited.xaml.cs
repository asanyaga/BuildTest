using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Agrimanagr.WPF.UI.Views.ReceiptView;
using Agrimanagr.WPF.UI.Views.UtilityViews;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;

namespace Agrimanagr.WPF.UI.Views.CommodityPurchase
{
    public partial class ListFarmers : Page
    {
        private ListFarmersViewModel _vm;
        public ListFarmers()
        {
            InitializeComponent();
            _vm = this.DataContext as ListFarmersViewModel;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name == btnClearRoute.Name)
                _vm.LoadDefaultSelected("route");
            if (btn.Name == btnClearBuyingCentre.Name)
                _vm.LoadDefaultSelected("centre");
            if (btn.Name == btnClearAll.Name)
            {
                txtSrchFarmer.Text = string.Empty;
                _vm.LoadDefaultSelected();
            }
            
        }

        private void txtSrchFarmer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_vm ==null)
                _vm = DataContext as ListFarmersViewModel;

            TextBox txtBox = sender as TextBox;
               _vm.FilterFarmersCommand.Execute(txtBox.Text);
        }

    
        private void DropDownOpened(object sender, System.EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            ComboPopUp popup = new ComboPopUp();
            object selected;
            switch (combo.Name)
            {
                case "cmbBuyingCentre":
                    _vm.LoadBuyingCentres();
                    if (popup.ShowDlg(sender, out selected))
                        _vm.SelectedBuyingCentre = selected as Centre;
                    break;

                case "cmbCommodity":
                    _vm.LoadCommodity();
                    if (popup.ShowDlg(sender, out selected))
                    _vm.SelectedCommodity = selected as Commodity;

                    break;

                case "cmbRoute":
                    _vm.LoadRoutes();
                    if (popup.ShowDlg(sender, out selected))
                    _vm.SelectedRoute = selected as Route;

                    break;
            }
        }
    }
}
