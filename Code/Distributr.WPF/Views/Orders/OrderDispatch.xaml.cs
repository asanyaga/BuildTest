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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using StructureMap;

namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for OrderDispatch.xaml
    /// </summary>
    public partial class OrderDispatch : Page
    {
        private OrderDispatchViewModel _vm;
        public OrderDispatch()
        {
            InitializeComponent();
            this.Loaded += OrderDispatch_Loaded;
            LocalizeControls();
            _vm = DataContext as OrderDispatchViewModel;
        }

        void OrderDispatch_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.SetupCommand.Execute(null);
        }

        void LocalizeControls()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            lblSalesman.Content = messageResolver.GetText("sl.createOrder.salesman_lbl");
            lblRoute.Content = messageResolver.GetText("sl.createOrder.route_lbl");
            colOrderRef.Header = messageResolver.GetText("sl.createOrder.grid.col.orderRef");
        }

    }
}
