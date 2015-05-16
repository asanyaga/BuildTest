using System.Windows;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.UI.Pages
{
    public partial class About : Window, IAbout
    {
        private CussonsAboutViewModel _vm;
        public About()
        {
            InitializeComponent();
            _vm = this.DataContext as CussonsAboutViewModel;
        }

        public void ShowAboutDialog()
        {

            this.ShowDialog();
        }
    }
}
