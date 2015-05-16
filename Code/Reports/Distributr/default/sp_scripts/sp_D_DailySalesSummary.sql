DROP PROCEDURE [dbo].[sp_D_DailySalesSummary]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_D_DailySalesSummary]
as
select dbo.fn_D_GetDaysTransValue() as DaysTransValue,
       Recoll.SaleValue as DaysRecollectionReturns,
       0 as PreviousTransOutstanding,
       0 as RecollectionOutstanding,
       0 as TotalOutstanding,
       case when DaysSales.Receipt_PaymentTypeId = 1 then DaysSales.ReceiptAmount else  0 end as CashReceiptFromSale,
       case when DaysSales.Receipt_PaymentTypeId = 2 then DaysSales.ReceiptAmount else  0 end as ChequeReceiptFromSale,
       case when DaysSales.Receipt_PaymentTypeId = 3 then DaysSales.ReceiptAmount else  0 end as MmoneyReceiptFromSale,
       case when Recoll.Receipt_PaymentTypeId = 1 then Recoll.ReceiptAmount else  0 end as CashRecollOnRecoll,
       case when Recoll.Receipt_PaymentTypeId = 2 then Recoll.ReceiptAmount else  0 end as ChequeRecollOnRecoll,
       case when Recoll.Receipt_PaymentTypeId = 3 then Recoll.ReceiptAmount else  0 end as MmoneyRecollOnRecoll,
       0 as TotalDaysCollection,
       0 as BalanceDue,
       'Sale_01' as SalesNo,
       GetDate() as SaleDateTime,
       GetDate() as RecollDateTime,
       '0123' as SaleCustomerCode,
       '9876' as RecollCustomerCode,
       'Sale_Customer' as SaleCustormerName,
       'Recoll_Customer' as RecollCustomerName
       
       from [dbo].[fn_D_GetDaysRecollectionReturns]()  Recoll,[dbo].[fn_D_GetDaysReceiptFromSales]() DaysSales
        
        
     
        