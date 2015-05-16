using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Routes;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Routes
{
    public partial class EditRoute : PageBase
    {
        private EditRouteViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public EditRoute()
        {
            InitializeComponent();
            _vm = this.DataContext as EditRouteViewModel;
            LabelControls();
            this.Loaded += new RoutedEventHandler(EditRoute_Loaded);
        }

        void LabelControls()
        {
            lblCode.Content = _messageResolver.GetText("sl.route.code");
            lblName.Content = _messageResolver.GetText("sl.route.name");
            btnSave.Content = _messageResolver.GetText("sl.route.save");
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (
                    MessageBox.Show( /*"Unsaved changes will be lost. Cancel creating route anyway?"*/
                        _messageResolver.GetText("sl.route.cancel.messagebox.prompt")
                        , _messageResolver.GetText("sl.route.cancel.messagebox.caption") /*"Distributr: Create Route"*/
                        , MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
            base.OnNavigatingFrom(sender, e);
        }

        void EditRoute_Loaded(object sender, RoutedEventArgs e)
        {
            Guid id = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource);
            _vm.SetUp();
            _vm.LoadById(id);
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void PageBase_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
