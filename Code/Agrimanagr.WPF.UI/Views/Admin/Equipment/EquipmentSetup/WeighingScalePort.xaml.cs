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

namespace Agrimanagr.WPF.UI.Views.Admin.Equipment.EquipmentSetup
{
    /// <summary>
    /// Interaction logic for WeighingScalePort.xaml
    /// </summary>
    public partial class WeighingScalePort : Window
    {
        public WeighingScalePort()
        {
            InitializeComponent();
        }

        private void OK_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
