using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;

namespace Distributr.WPF.Lib.ViewModels.Admin.WeighingScales
{
    public class ListWeighingScaleViewModel : ListingsViewModelBase
    {
        private PagenatedList<WeighScale> _pagedList;
        private IDistributorServiceProxy _proxy;

        public ListWeighingScaleViewModel()
        {
            WeighingScalesList = new ObservableCollection<VMWeighingScaleItem>();
        }

        #region properties

        public ObservableCollection<VMWeighingScaleItem> WeighingScalesList { get; set; }

        public const string CanManagePropertyName = "CanManage";
        private bool _canManage = false;

        public bool CanManage
        {
            get { return _canManage; }

            set
            {
                if (_canManage == value)
                {
                    return;
                }

                RaisePropertyChanging(CanManagePropertyName);
                _canManage = value;
                RaisePropertyChanged(CanManagePropertyName);
            }
        }

        public const string SelectedWeighingScalePropertyName = "SelectedWeighingScale";
        private VMWeighingScaleItem _selectedWeighingScale = null;

        public VMWeighingScaleItem SelectedWeighingScale
        {
            get { return _selectedWeighingScale; }

            set
            {
                if (_selectedWeighingScale == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedWeighingScalePropertyName);
                _selectedWeighingScale = value;
                RaisePropertyChanged(SelectedWeighingScalePropertyName);
            }
        }

        #endregion

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadWeighingScalesList();
        }

        private void LoadWeighingScalesList()
        {
            Application.Current.Dispatcher.BeginInvoke(
               new Action(
                   delegate
                   {
                       using (var c = NestedContainer)
                       {
                           var list = Using<IEquipmentRepository>(c).GetAll(ShowInactive).OfType<WeighScale>()
                               .Where(n =>
                                      (n.Name.ToLower().Contains(SearchText.ToLower()) ||
                                       n.Code.ToLower().Contains(SearchText.ToLower())));
                           
                           list = list.OrderBy(n => n.Name).ThenBy(n => n.Code);

                           _pagedList = new PagenatedList<WeighScale>(list.AsQueryable(), CurrentPage,
                                                                             ItemsPerPage,
                                                                             list.Count());
                           WeighingScalesList.Clear();


                           var devices = from equipment in XElement.Load(@"ScalePrinterAssetsDB.xml").Elements("Equipment") select equipment;

                           _pagedList.Select(
                               (scale, i) =>
                               Map(scale, i, devices.FirstOrDefault(n => Guid.Parse(n.Element("Id").Value) == scale.Id)))
                               .ToList().ForEach(WeighingScalesList.Add);

                           UpdatePagenationControl();
                       }
                   }));
        }

        private VMWeighingScaleItem Map(WeighScale weighingScale, int index, XElement xlmEquipConfig)
        {
            var mapped = new VMWeighingScaleItem { WeighingScale = weighingScale, RowNumber = index+1 };
            if (weighingScale._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            if (weighingScale._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            if (xlmEquipConfig != null) mapped = MapConfig(mapped, xlmEquipConfig);

            return mapped;
        }

        VMWeighingScaleItem MapConfig(VMWeighingScaleItem weighingScale, XElement xlmEquipConfig)
        {
            if (xlmEquipConfig.Element("EquipmentType").Value == EquipmentType.WeighingScale.ToString())
            {
                LocalEquipmentConfig settings = new LocalEquipmentConfig
                                                 {
                                                     Id = Guid.Parse(xlmEquipConfig.Element("Id").Value),
                                                     Code = xlmEquipConfig.Element("Code").Value,
                                                     Name = xlmEquipConfig.Element("Name").Value,
                                                     Port = xlmEquipConfig.Element("Port").Value,
                                                     Parity = xlmEquipConfig.Element("Parity").Value,
                                                     EquipmentType = xlmEquipConfig.Element("EquipmentType").Value,
                                                     BaudRate = xlmEquipConfig.Element("BaudRate").Value,
                                                     DataBits = xlmEquipConfig.Element("DataBits").Value,
                                                     Model = xlmEquipConfig.Element("Model").Value,
                                                 };
                weighingScale.ConfigSettings = settings;
                }
            return weighingScale;
        }

        private void Setup()
        {
            CanManage = true;
            PageTitle = "List Containers";
        }

        protected override void EditSelected()
        {
            if (SelectedWeighingScale != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/weighingscales/editweighingscale.xaml?" + SelectedWeighingScale.WeighingScale.Id,
                            UriKind.Relative));
        }

        protected override async void ActivateSelected()
        {
            using (var c = NestedContainer)
            {


                string action = SelectedWeighingScale.WeighingScale._Status == EntityStatus.Active
                             ? "deactivate"
                             : "activate";
                if (
                        MessageBox.Show("Are you sure you want to " + action + " this weighing scale?",
                                        "Agrimanagr: Activate Container", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.Cancel) return;

                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedWeighingScale == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.EquipmentActivateOrDeactivateAsync(SelectedWeighingScale.WeighingScale.Id);

                if(response.Success)
                {
                    if (action == "deactivate")
                    {
                        if (DeleteDeviceLocalSettings(SelectedWeighingScale.WeighingScale))
                            MessageBox.Show("A problem occurred while deleting the device local configuration settings.",
                                            "Device Local Configuration Settings Manager", MessageBoxButton.OK,
                                            MessageBoxImage.Exclamation);
                    }
                    
                }
                

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Weighing Scales", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override async void DeleteSelected()
        {
            if (
                    MessageBox.Show("Are you sure you want to delete this weighing scale?",
                                    "Agrimanagr: Delete Weighing Scale", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedWeighingScale == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.EquipmentDeleteAsync(SelectedWeighingScale.WeighingScale.Id);
                if(response.Success)
                {
                    Using<IEquipmentRepository>(c).SetAsDeleted(SelectedWeighingScale.WeighingScale);
                    if (DeleteDeviceLocalSettings(SelectedWeighingScale.WeighingScale))
                        MessageBox.Show("A problem occurred while deleting the device local configuration settings.",
                                        "Device Local Configuration Settings Manager", MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                }
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Weighing Scales", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedList.PageNumber, _pagedList.PageCount, _pagedList.TotalItemCount,
                                        _pagedList.IsFirstPage, _pagedList.IsLastPage);
        }

        bool DeleteDeviceLocalSettings(Equipment equipment)
        {
            XDocument docs = XDocument.Load(@"ScalePrinterAssetsDB.xml");

            foreach (var item in docs.Descendants("Equipment"))
            {
                if (item.Element("Id").Value == equipment.Id.ToString())
                {
                    item.Remove();
                    docs.Save(@"ScalePrinterAssetsDB.xml");
                    return true;
                }
            }
            return false;
        }
        #endregion

    }

    #region helpers

    public class VMWeighingScaleItem
    {
        public WeighScale WeighingScale { get; set; }
        public LocalEquipmentConfig ConfigSettings { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion
}
