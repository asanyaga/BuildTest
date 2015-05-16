using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
   public class ImporterViewModelBase:ViewModelBase
    {
       public ImporterViewModelBase()
       {
           NavigateCommand =new RelayCommand<string>(Navigate);
       }

       private RelayCommand _setupCommand = null;
       public RelayCommand SetupCommand
       {
           get { return _setupCommand ?? (_setupCommand = new RelayCommand(SetUp)); }
       }

       private RelayCommand _saveCommand = null;
       public RelayCommand SaveCommand
       {
           get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); }
       }
       private RelayCommand<Page> _loadPageCommand = null;
       public RelayCommand<Page> LoadPageCommand
       {
           get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand<Page>(LoadPage)); }
       }

       protected virtual void LoadPage(Page page) { }
       protected virtual void Save() { }
       protected virtual void SetUp() { }

       public RelayCommand<string> NavigateCommand { get; set; }

       protected PropertyInfo[] GetEntityGetFields(Type entity)
       {
           return entity.GetProperties();
       }

       protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }
        protected virtual void PutFocusOnControl(Control element)
        {
            if (element != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (System.Threading.ThreadStart)(() => element.Focus()));
        }

        private void Navigate(string url)
        {
            var uri = new Uri(url, UriKind.Relative);
            Messenger.Default.Send<Uri>(uri, "NavigationRequest");
        }

        #region Shared properties
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }
        #endregion
    }
}
