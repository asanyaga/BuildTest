using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Description for CustomMessageBox.
    /// </summary>
    public partial class CustomMessageBox : IDistributrMessageBox
    {
        /// <summary>
        /// Initializes a new instance of the CustomMessageBox class.
        /// </summary>
        /// 
        /// 
        public CustomMessageBox()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
            bhelper = new CustomMessageBoxItems();
            
        }

        private CustomMessageBoxItems bhelper;
        private DistributrMessageBoxItem _clickedButton;


        public DistributrMessageBoxResult ShowBox(List<DistributrMessageBoxButton> items, string text, string messageBoxTitle = "Distributr Message Box")
        {
           
            this.Title = messageBoxTitle;
            TextBlockMessage.Text = text;
            int count = 1;
            foreach (DistributrMessageBoxButton s in items)
            {
                 var bdetails = bhelper.MessageBoxButtonItem(s);
               
                Button b = new Button();
                b.Content = bdetails.ButtonText;
                b.Width = (bdetails.ButtonText.Length * 8) ;
                b.Margin = new Thickness(3, 0, 3, 0);
                b.Tag = s;
                b.Click += button_clicked;
                ButtonContainer.Children.Add(b);
                if (count == 1)
                {
                    _clickedButton = bdetails;
                }
                count++;
            }
            this.Owner = Application.Current.MainWindow;
            this.ShowDialog();
            var result = new DistributrMessageBoxResult();
            if (_clickedButton != null)
            {
                result.Url = _clickedButton.Url;
                result.Button = _clickedButton.Button;
            }
            else
            {
                var defaultb = bhelper.MessageBoxButtonItem(DistributrMessageBoxButton.None);
                result.Url = defaultb.Url;
                result.Button = defaultb.Button;
            }


            return result;
        }

        void button_clicked(object sender, RoutedEventArgs e)
        {
            var b = (Button) sender;
            var selectedb = (DistributrMessageBoxButton)b.Tag;
            _clickedButton = bhelper.MessageBoxButtonItem(selectedb);
            this.Close();
        }
    }
}