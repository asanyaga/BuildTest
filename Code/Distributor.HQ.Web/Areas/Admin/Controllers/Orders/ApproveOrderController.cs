using System;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Orders
{
    [Authorize]
    public class ApproveOrderController : Controller
    {
        
        private IApproveOrderViewModelBuilder _approveOrderViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public ApproveOrderController(IApproveOrderViewModelBuilder approveOrderViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _approveOrderViewModelBuilder = approveOrderViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
         [Authorize(Roles = "RoleViewOrder")]
        public ActionResult Index(string orderId,bool refesh=false)
        {
              if(refesh)
              {
                  Session["PurchaseOrderLineItemList"] = null;
              }
            try
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
              
                var vm = _approveOrderViewModelBuilder.Get(new Guid(orderId));
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleEditOrder")]
         public ActionResult LineItemEdit(Guid productId, Guid orderid)
        {
            var item = _approveOrderViewModelBuilder.GetLineItem(productId);
            item.DocumentId = orderid.ToString();
            return View(item);
        }

        [HttpPost]
        public ActionResult LineItemEdit(Guid orderid, Guid productId, decimal qty)
        {
            try
            {
                _approveOrderViewModelBuilder.AddUpdateLineItems(productId, qty, false, false);
                //_approveOrderViewModelBuilder.SaveLineItem(documentId, lineItemId, qty);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Orders Line Item", DateTime.Now);
                TempData["msg"] = "Order lineitem Successfully Edited";
                return RedirectToAction("Index", new { orderid = orderid });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                   return View();
            }
        }
        [Authorize(Roles = "RoleEditOrder")]
        public ActionResult LineItemAdd(Guid documentId)
        {
            try
            {

                var vm = _approveOrderViewModelBuilder.GetAddLineItem(documentId);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
              
                return View();
            }
        }

        [HttpPost]
        public ActionResult LineItemAdd(Guid documentId, Guid productId, int qty, bool? QuantityType)
        {
          
            try
            {
                _approveOrderViewModelBuilder.AddUpdateLineItems(productId, qty, true, QuantityType.Value);
               // _approveOrderViewModelBuilder.SaveNewLineItem(documentId, productId, qty);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Add", "Orders Line Item", DateTime.Now);
                TempData["msg"] = "Order Lineitem Successfully Added";
               // return RedirectToAction("Index", new { orderid = documentId });
                return RedirectToAction("Index", new { orderid = documentId });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
              
                return View();
            }
            
        }

        private static List<OrderLineItemViewModel> AddToSession(Guid productId, int qty, List<OrderLineItemViewModel> currentLineItems)
        {
            OrderLineItemViewModel workonLineItem = currentLineItems.FirstOrDefault(n => n.ProductId == productId);
            if (workonLineItem == null)
            {
                workonLineItem = new OrderLineItemViewModel();
                workonLineItem.ProductId = productId;
               // workonLineItem.Qty == qty;

                currentLineItems.Add(workonLineItem);
            }
            else
            {
                qty = qty;

            }
            return currentLineItems;
        }
        [Authorize(Roles = "RoleEditOrder")]
        public ActionResult LineItemRemove(Guid productId, Guid documentid)
        {
            try
            {
                _approveOrderViewModelBuilder.RemoveLineItem(productId);
                //_approveOrderViewModelBuilder.LineItemRemove(documentid, lineitemid);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Remove", "Orders Line Item", DateTime.Now);
                TempData["msg"] = "Order Lineitem Successfully Removed";
                return RedirectToAction("Index", new { orderid = documentid });
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                
                return View();
            }
        }
        //[HttpPost]
        //public ActionResult LineItemRemove(Guid lineitemid, Guid documentid,string removeReason)
        //{
        //    try
        //    {
        //        _approveOrderViewModelBuilder.LineItemRemove(documentid, lineitemid);
        //        _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Remove", "Orders Line Item", DateTime.Now);
        //        TempData["msg"] = "Order Lineitem Successfully Removed";
        //        return Json(new { ok = true, redirectTo = Url.Action("Index", new { orderid = documentid }) });
        //        //return RedirectToAction("Index", new { orderid = documentid });
        //    }
        //    catch (DomainValidationException dve)
        //    {
        //        ValidationSummary.DomainValidationErrors(dve, ModelState);
        //        ViewBag.msg = dve.Message;
        //        return View();
        //    }
        //    catch (Exception exx)
        //    {
        //        ViewBag.msg = exx.Message;
        //        return View();
        //    }
        //}

        [Authorize(Roles = "RoleApproveOrder")]
        public ActionResult Approve(Guid documentId)
        {
            try
            {

                _approveOrderViewModelBuilder.Approve(documentId);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Approve", "Orders", DateTime.Now);
                TempData["msg"] = "Order Successfully Approved";
                return RedirectToAction("ListAllOrders", "Orders");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                
                return View();
            }

        }
      //  [Authorize(Roles = "RoleApproveOrder")]
        //[HttpPost]
        //public ActionResult Reject(Guid documentId,string reason,ApproveOrderViewModel appvm,FormCollection frm)
        //{
        //    string reasn = ViewData["reason"]as string;
        //    try
        //    {
        //        _approveOrderViewModelBuilder.Reject(documentId);
        //        _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Reject", "Orders", DateTime.Now);
        //        TempData["msg"] = "Order Successfully Rejected";
        //        return RedirectToAction("ListAllOrders", "Orders");
        //    }
        //    catch (DomainValidationException dve)
        //    {
        //        ValidationSummary.DomainValidationErrors(dve, ModelState);
        //        ViewBag.msg = dve.Message;
        //        return View();
        //    }
        //    catch (Exception exx)
        //    {
        //        ViewBag.msg = exx.Message;
        //        return View();
        //    }

        //}

       // [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Reject(Guid documentId, string reason, ApproveOrderViewModel appvm, FormCollection frm)
        //{
        //    return View();
        //}
        [Authorize(Roles = "RoleApproveOrder")]
        [HttpPost]
        public ActionResult Reject(Guid documentId, string reason)
        {
            if (reason != "null")
            {
                try
                {
                    _approveOrderViewModelBuilder.Reject(documentId, reason);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Reject", "Orders", DateTime.Now);
                    TempData["msg"] = "Order Successfully Rejected";
                    //return RedirectToAction("ListAllOrders", "Orders");
                    return Json(new { ok = true, redirectTo = Url.Action("ListAllOrders", "Orders") });

                }
                catch (DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve, ModelState);
                    ViewBag.msg = dve.Message;
                    return View();
                }
                catch (Exception exx)
                {
                    ViewBag.msg = exx.Message;
                      return View();
                }
                //return Json(new { ok = true, data = "Sam", message = "ok" });
            }
            else
            {
                TempData["msg"] = "";
                return Json(new { ok = true, redirectTo = Url.Action("ListAllOrders", "Orders") });
                //return RedirectToAction("ListAllOrders", "Orders");
            }
        }

        public ActionResult Display(string orderId)
        {
            try
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                var vm = _approveOrderViewModelBuilder.Find(new Guid(orderId));
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

    }
}
