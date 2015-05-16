using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.HQ.Lib.Paging;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.IO;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using System.ComponentModel.DataAnnotations;
using Distributr.HQ.Lib.ViewModels.Admin.Transactional;
using System.Text;
using System.Data.OleDb;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Globalization;
using System.Configuration;
using System.Diagnostics;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.Validation;
using System.Data;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Orders
{
   [Authorize ]
   [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class OrdersController : Controller
    { 
       protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IListOrdersViewModelBuilder _listOrdersViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public OrdersController(IListOrdersViewModelBuilder listOrdersViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _listOrdersViewModelBuilder = listOrdersViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportOrders()
        {
            return View("ImportOrders", new OrderViewModel());
        }
        #region OldOrdersListing
        /*public ActionResult ListAllOrder(Guid? distributor, int? page, string ord, string StartDate, string EndDate, string orderRef, int? itemsperpage)
        {
            if (page.HasValue) page = page.Value;
            else page = 1;
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }

            ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
            try
            {
                if (distributor == null)
                {
                    distributor = Guid.Empty;
                }
                string command = ord;
                if (distributor == Guid.Empty && command == "Filter")
                {
                    ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
                    OrderViewModel ovm = _listOrdersViewModelBuilder.FilterOrdersByDate(StartDate, EndDate,page.Value,ViewModelBase.ItemsPerPage);
                    
                    return View(ovm);
                }
                else if (distributor != Guid.Empty && command == "Filter")
                {

                    try
                    {
                        DateTime.ParseExact(StartDate, "dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo);

                        ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
                        OrderViewModel filterOrdersVM = _listOrdersViewModelBuilder.FilterOrdersByDateDistributor(distributor.Value, StartDate, EndDate,page.Value,ViewModelBase.ItemsPerPage);
                        return View(filterOrdersVM);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.invalidDate = "Invalid Date";
                        // return RedirectToAction("ListAllOrders");
                        var ovm = _listOrdersViewModelBuilder.GetAllList();
                        var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                        var orderListContainer = new OrderViewModel
                        {
                            orderPagedList = orderPagedList,

                        };
                        return View(orderListContainer);
                    }

                }

                else if (command == "Export Orders")
                {
                    try
                    {
                        StringWriter sw = new StringWriter();
                        // sw.WriteLine("\"Issued On Behalf\",\"Outlet Code\",\"Order Date\",\"Status\",\"Product Code\",\"Quantity\",\"Net Value\",\"VAT\",\"Gross\",\"Doc Ref\"");
                        sw.WriteLine("\"Issued On Behalf\",\"Outlet Code\",\"Order Date\",\"Status\",\"Product Code\",\"Quantity\",\"Discount\",\"Doc Ref\"");
                        //List<AuditLogViewModel> log = _auditLogViewModelBuilder.GetAll().ToList();
                        //orders.net,
                        // orders.vat,
                        List<OrderViewModel> ordvm = null;
                        if (distributor == Guid.Empty)
                        {
                            ordvm = _listOrdersViewModelBuilder.GetAllOrdersByDate(StartDate, EndDate);
                        }
                        else
                        {
                            ordvm = _listOrdersViewModelBuilder.GetByDistDate(distributor.Value, StartDate, EndDate);
                        }
                        foreach (var orders in ordvm)
                        {

                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                              orders.isseuedOnBehalf,
                                              orders.outletCode,
                                              orders.orderDate.ToString("dd-MMM-yyyy"),
                                               orders.status,
                                               orders.productCode,
                                               orders.quantity,

                                               orders.discountName,
                                               orders.documentReference));

                        }
                        //DateTime.Now.ToString()+"-"+
                        string date = DateTime.Now.ToString("dd-MMM-yyyy");
                        string hd = date + "-" + StartDate + "to" + EndDate;
                        Response.AddHeader("Content-Disposition", "attachment; filename=OrderList-" + hd + ".csv");
                        Response.ContentType = "text/csv";
                        Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                        Response.Write(sw);
                        Response.End();
                        var ovm = _listOrdersViewModelBuilder.GetAllList();
                        var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                        var orderListContainer = new OrderViewModel
                        {
                            orderPagedList = orderPagedList,

                        };
                        return View(orderListContainer);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.msg = ex.Message;
                        try
                        {
                            //HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                            //hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "Order Export error:" + ex.Message);
                        }
                        catch (Exception exx)
                        { 
                        
                        }
                        return View();
                    }
                }
                else if (command == "Export Sales")
                {
                    try
                    {
                        StringWriter sw = new StringWriter();
                        sw.WriteLine("\"Issued On Behalf\",\"Outlet Code\",\"Sale Date\",\"Status\",\"Product Code\",\"Quantity\",\"Discount\",\"Doc Ref\"");

                        //List<AuditLogViewModel> log = _auditLogViewModelBuilder.GetAll().ToList();
                        List<OrderViewModel> ordvm = null;
                        if (distributor == Guid.Empty)
                        {
                            ordvm = _listOrdersViewModelBuilder.GetAllSales(StartDate, EndDate);
                        }
                        else
                        {
                            ordvm = _listOrdersViewModelBuilder.GetSalesByDistDate(distributor.Value, StartDate, EndDate);
                        }
                        foreach (var orders in ordvm)
                        {

                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                             orders.isseuedOnBehalf,
                                             orders.outletCode,
                                             orders.orderDate.ToString("dd-MMM-yyyy"),
                                              orders.status,
                                              orders.productCode,
                                              orders.quantity,

                                              orders.discountName,
                                              orders.documentReference));

                        }
                        string date = DateTime.Now.ToString("dd-MMM-yyyy");
                        string hd = date + "-" + StartDate + "to" + EndDate;
                        Response.AddHeader("Content-Disposition", "attachment; filename=SalesList-" + hd + ".csv");
                        Response.ContentType = "text/csv";
                        Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                        Response.Write(sw);
                        Response.End();
                        var ovm = _listOrdersViewModelBuilder.GetAllList();
                        var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                        var orderListContainer = new OrderViewModel
                        {
                            orderPagedList = orderPagedList,

                        };
                        return View(orderListContainer);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.msg = ex.Message;
                        try
                        {
                            //HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationManager.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                           // hqm.Send(ConfigurationManager.AppSettings["ServerEmail"], ConfigurationManager.AppSettings["MailGroup"], "Test", "Sales export error:" + ex.Message);
                        }
                        catch (Exception exx)
                        { }
                        return View();
                    }
                }
                else if (command == "Search")
                {
                    try
                    {
                        ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
                        OrderViewModel ovm = _listOrdersViewModelBuilder.SearchPOrders(orderRef,page.Value,ViewModelBase.ItemsPerPage);
                        //var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                        //var orderListContainer = new OrderViewModel
                        //{
                        //    orderPagedList = orderPagedList,

                        //};
                        //return View(orderListContainer);
                        return View(ovm);
                    }
                    catch (Exception ex)
                    {
                        return View();
                    }
                }
                else if (command == "Clear")
                {
                    return RedirectToAction("ListAllOrder");
                }
                else
                {
                    try
                    {
                        //OrderViewModel orderList = _listOrdersViewModelBuilder.GetOrdersSkipAndTake(page.Value, pageSiz, showinactive);

                        ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();

                        var ovm = _listOrdersViewModelBuilder.GetByDist(distributor.Value, page.Value, ViewModelBase.ItemsPerPage);

                        //var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                        //var orderListContainer = new OrderViewModel
                        //{
                        //    orderPagedList = orderPagedList,

                        //};
                        return View(ovm);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.msg = ex.Message;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ImportOrders(HttpPostedFileBase file)
        {
           
            try
            {

                var fileName = Path.GetFileName(file.FileName);

                var directory = Server.MapPath("~/Uploads");
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                var path = Server.MapPath("~/Uploads") + "\\" + fileName;
                file.SaveAs(path);
                DataTable dt = new DataTable();
                try
                {
                    dt = ImportDistributrFile.csvToDataTable(path, true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Upload a valid Order CSV FIle");
                    _log.Debug(ex);
                }
                try
                {


                    List<OrderViewModel> OrderViewModelContainer = new List<OrderViewModel>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string outletCode = dr["OutletCode"].ToString();
                        string distributorCode = dr["DistributorCode"].ToString();
                        string salesmanCode = dr["SalesmanCode"].ToString();
                        string productCode = dr["ProductCode"].ToString();
                        string OrderType = dr["OrderType"].ToString();
                        string quantity = dr["Quantity"].ToString();
                        string orderReference = dr["OrderReference"].ToString();
                        OrderViewModel pdvm;
                        if (OrderViewModelContainer.Any(p => p.OrderReference == orderReference))
                        {
                            pdvm = OrderViewModelContainer.First(p => p.OrderReference == orderReference);

                        }
                        else
                        {
                            pdvm = new OrderViewModel();
                            OrderViewModelContainer.Add(pdvm);
                        }
                        pdvm.outletCode = outletCode;
                        pdvm.distributorCode = distributorCode;
                        pdvm.salesManCode = salesmanCode;
                        pdvm.OrderReference = orderReference;
                        pdvm.documentReference = orderReference;
                        pdvm.productCode = productCode;

                        pdvm.orderViewModelLineItemVm.Add(new OrderViewModel.OrderLineItemViewModel()
                                                              {
                                                                  quantity = int.Parse(quantity),
                                                                  // Value = Decimal.Parse(value),
                                                                  // LineItemVatValue = Decimal.Parse(vat),
                                                                  LineItemSequenceNo = 1,
                                                                  productCode = productCode,
                                                                  LineItemType = OrderType
                                                              });
                        // _listOrdersViewModelBuilder.SaveOrder(pdvm);

                    }
                    _listOrdersViewModelBuilder.ProcessOrder(OrderViewModelContainer);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //string fileExtension = Path.GetExtension(fileName);
                //if (fileExtension == ".xlsx" || fileExtension==".xls")
                //{


                //    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;'";



                //    OleDbConnection conn = new OleDbConnection(connectionString);
                //    try
                //    {
                //        conn.Open();
                //        //Please Donot Delete this
                //       // OleDbCommand command = new OleDbCommand("SELECT OutletCode,DistributorCode,SalesmanCode,ProductCode,Vat,Value,OrderType,Quantity,OrderReference FROM [Sheet1$]", conn);
                //      //
                //        OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", conn);

                //        OleDbDataReader reader = command.ExecuteReader();

                //        List<OrderViewModel> OrderViewModelList = new List<OrderViewModel>();
                //        while (reader.Read())
                //        {
                //            if (reader["OutletCode"].ToString() != "" || reader["DistributorCode"].ToString() != "")
                //           //     if(!string.IsNullOrEmpty(reader.GetString(0))||!string.IsNullOrEmpty(reader.GetString(1)))
                //            {
                //                OrderViewModel pdvm;
                //                string outletCode = reader["OutletCode"].ToString();
                //                string distributorCode = reader["DistributorCode"].ToString();
                //                string salesmanCode = reader["SalesmanCode"].ToString();
                //                string productCode = reader["ProductCode"].ToString();
                //                //string vat = reader["Vat"].ToString();
                //                //string value = reader["Value"].ToString();
                //                string OrderType = reader["OrderType"].ToString();
                //                string quantity = reader["Quantity"].ToString();
                //                string orderReference = reader["OrderReference"].ToString();



                //                if (OrderViewModelList.Any(p => p.OrderReference == orderReference))
                //                {
                //                    pdvm = OrderViewModelList.First(p => p.OrderReference == orderReference);

                //                }
                //                else
                //                {
                //                    pdvm = new OrderViewModel();
                //                    OrderViewModelList.Add(pdvm);
                //                }
                //                pdvm.outletCode = outletCode;
                //                pdvm.distributorCode = distributorCode;
                //                pdvm.salesManCode = salesmanCode;
                //                pdvm.OrderReference = orderReference;
                //                pdvm.documentReference = orderReference;
                //                pdvm.productCode = productCode;

                //                pdvm.orderViewModelLineItemVm.Add(new OrderViewModel.OrderLineItemViewModel()
                //                {
                //                    quantity = int.Parse(quantity),
                //                   // Value = Decimal.Parse(value),
                //                   // LineItemVatValue = Decimal.Parse(vat),
                //                    LineItemSequenceNo = 1,
                //                    productCode = productCode,
                //                    LineItemType = OrderType
                //                });

                //                //_listOrdersViewModelBuilder.SaveOrder(pdvm);
                //                //}
                //            }
                //        }
                //        reader.Close();
                //        _listOrdersViewModelBuilder.ProcessOrder(OrderViewModelList);
                //    }

                //    catch (OleDbException ex)
                //    {
                //        ViewBag.msg = ex.ToString();
                //        return View();
                //    }

                //    finally
                //    {
                //        conn.Close();

                //    }

                //    fi = new FileInfo(path);

                //    fi.Delete();
                //    _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", " Orders", DateTime.Now);
                //    var tt = _listOrdersViewModelBuilder.GetUnImported();


                //    if (tt.Count != 0)
                //    {
                //        TempData["unImportedOrders"] = tt.ToList();
                //    }
                //    else
                //    {
                //        ViewBag.msg = "All Orders Successfully imported";
                //        TempData["msg"] = "All Orders Successfully imported";
                //    }
                //    return RedirectToAction("ListAllOrders");
                //}

                //else
                //{
                //    fi = new FileInfo(path);

                //    fi.Delete();
                //    ViewBag.msg = "Please upload excel file with extension .xlsx";
                //    return View();
                //}
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.ToString();
                return View();
            }
            ViewBag.msg = "All Orders Successfully imported";
            TempData["msg"] = "All Orders Successfully imported";

            return RedirectToAction("ListAllOrders");


        }*/
        #endregion

        public ActionResult ListAllOrders(bool? showInactive, Guid? distributor, string command, string StartDate, string EndDate, string orderRef = "", int itemsperpage=10, int page = 1)
        {
            // never remove this line below
            Session["PurchaseOrderLineItemList"] = null;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            Stopwatch stopWatch = new Stopwatch();
            if (command == "Clear")
            {
                orderRef = "";
            }
            ViewBag.searchParam = orderRef;
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
               
                stopWatch.Start();

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    ViewBag.NoOrders = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryOrders();
                query.OrderType=OrderType.DistributorToProducer;
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = orderRef;
                query.DocumentStatus = DocumentStatus.Confirmed;

                var endDate = Convert.ToDateTime(EndDate, CultureInfo.GetCultureInfo("en-GB").DateTimeFormat);
                var startDate = Convert.ToDateTime(StartDate, CultureInfo.GetCultureInfo("en-GB").DateTimeFormat);

                StartDate = StartDate != null ? Convert.ToDateTime(startDate).ToString("dd-MM-yyyy") : DateTime.Today.AddDays(1 - DateTime.Today.Day).ToString("dd-MM-yyyy");
                EndDate = EndDate != null ? Convert.ToDateTime(endDate).AddDays(1).ToString("dd-MM-yyyy") : DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
                query.Distributr = distributor ?? Guid.Empty;
                query.StartDate = DateTime.ParseExact(StartDate,"dd-MM-yyyy",null);
                query.EndDate = DateTime.ParseExact(EndDate, "dd-MM-yyyy", null);

                ViewBag.startDate = StartDate;
                ViewBag.endDate = EndDate;
                ViewBag.distributorId = distributor;

                var orderList = _listOrdersViewModelBuilder.Query(query);
                var data = orderList.Data;
                var count = orderList.Count;

                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.TotalMilliseconds);

                stopWatch.Reset();
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "List Order Timer", "Order Controller" + elapsedTime, DateTime.Now);
                _log.InfoFormat("List All Orders\tTime taken to get all orders" + elapsedTime);

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public ActionResult PendingOrders(int? distributor, int? page, string ord, string startDate, string endDate, int itemsperpage = 10)
        {
            if (page.HasValue)
                page = page.Value-1;// page = page.Value;
            else page = 0;//page=1;

            var query = new QueryOrders();
            int take = itemsperpage;
            int skip = page.Value * take;
            query.Skip = skip;
            query.Take = take;
            query.OrderType = OrderType.DistributorToProducer;
            query.DocumentStatus = DocumentStatus.Confirmed;
            query.EndDate = DateTime.Now;
            query.StartDate = DateTime.Now.AddDays(-90);

           var pendingOrderList=_listOrdersViewModelBuilder.Query(query);
           var data = pendingOrderList.Data;
           var count = pendingOrderList.Count;

            return View(data.ToPagedList(page.Value, itemsperpage, count));
            //OrderViewModel pendingOrderList = _listOrdersViewModelBuilder.GetAllPendingOrders(page.Value, itemsperpage);

            //return View(pendingOrderList);
        }
       public ActionResult DeliveredPurchaseOrders(int? distributor, string ord, string startDate, string endDate, int itemsperpage = 10, int page = 1)
        {
          
            int currentPageIndex = page < 0 ? 0 : page - 1;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            var query = new QueryOrders();
            int take = itemsperpage;
            int skip = currentPageIndex * take;
            query.Skip = skip;
            query.Take = take;
            query.OrderType = OrderType.DistributorToProducer;
            query.DocumentStatus = DocumentStatus.Closed;
            query.EndDate = DateTime.Now;
            query.StartDate = DateTime.Now.AddDays(-90);

            var deliveredOrderList = _listOrdersViewModelBuilder.Query(query);
            var data = deliveredOrderList.Data;
            var count = deliveredOrderList.Count;
            

            int pageSize = itemsperpage;

            return View(data.ToPagedList(currentPageIndex, itemsperpage, count));

           // OrderViewModel deliveredOrderList = _listOrdersViewModelBuilder.GetAllClosedPurchaseOrders(currentPageIndex, pageSize);

           //return View(deliveredOrderList);
        }
        public ActionResult ApprovedPurchaseOrders(int? distributor, int? page, string ord, string StartDate, string EndDate, int itemsperpage = 10)
        {
            if (page.HasValue)
                page = page.Value;
            else page = 0;

            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }
            var query = new QueryOrders();
            int take = itemsperpage;
            int skip = page.Value * take;
            query.Skip = skip;
            query.Take = take;
            query.OrderType = OrderType.DistributorToProducer;
            query.DocumentStatus = DocumentStatus.Approved;
            query.EndDate = DateTime.Now;
            query.StartDate = DateTime.Now.AddDays(-90);
            int pageSize = itemsperpage;

            var approvedOrderList = _listOrdersViewModelBuilder.Query(query);
            var data = approvedOrderList.Data;
            var count = approvedOrderList.Count;
           // OrderViewModel approvedOrderList = _listOrdersViewModelBuilder.GetAllApprovedOrders(page.Value, pageSize);
            
            return View(data.ToPagedList(page.Value, itemsperpage, count));
        }
    }
}
