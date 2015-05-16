using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder
{
    public interface IProducerViewModelBuilder
    {
        ProducerViewModel Get();
    }
}
