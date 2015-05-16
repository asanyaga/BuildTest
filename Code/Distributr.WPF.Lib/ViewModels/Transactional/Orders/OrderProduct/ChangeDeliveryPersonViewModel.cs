using System;
using System.Collections.ObjectModel;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct
{
    
    public class ChangeDeliveryPersonViewModel : DistributrViewModelBase
    {
        public event EventHandler RequestClose = (s, e) => { };
        public RelayCommand OkCommand { get; set; }
        public RelayCommand<Route> LoadCommand { get; set; }
        public ObservableCollection<DistributorSalesman> SalesmanLookUp { get; set; }
        public ChangeDeliveryPersonViewModel()
        {
            OkCommand = new RelayCommand(Ok);
            LoadCommand = new RelayCommand<Route>(LoadView);
            SalesmanLookUp = new ObservableCollection<DistributorSalesman>();
        }

        private void LoadView(Route route)
        {
            SalesmanLookUp.Clear();
            if (route == null)
                return;
            using(var c = NestedContainer)
            {
                var salemanAssigned = Using<ISalesmanRouteRepository>(c).GetAll().Where(s => s.Route.Id == route.Id).Select(s=>s.DistributorSalesmanRef.Id).Distinct();
                foreach(var item in salemanAssigned)
                {
                    var salesman = Using<ICostCentreRepository>(c).GetById(item) as DistributorSalesman;
                    if (salesman != null)
                        SalesmanLookUp.Add(salesman);
                }
            }
           // SelectedSalesman = DefaultSalesman;

        }
        public DistributorSalesman GetSalesman()
        {
            return SelectedSalesman;
        }

        private void Ok()
        {
            RequestClose(this, EventArgs.Empty);
        }
        /// <summary>
        /// The <see cref="SelectedSalesman" /> property's name.
        /// </summary>
        public const string SelectedSalesmanPropertyName = "SelectedSalesman";

        private DistributorSalesman _distributorsalesman = null;

        /// <summary>
        /// Sets and gets the SelectedSalesman property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DistributorSalesman SelectedSalesman
        {
            get
            {
                return _distributorsalesman;
            }

            set
            {
                if (_distributorsalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanPropertyName);
                _distributorsalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }
    }
}