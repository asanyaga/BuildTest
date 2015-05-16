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
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Microsoft.Reporting.WinForms;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for CustomReportViewer.xaml
    /// </summary>
    public partial class  CustomReportViewer : Window, ICustomReportViewer
    {
        public CustomReportViewer()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
        }
       

        public void ShowReportView(ReportViewer viewer)
        {
            
            this._reportViewer = viewer;
            ShowDialog();
        }
    }
}
