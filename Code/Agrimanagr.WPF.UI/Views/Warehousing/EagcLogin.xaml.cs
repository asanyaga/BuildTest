using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EagcLogin;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.Warehousing
{
    /// <summary>
    /// Interaction logic for EagcLogin.xaml
    /// </summary>
    public partial class EagcLogin : Window, ILoginPopup
    {
        public EagcLogin()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Login();
           
            if (!string.IsNullOrEmpty(EAGCLoginDetails.TokenId ) )
            {
                CloseLoginPopup(); 
            }
           
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
           
                CloseLoginPopup();
            

        }

        public LoginDetail CloseLoginPopup()
        {
            Close();
            return null;
        }

        public LoginDetail ShowLoginPopup()
        {
            ShowDialog();
            Close();
            return null;
        }



        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }
        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }
        public static string ApiUri
        {
            get { return ConfigurationManager.AppSettings["API_URI"]; }

        }




        public async void Login()
        {

            loginBtn.IsEnabled = false;
            Loading.Visibility = Visibility.Visible;
            using (var c = NestedContainer)
            {
                var token =await Using<IEagcServiceProxy>(c).Login(UsernameTextBox.Text, PasswordTextBox.Password);

                if (token == null || token.Result.ContactType != EagcContactType.VoucherCentreClerkType.ToString())
                {
                    MessageBox.Show("Wrong UserName or Password!", "Eagc Login");
                    loginBtn.IsEnabled = true;
                    Loading.Visibility = Visibility.Hidden;
                }
                else

                {
                    EAGCLoginDetails.TokenId = token.Result.Token;
                    CloseLoginPopup(); 
                }

                
            }
            
        }
       
    }
}
