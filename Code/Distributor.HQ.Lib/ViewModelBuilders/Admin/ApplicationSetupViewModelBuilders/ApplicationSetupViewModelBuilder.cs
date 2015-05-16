using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Distributr.Core;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Setup;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.ApplicationSetupViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ApplicationSetupViewModelBuilders
{
    public class ApplicationSetupViewModelBuilder : IApplicationSetupViewModelBuilder
    {
        private IApplicationSetup _applicationSetup;

        public ApplicationSetupViewModelBuilder(IApplicationSetup applicationSetup)
        {
            _applicationSetup = applicationSetup;
        }

        public ApplicationSetupViewModel Setup(ApplicationSetupViewModel appSetupVm, VirtualCityApp vcApp)
        {
            string serverName, dbName;
            appSetupVm.DatabaseExists = _applicationSetup.DatabaseExists(out serverName, out dbName);
            appSetupVm.DatabaseName = dbName;
            appSetupVm.DatabaseServer = serverName;
            appSetupVm.CompanyIsSetup = _applicationSetup.CompanyIsSetup(vcApp);
            appSetupVm.userType = (vcApp == VirtualCityApp.Agrimanagr ? UserType.AgriHQAdmin : UserType.HQAdmin);
           
            return appSetupVm;
        }

        public bool CreateCompanyAndSuperAdmin(ApplicationSetupViewModel appSetupVm)
        {

            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                appSetupVm.CompanyId = _applicationSetup.RegisterCompay(appSetupVm.CompanyName);
                if (appSetupVm.CompanyId == Guid.Empty)
                    throw new DomainValidationException(new ValidationResultInfo(),
                                                        "Unable to create company " + appSetupVm.CompanyName);
                if (appSetupVm.userType == UserType.HQAdmin)
                {
                    User admin = new User(Guid.NewGuid())
                                     {
                                         Username = "HQAdmin",
                                         Password = EncryptorMD5.GetMd5Hash(appSetupVm.AdminPassword),
                                         Mobile = appSetupVm.Mobile,
                                         PIN = appSetupVm.Pin,
                                         UserType = appSetupVm.userType,
                                         CostCentre = appSetupVm.CompanyId,
                                     };
                    if (_applicationSetup.RegisterSuperAdmin(admin))
                    {
                        scope.Complete();
                        return true;
                    }
                }
                if (appSetupVm.userType == UserType.AgriHQAdmin)
                {
                    
                    User admin = new User(Guid.NewGuid())
                    {
                        Username = "AgriHQAdmin",
                        Password = EncryptorMD5.GetMd5Hash(appSetupVm.AdminPassword),
                        Mobile = appSetupVm.Mobile,
                        PIN = appSetupVm.Pin,
                        UserType = appSetupVm.userType,
                        CostCentre = appSetupVm.CompanyId,
                    };
                    if (_applicationSetup.RegisterSuperAdmin(admin))
                    {
                        scope.Complete();
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
