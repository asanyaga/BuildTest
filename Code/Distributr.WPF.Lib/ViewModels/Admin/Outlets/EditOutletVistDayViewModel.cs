using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Outlets
{
    public class EditOutletVistDayViewModel:DistributrViewModelBase
    {

        public EditOutletVistDayViewModel()
        {
            VistDayItems = new ObservableCollection<OutletVistDay>();
            SaveCommand = new RelayCommand(Save);
            DoLoadCommand = new RelayCommand(DoLoad);
            LoadOutletCommand = new RelayCommand(LoadOutlet);
            RouteLookUpList = new ObservableCollection<RouteLookUp>();
            OutletLookUpList = new ObservableCollection<OutletLookUp>();
            LoadVisitItemCommand = new RelayCommand(LoadVisitItem);
            RouteDropDownOpenedCommand = new RelayCommand(RouteDropDownOpened);
            OutletDropDownOpenedCommand = new RelayCommand(OutletDropDownOpened);
        }

        private void OutletDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                if (SelectedRoute != null)
                {
                    SelectedOutlet = Using<IItemsLookUp>(container).SelectOutletByRoute(SelectedRoute.Id); //??default;
                }
                if (SelectedRoute == null || SelectedRoute.Name == "----Selected Route----" )
                    MessageBox.Show( " Select Route first" , "Distributr" ,MessageBoxButton.OKCancel);
                
                LoadVisitItem();

            }
        }

        private void LoadOutlet()
        {
            OutletLookUpList.Clear();
            var outletLookUp = new OutletLookUp {Id = Guid.Empty, Name = "--------Select Outlet --------"};
            OutletLookUp = outletLookUp;
            OutletLookUpList.Add(outletLookUp);
            if(RouteLookUp !=null && RouteLookUp.Id!=Guid.Empty)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    Using<ICostCentreRepository>(c).GetAll()
                                      .OfType<Outlet>()
                                      .Where(n => n._Status == EntityStatus.Active && n.Route.Id == RouteLookUp.Id)
                                      .ToList()
                                      .ForEach(t => OutletLookUpList.Add(new OutletLookUp {Id = t.Id, Name = t.Name}));
                }
            }
         
        }

        private void DoLoad()
        {
            VistDayItems.Clear();
            RouteLookUpList.Clear();
            var routeLookUp = new RouteLookUp {Id = Guid.Empty, Name = "--------Select route --------"};
            RouteLookUp = routeLookUp;
            RouteLookUpList.Add(routeLookUp);
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IRouteRepository>(c).GetAll().ToList().ForEach(t => RouteLookUpList.Add(new RouteLookUp {Id = t.Id, Name = t.Name}));
            }
        }

        private async void Save()
        {
            if (SelectedRoute == null)//RouteLookUp
            {
                MessageBox.Show("A Route has to be selected first", "Outlet Visit Day", MessageBoxButton.OK);
                return;
            }
            else if (SelectedOutlet == null)//OutletLookUp
            {
                MessageBox.Show("An Outlet has to be selected first", "Outlet Visit Day", MessageBoxButton.OK);
                return;
            }

            else if (!VistDayItems.Any(p => p.IsVistDay == true))
            {
                MessageBox.Show("Atleast one day has to be checked", "Outlet Visit Day", MessageBoxButton.OK);
                return;

            }
            else if(EffectiveDate<DateTime.Today)
            {
                MessageBox.Show("Visit day effective date cannot be in the past", "Outlet Visit Day", MessageBoxButton.OK);
                return;
            }
            else
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    DateTime d = DateTime.Now;
                    try
                    {
                        Guid Id = Guid.NewGuid();
                        List<OutletVisitDayItem> itemsToSave = new List<OutletVisitDayItem>();
                        var visitDays = VistDayItems.Where(n => n.IsVistDay).ToList();
                        foreach (var item in visitDays)
                        {

                            var itemtosave = new OutletVisitDayItem
                                                 {
                                                     MasterId = Guid.NewGuid(),
                                                     Day = (DayOfWeek) item.Id,
                                                     EffectiveDate = EffectiveDate,
                                                     OutletId = SelectedOutlet.Id,//OutletLookUp.Id,
                                                     StatusId = (int) EntityStatus.Active,
                                                 };

                            itemsToSave.Add(itemtosave);
                        }
                        response = await proxy.OutletVisitAddAsync(itemsToSave);
                        MessageBox.Show(response.ErrorInfo, "Outlet Visit Day", MessageBoxButton.OK,MessageBoxImage.Information);
                        RouteLookUp = RouteLookUpList.FirstOrDefault(n => n.Id == Guid.Empty);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(response.ErrorInfo, "Distributr: Outlet Visit Day", MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                    }


                    
                }
            }
        }

        private void LoadVisitItem()
        {
            VistDayItems.Clear();
            if (SelectedOutlet != null && SelectedOutlet.Id != Guid.Empty)
            {
               
                foreach (var bindingFlag in GetEnumValues<DayOfWeek>())
                {
                    VistDayItems.Add(new OutletVistDay { Id = (int)bindingFlag, IsVistDay = false, Name = bindingFlag.ToString() });
                }

                using (StructureMap.IContainer c = NestedContainer)
                {
                    List<OutletVisitDay> vist = Using<IOutletVisitDayRepository>(c)
                        .GetAll().Where(p => p.Outlet.Id == SelectedOutlet.Id && p.EffectiveDate <= DateTime.Now).ToList();

                    foreach (var item in vist)
                    {
                        var i = VistDayItems.FirstOrDefault(p => p.Id == (int) item.Day);
                        if (i != null)
                        {
                            i.IsVistDay = true;
                        }
                    }
                }
            }

        }
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return typeof(T).GetFields()
                .Where(x => x.IsLiteral)
                .Select(field => (T)field.GetValue(null));
        }


         private void RouteDropDownOpened()
         {

             using (var container = NestedContainer)
             {
                SelectedRoute=Using<IItemsLookUp>(container).SelectRoute();//??default;
                
             }

         }

        public ObservableCollection<RouteLookUp> RouteLookUpList { get; set; }
        public ObservableCollection<OutletLookUp> OutletLookUpList { get; set; }

        
        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = new Outlet(Guid.Empty){Name = "----Select Outlet-----"};
        public Outlet SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOutletPropertyName);
                _selectedOutlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }
        
        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = new Route(Guid.Empty){Name = "----Selected Route----"};
        public Route SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }

            set
            {
                if (_selectedRoute == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedRoutePropertyName);
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        public const string RouteLookUpPropertyName = "RouteLookUp";
        private RouteLookUp _RouteLookUp = null;
        public RouteLookUp RouteLookUp
        {
            get
            {
                return _RouteLookUp;
            }

            set
            {
                if (_RouteLookUp == value)
                {
                    return;
                }

                _RouteLookUp = value;
                RaisePropertyChanged(RouteLookUpPropertyName);
            }
        }

        public const string OutletLookUpPropertyName = "OutletLookUp";
        private OutletLookUp _OutletLookUp = null;
        public OutletLookUp OutletLookUp
        {
            get
            {
                return _OutletLookUp;
            }

            set
            {
                if (_OutletLookUp == value)
                {
                    return;
                }

                _OutletLookUp = value;
                RaisePropertyChanged(OutletLookUpPropertyName);
            }
        }

       
        public const string EffectiveDatePropertyName = "EffectiveDate";
        private DateTime _effectiveDate = DateTime.Now;
        public DateTime EffectiveDate
        {
            get
            {
                return _effectiveDate;
            }

            set
            {
                if (_effectiveDate == value)
                {
                    return;
                }

                _effectiveDate = value;
                RaisePropertyChanged(EffectiveDatePropertyName);
            }
        }

        public ObservableCollection<OutletVistDay> VistDayItems { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DoLoadCommand { get; set; }
        public RelayCommand LoadOutletCommand { get; set; }
        public RelayCommand LoadVisitItemCommand { get; set; }
        public RelayCommand RouteDropDownOpenedCommand { get; set; }
        public RelayCommand OutletDropDownOpenedCommand { get; set; }

    }

    public class RouteLookUp
    {
        public Guid Id { set; get; }
        public string Name {set;get;}
    }

    public class OutletLookUp
    {
        public Guid Id { set; get; }
        public string Name {set;get;}
       
    }
    public class OutletVistDay : DistributrViewModelBase
    {
       
        public const string IdPropertyName = "Id";
        private int _id = 0;
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

       
        public const string NamePropertyName = "Name";
        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        
        public const string IsVistDayPropertyName = "IsVistDay";
        private bool _vistDay = false;
        public bool IsVistDay
        {
            get
            {
                return _vistDay;
            }

            set
            {
                if (_vistDay == value)
                {
                    return;
                }

                _vistDay = value;
                RaisePropertyChanged(IsVistDayPropertyName);
            }
        }
    }
}
