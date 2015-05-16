using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Distributr.DatabaseSetup
{
    public class ClearDB
    {
        public void ClearDatabases()
        {
            ClearMainDatabase();
            ClearRoutingDatabase();
        }

        public void ClearRoutingDatabase()
        {
            string connectionstring = ConfigurationManager.AppSettings["routingdbconnectionstring"];
            if (!string.IsNullOrEmpty(connectionstring))
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                try
                {
                    conn.Open();
                    string sql = "delete from tblcommandroute";
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = sql;
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void ClearMainDatabase()
        {
            string connectionstring = ConfigurationManager.AppSettings["directconnectionstring"];

            SqlConnection conn = new SqlConnection(connectionstring);
            try
            {
                conn.Open();
                string sql =
                    @"update tblProductPackaging set returnableproduct=null;
delete from tblAuditLog;
                    delete from tblInventoryTransaction;
                delete from tblCertainValueCertainProductDiscountItem;
                delete from tblCertainValueCertainProductDiscount;
                delete from tblCompetitorProducts;
                delete from tblReOrderLevel;
                delete from tblChannelPackaging;
                delete from tblDiscountItem;
                delete from tblDiscounts;
                delete from tblCustomerDiscountItem;
                delete from tblCustomerDiscount;
                delete from tblSaleValueDiscountItems;
                delete from  tblSalemanRoute;
delete from tblSalemanRoute;
                delete from tblPromotionDiscountItem;
                delete from tblPromotionDiscount;
                delete from tblFreeOfChargeDiscount;
                delete from tblSaleValueDiscount;
                delete from tblProductDiscountGroupItem;
                delete from tblInventory;
                delete from tblLineItems;
                delete from tblDocument;
                delete from tblConsolidatedProductProducts;
                delete from tblPricingItem;
                delete from tblPricing;
                delete from tblTarget;
                delete from tblProduct;
                delete from tblProductFlavour;
                delete from tblProductBrand;

                delete from tblProductType;
                delete from tblProductPackaging;
                delete from tblProductPackagingType;
                delete from tblProductType;
                delete from tblPricingTier;
                delete from tblClientMasterDataTracker;
               
                delete from tblAccountTransaction;
                delete from tblAccount;
                delete from tblArea;
                delete from tblSocioEconomicStatus;
                delete from tblVATClassItem;
                
                delete from tblUsers;
 delete from tblContact;
                delete from tblCostCentreApplication;
                delete from tblCostCentre where routeid in (select routeid from tblroutes);
                delete from tblroutes ; 
delete from tblMaritalStatus;
                delete from tblCostCentre;
delete from tblVATClass;
delete from tblUserGroupRoles;
delete from tblUserGroup;
                delete from tblOutletType;
                delete from tblOutletCategory;
                delete from tblRegion;
                delete from tblDistrict;
                delete from tblProvince;
                delete from tblCountry;
                delete from tblTargetPeriod;
                delete from tblCompetitor;
                --delete from tblCooler;--cn
                --delete from tblCoolerType;--cn
                delete from tblProductDiscountGroup;
                delete from tblDiscountGroup;
delete from tblBankBranch;
delete from tblBank;
delete from tblSupplier;
delete from tblContactType;

";

                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                comm.ExecuteNonQuery();
            }

            finally
            {
                conn.Close();
            }
        }
    }
}
