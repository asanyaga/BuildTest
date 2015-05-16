using System;
using System.Windows.Controls;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    public class MainViewModel : MiddleWareViewModelBase
    {
      public MainViewModel()
      {
          ImportCommand=new RelayCommand<MenuItem>(ImportMasterData);
          Filepath = FileUtility.GetFilePath("masterdataimportpath");
      }
      

      private RelayCommand _closweindowrCommand;
      public RelayCommand CloseChildWindowCommand
      {
          get { return _closweindowrCommand ?? (_closweindowrCommand = new RelayCommand(ClosWindow)); }
      }

      private void ClosWindow()
      {
          RequestClose(this, null);
      }
      


      private RelayCommand<MenuItem> _settingsCommand;
      public RelayCommand<MenuItem> SettingsCommand
      {
          get { return _settingsCommand ?? (_settingsCommand = new RelayCommand<MenuItem>(LoadSettings)); }
      }

      private void LoadSettings(MenuItem menuItem)
      {
          if (!string.IsNullOrEmpty(menuItem.Name))
          {
              String url = "";

              switch (menuItem.Name.ToLower())
              {
                  case "workingdir":
                      using(var c=NestedContainer)
                      {
                         // Using<IShowWorkingFolderPopUp>(c).ShowPopUp();
                      }
                      
                      break;
              }
            
          }
      }

      public RelayCommand<MenuItem> ImportCommand { get; set; }

      private void ImportMasterData(MenuItem menuItem)
      {
          if (!string.IsNullOrEmpty(menuItem.Name))
          {
              String url = "";

              switch (menuItem.Name.ToLower())
              {
                  case "farmers":
                      url = "/views/Listfarmers.xaml";
                      break;
                  case "commoditytypes":
                      url = "/views/ListCommodityTypes.xaml";
                      break;
                  case "commodityownertypes":
                      url = "/views/ListCommodityOwnerTypes.xaml";
                      break;
                  case "commoditysuppliers":
                      url = "/views/ListCommoditySuppliers.xaml";
                      break;
                  case "commodity":
                      url = "/views/ListCommodities.xaml";
                      break;
              }
              NavigateCommand.Execute(url);
          }
      }

      
      public event EventHandler RequestClose = (s, e) => { };
      public const string FilepathPropertyName = "Filepath";
      private string _filepath = "";
      public string Filepath
      {
          get
          {
              return _filepath;
          }

          set
          {
              if (_filepath == value)
              {
                  return;
              }

              RaisePropertyChanging(FilepathPropertyName);
              _filepath = value;
              RaisePropertyChanged(FilepathPropertyName);

          }
      }
      public const string ProsessMessagePropertyName = "ProsessMessage";
      private string _prosessMessage = "";
      public string ProsessMessage
      {
          get { return _prosessMessage; }

          set
          {
              if (_prosessMessage == value)
              {
                  return;
              }

              RaisePropertyChanging(ProsessMessagePropertyName);
              _prosessMessage = value;
              RaisePropertyChanged(ProsessMessagePropertyName);
          }
      }
      
    }
}
