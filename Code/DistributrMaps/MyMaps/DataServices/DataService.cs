using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using MyMaps.Models.POCOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyMaps.DataServices
{
    public class DataService
    {

        public string GetTransaction(string req_type, DateTime sDate, DateTime eDate, string distributor = "", string salesman = "", string route = "", string resultID = "", string outlet = "" , string mapType = "")
        {
            string q_transactions = "map_spGetOutletTransactions";

            string q_distributor = "map_spGetDistributors";

            string q_salesman = "map_spGetSalesMen";

            string q_routes = "map_spGetRoutes";

            string q_outlets = "map_spGetOutlets";


            string q_locations = "";
            // default points
            string q_salespoints = "map_spGetSalesPoints";
          
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var sdate = sDate;
            string maptype = mapType;

            string data = "";

            var edate = eDate.AddHours(24).AddMilliseconds(-1);
            if (req_type.Equals("Distributor"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                    using (SqlCommand cmd = new SqlCommand(q_distributor, conn) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {

                            List<BaseDto> dis = new List<BaseDto>();
                            while (rdr.Read())
                            {
                                BaseDto d = new BaseDto();
                                d.ID = rdr["DistributorId"].ToString();
                                d.Name = rdr["Distributor"].ToString();
                                dis.Add(d);
                            }
                            data = JsonConvert.SerializeObject(dis);
                        }
                        
                    }
                }
            }
            else if (req_type.Equals("Salesman"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                    using (SqlCommand cmd = new SqlCommand(q_salesman, conn) { CommandType = CommandType.StoredProcedure })
                    {
                        String sdistributor = distributor;

                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);
                        cmd.Parameters.AddWithValue("uDistributor", sdistributor);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            List<BaseDto> sales = new List<BaseDto>();
                            while (rdr.Read())
                            {
                                BaseDto s = new BaseDto();
                                s.ID = rdr["SalesmanID"].ToString();
                                s.Name = rdr["Salesman"].ToString();
                                sales.Add(s);
                            }
                            data = JsonConvert.SerializeObject(sales);
                        }
                    }
                }
            }
            else if (req_type.Equals("Routes"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                    using (SqlCommand cmd = new SqlCommand(q_routes, conn) { CommandType = CommandType.StoredProcedure })
                    {


                        String sdistributor = distributor;
                        String ssalesman = salesman;

                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);
                        cmd.Parameters.AddWithValue("uDistributor", sdistributor);
                        cmd.Parameters.AddWithValue("uSalesman", ssalesman);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                           List<BaseDto> routes = new List<BaseDto>();
                            while (rdr.Read())
                            {
                                BaseDto r = new BaseDto();
                                r.ID = rdr["RouteID"].ToString();
                                r.Name = rdr["Route"].ToString();
                                routes.Add(r);
                            }
                            data = JsonConvert.SerializeObject(routes);
                        }
                    }
                }
            }
            else if (req_type.Equals("Outlet"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                    using (SqlCommand cmd = new SqlCommand(q_outlets, conn) { CommandType = CommandType.StoredProcedure })
                    {

                        String sdistributor = distributor;
                        String ssalesman = salesman;
                        String sroute = route;

                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);
                        cmd.Parameters.AddWithValue("uDistributor", sdistributor);
                        cmd.Parameters.AddWithValue("uSalesman", ssalesman);
                        cmd.Parameters.AddWithValue("uRoute", sroute);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            List<BaseDto> outs = new List<BaseDto>();
                            while (rdr.Read())
                            {
                                BaseDto o = new BaseDto();
                                o.ID = rdr["OutletId"].ToString();
                                o.Name = rdr["Outlet"].ToString();
                                outs.Add(o);
                            }
                            data = JsonConvert.SerializeObject(outs);
                        }
                    }
                }
            }
            else if (req_type.Equals("Transactions"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                    using (SqlCommand cmd = new SqlCommand(q_transactions, conn) { CommandType = CommandType.StoredProcedure })
                    {

                        String stransaction = resultID;

                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);
                        cmd.Parameters.AddWithValue("sResultID", stransaction);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            
                            List<Transaction> transactions = new List<Transaction>();
                            while (rdr.Read())
                            {
                                Transaction t = new Transaction();
                                t.ID = rdr["row"].ToString();
                                t.SalesAmount = rdr["SaleAmount"].ToString();
                                t.DocumentReference = rdr["DocumentReference"].ToString();
                                t.DocumentDateIssued = rdr["DocumentDateIssued"].ToString();
                                t.OutLet = rdr["Outlet"].ToString();
                                t.Salesman = rdr["Salesman"].ToString();
                                t.SaleDiscount = rdr["SaleDiscount"].ToString();
                                t.ProductDiscount = rdr["ProductDiscount"].ToString();
                                transactions.Add(t);
                            }
                            data = JsonConvert.SerializeObject(transactions);
                        }
                    }
                }
            }
            else if (req_type.Equals("Locations"))
            {
                using (SqlConnection conn = new SqlConnection(Ctx))
                {
                   q_locations = q_salespoints;

                    using (SqlCommand cmd = new SqlCommand(q_locations, conn) { CommandType = CommandType.StoredProcedure })
                    {

                       
                        String sdistributor = distributor;
                        String ssalesman = salesman;
                        String sroute = route;
                        String soutlet =outlet;

                        cmd.Parameters.AddWithValue("sDate", sdate);
                        cmd.Parameters.AddWithValue("eDate", edate);
                        cmd.Parameters.AddWithValue("uDistributor", sdistributor);
                        cmd.Parameters.AddWithValue("uSalesman", ssalesman);
                        cmd.Parameters.AddWithValue("uOutlet", soutlet);
                        cmd.Parameters.AddWithValue("uRoute", sroute);

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                           List<Location> locs = new List<Location>();
                            while (rdr.Read())
                            {
                                Location l = new Location();
                                l.ID = rdr["ResultID"].ToString();
                                l.Lat = rdr["Latitude"].ToString();
                                l.Lon = rdr["Longitude"].ToString();
                                l.Route = rdr["Route"].ToString();
                                try
                                {
                                    l.Sales_Amount = Convert.ToDouble(rdr["SaleAmount"].ToString());
                                }
                                catch (Exception ex)
                                {

                                }
                                l.Outlet = rdr["Outlet"].ToString();
                                l.DocDate = rdr["DocumentDateIssued"].ToString();

                                try
                                {
                                    l.ReasonNotSold = rdr["ReasonNotSold"].ToString();
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    l.isDeviation = Int32.Parse(rdr["isDeviation"].ToString());
                                }
                                catch (Exception)
                                {

                                }

                                locs.Add(l);
                            }
                            data = JsonConvert.SerializeObject(locs);
                        }
                    }
                }
            }
            return data;
        }

        protected string Ctx
        {
            get
            {
                return System.Configuration.ConfigurationManager.
                    ConnectionStrings["datacontext"].ConnectionString;
            }
        }

        protected string SearchVar(List<string> x, string content)
        {
            String result = "";
            foreach (String y in x)
            {
                if (!string.IsNullOrEmpty(y) && !string.IsNullOrWhiteSpace(y) && !string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
                {
                    string pcontent = content + ":";
                    int len_p = content.Length + 1;
                    string yquery = y.Substring(0, len_p);
                    if (pcontent.Equals(yquery))
                    {
                        int len_y = y.Length - len_p;
                        result = y.Substring(len_p, len_y);
                        return result;
                    }
                }
            }
            return result;
        }
    
    }
}