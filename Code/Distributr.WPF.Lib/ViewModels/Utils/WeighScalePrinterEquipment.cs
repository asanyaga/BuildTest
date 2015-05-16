using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.EquipmentEntities;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class WeighScalePrinterEquipment : Equipment
    {
        public WeighScalePrinterEquipment(Guid id) : base(id) { }
        public int BaudRate { get; set; }
        public string Port { get; set; }
    }
}
