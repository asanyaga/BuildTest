﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Distributr.Core.Data.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MainOrderResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MainOrderResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Distributr.Core.Data.Resources.MainOrderResource", typeof(MainOrderResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --;with cte as (select  o.Id, 
        ///-- o.DocumentDateIssued,
        ///-- o.DocumentReference,
        ///-- o.SaleDiscount ,
        ///-- (select ISNULL(sum(round((il.Quantity * il.Value) + (il.Vat * il.Quantity),2,1)),0) as InvoiceAmount
        ///--  from tblDocument i
        ///--  join tblLineItems il on il.DocumentID=i.id
        ///--  where i.DocumentTypeId=5 and i.DocumentParentId=o.id) InvoiceAmount,
        ///-- (select ISNULL(sum(rl.Value ),0) as InvoiceAmount
        ///--  from tblDocument r
        ///--  join tblLineItems rl on rl.DocumentID=r.id
        ///--  where r.DocumentTypeId=8 an [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Outstanding {
            get {
                return ResourceManager.GetString("Outstanding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ;with cte as (select  o.Id, 
        /// o.DocumentDateIssued as [Required],
        /// o.DocumentReference,
        /// o.ExtDocumentReference,
        /// (select Name from tblCostCentre where CostCentreType=4 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Salesman,
        /// (select Name from tblCostCentre where CostCentreType=2 and  (o.DocumentIssuerCostCentreId=id or o.DocumentRecipientCostCentre=id))as Distributor,
        /// (select Name from tblCostCentre where CostCentreType=5 and  o.OrderIssuedOnBehalfOfCC=id )as Outlet,        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string OutstandingOutletOrders {
            get {
                return ResourceManager.GetString("OutstandingOutletOrders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ;with cte as (
        ///select  (select ISNULL(sum(rl.Value ),0) as InvoiceAmount
        ///		  from tblDocument r
        ///		  join tblLineItems rl on rl.DocumentID=r.id
        ///		  where r.DocumentTypeId=8 and r.DocumentParentId=o.id
        ///		) as ReceiptAmount,
        ///		(select ISNULL(sum((cl.Quantity * cl.Value) +(cl.Vat * cl.Quantity)),0) as CreditAmount
        ///		  from tblDocument c
        ///		  join tblLineItems cl on cl.DocumentID=c.id
        ///		  where c.DocumentTypeId=10 and c.DocumentParentId=o.Id
        ///		  ) CreditAmount
        /// from tblDocument o
        /// where o.DocumentType [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PaidAmount {
            get {
                return ResourceManager.GetString("PaidAmount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to declare @startdate  datetime,@enddate  datetime;
        ///declare @ordertypeId  int,@pageStart int,@pageEnd int;
        ///set @pageStart=&apos;{0}&apos;;
        ///set @pageEnd=&apos;{1}&apos;;
        ///set @ordertypeId=&apos;{2}&apos;;
        ///set @startdate=&apos;{3}&apos;;
        ///set @enddate=&apos;{4}&apos;
        ///
        ///;with cteorder as (
        /// select ROW_NUMBER() Over (order by o.DocumentDateIssued desc) as [Row],COUNT(*) OVER() AS [RowCount], o.* from tblDocument o 
        /// where o.DocumentTypeId=1
        ///  and o.DocumentStatusId=1  
        ///  and o.OrderOrderTypeId =@ordertypeId
        ///  and o.Id=o.OrderParentId 
        ///  and o.DocumentD [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PendingApproval {
            get {
                return ResourceManager.GetString("PendingApproval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  Select Distinct o.OrderParentId
        /// from [tblDocument] as o 
        /// join tblLineItems items on items.DocumentID=o.Id 
        /// where o.DocumentDateIssued between &apos;{0}&apos; and &apos;{1}&apos;
        /// and items.LineItemStatusId=3.
        /// </summary>
        internal static string PendingDispatch {
            get {
                return ResourceManager.GetString("PendingDispatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  Select Distinct o.OrderParentId
        /// from [tblDocument] as o 
        /// join tblLineItems items on items.DocumentID=o.Id 
        ///  where o.DocumentTypeId=1 and o.OrderOrderTypeId=2 and o.DocumentDateIssued between &apos;{0}&apos; and &apos;{1}&apos;
        /// and items.LineItemStatusId=3.
        /// </summary>
        internal static string PurchaseOrdersPendingDispatch {
            get {
                return ResourceManager.GetString("PurchaseOrdersPendingDispatch", resourceCulture);
            }
        }
    }
}
