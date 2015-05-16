using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Resources.Util;
using System.Linq;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.ViewModels.Admin.ReorderLevel
{
    public class ReorderLevelViewModel : DistributrViewModelBase
    {

        public ReorderLevelViewModel()
        {

            ReorderLevels = new ObservableCollection<ReorderLevelLineItem>();
        }

        public ObservableCollection<ReorderLevelLineItem> ReorderLevels { get; set; }
        public const string DistrbutorPropertyName = "Distrbutor";
        private string _distributor = "";
        public string Distrbutor
        {
            get
            {
                return _distributor;
            }

            set
            {
                if (_distributor == value)
                {
                    return;
                }

                var oldValue = _distributor;
                _distributor = value;
                RaisePropertyChanged(DistrbutorPropertyName);
            }
        }

        public const string TitlePropertyName = "Title";
        private string _title = "";
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                var oldValue = _title;
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        public const string LoadingStatusPropertyName = "LoadingStatus";
        private string _loadingStatus = "Loading ... 0%\nPlease wait.";
        public string LoadingStatus
        {
            get
            {
                return _loadingStatus;
            }

            set
            {
                if (_loadingStatus == value)
                {
                    return;
                }

                var oldValue = _loadingStatus;
                _loadingStatus = value;
                RaisePropertyChanged(LoadingStatusPropertyName);
            }
        }

        public const string LoadingPropertyName = "Loading";
        private bool _loading = true;
        public bool Loading
        {
            get
            {
                return _loading;
            }

            set
            {
                if (_loading == value)
                {
                    return;
                }

                var oldValue = _loading;
                _loading = value;
                RaisePropertyChanged(LoadingPropertyName);
            }
        }

        public void LoadReorderLevels()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Loading = true;
                ReorderLevels.Clear();
                var distributrId = Using<IConfigService>(c).Load().CostCentreId;
                Distrbutor = Using<ICostCentreRepository>(c).GetById(distributrId).Name;
                var distInventory = Using<IInventoryRepository>(c).GetByWareHouseId(distributrId);
                int countAll = Using<IReOrderLevelRepository>(c).GetCount();
                Title = GetLocalText(" sl.reorderlevel.title") /* "Product Reorder Levels for"*/+ " " +
                        Distrbutor;

                var items = Using<IReOrderLevelRepository>(c).GetAll().Where(n => n.DistributorId.Id == distributrId).OrderBy(
                    n => n.ProductId.Description)
                                                .Select((n, i) =>
                                                    {
                                                        var rl = new ReorderLevelLineItem
                                                            {
                                                                RowNumber = i + 1,
                                                                ProductId = n.ProductId.Id,
                                                                ProductCode = n.ProductId.ProductCode,
                                                                ProductName = n.ProductId.Description,
                                                                ReorderLevel = n.ProductReOrderLevel,
                                                                DateSet = n._DateLastUpdated.Date
                                                            };
                                                        Inventory inv =
                                                            distInventory.FirstOrDefault(
                                                                f => f.Product.Id == n.ProductId.Id);
                                                        rl.Available = inv == null ? 0 : inv.Balance;

                                                        ReorderLevels.Add(rl);

                                                        LoadingStatus = "Loading ...\nPlease wait.";
                                                        double percent = (((double) (i + 1))*100)/(double) countAll;
                                                        LoadingStatus = "Loading ... " + (int) percent +
                                                                        "%\nPlease wait.";

                                                        return rl;
                                                    }).ToList();

                //items.ToList().ForEach(ReorderLevels.Add);
                Loading = false;
            }
        }

        public string RaiseAlert()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var distributrId = Using<IConfigService>(c).Load().CostCentreId;
                var distInventory = Using<IInventoryRepository>(c).GetByWareHouseId(distributrId).ToList();
                var items =
                   Using<IReOrderLevelRepository>(c).GetAll().Where(n => n.DistributorId.Id == distributrId).OrderBy(
                        n => n.ProductId.Description).ToList();

                var belowReorderLevel = new List<ReorderLevelLineItem>();

                foreach (ReOrderLevel item in items)
                {
                    Inventory inv = distInventory.FirstOrDefault(n => n.Product.Id == item.ProductId.Id);
                    decimal invBal = inv == null ? 0 : inv.Balance;
                    if (invBal < item.ProductReOrderLevel)
                    {
                        belowReorderLevel.Add(new ReorderLevelLineItem
                            {
                                Available = invBal,
                                ReorderLevel = item.ProductReOrderLevel,
                                ProductName = item.ProductId.Description,
                                ProductCode = item.ProductId.ProductCode,
                                ProductId = item.ProductId.Id,
                                DateSet = item._DateLastUpdated.Date
                            });
                    }
                }

                string msg = string.Empty;
                if (belowReorderLevel.Count > 0)
                {
                    msg = belowReorderLevel.Aggregate(msg,
                                                      (current, item) =>
                                                      current +
                                                      ("\t- " + item.ProductName + ";  "
                                                       + GetLocalText("sl.reorderlevel.message.level")
                                                       /*"Reorder Level"*/
                                                       + ": " + item.ReorderLevel + ";  "
                                                       + GetLocalText("sl.reorderlevel.message.available")
                                                       /*"Available Inv"*/
                                                       + ": " + item.Available + "\n")
                        );
                }

                return msg;
            }
        }
    }

    public class ReorderLevelLineItem
    {
        public int RowNumber { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal Available { get; set; }
        public DateTime DateSet { get; set; }
    }
}
