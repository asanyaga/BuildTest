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
using System.Windows.Shapes;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;
using Integration.QuickBooks.Lib.UI;

namespace Integration.QuickBooks.WPF.UI.Views
{
    public partial class About : Window, IAbout
    {
        private QBAboutViewModel _vm;
        public About()
        {
            InitializeComponent();
            _vm = this.DataContext as QBAboutViewModel;
        }

        public void ShowAboutDialog()
        {

            this.ShowDialog();
        }
    }
}
