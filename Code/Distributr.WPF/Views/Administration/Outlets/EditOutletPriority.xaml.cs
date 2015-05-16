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
    public partial class EditOutletPriority : Page
    {
        EditOutletPriorityViewModel _vm;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public EditOutletPriority()
        {
            InitializeComponent();
            LabelControls();
        }

        void LabelControls()
        {
            lblTitle.Content = _messageResolver.GetText("sl.outletPriority.title");
            lblRoute.Content = _messageResolver.GetText("sl.outletPriority.route");
            lblEffectiveDate.Content = _messageResolver.GetText("sl.outletPriority.effectivedate");
            btnMoveTop.Content = _messageResolver.GetText("sl.outletPriority.top");
            btnMoveUp.Content = _messageResolver.GetText("sl.outletPriority.up");
            btnMoveDown.Content = _messageResolver.GetText("sl.outletPriority.down");
            btnMoveBottom.Content = _messageResolver.GetText("sl.outletPriority.bottom");
            btnQuickSet.Content = _messageResolver.GetText("sl.outletPriority.quickset");
            btnSave.Content = _messageResolver.GetText("sl.outletPriority.save");
            btnCancel.Content = _messageResolver.GetText("sl.outletPriority.cancel");

            //grid
            colName.Header = _messageResolver.GetText("sl.outletPriority.grid.col.outlet");
            colPriority.Header = _messageResolver.GetText("sl.outletPriority.grid.col.priority");
        }

        private void btnQuickSet_Click(object sender, RoutedEventArgs e)
        {
            QuickSetPriority qsp = new QuickSetPriority();
            qsp.ShowDialog();
        }

        private void pgOutletPriority_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as EditOutletPriorityViewModel;
            _vm.LoadRoutes();
        }

    }
}
