using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders
{
   public interface IProductPackagingSummaryViewBuilder
    {
       void AddProduct(Guid productId, decimal quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix = true);
        List<PackagingSummary> GetProductSummary();
        void ClearBuffer();
        void RemoveProduct(Guid productId);
        List<PackagingSummary> GetProductSummaryByProduct(Guid productId, decimal quantity, bool isOneToOne = false);
        decimal GetProductQuantityInBulk(Guid productId);
       List<PackagingSummary> GetMixedPackContainers(List<PackagingSummary> returnableProducts);
    }
   public interface IProductPackagingSummaryClient
   {
       void AddProduct(string key,Guid productId, decimal quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix = true);
       List<PackagingSummary> GetProductSummary(string key);
       void ClearBuffer(string key);
       void RemoveProduct(string key, Guid productId);
       List<PackagingSummary> GetProductSummaryByProduct( Guid productId, decimal quantity, bool isOneToOne = false);
       decimal GetProductQuantityInBulk(Guid productId);
       
   }
   public class ProductPackagingSummary
   {
       public Product Product { set; get; }
       public decimal Quantity { set; get; }
       public bool IsOneToOne { set; get; }
       public bool IsMix { set; get; }

   }

   public class PackagingSummary
   {
       public Product Product { set; get; }
       public decimal Quantity { set; get; }
       public bool IsEditable { set; get; }
       public bool IsAuto { set; get; }
       public Guid ParentProductId { set; get; }
   }
}
