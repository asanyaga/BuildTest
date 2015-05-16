using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.EquipmentRepository;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class HubDevice : DistributrViewModelBase
    {
        public HubDevice()
        {
            EquipmentList = new ObservableCollection<Equipment>();
        }

        public static int BaudRate { get; set; }

        public static string Port { get; set; }

        /// <summary>
        /// The <see cref="SelectedEquipment" /> property's name.
        /// </summary>
        public const string SelectedEquipmentPropertyName = "SelectedEquipment";

        private Equipment _SelectedEquipment = null;

        /// <summary>
        /// Gets the AssetInstance property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event.
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Equipment SelectedEquipment
        {
            get { return _SelectedEquipment; }

            set
            {
                if (_SelectedEquipment == value)
                {
                    return;
                }

                var oldValue = _SelectedEquipment;
                _SelectedEquipment = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedEquipmentPropertyName);
            }
        }

        public ObservableCollection<Equipment> EquipmentList { get; set; }

        public void GetDevices(EquipmentType equipmentType, string title = "")
        {
            EquipmentList.Clear();
            using (StructureMap.IContainer c = NestedContainer)
            {
                var equipments =
                    Using<IEquipmentRepository>(c).GetAll().Where(p => p.EquipmentType == equipmentType).ToList();
                foreach (var equipment in equipments)
                {
                    EquipmentList.Add(new Equipment(equipment.Id)
                                          {
                                              Code = equipment.Code,
                                              EquipmentNumber = equipment.EquipmentNumber,
                                              Description = equipment.Description,
                                              EquipmentType = equipment.EquipmentType,
                                              Model = equipment.Make,
                                              CostCentre = equipment.CostCentre,
                                              Name = equipment.Name,
                                              _Status = equipment._Status,
                                              _DateCreated = equipment._DateCreated,
                                              _DateLastUpdated = equipment._DateLastUpdated

                                          });
                }
            }
        }
    }
}
