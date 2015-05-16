DROP PROCEDURE [dbo].[sp_D_FilterBy]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_D_FilterBy]
as
 select 0 as FilterById,
       'Server Date' as FilterByName
 union 
 select 1 as FilterById,
       'Sale Date' as FilterByName
  
 --  Exec [dbo].[sp_D_FilterBy]
     
 Go