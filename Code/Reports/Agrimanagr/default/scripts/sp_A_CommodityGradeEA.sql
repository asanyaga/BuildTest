IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_A_CommodityGradeEA')
   exec('CREATE PROCEDURE [sp_A_CommodityGradeEA] AS BEGIN SET NOCOUNT ON; END')
GO

Alter PROCEDURE sp_A_CommodityGradeEA 

@CommodityId AS NVARCHAR(50)  
 
AS
if  @CommodityId='ALL'  begin set @CommodityId='%' end

BEGIN		
	SELECT  'ALL' AS Id, 'ALL' AS Name
	UNION
	select convert(varchar(50),tblCommodityGrade.Id) as Id , tblCommodityGrade.Name
	from tblCommodity INNER JOIN
	tblCommodityGrade ON tblCommodity.Id = tblCommodityGrade.CommodityId
	Where (CONVERT(NVARCHAR(50),tblCommodity.Id) LIKE ISNULL(@CommodityId, N'%'))	
	order by name	
END;

--EXEC sp_A_CommodityGradeEA @CommodityId='ALL'
