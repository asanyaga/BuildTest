using MyMaps.Models.POCOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyMaps.DataServices
{
    public class Sales_Map : DataService
    {
        public string SendRequest(String request, String conditions)
        {
            String res = "";

            if (request.Equals("Sales_Map_GetDistributors"))
            {
                res = this.Sales_Map_GetDistributors(conditions);
            }
            else if (request.Equals("Sales_Map_GetSalesMen"))
            {
                res = this.Sales_Map_GetSalesMen(conditions);
            }
            else if (request.Equals("Sales_Map_GetRoutes"))
            {
                res = this.Sales_Map_GetRoutes(conditions);
            }
            else if (request.Equals("Sales_Map_GetOutlets"))
            {
                res = this.Sales_Map_GetOutlets(conditions);
            }
            else if (request.Equals("Sales_Map_GetPoints"))
            {
                res = this.Sales_Map_GetPoints(conditions);
            }

            return res;
        }

        public String Sales_Map_GetPoints(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sdate = this.SearchVar(conds, "sDate");
            String edate = this.SearchVar(conds, "eDate");
            String distributor = this.SearchVar(conds, "Distributor");
            String salesman = this.SearchVar(conds, "Salesman");
            String route = this.SearchVar(conds, "Route");
            String outlet = this.SearchVar(conds, "Outlet");


            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spGetSalesPoints", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("sDate", sdate);
                    cmd.Parameters.AddWithValue("eDate", edate);
                    cmd.Parameters.AddWithValue("uDistributor ", distributor);
                    cmd.Parameters.AddWithValue("uSalesman ", salesman);
                    cmd.Parameters.AddWithValue("uRoute ", route);
                    cmd.Parameters.AddWithValue("uOutlet ", outlet);
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<Location> locs = new List<Location>();
                        double total = 0;
                        double count = 0;
                        double min = 0;
                        double max = 0;
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

                            double sval = 0;

                            try{ sval = Convert.ToDouble(l.Sales_Amount); }catch{ }
                            locs.Add(l);
                            total = total + sval;
                            count++;
                        }

                        Location min_result = locs.Aggregate(locs[0],
                            delegate(Location Best, Location Candidate)
                            {
                                double valBest = 0;
                                try { valBest = Convert.ToDouble(Best.Sales_Amount); }
                                catch { }

                                double valCandidate = 0;
                                try { valCandidate = Convert.ToDouble(Candidate.Sales_Amount); }
                                catch { }

                                return (valBest > valCandidate) ? Candidate : Best;
                            }
                        );

                        try { min = Convert.ToDouble(min_result.Sales_Amount); }
                        catch { }

                        Location max_result = locs.Aggregate(locs[0],
                            delegate(Location Best, Location Candidate)
                            {
                                double valBest = 0;
                                try { valBest = Convert.ToDouble(Best.Sales_Amount); }
                                catch { }

                                double valCandidate = 0;
                                try { valCandidate = Convert.ToDouble(Candidate.Sales_Amount); }
                                catch { }

                                return (valBest > valCandidate) ? Best : Candidate;
                            }
                        );

                        try { max = Convert.ToDouble(max_result.Sales_Amount); }
                        catch { }

                        foreach (Location c in locs)
                        {
                            try { 

                                double x = Convert.ToDouble(c.Sales_Amount);
                                c.HeatScore = Math.Round((x / max),2);
                            }
                            catch 
                            { }
                            
                        }

                        data = JsonConvert.SerializeObject(locs);
                    }
                }
            }

            return data;
        }

        public String Sales_Map_GetDistributors(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sdate = this.SearchVar(conds, "sDate");
            String edate = this.SearchVar(conds, "eDate");
            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spGetDistributors", conn) { CommandType = CommandType.StoredProcedure })
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

            return data;
        }

        public String Sales_Map_GetSalesMen(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sdate = this.SearchVar(conds, "sDate");
            String edate = this.SearchVar(conds, "eDate");
            String distributor = this.SearchVar(conds, "Distributor");

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spGetSalesMen", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("sDate", sdate);
                    cmd.Parameters.AddWithValue("eDate", edate);
                    cmd.Parameters.AddWithValue("uDistributor", distributor);

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<BaseDto> dis = new List<BaseDto>();
                        while (rdr.Read())
                        {
                            BaseDto d = new BaseDto();
                            d.ID = rdr["SalesmanID"].ToString();
                            d.Name = rdr["Salesman"].ToString();
                            dis.Add(d);
                        }
                        data = JsonConvert.SerializeObject(dis);
                    }
                }
            }

            return data;
        }

        public String Sales_Map_GetRoutes(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sdate = this.SearchVar(conds, "sDate");
            String edate = this.SearchVar(conds, "eDate");
            String distributor = this.SearchVar(conds, "Distributor");
            String salesman = this.SearchVar(conds, "Salesman");

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spGetRoutes", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("sDate", sdate);
                    cmd.Parameters.AddWithValue("eDate", edate);
                    cmd.Parameters.AddWithValue("uDistributor", distributor);
                    cmd.Parameters.AddWithValue("uSalesman", salesman);

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<BaseDto> dis = new List<BaseDto>();
                        while (rdr.Read())
                        {
                            BaseDto d = new BaseDto();
                            d.ID = rdr["RouteID"].ToString();
                            d.Name = rdr["Route"].ToString();
                            dis.Add(d);
                        }
                        data = JsonConvert.SerializeObject(dis);
                    }
                }
            }

            return data;
        }

        public String Sales_Map_GetOutlets(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sdate = this.SearchVar(conds, "sDate");
            String edate = this.SearchVar(conds, "eDate");
            String distributor = this.SearchVar(conds, "Distributor");
            String salesman = this.SearchVar(conds, "Salesman");
            String route = this.SearchVar(conds, "Route");

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spGetOutlets", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("sDate", sdate);
                    cmd.Parameters.AddWithValue("eDate", edate);
                    cmd.Parameters.AddWithValue("uDistributor", distributor);
                    cmd.Parameters.AddWithValue("uSalesman", salesman);
                    cmd.Parameters.AddWithValue("uRoute", route);

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<BaseDto> dis = new List<BaseDto>();
                        while (rdr.Read())
                        {
                            BaseDto d = new BaseDto();
                            d.ID = rdr["OutletId"].ToString();
                            d.Name = rdr["Outlet"].ToString();
                            dis.Add(d);
                        }
                        IEnumerable<IGrouping<string, BaseDto>> dto_list = dis.GroupBy(n => n.ID);
                        List<BaseDto> dis2 = new List<BaseDto>();
                        foreach (IGrouping<string, BaseDto> g in dto_list)
                        {
                            BaseDto a = new BaseDto();
                            foreach (BaseDto bt in g)
                            {
                                a = bt;
                            }
                            dis2.Add(a);
                        }


                        data = JsonConvert.SerializeObject(dis2);
                    }
                }
            }

            return data;

        }
    }
}