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
using Distributr.WPF.Lib.ViewModels.Sync;

namespace Distributr.WPF.UI.Views.Archiving
{
    public partial class Archiving : Page
    {
        public Archiving()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Archiving_Loaded);
        }

        void Archiving_Loaded(object sender, RoutedEventArgs e)
        {
            ArchiveViewModel model = DataContext as ArchiveViewModel;
            model.LoadItemToArchive();
        }
    }
}
