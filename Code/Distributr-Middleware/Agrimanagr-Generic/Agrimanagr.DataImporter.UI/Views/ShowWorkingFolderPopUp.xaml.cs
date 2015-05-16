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
using Agrimanagr.DataImporter.Lib.Utils;
using Agrimanagr.DataImporter.Lib.ViewModels;

namespace Agrimanagr.DataImporter.UI.Views
{
    /// <summary>
    /// Interaction logic for ShowWorkingFolderPopUp.xaml
    /// </summary>
    public partial class ShowWorkingFolderPopUp : Window, IShowWorkingFolderPopUp
    {

        private MainViewModel _vm;
        public ShowWorkingFolderPopUp()
        {
            InitializeComponent();
            _vm = DataContext as MainViewModel;
            _vm.RequestClose += (s, e) => this.Close();
        }
        public void ShowPopUp()
        {
            ShowDialog();
        }
    }
}
