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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct;

namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for ChangeSalesmanToDeliveryPopUp.xaml
    /// </summary>
    public partial class ChangeSalesmanToDeliveryPopUp : Window, IChangeSalesmanToDeliveryPopUp
    {
        private ChangeDeliveryPersonViewModel _vm;
        public ChangeSalesmanToDeliveryPopUp()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
            _vm = DataContext as ChangeDeliveryPersonViewModel;
            _vm.RequestClose += (s, e) => this.Close();

        }

        public DistributorSalesman ShowPopUp(Route route)
        {
            _vm.LoadCommand.Execute(route);
            ShowDialog();
            return _vm.GetSalesman();
        }
    }
}
