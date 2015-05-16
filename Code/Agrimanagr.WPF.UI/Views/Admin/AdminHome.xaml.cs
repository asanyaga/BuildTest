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
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views.Admin
{
    public partial class AdminHome : Page
    {
        public AdminHome()
        {
            InitializeComponent();
          // Messenger.Default.Register<Uri>(this, "AdminNavigationRequest", (uri) => this.FrameAdmin.Navigate(uri));
        }
    }
}
