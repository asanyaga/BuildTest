using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception;

namespace Agrimanagr.WPF.UI.Views.CommodityReception
{
    public partial class ListCommodityReception : UserControl
    {
        private ListCommodityReceptionViewModel _vm;
        private Dictionary<Guid, AgriDocumentListItem> selectedItems;
        bool toggleAll = true;

        public ListCommodityReception()
        {
            InitializeComponent();
            Loaded += ListCommodityReception_Loaded;
            _vm = this.DataContext as ListCommodityReceptionViewModel;
            _vm.ClearViewModel();
            selectedItems = new Dictionary<Guid, AgriDocumentListItem>();
        }

        private void ListCommodityReception_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionView awaitingReceptionView =
                (CollectionView) CollectionViewSource.GetDefaultView(dgAwaitingReception.Items);
            CollectionView awaitingStorageView =
                (CollectionView) CollectionViewSource.GetDefaultView(dgAwaitingStorage.Items);
            ((INotifyCollectionChanged) awaitingReceptionView).CollectionChanged += CommodityReception_CollectionChanged;
            ((INotifyCollectionChanged) awaitingStorageView).CollectionChanged += CommodityReception_CollectionChanged;
        }

        private void CommodityReception_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemCollection collection = (sender as ItemCollection);
            if (collection.Count > 0)
            {
                var item = collection[collection.Count - 1];
                DataGrid dataGrid = tclReception.FindVisualChild<DataGrid>();
                DataGridRow currentRow = dataGrid.GetDataGridRowForItem(item);

                if (currentRow != null)
                {
                    if (selectedItems.Any(n => n.Key == ((AgriDocumentListItem)item).DocumentId))
                    {
                        var myCell = currentRow.GetCell(dataGrid, 0);
                        CheckBox ck = myCell.GetVisualChild<CheckBox>();
                        ck.IsChecked = true;
                    }
                }
            }
        }

        private void PieSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = -1;
            if (((PieSeries)sender).SelectedItem != null)
                index = ((CommodityPeiChartItem)((PieSeries)sender).SelectedItem).Tab;
            if (index > -1)
            {
                tclReception.SelectedIndex = index;
            }
        }

        private void tclReception_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;
            LoadList();
        }

        void LoadList()
        {
            if ((tclReception.SelectedIndex == tclReception.Items.IndexOf(tbAwaitingStorage)))
            {
                _vm.LoadAwaitingStorage();
            }
            else if ((tclReception.SelectedIndex == tclReception.Items.IndexOf(tbCompleteReception)))
            {
                _vm.LoadCompleteReceptions();
            }
            ((PieSeries)pcReceptionStatus.Series[0]).ItemsSource = new CommodityReceptionCollection(_vm);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {

            DataGrid dataGrid = tclReception.FindVisualChild<DataGrid>();
            if (dataGrid == null)
                return;

            IEnumerable<DataGridRow> gridRows = dataGrid.GetDataGridRows();
            foreach (DataGridRow row in gridRows)
            {
                var myCell = row.GetCell(dataGrid, 0);
                CheckBox ck = myCell.GetVisualChild<CheckBox>();
                ck.IsChecked = true;

                var entity = row.DataContext as AgriDocumentListItem;
                if (!selectedItems.Any(n => n.Key == entity.DocumentId))
                    selectedItems.Add(entity.DocumentId, entity);
            }
            toggleAll = true;
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!toggleAll)
                return;

            DataGrid dataGrid = tclReception.FindVisualChild<DataGrid>();
            if (dataGrid == null)
                return;

            IEnumerable<DataGridRow> gridRows = dataGrid.GetDataGridRows();
            foreach (DataGridRow row in gridRows)
            {
                var myCell = row.GetCell(dataGrid, 0);
                CheckBox ck = myCell.GetVisualChild<CheckBox>();
                ck.IsChecked = false;

                var entity = row.DataContext as AgriDocumentListItem;
                if (selectedItems.Any(n => n.Key == entity.DocumentId))
                    selectedItems.Remove(entity.DocumentId);
            }
        }

        private void chkSelect_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            var doc = chk.DataContext as AgriDocumentListItem;
            if (!selectedItems.Any(n => n.Key == doc.DocumentId))
                selectedItems.Add(doc.DocumentId, doc);
        }

        private void chkSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            var doc = chk.DataContext as AgriDocumentListItem;
            toggleAll = false;
            if (selectedItems.Any(n => n.Key == doc.DocumentId))
                selectedItems.Remove(doc.DocumentId);
            chkAwaitingReceptionSelectAll.IsChecked = false;
            chkAwaitingStorageSelectAll.IsChecked = false;
        }

        private void hpReceiveCommodity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAwaitingReceptionPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAwaitingReceptionNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearchIncomplete_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnBatch_Click(object sender, RoutedEventArgs e)
        {
            BatchCommodity batch = new BatchCommodity();
            batch.ShowDialog();
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            //if (selectedItems.Count < 1)
            //{
            //    MessageBox.Show("You must select item to store from the grid.", "Agrimanagr: Store Commodity",
            //                    MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            //StoreCommodity store = new StoreCommodity(null);
            //if (store.ShowDialog().Value)
            //{
            //    DialogClosed();
            //}
        }

        private void hpStore_Click(object sender, RoutedEventArgs e)
        {
            //Hyperlink hl = (Hyperlink) sender;
            //var doc = hl.DataContext as AgriDocumentListItem;
            //StoreCommodity store = new StoreCommodity(new Dictionary<Guid, AgriDocumentListItem> {{doc.DocumentId, doc}});
            //if ((bool) store.ShowDialog())
            //{
            //    DialogClosed();
            //}
        }

        void DialogClosed()
        {
            selectedItems.Clear();
            toggleAll = false;
            chkAwaitingReceptionSelectAll.IsChecked = false;
            chkAwaitingStorageSelectAll.IsChecked = false;
            LoadList();
        }

        private void dgAwaitingStorage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void hpDocRef_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = ((Hyperlink)sender);
            var doc = hl.DataContext as AgriDocumentListItem;
           // DocumentDetails dd = new DocumentDetails(doc.DocumentId);
           // dd.ShowDialog();
        }

        private void btnAwaitingStoragePrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAwaitingStorageNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCompleteReceptionPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCompleteReceptionNext_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
