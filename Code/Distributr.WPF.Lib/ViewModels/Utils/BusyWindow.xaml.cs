using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Distributr.SL.Lib.ViewModels.Utils
{
    public partial class BusyWindow : ChildWindow
    {
        public BusyWindow()
        {
            InitializeComponent();
            this.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(BusyWindow_Closing);
        }

        void BusyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == false)
                e.Cancel = true;
        }

        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

