using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders.Impl
{
    public class CentreViewModelBuilder : ICentreViewModelBuilder
    {
        private ICentreTypeRepository _centreTypeRepository;
        private IRouteRepository _routeRepository;
        private ICentreRepository _centreRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepository;
        private IRegionRepository _regionRepository;
        private ICostCentreRepository _costCentreRepository;
        private IMasterDataUsage _masterDataUsage;

        public CentreViewModelBuilder(ICentreTypeRepository centreTypeRepository, IRouteRepository routeRepository,
                                      ICentreRepository centreRepository,
                                      IMasterDataAllocationRepository masterDataAllocationRepository, IRegionRepository regionRepository, ICostCentreRepository costCentreRepository, IMasterDataUsage masterDataUsage)
        {
            _centreTypeRepository = centreTypeRepository;
            _routeRepository = routeRepository;
            _centreRepository = centreRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _regionRepository = regionRepository;
            _costCentreRepository = costCentreRepository;
            _masterDataUsage = masterDataUsage;
        }

        public CentreViewModel Setup(CentreViewModel vm)
        {
            vm.HubList = new SelectList(GetHubsList(), "Id", "Name");
            vm.CentreTypesList = new SelectList(GetCentreTypeList(), "Key", "Value");
            vm.RouteList = GetRouteList();

            return vm;
        }

        IEnumerable<Hub> GetHubsList()
        {
            return _costCentreRepository.GetAll().OfType<Hub>().OrderBy(n => n.Name).ToList();
        }

        Dictionary<Guid, string> GetCentreTypeList()
        {
            return _centreTypeRepository.GetAll().ToList().OrderBy(c => c.Name)
                .Select(c => new {c.Id, c.Name}).ToDictionary(c => c.Id, c => c.Name);
        }

        public IList<CentreViewModel> GetAll(bool inactive = false)
        {
            var centres = _centreRepository.GetAll(inactive).Select(Map).ToList();
            return centres;
        }

        private CentreViewModel Map(Centre centre)
        {
            CentreViewModel centreViewModel = new CentreViewModel();
            centreViewModel.Id = centre.Id;
            centreViewModel.Name = centre.Name;
            centreViewModel.Code = centre.Code;
            centreViewModel.Description = centre.Description;
            centreViewModel.IsActive = centre._Status == EntityStatus.Active ? true : false;
            centreViewModel.SelectedHubId = centre.Hub.Id;
            centreViewModel.SelectedHubName = centre.Hub.Name;
            centreViewModel.SelectedRouteId = centre.Route != null ? centre.Route.Id : Guid.Empty;
            centreViewModel.SelectedRouteName = centre.Route != null ? centre.Route.Name : "";

            if (centre.CenterType != null)
            {
                Guid typeId = centre.CenterType.Id;
                centreViewModel.CenterTypeId = _centreTypeRepository.GetById(typeId).Id;
                centreViewModel.CentreTypeName = _centreTypeRepository.GetById(typeId).Name;
            }

            return centreViewModel;
        }

        public List<CentreViewModel> SearchCentres(string srchParam, bool inactive = false)
        {
            var centres =
                _centreRepository.GetAll(inactive).Where(c => (c.Name.ToLower().Contains(srchParam.ToLower())
                                                               || c.Code.ToLower().Contains(srchParam.ToLower())));
            return centres.Select(c => Map(c)).ToList();

        }

        public CentreViewModel Get(Guid id)
        {
            Centre centre = _centreRepository.GetById(id);

            if (centre == null) return null;

            return Map(centre);
        }

        public void Save(CentreViewModel centreviewmodel)
        {
            if (centreviewmodel.SelectedRouteId == Guid.Empty)
                throw new Exception("Select a valid route.");

            Centre centre = new Centre(centreviewmodel.Id);
            centre.Code = centreviewmodel.Code;
            centre.Name = centreviewmodel.Name;
            centre.Description = centreviewmodel.Description;
            centre.CenterType = _centreTypeRepository.GetById(centreviewmodel.CenterTypeId);
            centre.Route = _routeRepository.GetById(centreviewmodel.SelectedRouteId);
            centre.Hub = _costCentreRepository.GetById(centreviewmodel.SelectedHubId) as Hub;

            _centreRepository.Save(centre);
        }

        public void SetInactive(Guid Id)
        {
            Centre centre = _centreRepository.GetById(Id);
            if(_masterDataUsage.CheckAgriCentreIsUsed(centre,EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate centre. Centre is allocated to commodity producers. Remove allocation first to continue");
            }
            _centreRepository.SetInactive(centre);
        }

        public void SetActive(Guid Id)
        {
            Centre centre = _centreRepository.GetById(Id);
            _centreRepository.SetActive(centre);
        }

        public void SetAsDeleted(Guid Id)
        {
            Centre centre = _centreRepository.GetById(Id);
            if (_masterDataUsage.CheckAgriCentreIsUsed(centre, EntityStatus.Deleted))
            {
                throw new Exception(
                    "Cannot delete centre. Centre is allocated to commodity producers. Remove allocation first to continue");
            }
            _centreRepository.SetAsDeleted(centre);
        }

        public SelectList GetRouteList(Guid hubId = new Guid())
        {
            var @default = new Route(Guid.Empty) { Name = "---Select route---" };
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = @default.Id.ToString(), Text = @default.Name, Selected = true });

            IEnumerable<Route> routes ;
            if (hubId == Guid.Empty)
                routes = _routeRepository.GetAll();
            else
                routes = _routeRepository.GetAll().Where(n => n.Region.Id == ((Hub)_costCentreRepository.GetById(hubId)).Region.Id);

            routes.ToList().OrderBy(r => r.Name).ToList()
                .ForEach(n => selectList.Add(new SelectListItem
                                                 {
                                                     Value = n.Id.ToString(),
                                                     Text = n.Name,
                                                 }));

            return new SelectList(selectList,"Value", "Text");

        }

        QueryResult<CentreViewModel> ICentreViewModelBuilder.Query(QueryStandard query)
        {
            var queryResult = _centreRepository.Query(query);
            var result = new QueryResult<CentreViewModel>();
            result.Data = queryResult.Data.OfType<Centre>().Select(Map).ToList();
            result.Count = queryResult.Count;
            return result;
        }

       /* public IList<CentreViewModel> Query(QueryStandard query)
        {
            var centres = _centreRepository.Query(query).Data.OfType<Centre>().Select(Map).ToList();
            return centres;
        }*/
    }
}
