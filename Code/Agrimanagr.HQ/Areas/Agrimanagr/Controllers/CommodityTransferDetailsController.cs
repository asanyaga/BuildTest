using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommodityTransferDetailsController : Controller
    {
        //
        // GET: /Agrimanagr/CommodityTransferDetails/

        private readonly ICommodityTransferDetailViewModelBuilder _commodityTransferDetailViewModelBuilder;

        public CommodityTransferDetailsController(ICommodityTransferDetailViewModelBuilder commodityTransferDetailViewModelBuilder)
        {
            _commodityTransferDetailViewModelBuilder = commodityTransferDetailViewModelBuilder;
        }


        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(Guid id, bool? showInactive, int? page, int? itemsperpage)
        {
            var ls = _commodityTransferDetailViewModelBuilder.GetById(id);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Approve(Guid id)
        {
            _commodityTransferDetailViewModelBuilder.Approve(id);
            return RedirectToAction("List", "CommodityTransfer");
        }

    }
}
