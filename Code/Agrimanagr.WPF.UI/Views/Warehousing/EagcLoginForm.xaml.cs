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
using Distributr.WPF.Lib.ViewModels.Warehousing;

namespace Agrimanagr.WPF.UI.Views.Warehousing
{
    /// <summary>
    /// Interaction logic for EagcLoginForm.xaml
    /// </summary>
    public partial class EagcLoginForm : Page
    {

        private EagcLoginViewModel vm;
        public EagcLoginForm()
        {
            InitializeComponent();
            vm = DataContext as EagcLoginViewModel;
        }
        public EagcLoginForm(EagcLoginViewModel context)
            : this()
        {
            this.DataContext = context;
          
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
     
       
     

       
    }
}
