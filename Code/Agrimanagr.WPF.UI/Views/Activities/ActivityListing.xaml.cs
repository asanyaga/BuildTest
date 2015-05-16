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
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Activity;

namespace Agrimanagr.WPF.UI.Views.Activities
{
    /// <summary>
    /// Interaction logic for ActivityListing.xaml
    /// </summary>
    public partial class ActivityListing : Page
    {
        public ActivityListing()
        {
            InitializeComponent();
        }

        private void hldetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Hyperlink) sender;
            if (selectedItem != null)
            {
                var vm = DataContext as ActivityListingViewModel;
                vm.DetailsPopUpCommand.Execute((ActivityItem) selectedItem.Tag);

            }

        }
    }
}
