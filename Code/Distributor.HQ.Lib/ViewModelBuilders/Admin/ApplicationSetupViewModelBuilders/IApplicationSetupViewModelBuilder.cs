using Distributr.Core;
using Distributr.HQ.Lib.ViewModels.Admin.ApplicationSetupViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ApplicationSetupViewModelBuilders
{
    public interface IApplicationSetupViewModelBuilder
    {
        ApplicationSetupViewModel Setup(ApplicationSetupViewModel appSetupVm, VirtualCityApp vcApp);
        bool CreateCompanyAndSuperAdmin(ApplicationSetupViewModel appSetupVm);
    }
}
