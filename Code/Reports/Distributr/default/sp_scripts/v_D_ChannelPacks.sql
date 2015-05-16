DROP VIEW [dbo].[v_D_ChannelPacks]
GO
CREATE VIEW [dbo].[v_D_ChannelPacks]
AS
SELECT dbo.tblOutletType.id AS OutletTypeId, 
       dbo.tblOutletType.Name AS OutletTypeName, 
       dbo.tblChannelPackaging.id AS ChannelPackId, 
       dbo.tblProductPackaging.Name AS ChannelPackName, 
       dbo.tblProduct.id AS ProductId, 
       dbo.tblProduct.Description AS ProductName
FROM   dbo.tblChannelPackaging 
 JOIN  dbo.tblOutletType ON dbo.tblChannelPackaging.OutletTypeId = dbo.tblOutletType.id 
 JOIN  dbo.tblProductPackaging ON dbo.tblChannelPackaging.PackagingId = dbo.tblProductPackaging.Id 
 JOIN  dbo.tblProduct ON dbo.tblProductPackaging.Id = dbo.tblProduct.PackagingId
WHERE     (dbo.tblChannelPackaging.IsChecked = 1)