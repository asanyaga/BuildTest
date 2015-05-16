using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Models;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Util;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Distributr.HQ.Web.Controllers
{
   
   [Authorize]
    public class ApiController : Controller
    {
        //
        // GET: /Api/
        private IDropdownRepository _dropdownRepository;
        private IProductPackagingSummaryClient _summaryView;
        private IPurchaseOrderViewModelBuilder _purchaseOrderViewModelBuilder;


        public ApiController(IDropdownRepository dropdownRepository, IProductPackagingSummaryClient summaryView, IPurchaseOrderViewModelBuilder purchaseOrderViewModelBuilder)
        {
            _dropdownRepository = dropdownRepository;
            _summaryView = summaryView;
            _purchaseOrderViewModelBuilder = purchaseOrderViewModelBuilder;
        }

        public  void PagingParam( out int take, out int skip,out string search)
        {

            int page = 0;
            var parameters = Request.QueryString;
            Int32.TryParse(parameters["page"], out page);
            Int32.TryParse(parameters["per_page"], out take);


            skip = (page - 1) * take;
             search = parameters["search"];
        }
        public JsonResult GetDistributor()
        {
            int take;
            int skip;
            string search ;
            PagingParam(out take, out skip, out search);
            var response = _dropdownRepository.GetDistributors(skip, take, search);
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
       
        }
        public JsonResult GetSaleProducts()
        {
            int take;
            int skip;
            string search;
            PagingParam(out take, out skip, out search);
            var response = _dropdownRepository.GetSaleProduct(skip, take, search);
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CalculatePurchaseOrderItemSummary(Guid productId,decimal quantity,bool isBulk)
        {
            
            if(isBulk)
            {
                var bulquantity = _summaryView.GetProductQuantityInBulk(productId);
                quantity = quantity*bulquantity;
            }
          
            var response = _summaryView.GetProductSummaryByProduct(productId, quantity);
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CalculatePurchaseOrderItemFullSummary(Guid productId, decimal quantity, bool isBulk,string key,bool isEdit)
        {
            
            if(isBulk)
            {
                var bulquantity = _summaryView.GetProductQuantityInBulk(productId);
                quantity = quantity*bulquantity;
            }
            _summaryView.AddProduct(key, productId, quantity, false, isEdit);
            var response = _summaryView.GetProductSummary(key);
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeletePurchaseOrderItem(Guid productId, string key)
        {
            _summaryView.RemoveProduct(key, productId);
            var response = _summaryView.GetProductSummary(key);
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SavePurchaseOrder(PurchaseOrderViewModel model)
        {
            ResponseBasic response= new ResponseBasic();
            try
            {
                var user = this.User.Identity.Name;
                model.Username = user;
                _purchaseOrderViewModelBuilder.Save(model);
                response.Result = "Ok";
                response.ResultInfo = "Purchase order saved and approved successfully";
            }catch(Exception ex)
            {
                response.Result = "Fail";
                response.ErrorInfo = ex.Message;

            }
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult SetItemsPerpage(int pagesize)
        {
            ResponseBasic response = new ResponseBasic();
            try
            {
                DistributorWebHelper.SetItemPerPage(pagesize);
            }
            catch (Exception ex)
            {
               

            }
            return Json(response, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }
        
    }
}
