using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using System.Xml.Linq;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views.Admin.Equipment.EquipmentSetup
{
    /// <summary>
    /// Interaction logic for WeighingScaleSetup.xaml
    /// </summary>
    public partial class WeighingScaleSetup : Window
    {
        private EquipmentSetupViewModel _vm = null;

        public WeighingScaleSetup(LocalEquipmentConfig equipment = null)
        {
            InitializeComponent();
            _vm = this.DataContext as EquipmentSetupViewModel;
            _vm.SetUp();
            if (equipment != null)
            {
                _vm.EquipmentId = equipment.Id;
                _vm.SelectedEquipmentType = (EquipmentType) Enum.Parse(typeof (EquipmentType), equipment.EquipmentType);
                

            }
            _vm.RequestClose += (s, e) => this.Close();
        }



        private void DeviceSetupLoaded(object sender, RoutedEventArgs e)
        {
           if(_vm.EquipmentId !=Guid.Empty)
           {
               _vm.GetWeighScaleByIdCommand.Execute(null);
           }

        }
       

       

    }
}
