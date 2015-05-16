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
using Distributr.Core.Domain.Master;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Outlets
{
    public partial class ListOutlets : Page
    {
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ListOutlets()
        {
            InitializeComponent();
            LabelControls();
            
        }

        void LabelControls()
        {
            //lblTitle.Content = _messageResolver.GetText("sl.listOutlet.title");
            //chkLoadAll.Content = _messageResolver.GetText("sl.outlets.list.loadall");
            chkLoadApproved.Content = _messageResolver.GetText("sl.outlets.list.loadapproved");
            chkLoadUnApproved.Content = _messageResolver.GetText("sl.outlets.list.loadunapproved");
            chkApproveAll.Content = _messageResolver.GetText("sl.outlets.list.selectall");
            btnAdd.Content = _messageResolver.GetText("sl.outlets.list.add");
            btnApprove.Content = _messageResolver.GetText("sl.outlets.list.approve");

            //grid
            colName.Header = _messageResolver.GetText("sl.outlets.list.name");
            colCode.Header = _messageResolver.GetText("sl.outlets.list.code");
            colRoute.Header = _messageResolver.GetText("sl.outlets.list.route");
            colCategory.Header = _messageResolver.GetText("sl.outlets.list.category");
            colType.Header = _messageResolver.GetText("sl.outlets.list.type");
            colTier.Header = _messageResolver.GetText("sl.outlets.list.tier");
            colVat.Header = _messageResolver.GetText("sl.outlets.list.vat");
            colDiscGp.Header = _messageResolver.GetText("sl.outlets.list.discgp");
            colLat.Header = _messageResolver.GetText("sl.outlets.list.lat");
            colLong.Header = _messageResolver.GetText("sl.outlets.list.long");
            colApproved.Header = _messageResolver.GetText("sl.outlets.list.approved");
        }

       

        private void chkApprove_Click(object sender, RoutedEventArgs e)
        {
          
                //var hl = sender as CheckBox;
                //ListOutletItem selectedOutlet = hl.DataContext as ListOutletItem;
                //_vm.SelectedOutletId = selectedOutlet.Id;
                //var outletTobeApproved = _vm.Outlets.FirstOrDefault(p => p.Id == _vm.SelectedOutletId);
                //if (!outletTobeApproved.ProductPricingTier.Contains("--Product Pricing Tier Not Set--"))
                //{
                //    _vm.Outlets.FirstOrDefault(p => p.Id == _vm.SelectedOutletId).isApproved = true;
                //}
                //else
                //{
                //    hl.IsChecked = false;
                //    throw new Exception("You must select pricing tier for outlet before approving");
                   
                //}
            
            
        }

        private void chkLoadAll_Click(object sender, RoutedEventArgs e)
        {
            //if (((CheckBox)sender).IsChecked == true)
            //{
            //    _vm.OutletsToLoad = ListOutletsViewModel.enumOutletsToLoad.All;
            //    chkLoadApproved.IsChecked = false;
            //    chkLoadUnApproved.IsChecked = false;
            //    chkApproveAll.IsChecked = false;
            //    _vm.LoadOutlets();
            //    dgOutlets.ItemsSource = _vm.Outlets;
            //}
        }

        private void chkLoadApproved_Click(object sender, RoutedEventArgs e)
        {
        //    if (((CheckBox)sender).IsChecked == true)
        //    {
        //        _vm.OutletsToLoad = ListOutletsViewModel.enumOutletsToLoad.Approved;
        //        chkLoadAll.IsChecked = false;
        //        chkLoadUnApproved.IsChecked = false;
        //        chkApproveAll.IsChecked = false;
        //        _vm.LoadOutlets();
        //        dgOutlets.ItemsSource = _vm.Outlets;
        //    }
        }

        private void chkLoadUnApproved_Click(object sender, RoutedEventArgs e)
        {
            //if (((CheckBox)sender).IsChecked == true)
            //{
            //    _vm.OutletsToLoad = ListOutletsViewModel.enumOutletsToLoad.UnApproved;
            //    chkLoadApproved.IsChecked = false;
            //    chkLoadAll.IsChecked = false;
            //    chkApproveAll.IsChecked = false;
            //    _vm.LoadOutlets();
            //    dgOutlets.ItemsSource = _vm.Outlets;
            //}
        }

        private void chkApproveAll_Click(object sender, RoutedEventArgs e)
        {
            //if (((CheckBox)sender).IsChecked == true)
            //{
            //    _vm.ApproveAllSelected();
            //}
            //else
            //{
            //    _vm.UnApproveAllSelected();
            //}
            //dgOutlets.ItemsSource = null;
            //dgOutlets.ItemsSource = _vm.Outlets;
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
       {
        //    if (_vm.Outlets.Where(n => n.CanApproveOrUnApprove && n.isApproved).Count() == 0)
        //    {
        //        MessageBox.Show("You must select unapproved outlets first.",
        //                         "Distributr: Approve Outlets", MessageBoxButton.OK);
        //        return;
        //    }
        //    if (MessageBox.Show("Are you sure you want to approve all unapproved outlets?",
        //                        "Distributr: Approve Outlets", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //    {

        //        List<Guid> outletIds = new List<Guid>();
        //      foreach (ListOutletItem item in _vm.Outlets.Where(n => n.CanApproveOrUnApprove && !n.isApproved))
        //        {
        //            outletIds.Add(item.Id);
        //        }
        //        _vm.ApproveOutlet();

        //        _vm.ReloadList();
        //        dgOutlets.ItemsSource = null;
        //        dgOutlets.ItemsSource = _vm.Outlets;
        //    }
        }

        private void dgOutlets_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            //StackPanel spEdit = dgOutlets.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as StackPanel;
            //Hyperlink edit = spEdit.Children[0] as Hyperlink;
            //edit.Content = _messageResolver.GetText("sl.outlets.list.edit");

            //HyperlinkButton deactivate = spEdit.Children[2] as HyperlinkButton;
            //deactivate.Content = _messageResolver.GetText("sl.outlets.list.deactivate");

            // Hyperlink delete = spEdit.Children[4] as HyperlinkButton;
            // delete.Content = _messageResolver.GetText("sl.outlets.list.delete");
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            //_vm = this.DataContext as ListOutletsViewModel;
            //_vm.ShowInactive = true;
            //_vm.GetOutlets();
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            //_vm = this.DataContext as ListOutletsViewModel;
            //_vm.ShowInactive = false;
            //_vm.GetOutlets();
        }

        
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            //var hl = sender as Hyperlink;
            //string url;
            //url = "views/administration/outlets/editoutlet.xaml?" + hl.Tag;
            //NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

       
    }
}
