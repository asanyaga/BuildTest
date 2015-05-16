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
using Distributr.WPF.Lib.ViewModels.Admin.ReorderLevel;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.ReorderLevel
{
    public partial class ReorderLevel : Page
    {
        private ReorderLevelViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ReorderLevel()
        {
            InitializeComponent();
            LabelControls();
            Loaded += new RoutedEventHandler(ReorderLevel_Loaded);
        }

        void LabelControls()
        {
            //grid
            colProdCode.Header = _messageResolver.GetText("sl.reorderlevel.grid.col.prodcode");
            colProdName.Header = _messageResolver.GetText("sl.reorderlevel.grid.col.proname");
            colReorderLevel.Header = _messageResolver.GetText("sl.reorderlevel.grid.col.reorderlevel");
            colAvailable.Header = _messageResolver.GetText("sl.reorderlevel.grid.col.available");

        }

        void ReorderLevel_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ReorderLevelViewModel;
            _vm.LoadReorderLevels();
        }

        // Executes when the user navigates to this page.
        protected  void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
