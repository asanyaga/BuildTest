using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    public class PageBase : Page
    {
        public PageBase()
        {
            Loaded += PageBase_Loaded;
            Unloaded += PageBase_Unloaded;
        }

        void PageBase_Unloaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
                NavigationService.Navigating -= OnNavigatingFrom;
        }

        void PageBase_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigating += OnNavigatingFrom;
        }

        protected virtual void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Cancel == false)
                if (NavigationService != null)
                    NavigationService.Navigating -= OnNavigatingFrom;
        }
    }
}

