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

namespace Agrimanagr.WPF.UI.Views.Warehousing
{
    /// <summary>
    /// Interaction logic for AddWarehouseEntryFormPage.xaml
    /// </summary>
    public partial class AddWarehouseEntryFormPage : Page
    {
        public AddWarehouseEntryFormPage()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            btnWeigh.IsEnabled = true;
            int val = 0;
            txtWeight.Text = val.ToString();
        }





    }
}
