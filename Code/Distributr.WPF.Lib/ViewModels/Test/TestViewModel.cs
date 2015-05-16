using System.Collections.ObjectModel;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using GalaSoft.MvvmLight;
using Distributr.Core.Domain.Master.ProductEntities;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WPF.Lib.ViewModels.Test
{
    public class TestViewModel : DistributrViewModelBase
    {
        

        
        public TestViewModel()
        {
          

            LoadProductBrands();
            Run = new RelayCommand(DoRun);
            cTest = new RelayCommand(cDocTest);
            Name = "Hello";
        }

        void DoRun()
        {
            //string test = "1";
            
            Name = "Called by DoRun.";
        }

        
        void cDocTest()
        {
            Name = "Hello chris n. :)";
            LoadAreas();
        }

        void LoadProductBrands()
        {
            using (var c = NestedContainer)
            {
                var items =Using<IProductBrandRepository>(c).GetAll();
                ProductBrands = new ObservableCollection<ProductBrand>();
                foreach (var item in items)
                {
                    ProductBrands.Add(item);
                }
            }
        }

        void AddTestProducts()
        {
        }

        void LoadAreas()
        {
            using (var c = NestedContainer)
            {
                var items =Using<IAreaRepository>(c).GetAll();
                Areas = new ObservableCollection<Area>();
                foreach (var area in Areas)
                {
                    Areas.Add(area);
                }
            }

        }

        public ObservableCollection<ProductBrand> ProductBrands { get; set; }

        
        public ObservableCollection<Area> Areas { get; set; }

        public RelayCommand Run { get; private set; }
        
        public RelayCommand cTest { get; private set; }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
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

                var oldValue = _name;
                _name = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);

            }
        }
    }
}
