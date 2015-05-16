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
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.AuditLogs;
using StructureMap;

namespace Distributr.WPF.UI.Views.Reports
{
    /// <summary>
    /// Interaction logic for AuditLog.xaml
    /// </summary>
    public partial class AuditLog : Page
    {
        AuditLogViewModel vm;
        public AuditLog()
        {
            InitializeComponent();
            LabelControls();
            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.95);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.95);
            double dataGridHeight = (newLayoutRootHeight * 0.88);

            LayoutRoot.Width = newLayoutRootWidth;
            LayoutRoot.Height = newLayoutRootHeight;
            dgAuditLog.Height = dataGridHeight;
        }

        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            //lblTitle.Content = messageResolver.GetText("sl.auditLog.title_lbl");
            lblStartDate.Content = messageResolver.GetText("sl.auditLog.startDate_lbl");
            lblEndDate.Content = messageResolver.GetText("sl.auditLog.endDate_lbl");
            lblSearchByModule.Content = messageResolver.GetText("sl.auditLog.searchByModule_lbl");
            cmdClear.Content = messageResolver.GetText("sl.auditLog.clear_btn");
            cmdSearch.Content = messageResolver.GetText("sl.auditLog.search_btn");
            cmdGenerate.Content = messageResolver.GetText("sl.auditLog.runReport_btn");
        }

        // Executes when the user navigates to this page.THIS DOESN'T EXECUTE
        //protected  void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    try
        //    {
        //        vm = DataContext as AuditLogViewModel;
        //        vm.DoLoadCommand.Execute(null);
        //        //vm.LoadAuditLog.Execute(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm = DataContext as AuditLogViewModel;
                vm.SearchParameter = null;
                if (!isValid(StartDateDP.Text))
                {
                    MessageBox.Show("Start date is not valid");
                    StartDateDP.Focus();
                    return;
                }
                if (!isValid(EndDateDP.Text))
                {
                    MessageBox.Show("End date is not valid");
                    EndDateDP.Focus();
                    return;
                }
                if (vm.StartDate > vm.EndDate)
                    MessageBox.Show("Start Date should not be greater than End Date");
                else
                {
                    vm.LoadAuditLog.Execute(null);
                   // dgAuditLog.ItemsSource = vm.AuditLogList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool isValid(string str)
        {
            try
            {
                DateTime.Parse(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void cmdSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm = DataContext as AuditLogViewModel;
                vm.SearchCommand.Execute(null);
                dgAuditLog.ItemsSource = vm.AuditLogList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

     
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vm = DataContext as AuditLogViewModel;
                vm.DoLoadCommand.Execute(null);
                //vm.LoadAuditLog.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
