using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Distributr.Core.ClientApp;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Distributr.WPF.Lib.ViewModels
{
    public abstract class DistributrViewModelBase : ViewModelBase
    {
        #region Declarations
        public IProductPackagingSummaryService _productPackagingSummaryService;

        protected int DefaultItemsToLoad
        {
            get
            {
                using (var c = NestedContainer)
                {

                    var recordsPerPageSetting =
                        Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.RecordsPerPage);

                    return recordsPerPageSetting != null
                               ? Convert.ToInt32(recordsPerPageSetting.SettingValue)
                               : 20;
                }
            }
        }

        #endregion
        protected DistributrViewModelBase()
        {
            NavigateCommand = new RelayCommand<string>(Navigate);
            ConfirmNavigatingAway = true;
            _productPackagingSummaryService = ObjectFactory.GetInstance<IProductPackagingSummaryService>();
          

        }

       
        #region Properties
        public bool ConfirmNavigatingAway { get; set; }

        private RelayCommand<Page> _loadPageCommand = null;
        public RelayCommand<Page> LoadPageCommand
        {
            get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand<Page>(LoadPage)); }
        }

        protected virtual void LoadPage(Page page) { }


        internal List<string> validationResultInfos = new List<string>();

        public const string NavigateCommandPropertyName = "NavigateCommand";
        private RelayCommand<string> _navigateCommand = null;
        public RelayCommand<string> NavigateCommand
        {
            get
            {
                return _navigateCommand;
            }

            set
            {
                if (_navigateCommand == value)
                {
                    return;
                }

                var oldValue = _navigateCommand;
                _navigateCommand = value;

                RaisePropertyChanged(NavigateCommandPropertyName);
            }
        }

        public const string CloseCommandPropertyName = "CloseCommand";
        private RelayCommand<object> _closeCommand = null;
        public RelayCommand<object> CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<object>(
                        this.CloseTab_Execute,
                        this.CloseTab_CanExecute
                        );
                }
                return _closeCommand;
            }
        }
         
        private RelayCommand<TextBox> _invalidateSpecialCharactersOnKeyUpCommand = null;
        public RelayCommand<TextBox> InvalidateSpecialCharactersOnKeyUpCommand
        {
            get
            {
                return _invalidateSpecialCharactersOnKeyUpCommand ??
                       (_invalidateSpecialCharactersOnKeyUpCommand =
                        new RelayCommand<TextBox>(InvalidateSpecialCharactersOnKeyUp));
            }
        }

        private RelayCommand<KeyEventArgs> _allowNumbersOnlyOnKeyDownCommand = null;
        public RelayCommand<KeyEventArgs> AllowNumbersOnlyOnKeyDownCommand
        {
            get
            {
                return _allowNumbersOnlyOnKeyDownCommand ??
                       (_allowNumbersOnlyOnKeyDownCommand =
                        new RelayCommand<KeyEventArgs>(AllowNumbersOnlyOnKeyDown));
            }
        }

        #endregion
    

        protected virtual void PutFocusOnControl(Control element)
        {
            if (element != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (System.Threading.ThreadStart)(() => element.Focus()));
        }

        protected static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        protected void SendLoggedInRequestMessage(bool[] loginVals)
        {
            //cn: 1 = loginSuccess?, 2 = isInitialLogin?
            Messenger.Default.Send<bool[]>(loginVals, "LoginMessage");
        }
       
        public void Navigate(string url)
        {
           SendNavigationRequestMessage(new Uri(url, UriKind.Relative));
        }
        protected void SendNavigationRequestMessage(Uri uri)
        {
            Messenger.Default.Send<Uri>(uri, "NavigationRequest");
        }
        public decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            int tmp = (int)Math.Truncate(step * value);
            return tmp / step;
        }
       
        public bool IsValid()
        {
            bool retVal = true;
            validationResultInfos.Clear();
            ValidationResultInfo vris = this.BasicValidation();
            if (!vris.IsValid)
            {
                vris.Results.ForEach(n => validationResultInfos.Add(n.ErrorMessage));

                var message = string.Join(Environment.NewLine, validationResultInfos);

                var app = GetConfigParams().AppId == Core.VirtualCityApp.Agrimanagr ? "Agrimanagr" : "Distributr";
                MessageBox.Show(message, app + ": Invalid Field(s)", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                retVal = false;
            }
           

            return retVal;
        }

        public bool IsValid(object entity)
        {
            bool retVal = true;
            validationResultInfos.Clear();
            ValidationResultInfo vris = entity.BasicValidation();
            if (!vris.IsValid)
            {
                vris.Results.ForEach(n => validationResultInfos.Add(n.ErrorMessage));

                var message = string.Join(Environment.NewLine, validationResultInfos);

                var app = GetConfigParams().AppId == Core.VirtualCityApp.Agrimanagr ? "Agrimanagr" : "Distributr";
                MessageBox.Show(message, app + ": Invalid Field(s)", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                retVal = false;
            }

            return retVal;
        }

        private void CloseTab_Execute(object control)
        {
            TabItem ti = control as TabItem;
            if (ti != null)
            {
                TabControl tabControl = ti.Parent as TabControl;
                try
                {
                    WebBrowser wb = (WebBrowser)ti.Content;
                    wb.Dispose();
                }
                catch { }

                ti.Content = null;
                tabControl.Items.Remove(ti);
            }
        }

        private bool CloseTab_CanExecute(object control)
        {
            TabItem ti = control as TabItem;
            if (ti != null)
            {
                TabControl tabControl = ti.Parent as TabControl;
                if (ti != tabControl.Items[0])
                    return ti.IsEnabled;
            }
            return false;
        }

       

        void InvalidateSpecialCharactersOnKeyUp(TextBox sender)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void AllowNumbersOnlyOnKeyDown(KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        #region CommonViewModelGetters

        protected string GetLocalText(string key)
        {
            return ObjectFactory.GetInstance<IMessageSourceAccessor>().GetText(key);
        }

        public Config GetConfigParams()
        {
            Config config;

            using (StructureMap.IContainer cont = NestedContainer)
            {
                config = Using<IConfigService>(cont).Load();
            }
            return config;
        }

        public ViewModelParameters GetConfigViewModelParameters()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                return Using<IConfigService>(cont).ViewModelParameters;
            }
        }

        public LineItemPricingInfo GetLineItemPricing(PackagingSummary ps, Guid outletId)
        {
            LineItemPricingInfo info;
            using (StructureMap.IContainer cont = NestedContainer)
            {
                info = Using<IDiscountProWorkflow>(cont).GetLineItemPricing(ps, outletId);
            }
            return info;
        }

        public object GetEntityById(Type entityType, Guid id)
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                var _orderService = Using<IMainOrderRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                ICostCentreRepository _costCentreRepository = Using<ICostCentreRepository>(container);
                IRouteRepository _routeService = Using<IRouteRepository>(container);
                ICommodityPurchaseRepository _commodityPurchaseRepository = Using<ICommodityPurchaseRepository>(container);
                ICommodityReceptionRepository _commodityReceptionRepository = Using<ICommodityReceptionRepository>(container);
                IReceivedDeliveryRepository _receivedDeliveryRepository = Using<IReceivedDeliveryRepository>(container);
               // ISourcingDocumentRepository _sourcingDocumentRepository = Using<ISourcingDocumentRepository>(container);

                if (entityType == typeof(User))
                {
                    var user = _userService.GetById(id);
                    return user;
                }
                if (entityType == typeof(Route))
                {
                    var route = _routeService.GetById(id);
                    return route;
                }
                if (entityType == typeof (MainOrder))
                {
                    var order = _orderService.GetById(id);
                    return order;
                }
                if(entityType == typeof(Product))
                {
                    Product product = Using<IProductRepository>(container).GetById(id);
                    return product;
                }
                if (entityType == typeof(CommodityPurchaseNote))
                {
                    CommodityPurchaseNote purchaseNote = _commodityPurchaseRepository.GetById(id) as CommodityPurchaseNote;
                    return purchaseNote;
                }
                if (entityType == typeof(CommodityReceptionNote))
                {
                    CommodityReceptionNote receptionNote = _commodityReceptionRepository.GetById(id) as CommodityReceptionNote;
                    return receptionNote;
                }
                if (entityType == typeof(ReceivedDeliveryNote))
                {
                    ReceivedDeliveryNote receivedDeliveryNote = _receivedDeliveryRepository.GetById(id) as ReceivedDeliveryNote;
                    return receivedDeliveryNote;
                }
                if (entityType == typeof(SourcingDocument))
                {
                  // SourcingDocument receptionNote = _sourcingDocumentRepository.GetById(id);
                  // return receptionNote;
                }
                
                if(entityType == typeof(CostCentre))
                {
                    CostCentre cc = _costCentreRepository.GetById(id);
                    return cc;
                }
            }
            return null;
        }

      

        public string GetDocumentReference(string docType, string orderDocRef)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                return Using<IGetDocumentReference>(c).GetDocReference(docType,orderDocRef);
            }
        }

        public void AddLogEntry(string module, string activity)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IAuditLogWFManager>(c).AuditLogEntry(module, activity);
            }
        }

        public string SplitByCaps(string strInput)
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                return Using<IOtherUtilities>(container).BreakStringByUpperCB(strInput);
            }
        }

        #endregion

        #region Nested Container Helpers
        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }
        protected NestedServices GetNestedServices()
        {
            return new NestedServices(NestedContainer);
        }

        /// <summary>
        /// Helper class to abstract ugly NestedContainer bits
        /// </summary>
        public class NestedServices : IDisposable
        {
            public NestedServices(IContainer container)
            {
                Container = container;
                ConfigService = container.GetInstance<IConfigService>();
                SetupApplication = container.GetInstance<ISetupApplication>();
                UserRepository = container.GetInstance<IUserRepository>();
                OtherUtilities = container.GetInstance<IOtherUtilities>();
                SettingsRepository = container.GetInstance<ISettingsRepository>();

            }
            public IContainer Container { get; private set; }
            
            //Commonly used services in viewmodels
            public IConfigService ConfigService { get; private set; }
            public IUserRepository UserRepository { get; private set; }
            public ISetupApplication SetupApplication { get; private set; }
            public IOtherUtilities OtherUtilities { get; private set; }
            public ISettingsRepository SettingsRepository { get; private set; }
            public T Using<T>()
            {
                return Container.GetInstance<T>();
            }

            public void Dispose()
            {
               Container.Dispose();
            }
        }

        #endregion

    }

    public class MasterDataDropDownValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MasterEntity me = value as MasterEntity;
            if (me != null && me.Id == Guid.Empty)
            {
                var type = me.GetType();
                return new ValidationResult("Must select " + type.ToString().Split('.').Last() + " from dropdown list");
            }

            return null;
        }
    }
}
