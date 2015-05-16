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
using Agrimanagr.WPF.UI.Views.Admin.Equipment.EquipmentSetup;
using Distributr.WPF.Lib.ViewModels.Utils;

namespace Agrimanagr.WPF.UI.Views.Admin.Equipment
{
    /// <summary>
    /// Interaction logic for ListWeighingScales.xaml
    /// </summary>
    public partial class ListWeighingScales : UserControl
    {
        private ListWeingScalesViewModel _vm; 
        public ListWeighingScales()
        {
            InitializeComponent();
            _vm = DataContext as ListWeingScalesViewModel;

        }

        private void ListWeighScalesLoaded(object sender, RoutedEventArgs e)
        {
            if(_vm !=null)
                _vm.LoadDevicesCommand.Execute(null);
        }

        private void hpEditWeighScale_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            LocalEquipmentConfig context = link.DataContext as LocalEquipmentConfig;
            if (context != null)
            {
                _vm.SelectedLocalEquipment = context;
               WeighingScaleSetup setup = new WeighingScaleSetup(_vm.SelectedLocalEquipment);
                setup.ShowDialog();
                _vm.LoadDevicesCommand.Execute(null);
            }
        }

        private void hpDeletWeighScale_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are sure you want to delete this device?", "Agrimanagr Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (_vm == null)
                    _vm = DataContext as ListWeingScalesViewModel;
                try
                {
                    Hyperlink link = sender as Hyperlink;
                   LocalEquipmentConfig context = link.DataContext as LocalEquipmentConfig;
                    if (context != null)
                    {
                        _vm.SelectedLocalEquipment = context;
                        _vm.DeletedDeviceCommand.Execute(null);
                        _vm.LoadDevicesCommand.Execute(null);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        private void txtSrch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            WeighingScaleSetup setup = new WeighingScaleSetup();
            setup.ShowDialog();
            if(_vm !=null)
            _vm.LoadDevicesCommand.Execute(null);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void data_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
