using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views.Admin.AdminMenuControls
{
    public class MenuControlBase : UserControl
    {
        public MenuControlBase()
        {
        }

        protected void hlVMenu_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            Page pg = GetDependencyObjectFromVisualTree(this, typeof(Page)) as Page;

            Messenger.Default.Send<Uri>(new Uri(hl.NavigateUri.OriginalString, UriKind.Relative), "NavigationRequest");
        }

        protected void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            tvi.IsExpanded = !tvi.IsExpanded;
            ToggleTreeViewItems(sender as TreeView, tvi);
        }

        private void ToggleTreeViewItems(TreeView tv, TreeViewItem selected)
        {
            foreach (var item in tv.Items)
            {
                TreeViewItem tvi = (TreeViewItem)(tv.ItemContainerGenerator.ContainerFromIndex(tv.Items.IndexOf(item)));
                if (!tvi.IsSelected)
                    tvi.IsExpanded = false;
            }
        }
        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            DependencyObject parent = startObject;

            while (parent != null)
            {

                if (type.IsInstanceOfType(parent))

                    break;

                else

                    parent = VisualTreeHelper.GetParent(parent);

            }

            return parent;

        }
    }
}
