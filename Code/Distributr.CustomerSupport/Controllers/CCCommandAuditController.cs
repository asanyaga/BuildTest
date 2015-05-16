using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Distributr.CustomerSupport.Code.CCAudit;
using Distributr.CustomerSupport.Paging;
using Distributr.WSAPI.Lib.Services.CommandAudit;

namespace Distributr.CustomerSupport.Controllers
{
    public class CCCommandAuditController : Controller
    {
        private readonly ICCCommandAuditViewModelBuilder _commandAuditViewModelBuilder;

        public CCCommandAuditController(ICCCommandAuditViewModelBuilder commandAuditViewModelBuilder)
        {
            _commandAuditViewModelBuilder = commandAuditViewModelBuilder;
        }

        public ActionResult CommandProcessing(int? page, int? itemsPerPage, Guid? costCentreId = null, 
            Guid? costCentreAppId = null, CommandProcessingStatus? status = null)
        {
            var vm = new CCCommandAuditViewModel();
            if (itemsPerPage != null)
            {
                Paginator.ItemsPerPage = itemsPerPage.Value;
            }
            vm.PageIndex = page.HasValue ? page.Value - 1 : 0;
            vm.PageSize = Paginator.ItemsPerPage;
            if (Request.IsAjaxRequest())
            {
                if (costCentreId == null || costCentreAppId == null)
                {
                    return PartialView("_CommandProcessingList", vm);
                }

                vm.CostCentreId = costCentreId.Value;
                vm.CostCentreAppId = costCentreAppId.Value;
                if (status != null)
                {
                    vm.CommandProcessingStatus = status.Value;
                }
                int count;
                vm.Details = _commandAuditViewModelBuilder.GetCommandProcessingDetails(
                    vm.CostCentreId, vm.CostCentreAppId, vm.PageIndex, vm.PageSize, vm.CommandProcessingStatus, out count);
                vm.TotalItemCount = count;
                vm.AuditItems = vm.Details.Items.ToPagedList(vm.PageIndex, vm.PageSize, vm.TotalItemCount);
                return PartialView("_CommandProcessingList", vm);
            }

            var costCentres = new Dictionary<Guid, string>();
            costCentres.Add(Guid.Empty, "---Select Cost Centre---");
            _commandAuditViewModelBuilder.GetCostCentres()
                .OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name).ToList()
                .ForEach(n => costCentres.Add(n.Key, n.Value));

            var statusTypes = EnumHelper.EnumToList<CommandProcessingStatus>()
                .ToDictionary(n => (int)n, n => n.ToString());
            
            ViewBag.CostCentres = costCentres;
            ViewBag.StatusTypes = statusTypes;
            return View(vm);
        }

        public ActionResult CommandRouting(int? page, int? itemsPerPage, string date, Guid? costCentreId = null)
        {
            var vm = new CCCommandAuditViewModel();
            if (itemsPerPage != null)
            {
                Paginator.ItemsPerPage = itemsPerPage.Value;
            }
            vm.PageIndex = page.HasValue ? page.Value - 1 : 0;
            vm.PageSize = Paginator.ItemsPerPage;
            if(Request.IsAjaxRequest())
            {
                if(costCentreId == null)
                {
                    return PartialView("_CommandRoutingList", vm);
                }
                DateTime dt;
                DateTime.TryParse(date, out dt);
                vm.Date = new DateTime(dt.Year, 1, 1).AddDays(dt.DayOfYear - 1).ToString("dd-MMM-yyyy");
                vm.CostCentreId = costCentreId.Value;
                int count;
                var routingDetails = _commandAuditViewModelBuilder.GetCommandRoutingItems(
                    vm.CostCentreId, dt, vm.PageIndex, vm.PageSize, out count);
                vm.TotalItemCount = count;
                vm.RoutingItems = routingDetails.ToPagedList(vm.PageIndex, vm.PageSize, vm.TotalItemCount);

                return PartialView("_CommandRoutingList", vm);
            }
            vm.Date = new DateTime(DateTime.Now.Year, 1, 1).AddDays(DateTime.Now.DayOfYear - 1).ToString("dd-MMM-yyyy");
            var costCentres = new Dictionary<Guid, string>();
            costCentres.Add(Guid.Empty, "---Select Cost Centre---");
            _commandAuditViewModelBuilder.GetCostCentres()
                .OrderBy(n => n.CostCentreType).ThenBy(n => n.Name)
                .ToDictionary(n => n.Id, n => new { n.Name, n.CostCentreType}).ToList()
                .ForEach(n =>
                             {
                                 var value = n.Value.Name + " (" + n.Value.CostCentreType.ToString() + ")";
                                 costCentres.Add(n.Key, value);
                             });

            ViewBag.CostCentres = costCentres;
            return View(vm);
        }

        public ActionResult CommandAudit(int? page, int? itemsPerPage, Guid? costCentreId = null, Guid? costCentreAppId = null, 
CommandProcessingStatus? status = null)
        {
            var vm = new CCCommandAuditGroupViewModel();
            if (itemsPerPage != null)
            {
                Paginator.ItemsPerPage = itemsPerPage.Value;
            }
            vm.PageIndex = page.HasValue ? page.Value - 1 : 0;
            vm.PageSize = Paginator.ItemsPerPage;
            if (Request.IsAjaxRequest())
            {
                if (costCentreId == null || costCentreAppId == null)
                {
                    return PartialView("_CommandAuditList", vm);
                }

                vm.CostCentreId = costCentreId.Value;
                vm.CostCentreAppId = costCentreAppId.Value;
                int count;
                vm.Details = _commandAuditViewModelBuilder.GetCommandAuditDetails(
                    vm.CostCentreId, vm.CostCentreAppId, vm.PageIndex, vm.PageSize, out count);
                vm.TotalItemCount = count;
                vm.AuditGroupItems = vm.Details.GroupItems.ToPagedList(vm.PageIndex, vm.PageSize, vm.TotalItemCount);
                return PartialView("_CommandAuditList", vm);
            }

            var costCentres = new Dictionary<Guid, string>();
            costCentres.Add(Guid.Empty, "---Select Cost Centre---");
            _commandAuditViewModelBuilder.GetCostCentres()
                .OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name).ToList()
                .ForEach(n => costCentres.Add(n.Key, n.Value));

            var statusTypes = EnumHelper.EnumToList<CommandProcessingStatus>()
                .ToDictionary(n => (int)n, n => n.ToString());

            ViewBag.CostCentres = costCentres;
            ViewBag.StatusTypes = statusTypes;
            return View(vm);
        }


        [HttpPost]
        public ActionResult Applications(Guid costCentreId)
        {
            try
            {
                var apps = _commandAuditViewModelBuilder.GetCostCentreApplications(costCentreId);
                var items = apps.Select(app => new
                                                   {
                                                       id = app.Id,
                                                       status = app._Status.ToString()
                                                   });
                var json = Json(new {ok = true, data = items});
                return json;
            }
            catch(Exception ex)
            {
                return Json(new {ok = false, data = "", message = ex.InnerException.Message});
            }
        }
    }

    #region Enum Helper
    public class EnumHelper
    {
        public static List<T> EnumToList<T>()
        {
            Type enumType = typeof (T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof (Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);

            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T) Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }
    }
    #endregion
}
