using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class InventoryInStorageViewModel : DistributrViewModelBase
    {
        public ObservableCollection<Stores> LineItems { get; set; }
        public RelayCommand TransferInventoryCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        public InventoryInStorageViewModel()
        {
            TransferInventoryCommand = new RelayCommand(Transfer);
            LoadedCommand = new RelayCommand(LoadStoredItems);
            LineItems = new ObservableCollection<Stores>();
        }

        private void Transfer()
        {
            using (var c = NestedContainer)
            {
                Using<IInventoryTransferPopUp>(c).ShowCommodityTransfer();
            }
        }

        private void LoadStoredItems()
        {
            LineItems.Clear();
             using (var c = NestedContainer)
             {
                 foreach (CommodityStorageNote item in Using<ICommodityStorageRepository>(c).GetAll() )
                 {
                     foreach (var itemLine in item.LineItems.Where( n => !n.LineItemStatus.Equals(SourcingLineItemStatus.Transfered)))
                     {
                         LineItems.Add(new Stores()
                                                       {
                                                           StoreName = item.DocumentRecipientCostCentre.Name,
                                                           CommodityName = itemLine.Commodity.Name,
                                                           CommodityGrade = itemLine.CommodityGrade.Name,
                                                           Weight = itemLine.Weight
                                                       });
                     }
                     
                 }
             }
        }

        public class Stores : ViewModelBase
        {
            public const string WeightPropertyName = "Weight";
            private decimal _weight = 0m;
            public decimal Weight
            {
                get
                {
                    return _weight;
                }

                set
                {
                    if (_weight == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(WeightPropertyName);
                    _weight = value;
                    RaisePropertyChanged(WeightPropertyName);
                }
            }

            public const string CommodityNamePropertyName = "CommodityName";
            private string _commodityName;
            public string CommodityName
            {
                get
                {
                    return _commodityName;
                }

                set
                {
                    if (_commodityName == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(CommodityNamePropertyName);
                    _commodityName = value;
                    RaisePropertyChanged(CommodityNamePropertyName);
                }
            }

            public const string StoreNamePropertyName = "StoreName";
            private string _storeName;
            public string StoreName
            {
                get
                {
                    return _storeName;
                }

                set
                {
                    if (_storeName == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(StoreNamePropertyName);
                    _storeName = value;
                    RaisePropertyChanged(StoreNamePropertyName);
                }
            }

            public const string CommodityGradePropertyName = "CommodityGrade";
            private string _commodityGrade;
            public string CommodityGrade
            {
                get
                {
                    return _commodityGrade;
                }

                set
                {
                    if (_commodityGrade == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(WeightPropertyName);
                    _commodityGrade = value;
                    RaisePropertyChanged(WeightPropertyName);
                }
            }

        }
    }
}
