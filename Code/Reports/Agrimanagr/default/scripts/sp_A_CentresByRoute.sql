IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CentresByRoute')
   exec('CREATE PROCEDURE [sp_A_CentresByRoute] AS BEGIN SET NOCOUNT ON; END')
GO
Alter PROCEDURE sp_A_CentresByRoute
	 @routeId varchar(50)=NULL     
AS
BEGIN
    if(@routeId='' or @routeId='ALL') set @routeId=null;
	
	SELECT  'ALL' AS RouteId, 'ALL' AS Name
	UNION
	select LOWER(convert(varchar(50),id)) CentreId, Name from tblcentre 
	where (1=1)
	and(@routeid is null or routeid=@routeId)
	order by name
END;

--EXEC sp_A_CentresByRoute @routeid='ALL'

