using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels;

namespace Integration.Cussons.WPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CussonsMainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) =>
            {
                this.ContentFrame.Navigate(uri);
            });
            _vm = DataContext as CussonsMainWindowViewModel;
            _vm.ExitApplicationEventHandler += (s, e) => this.Close();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Integration: Exit Application",
                                MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }
    }
}
