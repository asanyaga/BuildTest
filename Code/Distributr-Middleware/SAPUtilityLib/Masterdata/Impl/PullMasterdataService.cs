using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using SAPbobsCOM;

namespace SAPUtilityLib.Masterdata.Impl
{
   
    internal class PullMasterdataService : SAPUtilsBase, IPullMasterdataService
    {
        public PullMasterdataService()
        {
            MasterDataList = new List<ImportEntity>();
        }
        public DateTime GetCurrentDatetime()
        {
           
            var time = Company.GetCompanyTime().Split(':');
            var date = Company.GetCompanyDate();
            int hour = 12;
            int.TryParse(time[0], out hour);
            int min = 0;
            int.TryParse(time[1], out min);
            return new DateTime(date.Year, date.Month, date.Day, hour, min, 0);
        }
        public SyncBasicResponse Import(string masterData)
        {
            masterdataEntity = masterData;
           MasterDataList=new List<ImportEntity>();
          
            switch (masterdataEntity)
            {
                case "Outlet":
                    return GenerateCustomers();
                case "DistributorSalesman":
                    return GenerateSalesman();
                case "SaleProduct":
                    return GenerateProducts();
                case "PricingTier":
                    return GeneratePricingTier();
                case "Pricing":
                    return GenerateProductPrices();
                case "Supplier":
                    return GenerateSuppliers();
                case "VatClass":
                    return GenerateVatClass();
                case "Bank":
                    return GenerateBanks();
                case "Region":
                    return GenerateRegions();
                case "Route":
                    return GenerateRoutes(); //create routes from region
                case "Distributor":
                    return GenerateDistributors();
                case "Country":
                    return GenerateCountries();
                case "shipto":
                 return GenerateSHipTo();
                case "ProductBrand":
                    return GenerateProductBrands();

            }

            return new SyncBasicResponse{Info="Invalid master type"};
        }
        private SyncBasicResponse GenerateCountries()
        {
           
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery(
                    @"SELECT  Location as Name,Location as [Description],(SELECT code from OCRY where name like '%kenya%' ) as Country  from OLCT");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateDistributors()

        {
         
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery(@"SELECT WhsCode,WhsName, region.Location region,IntrnalKey,VatGroup from OWHS wh
                            join OLCT as region on wh.Location=region.Code 
							where WhsCode like '%MAIN%'");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }
            return response;
        }
        private SyncBasicResponse GenerateVatClass()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery("SELECT Code,Code+'-'+Name as Name,Rate/100 as Rate from OVTG");
               return  DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);

            }

            return response;
        }
        private SyncBasicResponse GenerateSuppliers()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery("SELECT CardCode,CardName from OCRD where CardType='S'");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateRoutes()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               // rs.DoQuery(" SELECT  GroupName as Name,GroupCode as code,GroupName as regionname from ocrg");
               // rs.DoQuery("SELECT  Location as Name,code,Location  as regionname from OLCT");
                rs.DoQuery(@";with cte as(
SELECT  GroupName as Name,GroupCode as code, 
(SELECT top 1  Location  from OLCT where SUBSTRING(Location, 1, 3)=SUBSTRING(GroupName, 1, 3)) as regionname
  from ocrg)
  select * from cte
  where  regionname is not null");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateRegions()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery("SELECT  Location as Name,Location as Description from OLCT");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateBanks()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery("SELECT BankCode,BankName,BankName from ODSC");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateProductBrands()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                rs.DoQuery("SELECT ItmsGrpCod,ItmsGrpNam from  OITB"); //I use product groups for now from SAP
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);

            }
            return response;
        }
        private SyncBasicResponse GeneratePricingTier()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset) Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                rs.DoQuery("SELECT ListNum, ListName from OPLN");
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);


            }
            return response;
        }
        private SyncBasicResponse GenerateProductPrices()
        {
            var response = new SyncBasicResponse();
            try
            {
                    var syncTracker = MasterDataSyncConfiguration.Load();
            var tracker = syncTracker.Item.FirstOrDefault(s => s.Collective == MasterDataCollective.Pricing);
            DateTime lastsyncDateTime = tracker != null ? tracker.LastSyncDateTime : new DateTime(1940, 1, 1);
                Recordset rs = (Recordset) Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = string.Format(@"select price.ItemCode,PriceList, 

                            cast(ROUND(Price/product.SalPackUn,2) as numeric(36,2)) as UnitPrice
                              from ITM1 price
                              join OITM product on product.ItemCode=price.ItemCode
                            WHERE price.ItemCode in(
                            SELECT ItemCode from OITM product
                             where product. QryGroup64='Y'
                            AND product.ItemName is not null and (product.UpdateDate>='{0}' or product.CreateDate>='{0}')
                            --select active products only
                            AND validFor='Y') ", lastsyncDateTime.ToString("yyyy-MM-dd"));
                rs.DoQuery(query);
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);

            }
            return response;
        }
        private SyncBasicResponse GenerateProducts()
        {
            var response = new SyncBasicResponse();
            try
            {
                 var syncTracker = MasterDataSyncConfiguration.Load();
            var tracker = syncTracker.Item.FirstOrDefault(s => s.Collective == MasterDataCollective.SaleProduct);
            DateTime lastsyncDateTime = tracker != null ? tracker.LastSyncDateTime : new DateTime(1940, 1, 1);
                Recordset rs = (Recordset) Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query =string.Format( @" SELECT ItemCode,ItemName,'0.0' as exfactory,ItemType as producttype,
                                 productGroup.ItmsGrpNam as brand,'default' as flavour,'default'as packagingtype,VatGourpSa from OITM product
                                 join OITB productGroup on product.ItmsGrpCod=productGroup.ItmsGrpCod                                
                                 where   QryGroup64='Y'
                                and product.ItemName is not null                               
                                and validFor='Y' and (product.UpdateDate>='{0}' or product.CreateDate>='{0}')


	   ", lastsyncDateTime.ToString("yyyy-MM-dd"));
                //productcode,description,exfactoryprice,producttypecode,brandcode,flavour,packagingtype,vatclass,returnabletype*,returnablenamecode
                rs.DoQuery(query);
                return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }
            return response;
        }
        private SyncBasicResponse GenerateCustomers()
        {
            var syncTracker = MasterDataSyncConfiguration.Load();
            var tracker = syncTracker.Item.FirstOrDefault(s => s.Collective == MasterDataCollective.Outlet);
            DateTime lastsyncDateTime = tracker != null ? tracker.LastSyncDateTime : new DateTime(1940, 1, 1);
            var response = new SyncBasicResponse();
            try
            {
                //todo go=>we filter outlets for NRB-VAN4 and NRB-VAN10 only.Others to be decided later
               Recordset rs = (Recordset) Company.GetBusinessObject(BoObjectTypes.BoRecordset);
               string query = string.Format(@";with cte as (
SELECT CardCode,CardName,
(case  when SUBSTRING(outlet.CardCode, 2, 3)= 'NRB'   then 'NBO-MAIN'
       when SUBSTRING(outlet.CardCode, 2, 3)= 'KSM'   then 'KSM-MAIN'
	   when SUBSTRING(outlet.CardCode, 2, 3)= 'MSA'   then 'MSA-MAIN' 
	   when SUBSTRING(outlet.CardCode, 2, 3)= 'EMB'   then 'EMB-MAIN' 
	   else  NULL end)  as DistributorCode,		 
	  rout.GroupCode as [group Code],
	   'default' as OutletCategory,'default' as OutletType,
	   '' as DiscountGroup,ListNum,VatGroup,''as specialtier,
	   '0' as Latitude,'0' as longitude   
	  from OCRD outlet 
	  join OCRG customerGroup on outlet.GroupCode=customerGroup.GroupCode
	  join (SELECT grp.GroupCode from ocrg  grp left join OLCT loc on SUBSTRING(Location, 1, 3)=SUBSTRING(GroupName, 1, 3) where loc.Location is not null)
	      rout on rout.GroupCode= customerGroup.GroupCode
	    where CardType='C'  and QryGroup61='Y'  and (outlet.UpdateDate>='{0}' or outlet.CreateDate>='{0}')
 )
select * from cte where DistributorCode is not null and cardname is not null



", lastsyncDateTime.ToString("yyyy-MM-dd"));
               rs.DoQuery(query);
               return DampToFolder(rs);
            }
            catch (Exception ex)
            {
                FileUtility.LogError(ex.Message);
            }

            return response;
        }
        private SyncBasicResponse GenerateSHipTo()
        {
            var response = new SyncBasicResponse();
            try
            {
                Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                rs.DoQuery("SELECT CardCode,address,Street,Block,City from CRD1");

              return  DampToFolder(rs);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message; 
                FileUtility.LogError(ex.Message);
            }
            return response;
        }
        private SyncBasicResponse GenerateSalesman()
        {
            var response = new SyncBasicResponse();
            try
            {

                Recordset rs = (Recordset) Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               
                //go=>Towfiq extract warehouses van10,van4 as salesman..subsequent salesmen will have to be generated from here as warehouses
                var towfiq = @";with cte as(
	SELECT WhsCode,WhsName,
	(select WhsCode from OWHS where SUBSTRING(WhsCode, 1, 3)=SUBSTRING(sm.WhsCode, 1, 3) and WhsCode like '%MAIN%') as DistributorCode
	FROM OWHS sm
	 WHERE U_Company ='R' and WhsCode like '%VAN%' 								
)
select * from cte where DistributorCode is not null	
";
                rs.DoQuery(towfiq);
                return DampToFolder(rs);

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Info = ex.Message;
                FileUtility.LogError(ex.Message);
            }
            return response;
        }

        #region Helpers

       
       
        #endregion
    }
    public class BaseImportObject
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }
        public string Field9 { get; set; }
        public string Field10 { get; set; }
        
    }

}
