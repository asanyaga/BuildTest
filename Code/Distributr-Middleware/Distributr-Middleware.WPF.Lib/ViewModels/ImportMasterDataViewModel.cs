using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Distributr.Core.Domain.Master;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
    public class ImportMasterDataViewModel : MiddleWareViewModelBase
    {
        private IMasterDataImportService _masterDataImportService;
        
        private Dictionary<MasterDataItem, List<MasterDataValidationAndImportResponse>> _dictionary;

        public ImportMasterDataViewModel()
        {
            MasterDataItemsList = new ObservableCollection<MasterDataItem>();
            MasterDataFilepath = FileUtility.GetFilePath("masterdataimportpath");
            _masterDataImportService = ObjectFactory.GetInstance<IMasterDataImportService>();
         
            _dictionary=new Dictionary<MasterDataItem, List<MasterDataValidationAndImportResponse>>();
        }

        private RelayCommand _importSelectedMasterdata;

        public RelayCommand ImportMasterdata
        {
            get { return _importSelectedMasterdata ?? (new RelayCommand(ImportSelected)); }
        }

        private RelayCommand _loadPageCommand;
        public RelayCommand LoadPageCommand
        {
            get { return _loadPageCommand ?? new RelayCommand(Loadpage); }
        }
        private RelayCommand _cancelCommand;
        public RelayCommand CancelImportCommand
        {
            get { return _cancelCommand ?? (new RelayCommand(Refresh)); }
        }


        
        private RelayCommand<CheckBox> _importAllMasterdata;
        public RelayCommand<CheckBox> ImportAllMasterdata
        {
            get { return _importAllMasterdata ?? (new RelayCommand<CheckBox>(ImportAll)); }
        }

        private RelayCommand _viewSelectedErrors;
        public RelayCommand ViewSelectedErriorCommand
        {
            get { return _viewSelectedErrors ?? (new RelayCommand(LoadErrors)); }
        }
        


        private void Loadpage()
        {
            LoadMasterDataCollective();
        }

        private void Refresh()
        {
            LoadMasterDataCollective();
            _dictionary.Clear();
            SelectedMasterData = null;
        }
        protected virtual async void ImportAll(CheckBox checkBox)
        {
            if(checkBox==null)return;

            var importAll=checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
            if(importAll)
            {
                var confirm =
                    MessageBox.Show(
                        "Are you sure you want to import all master data?\n " +
                        "This process can take upto several hours depending on import file sizes and your network speed\n"
                        +"It's STRONGLY recommended you import one masterdata at a time","Middleware",MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes;
                if(confirm)
                {
                    var selected = MasterDataItemsList.OrderBy(p => p.RowNumber).ToList();
                    _dictionary.Clear();
                    foreach (var masterDataItem in selected)
                    {
                        SelectedMasterData = masterDataItem;
                       var success=await ImportMasterDataItem();
                        if(!success)
                        {
                            MessageBox.Show("One or more of the masterdata failed to import,Try selecting one masterdata at a time");
                            break;
                        }
                    }
                }
            }
          
        }

        protected virtual void ImportSelected()
        {
            ImportMasterDataItem();
        }
      
        private async Task<bool> ImportMasterDataItem()
        {
            return await Task.Run(async() =>
                                {


                                    using (var c = NestedContainer)
                                    {
                                        bool isSuccess = false;
                                        var selected = SelectedMasterData;
                                        if (selected == null)
                                        {
                                            MessageBox.Show("Select at least one master data type", "Middleware Service",
                                                            MessageBoxButton.OK, MessageBoxImage.Stop);
                                            return isSuccess;
                                        }
                                        var service = Using<IMasterDataImportService>(c);
                                        
                                        var success =
                                            await
                                            service.Import(MasterDataFilepath,
                                                           (MasterDataCollective)
                                                           Enum.Parse(typeof (MasterDataCollective), selected.MasterData));

                                        if (success)
                                        {
                                            var imports = service.GetImportItems().ToList();
                                            if (imports.Count <= 0)
                                            {
                                                MessageBox.Show("No master data items to upload",
                                                                "Distributr Middleware", MessageBoxButton.OK);
                                                LoadMasterDataCollective();
                                                return true;

                                            }

                                            try
                                            {

                                                var responses = await _masterDataImportService.Upload(imports);


                                                if (responses.ValidationResults == null || responses.Result == null ||
                                                    responses.Result.Contains("Error"))
                                                {
                                                    UpdateSuccessMessage(false, 1);
                                                    var error =responses !=null&&!string.IsNullOrEmpty(responses.ResultInfo)?responses.ResultInfo:
                                                        string.Format(
                                                            "There is an unknown error response from server..import {0} failed",
                                                            selected.MasterData);

                                                    MessageBox.Show(error, "Distributr Middleware", MessageBoxButton.OK);
                                                    return isSuccess;

                                                }
                                                if (responses != null && responses.ValidationResults.Any())
                                                {

                                                    _dictionary.Add(selected,
                                                                    new List<MasterDataValidationAndImportResponse>()
                                                                        {responses});
                                                    if (responses.ValidationResults.Any(n => !n.IsValid))
                                                    {
                                                        UpdateSuccessMessage(false,
                                                                             responses.ValidationResults.Count(
                                                                                 n => !n.IsValid));

                                                    }
                                                    else
                                                    {
                                                        UpdateSuccessMessage(true);
                                                        isSuccess = true;
                                                    }
                                                }
                                                else if (responses != null && responses.Result == "Success" &&
                                                         (responses.ValidationResults.Count == 0 ||
                                                          responses.ValidationResults.All(n => n.IsValid)))
                                                {
                                                    UpdateSuccessMessage(true);
                                                    isSuccess = true;
                                                }
                                            }
                                            catch (AggregateException ex)
                                            {
                                                var errors = ex.InnerExceptions.Aggregate(string.Empty,
                                                                                          (current, error) =>
                                                                                          current + error);
                                                MessageBox.Show(errors, "", MessageBoxButton.OK, MessageBoxImage.Error);
                                                return isSuccess;
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Error=>" + ex.Message, "Distributr Middleware",
                                                                MessageBoxButton.OK);
                                                return isSuccess;
                                            }

                                        }
                                        return isSuccess;
                                    }
                                });
            
            
        }
      
        void LoadErrors()
        {
            var selected = SelectedMasterData;
            if(selected==null)return;
            List<MasterDataValidationAndImportResponse> vResults;
            _dictionary.TryGetValue(selected, out vResults);
            if(vResults !=null && vResults.Any())
            {
                var res = vResults.SelectMany(p => p.ValidationResults).AsEnumerable();
                ViewErrors(res);
            }
            else
            {
                MessageBox.Show("No errors to display");
            }
        }
       void ViewErrors(IEnumerable<StringifiedValidationResult> vResults)
        {
            Application.Current.Dispatcher.BeginInvoke(
                  new Action(
                      delegate
                      {
                          using (var cont = NestedContainer)
                          {
                              var invalids = vResults.Where(n => !n.IsValid).ToList();
                              Using<IImportValidationPopUp>(cont).ShowPopUp(invalids);
                          }
                      }));
        }
        void UpdateSuccessMessage(bool isSuccess,int errorCount=0)
        {
            Application.Current.Dispatcher.BeginInvoke(
               new Action(delegate
               {
                   if(SelectedMasterData !=null)
                   {
                       SelectedMasterData.ImageProcessPath = isSuccess ? "Success" : string.Format("Errors{0}",errorCount);
                       SelectedMasterData.ShowImportResult = Visibility.Visible;
                   }
               }));
           

        }
        static internal ImageSource doGetImageSourceFromResource(bool isSuccess)
        {
            string psAssemblyName=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string psResourceName = isSuccess ? "sync2.jpg" : "cancel.jpg";
            Uri oUri = new Uri("/"+psAssemblyName + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

       public ObservableCollection<MasterDataItem> MasterDataItemsList { get; set; } 

       private void LoadMasterDataCollective()
       {
           Application.Current.Dispatcher.BeginInvoke(
               new Action(delegate
                              {
                                  MasterDataItemsList.Clear();
                                  _dictionary.Clear();
                                  var allItems = new List<string>()
                                                     {
                                                         "Country",
                                                         "Region",
                                                         "Area",
                                                         "Province",
                                                         "District",
                                                         "OutletCategory",
                                                         "OutletType",
                                                         "Bank",
                                                         "BankBranch",
                                                         "Supplier",
                                                         "PricingTier",
                                                         "VatClass",
                                                         "ProductType",
                                                         "ProductBrand",
                                                         "ProductFlavour",
                                                         "ProductPackagingType",
                                                         "SaleProduct",
                                                         "ProductPackaging",
                                                         "Pricing",
                                                         "DiscountGroup",
                                                         "ProductGroupDiscount",
                                                         "PromotionDiscount",
                                                         "Distributor",
                                                         "DistributorSalesman",
                                                         "Route",
                                                         "Outlet"
                                                     };
                                     
                                  if (string.IsNullOrEmpty(SearchText))
                                  {
                                      allItems.Select(Map).ToList().ForEach(MasterDataItemsList.Add);
                                  }
                                  else
                                  {

                                      allItems =
                                          allItems.Where(
                                              p =>p.StartsWith(SearchText.ToLower()) ||
                                              p.ToLower().Contains(SearchText.ToLower()))
                                              .ToList();
                                      allItems.Select(Map).ToList().ForEach(MasterDataItemsList.Add);

                                  }

                              }));
       }

       private MasterDataItem Map(string n,int index)
       {
           if (string.IsNullOrEmpty(n)) return null;
           return new MasterDataItem()
                      {
                          Description = n,
                          MasterData = n,
                          RowNumber = index + 1,
                          ShowImportResult = Visibility.Collapsed,
                          ImageProcessPath ="False"
                      };
       }
        
       public const string SearchTextPropertyName = "SearchText";
       private string _searchText = "";
       protected string SearchText
       {
           get { return _searchText; }

           set
           {
               if (_searchText == value)
               {
                   return;
               }

               RaisePropertyChanging(SearchTextPropertyName);
               _searchText = value;
               RaisePropertyChanged(SearchTextPropertyName);
           }
       }

       public const string SelectedMasterDataPropertyName = "SelectedMasterData";
       private MasterDataItem _selectedMasterData = null;
       public MasterDataItem SelectedMasterData
       {
           get { return _selectedMasterData; }

           set
           {
               if (_selectedMasterData == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedMasterDataPropertyName);
               _selectedMasterData = value;
               RaisePropertyChanged(SelectedMasterDataPropertyName);
           }
       }




       public void ReceiveMessage(string obj)
       {
           ActivityMessage = obj;
           FileUtility.LogCommandActivity(obj);
       }
    }

    public class MasterDataItem:ViewModelBase
    {
        public int RowNumber { get; set; }
        public string MasterData { get; set; }
        public string Description { get; set; }

        public const string ImageProcessPathPropertyName = "ImageProcessPath";
        private string _sucessImage;
        public string ImageProcessPath
        {
            get { return _sucessImage; }

            set
            {
                if (_sucessImage == value)
                {
                    return;
                }

                RaisePropertyChanging(ImageProcessPathPropertyName);
                _sucessImage = value;
                RaisePropertyChanged(ImageProcessPathPropertyName);
            }
        }

        public const string ShowImportResultPropertyName = "ShowImportResult";
        private Visibility  _showResult=Visibility.Collapsed;
        public Visibility ShowImportResult
        {
            get { return _showResult; }

            set
            {
                if (_showResult == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowImportResultPropertyName);
                _showResult = value;
                RaisePropertyChanged(ShowImportResultPropertyName);
            }
        }
        

        public const string IsCheckedPropertyName = "IsChecked";
        private bool  _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);

            }
        }
    }
}
