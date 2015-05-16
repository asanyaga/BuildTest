using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.EquipmentRepository;

namespace Distributr.WPF.Lib.ViewModels.Admin.Containers
{
    public class ListContainerTypesViewModel : ListingsViewModelBase
    {
        private PagenatedList<ContainerType> _pagedList;

        public ListContainerTypesViewModel()
        {
            ContainerTypesList = new ObservableCollection<VMContainerTypeItem>();
        }

        #region properties

        public ObservableCollection<VMContainerTypeItem> ContainerTypesList { get; set; }

        public const string SelectedContainerTypePropertyName = "SelectedContainerType";
        private VMContainerTypeItem _selectedContainerType = null;

        public VMContainerTypeItem SelectedContainerType
        {
            get { return _selectedContainerType; }

            set
            {
                if (_selectedContainerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContainerTypePropertyName);
                _selectedContainerType = value;
                RaisePropertyChanged(SelectedContainerTypePropertyName);
            }
        }

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

        #endregion

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadContainerTypesList();
        }

        private void LoadContainerTypesList()
        {
            using (var c = NestedContainer)
            {
                var list = Using<IContainerTypeRepository>(c).GetAll(ShowInactive)
                    .Where(n => n.Name.ToLower().Contains(SearchText.ToLower()) ||
                                n.Code.ToLower().Contains(SearchText.ToLower()))
                    .OrderBy(n => n.Name).ThenBy(n => n.Code);
                _pagedList = new PagenatedList<ContainerType>(list.AsQueryable(), CurrentPage, ItemsPerPage,
                                                              list.Count());
                ContainerTypesList.Clear();
                _pagedList.Select(Map).ToList().ForEach(ContainerTypesList.Add);
                UpdatePagenationControl();
            }
        }

        private VMContainerTypeItem Map(ContainerType ct, int index)
        {
            var mapped = new VMContainerTypeItem {ContainerType = ct, RowNumber = index};
            if (ct._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (ct._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        private void Setup()
        {
            PageTitle = "List of Container Types";
        }

        protected override void EditSelected()
        {
            if (SelectedContainerType != null)
                SendNavigationRequestMessage(
                    new Uri("/views/admin/containers/editcontainertype.xaml?" + SelectedContainerType.ContainerType.Id,
                            UriKind.Relative));
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
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

        #endregion

    }

    #region helpers

    public class VMContainerTypeItem
    {
        public ContainerType ContainerType { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
    }

    #endregion

}
