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
using Distributr.Core.Domain.Master;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Outlets
{
    /// <summary>
    /// Interaction logic for EditOutletTargets.xaml
    /// </summary>
    public partial class EditOutletTargets : Page
    {
       private EditOutletTargetsViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        private bool _isInitialized;
        public EditOutletTargets()
        {
            _isInitialized = false;
            InitializeComponent();
            _isInitialized = true;
            LabelControls();
            Loaded += new RoutedEventHandler(OutletTargets_Loaded);

            double widthParent = OtherUtilities.ContentFrameWidth * 0.85;
            double widthChild = OtherUtilities.ContentFrameWidth * 0.95;
            LayoutRoot.Width = widthParent;
        }

        void LabelControls()
        {
            lblRoute.Content = _messageResolver.GetText("sl.target.route");
            lblOutlet.Content = _messageResolver.GetText("sl.target.outlet");
            lblOutletTargetSummary.Content = _messageResolver.GetText("sl.target.outletTargetSummary");
            lblPeriod.Content = _messageResolver.GetText("sl.target.period");
            lblFrom.Content = _messageResolver.GetText("sl.target.from");
            lblTo.Content = _messageResolver.GetText("sl.target.to");
            lblSetTargetAs.Content = _messageResolver.GetText("sl.target.setTargetAs");
            rbQty.Content = _messageResolver.GetText("sl.target.qty");
            rbAmount.Content = _messageResolver.GetText("sl.target.amount");
            lblTarget.Content = _messageResolver.GetText("sl.target.target");
            btnAddNew.Content = _messageResolver.GetText("sl.target.addNew");
          /*  btnSave.Content = _messageResolver.GetText("sl.target.save");*/
            btnCancel.Content = _messageResolver.GetText("sl.target.cancel");

            //grid
            //dgTargets.Columns.GetByName("colTarget.Header = _messageResolver.GetText("sl.target.grid.col.target");
            //dgTargets.Columns.GetByName("colTargetAs.Header = _messageResolver.GetText("sl.target.grid.col.targetAs");
            //dgTargets.Columns.GetByName("colPeriod.Header = _messageResolver.GetText("sl.target.grid.col.period");
            //dgTargets.Columns.GetByName("colFrom.Header = _messageResolver.GetText("sl.target.grid.col.from");
            //dgTargets.Columns.GetByName("colTo.Header = _messageResolver.GetText("sl.target.grid.col.to");
        }

        private Dictionary<string, Control> dictValidationControls = new Dictionary<string, Control>();
        void OutletTargets_Loaded(object sender, RoutedEventArgs e)
        {
            dictValidationControls.Add("Target", txtTarget);
            dictValidationControls.Add("SelectedRoute", cmbRoutes);
            dictValidationControls.Add("SelectedOutlet", cmbOutlets);
            dictValidationControls.Add("SelectedTargetPeriod", cmbPeriod);
            OnNavigatedTo(null);
        }

        // Executes when the user navigates to this page.
        protected  void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm = DataContext as EditOutletTargetsViewModel;
            _vm.ClearViewModel();
            _vm.Setup();
            try
            {
                _vm.TargetId = PresentationUtility.ParseIdFromUri(e.Uri);
            }catch{}

            if (_vm.TargetId != Guid.Empty)
            {
                _vm.LoadTarget();
                // ck:
                //_vm.LoadOutlets();
            }
            else
            {
                _vm.LoadRoutes();
                _vm.LoadTargetPeriods();
            }
        }

        protected  void OnNavigatedFrom(NavigationEventArgs e)
        {
            ClearTooltips();
        }

       /* private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInvalid())
            {
                if (txtTarget.Text.Trim() == "" || Convert.ToDouble(txtTarget.Text) <= 0)
                {
                    MessageBox.Show("For target enter a number greater than 0.");
                    return;
                }
                _vm.Save();
            }
            dgTargets.ItemsSource = _vm.OutletTargets;
        }*/

    /*    bool IsInvalid()
        {
            var validationContext = new ValidationContext(LayoutRoot.DataContext, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            var isInvalid = false;

            var selectedRoute = cmbRoutes.SelectedText as MasterEntity;
            if (selectedRoute.Id == Guid.Empty)
            {
                ValidationResult vr = new ValidationResult(true, _messageResolver.GetText("sl.target.validation.selectroute")/*"Select route"#1#);
                HighlightControl(cmbRoutes, vr);
                isInvalid = true;
            }

            var selectedOutlet = cmbOutlets.SelectedValue as MasterEntity;
            if (selectedOutlet == null || selectedOutlet.Id == Guid.Empty)
            {
                ValidationResult vr = new ValidationResult(true, _messageResolver.GetText("sl.target.validation.selectoutelt")/*"Select outlet"#1#);
                HighlightControl(cmbOutlets, vr);
                isInvalid = true;
            }

            var selectedPeriod = cmbPeriod.SelectedValue as MasterEntity;
            if (selectedPeriod.Id == Guid.Empty)
            {
                ValidationResult vr = new ValidationResult(true, _messageResolver.GetText("sl.target.validation.selecttargetperiod")/*"Select target period"#1#);
                HighlightControl(cmbPeriod, vr);
                isInvalid = true;
            }

            //if (Convert.ToDouble(txtTarget.Text) <= 0)
            //{
            //    ValidationResult vr = new ValidationResult("Enter target value greater than 0");
            //    HighlightControl(cmbPeriod, vr);
            //    isInvalid = true;
            //}

            if (!Validator.TryValidateObject(LayoutRoot.DataContext, validationContext, validationResults as ICollection<System.ComponentModel.DataAnnotations.ValidationResult>))
            {
                //foreach (var error in validationResults)
                //    HighlightControl(dictValidationControls[error.MemberNames.First()], error);
                isInvalid = true;
            }

            return isInvalid;
        }*/

        #region Validation
        void HighlightControl(Control control, ValidationResult result)
        {
            //ToolTip tooltip = GetTooltip(control);

            //tooltip.DataContext = result.ErrorMessage;

            //tooltip.Template = this.Resources["ValidationToolTipTemplate"] as ControlTemplate;

            if (!control.Focus())
                VisualStateManager.GoToState(control, "InvalidUnfocused", true);
            else
                VisualStateManager.GoToState(control, "InvalidFocused", true);
        }

        void ClearTooltips()
        {
            foreach (var item in dictValidationControls.Keys)
            {
                ToolTip tt = GetTooltip(dictValidationControls[item]);
                tt.Template = null;
                tt.DataContext = null;
            }
        }

        private ToolTip GetTooltip(Control control)
        {
           
            var border = ((Border)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(control, 0), 3));

            return border.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
        }
        #endregion

        private void txtTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_vm.RunTargetChangedCommand();
            //lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
            //lstOutletTargetInfo.ItemsSource = null;
            //lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }

        private void txtTarget_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        private void cmbOutlets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _vm.RunOutletChangedCommand();
            dgTargets.ItemsSource = _vm.OutletTargets;
            //lstOutletTargetInfo.ItemsSource = null;
            //lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }

        private void dgTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (_vm.IsEditing || _vm.AddingNew)
                return;
            _vm.RunSelectedTargetChangedCommand();
            lstOutletTargetInfo.ItemsSource = null;
            lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }

        private void cmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _vm.RunTargetPeriodChangedCommand();
            lstOutletTargetInfo.ItemsSource = null;
            lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }

     /*   private void cmbOutlets_Loaded(object sender, RoutedEventArgs e)
        {
            if (cmbOutlets.Items.Count > 0)
                cmbOutlets.SelectedIndex = 0;
        }
*/
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            _vm.RunEditCommand();
        }

        private void hlDeactivate_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedOutletTarget.EntityStatus == (int)EntityStatus.Active)
                _vm.DeactivateTarget();
            else if (_vm.SelectedOutletTarget.EntityStatus == (int)EntityStatus.Inactive)
                _vm.ActivateTarget();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            _vm.DeleteTarget();
        }

        private void dgTargets_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            //StackPanel spEdit = dgTargets.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as StackPanel;

            //Hyperlink edit = spEdit.Children[0] as Hyperlink;
            //edit.Content = _messageResolver.GetText("sl.target.grid.col.edit");

            //HyperlinkButton delete = spEdit.Children[4] as HyperlinkButton;
            //delete.Content = _messageResolver.GetText("sl.target.grid.col.delete");
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            _vm.ShowInactive = true;
            _vm.RunOutletChangedCommand();
            dgTargets.ItemsSource = _vm.OutletTargets;
            //lstOutletTargetInfo.ItemsSource = null;
            //lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            _vm.ShowInactive = false;
            _vm.RunOutletChangedCommand();
            dgTargets.ItemsSource = _vm.OutletTargets;
            //lstOutletTargetInfo.ItemsSource = null; 
            //lstOutletTargetInfo.ItemsSource = _vm.TargetInfo;
        }
 
    }
}
