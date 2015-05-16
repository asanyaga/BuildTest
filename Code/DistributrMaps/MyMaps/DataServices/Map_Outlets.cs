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
    public class Map_Outlets : DataService
    {
        public string SendRequest(String request, String conditions)
        {
            String res = "";

            if (request.Equals("Map_Outlets_GetDistributors"))
            {
                res = this.Map_Outlets_GetDistributors(conditions);
            }
            else if (request.Equals("Map_Outlets_SaveOutlet"))
            {
                res = this.Map_Outlets_SaveOutlet(conditions);
            }
            else if (request.Equals("Map_Outlets_GetPoints"))
            {
                res = this.Map_Outlets_GetPoints(conditions);
            }
            else if (request.Equals("Map_Outlets_GetMyPoints"))
            {
                res = this.Map_Outlets_GetMyPoints(conditions);
            }

            return res;
        }

        public String Map_Outlets_GetPoints(String conditions)
        {
            String data = "";

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spMap_Outlets_GetPoints", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<Location> locs = new List<Location>();
                        while (rdr.Read())
                        {
                            Location l = new Location();
                            l.Lat = rdr["Latitude"].ToString();
                            l.Lon = rdr["Longitude"].ToString();
                            l.Outlet = rdr["OutletName"].ToString();
                            l.OutletID = rdr["OutletID"].ToString();
                            l.Distributor = rdr["Competitor"].ToString();
                            l.DistributorID = rdr["CompetitorID"].ToString();

                            locs.Add(l);
                        }
                        data = JsonConvert.SerializeObject(locs);
                    }
                }
            }

            return data;
        }

        public String Map_Outlets_SaveOutlet(String conditions)
        {
            List<string> conds = new List<string>(conditions.Split('|'));
            String data = "";
            String sOutLetName = this.SearchVar(conds, "sOutletName");
            String sLatitute = this.SearchVar(conds, "sLatitute");
            String sLongitude = this.SearchVar(conds, "sLongitude");
            String uCompetitor = this.SearchVar(conds, "uCompetitor");
            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spMap_Outlets_SaveOutlet", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("sOutletName", sOutLetName);
                    cmd.Parameters.AddWithValue("sLatitute", sLatitute);
                    cmd.Parameters.AddWithValue("sLongitude", sLongitude);
                    cmd.Parameters.AddWithValue("uCompetitor", uCompetitor);
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {

                    }
                    data = "1";
                }
            }

            return data;
        }

        public String Map_Outlets_GetDistributors(String conditions)
        {
            String data = "";

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spMap_Outlets_GetDistributors", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<Location> locs = new List<Location>();
                        while (rdr.Read())
                        {
                            Location l = new Location();
                            l.Distributor = rdr["Competitor"].ToString();
                            l.DistributorID = rdr["CompetitorID"].ToString();
                            try
                            {
                                l.HeatScore = Convert.ToInt32(rdr["OutletCount"].ToString());
                            }catch(Exception e){

                            }

                            locs.Add(l);
                        }
                        data = JsonConvert.SerializeObject(locs);
                    }
                }
            }

            return data;
        }

        public String Map_Outlets_GetMyPoints(String conditions)
        {
            String data = "";

            using (SqlConnection conn = new SqlConnection(Ctx))
            {
                using (SqlCommand cmd = new SqlCommand("map_spMap_Outlets_GetMyPoints", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        List<Location> locs = new List<Location>();
                        while (rdr.Read())
                        {
                            Location l = new Location();
                            l.Lat = rdr["Latitude"].ToString();
                            l.Lon = rdr["Longitude"].ToString();
                            l.Outlet = rdr["OutletName"].ToString();
                            l.OutletID = rdr["OutletID"].ToString();

                            locs.Add(l);
                        }
                        data = JsonConvert.SerializeObject(locs);
                    }
                }
            }

            return data;
        }
    }
}