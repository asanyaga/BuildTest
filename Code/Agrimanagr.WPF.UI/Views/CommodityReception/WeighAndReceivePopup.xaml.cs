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
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception;

namespace Agrimanagr.WPF.UI.Views.CommodityReception
{
    /// <summary>
    /// Interaction logic for WeighAndReceivePopup.xaml
    /// </summary>
    public partial class WeighAndReceivePopup : Window, IWeighAndReceivePopUp
    {
        private WeighReceiveDeliveryViewModelPopUp _vm;
        public WeighAndReceivePopup()
        {
            InitializeComponent();
             this.CenterWindowOnScreen();
             this.Owner = Application.Current.MainWindow;
            _vm = this.DataContext as WeighReceiveDeliveryViewModelPopUp;
            _vm.RequestClose += (s, e) => this.Close();
        }

        public void ShowWeighAndReceive(Guid documentId)
        {
            _vm.SetUp(documentId);
            this.ShowDialog();
        }
    }
}
