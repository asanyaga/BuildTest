/*
  In App.xaml:
  <Application.Resources>
      <vm:ImporterViewModelBase xmlns:vm="clr-namespace:Agrimanagr.DataImporter.Lib.ViewModels"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ImportSettingsViewModel>();
        }
        

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",Justification = "This non-static member is needed for data binding purposes.")]
        public ImportSettingsViewModel ImportSettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImportSettingsViewModel>();
            }
        }
    }
}