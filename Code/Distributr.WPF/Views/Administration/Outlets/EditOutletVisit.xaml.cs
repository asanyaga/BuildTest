using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Outlets
{
    /// <summary>
    /// Interaction logic for EditOutletVisit.xaml
    /// </summary>
    public partial class EditOutletVisit : Page
    {
        private EditOutletVistDayViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public EditOutletVisit()
        {
            InitializeComponent();
            LabelControls();
        }
        void LabelControls()
        {
            lblRoute.Content = _messageResolver.GetText("sl.outletVisitDay.route");
            lblOutlet.Content = _messageResolver.GetText("sl.outletVisitDay.outlet");
            lblEffectiveDate.Content = _messageResolver.GetText("sl.outletVisitDay.effectiveDate");
            btnSave.Content = _messageResolver.GetText("sl.outletVisitDay.save");
            btncancel.Content = _messageResolver.GetText("sl.outletVisitDay.cancel");

            //grid
            ColName.Header = _messageResolver.GetText("sl.outletVisitDay.grid.col.Name");
            ColCheckbox.Header = _messageResolver.GetText("sl.outletVisitDay.grid.col.isvisitday");
        }

        // Executes when the user navigates to this page.
        protected  void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm = this.DataContext as EditOutletVistDayViewModel;
            _vm.DoLoadCommand.Execute(null);
        }

       

        private void OutletVisitDayPage_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as EditOutletVistDayViewModel;
            _vm.DoLoadCommand.Execute(null);
        }

        private void OutvisitdayCancel(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Are you sure you want to close this page?", "Distributr Warning",
                                      MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(res==MessageBoxResult.Yes)
            {
                _vm.Navigate(@"\Views\Administration\Outlets\ListOutlets.xaml");
            }
        }
    }
}
