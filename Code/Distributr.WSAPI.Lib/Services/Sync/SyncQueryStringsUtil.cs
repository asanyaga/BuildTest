using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WSAPI.Lib.Services.Sync
{
  public static class SyncQueryStringsUtil
    {
      public static string GetProductGroupDiscountQuery
      {
          get { return @" SELECT p.[Id] as MasterId
                              ,p.[DiscountGroup] as DiscountGroupMasterId
                              ,p.[IM_DateCreated] as DateCreated
                              ,p.[IM_DateLastUpdated] as DateLastUpdated
                              ,p.[IM_Status]  as StatusId 
                              ,pItem.id as LineItemId
                              ,pItem.DiscountRate as DiscountRate
                              ,pItem.EffectiveDate as EffectiveDate
                              ,pItem.EndDate as EndDate
                              ,pItem.ProductRef as ProductMasterId
                               FROM   [tblProductDiscountGroup] p
                                           OUTER APPLY (SELECT TOP 1 *
                                            FROM   [tblProductDiscountGroupItem] pItem
                                            WHERE  pItem.ProductDiscountGroup = p.id and pItem.IM_Status !=3
                                            ORDER  BY pItem.EffectiveDate desc) pItem
                               WHERE p.IM_Status !=3"; }
      }


      public static string GetDeletedProductGroupDiscountQuery
      {
          get
          {
              return @" SELECT p.[Id] as MasterId
                              ,p.[DiscountGroup] as DiscountGroupMasterId
                              ,p.[IM_DateCreated] as DateCreated
                              ,p.[IM_DateLastUpdated] as DateLastUpdated
                              ,p.[IM_Status]  as StatusId 
                              ,pItem.id as LineItemId
                              ,pItem.DiscountRate as DiscountRate
                              ,pItem.EffectiveDate as EffectiveDate
                              ,pItem.EndDate as EndDate
                              ,pItem.ProductRef as ProductMasterId
                               FROM   [tblProductDiscountGroup] p
                                           OUTER APPLY (SELECT TOP 1 *
                                            FROM   [tblProductDiscountGroupItem] pItem
                                            WHERE  pItem.ProductDiscountGroup = p.id and pItem.IM_Status !=3
                                            ORDER  BY pItem.EffectiveDate desc) pItem
                               WHERE p.IM_Status !=3";//Not neccessarily true
          }
      }

      public static string GetProductPricingQuery
      {
          get
          {
              return
                  @"SELECT p.[Id] as MasterId
                          ,p.[ProductRef] as ProductMasterId
                          ,p.[IM_DateCreated] as DateCreated
                          ,p.[IM_DateLastUpdated] as DateLastUpdated
                          ,p.[IM_Status]  as StatusId 
                          ,p.[Tier] as ProductPricingTierMasterId
                          ,pItem.id as LineItemId
                        ,pItem.Exfactory as ExFactoryRate
                        ,pItem.EffecitiveDate as EffectiveDate
                        ,pItem.SellingPrice as SellingPrice    
                        FROM   [tblPricing] p
                           OUTER APPLY (SELECT TOP 1 *
                                        FROM   [tblPricingItem] pItem
                                        WHERE  pItem.PricingId = p.id and pItem.IM_Status !=3
                                        ORDER  BY pItem.EffecitiveDate desc) pItem
                        WHERE p.IM_Status !=3";
          }
      }
    }
}
