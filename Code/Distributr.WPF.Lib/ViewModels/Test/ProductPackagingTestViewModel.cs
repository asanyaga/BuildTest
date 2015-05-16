using Distributr.Core.Repository.Master.ProductRepositories;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Master.ProductEntities;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Test
{
    public class ProductPackagingTestViewModel:DistributrViewModelBase
    {
        public ProductPackagingTestViewModel()
        {
            LoadProductPackagings();
            Name = "Product Packaging";
            Run = new RelayCommand(DoRun);
        }

        void DoRun()
        {
            //string tt = "1";
        }

        void LoadProductPackagings()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                var pItems = Using<IProductPackagingRepository>(cont).GetAll();
                productPackaging = new ObservableCollection<ProductPackaging>();

                foreach (var item in pItems)
                {
                    productPackaging.Add(item);
                }
            }
        }
        public ObservableCollection<ProductPackaging> productPackaging { get; set; }

        public RelayCommand Run { get; private set; }

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
