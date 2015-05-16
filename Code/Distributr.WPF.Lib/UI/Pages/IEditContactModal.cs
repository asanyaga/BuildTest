using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WPF.Lib.UI.Pages
{
    public interface IEditContactModal
    {
        bool AddUserContact(Contact contactToEdit, out Contact contactReturned);
    }
}
