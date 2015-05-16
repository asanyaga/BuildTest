using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Distributr.WPF.UI.Utility
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

