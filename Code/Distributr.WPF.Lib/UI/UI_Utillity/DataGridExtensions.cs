using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    public static class DataGridExtensions
    {
        public static DataGridRow GetDataGridRowForItem(this DataGrid grid, object item)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource)
                return null;
            var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
            if (row == null)
            {
                grid.UpdateLayout();
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(item);
            }
            return row;
        }

        public static IEnumerable<DataGridRow> GetDataGridRows(this DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        public static DataGridCell GetCell(this DataGridRow rowContainer, DataGrid dataGrid, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                if (presenter == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;
            }
            return null;
        }
        public static T GetVisualChild<T>(this Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridColumn GetByName(this ObservableCollection<DataGridColumn> col, string name)
        {
            return col.SingleOrDefault(p =>
                (string)p.GetValue(FrameworkElement.NameProperty) == name
            );
        }
    }
}
