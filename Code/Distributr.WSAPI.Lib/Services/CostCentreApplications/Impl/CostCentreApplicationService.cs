using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.Routing.Repository;
using log4net;

namespace Distributr.WSAPI.Lib.Services.CostCentreApplications.Impl
{
    /* ----  May2015_Notes -----------
 Management of CCAppId
 */
    public class CostCentreApplicationService : ICostCentreApplicationService
    {
        ICostCentreRepository _costCentreRepository;
        ICostCentreApplicationRepository _costCentreApplicationRepository;
        IUserRepository _userRepository;
        private ILog _log = LogManager.GetLogger("CostCentreApplicationService");
        private ICommandEnvelopeRouteOnRequestCostcentreRepository _envelopeRouteOnRequestCostcentreRepository;
        private IClientMasterDataTrackerRepository _clientMasterDataTrackerRepository;

        public CostCentreApplicationService(IClientMasterDataTrackerRepository clientMasterDataTrackerRepository,ICostCentreRepository costCentreRepository, ICostCentreApplicationRepository costCentreApplicationRepository, IUserRepository userRepository, ICommandRoutingOnRequestRepository commandRoutingRepository, ICommandEnvelopeRouteOnRequestCostcentreRepository envelopeRouteOnRequestCostcentreRepository)
        {
            _costCentreRepository = costCentreRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _userRepository = userRepository;
            _envelopeRouteOnRequestCostcentreRepository = envelopeRouteOnRequestCostcentreRepository;
         
            _clientMasterDataTrackerRepository = clientMasterDataTrackerRepository;
        }

        public CreateCostCentreApplicationResponse CreateCostCentreApplication(Guid costCentreId,
            string applicationDescription)
        {
            if (string.IsNullOrEmpty(applicationDescription))
            {
                _log.Info("Failed to create cost centre application - you must include a description for the cost centre application");
                return new CreateCostCentreApplicationResponse
                           {
                               CostCentreApplicationId = Guid.Empty,
                               ErrorInfo = "you must include a description for the cost centre application"
                           };
            }
            CostCentre cc = _costCentreRepository.GetById(costCentreId);
            if (cc==null)
            {
                _log.Info("Failed to create cost centre application - you must include a valid cost centre in order to create a cost centre application");
                return new CreateCostCentreApplicationResponse
                           {
                               CostCentreApplicationId = Guid.Empty,
                               ErrorInfo =
                                   "you must include a valid cost centre in order to create a cost centre application"
                           };
            }
          
           
            //CostCentre cc = _costCentreRepository.GetById(costCentreId);
           


            CostCentreApplication app = new CostCentreApplication(Guid.NewGuid())
            {
                CostCentreId = costCentreId,
                Description = applicationDescription,
            };
            if (cc is DistributorSalesman )
            {
              var apps= _costCentreApplicationRepository.GetByCostCentreId(costCentreId).Where(s=>s._Status==EntityStatus.Active).ToList();
                apps.ForEach(n => _costCentreApplicationRepository.SetInactive(n));
                StartCleanUpThread(cc.Id, apps.Select(s=>s.Id).ToList());
               

            }
            if ( cc is Distributor ||  cc  is  Hub)
            {
                var application = _costCentreApplicationRepository.GetByCostCentreId(costCentreId).Where(s => s._Status == EntityStatus.Active).FirstOrDefault();
               // _costCentreRepository.Save(cc);
              
                if (application != null)
                {
                    _clientMasterDataTrackerRepository.IntializeApplication(application.Id);
                    app.Id = application.Id;
                }


            }
           
            Guid newCostCentreApplicationId = _costCentreApplicationRepository.Save(app);

            _log.InfoFormat("Created new cost centre application id {0} ", newCostCentreApplicationId);
            //also update command routing table so that it contains existing commands for the new costcentreapplicationid
          

            return new CreateCostCentreApplicationResponse { CostCentreApplicationId = newCostCentreApplicationId, ErrorInfo = "Success" };

        }

        public void CleanUp(Guid costcentreId, List<Guid> applicationId)
        {
            _envelopeRouteOnRequestCostcentreRepository.MarkEnvelopeAsInvalid(costcentreId);
          //  applicationId.ForEach(n => _commandRoutingRepository.CleanApplication(n));
        }

        public Thread StartCleanUpThread(Guid costcentreId, List<Guid> applicationId)
        {
            var t = new Thread(() => CleanUp(costcentreId, applicationId));
            t.Start();
            return t;
        }
       


        public CostCentreLoginResponse CostCentreLogin(string username, string password, string userType)
        {
            User user = _userRepository.Login(username.ToLower(), password);
            bool error = false;
            if (user == null)
                error = true;
            if (!error)
            {
                _log.InfoFormat("Successful login for {0}", username);
                string userty = user.UserType.ToString();
                if (userty.Equals(userType))
                    
                    return new CostCentreLoginResponse() { CostCentreId = user.CostCentre, ErrorInfo = "Success" };
                //return new CostCentreLoginResponse() { CostCentreId = user.CostCentre, ErrorInfo = "Please enter a valid Username and Password.\nInvalid account type detected." };
            }
            _log.InfoFormat("Failed to login {0}", username);
            return new CostCentreLoginResponse() { CostCentreId = Guid.Empty, ErrorInfo = "Please enter a valid Username and Password" };
        }

        public bool IsCostCentreActive(Guid appId)
        {
            CostCentreApplication app = _costCentreApplicationRepository.GetById(appId);
            if (app == null)
                return false;
            else if (app._Status == EntityStatus.Active)
                return true;
            else
                return false;
        }


        public Guid GetCostCentreFromCostCentreApplicationId(Guid appId)
        {
            CostCentreApplication app = _costCentreApplicationRepository.GetById(appId);
            if (app == null)
                return Guid.Empty;
            return app.CostCentreId;

        }
    }
}
