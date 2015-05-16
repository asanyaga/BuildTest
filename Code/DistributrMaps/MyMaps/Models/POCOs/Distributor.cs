using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMaps.Models.POCOs
{
    public class Transaction
    {
        public string ID { get; set; }
        public string SalesAmount { get; set; }
        public string DocumentReference { get; set; }
        public string DocumentDateIssued { get; set; }
        public string OutLet { get; set; }
        public string Salesman { get; set; }
        public string SaleDiscount { get; set; }
        public string ProductDiscount { get; set; }
    }

    public class Location
    {
        public string ID { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string Route { get; set; }

        private double sale_amount;
        public double Sales_Amount { 
            get{
                return Math.Round(this.sale_amount, 2);
            }
            set{
                this.sale_amount = value;
            }
        }
        public string Outlet { get; set; }
        public string OutletID { get; set; }
        public string DocDate { get; set; }
        public string Distributor { get; set; }
        public string DistributorID { get; set; }
        public int isDeviation { get; set; }

        // ============= deviation extra details ==============

        public string MapType { get; set; }
        public int isZeroSale { get; set; }
        public string ReasonNotSold { get; set; }
        public double TargetValue { get; set; }
        public double SalesValue { get; set; }
        public double TargetQuantity { get; set; }
        public double SalesQuantity { get; set; }
        public double HeatScore { get; set; }

        // =====================================================
        private string full_details; 
        public string Full_Details
        {
            //get details of the outlet if its not a deviation
            get
            {
                if (!string.IsNullOrWhiteSpace(this.full_details))
                {
                    return this.full_details;
                }
                else
                {
                    return "Outlet: " + this.Outlet + " <br/> " + " Transaction Date : " + this.DocDate + " <br/> " + "Amount: " + String.Format("{0:n}", this.Sales_Amount);
                }

            }
            set{
                this.full_details = value;
            }
        }

        private string link;
        public string Link {
            get
            {
                if(string.IsNullOrWhiteSpace(this.link)){
                    this.link = "<a href='#?sdate=?edate=?outlet='>View Report</a>";
                }
                return this.link;
            }
            set{
                this.link = value;
            } 
        }

    }

    public class BaseDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}