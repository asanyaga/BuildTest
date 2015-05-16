/****** Object:  Index [UQ__tblAsset__737584F66F5D5F7F]    Script Date: 08/24/2012 08:24:18 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tblAssetCategory]') AND name = N'UQ__tblAsset__737584F66F5D5F7F')
ALTER TABLE [dbo].[tblAssetCategory] DROP CONSTRAINT [UQ__tblAsset__737584F66F5D5F7F]
GO

GO

/****** Object:  Index [UQ__tblAsset__737584F6465B49EC]    Script Date: 08/24/2012 08:27:06 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tblAssetStatus]') AND name = N'UQ__tblAsset__737584F6465B49EC')
ALTER TABLE [dbo].[tblAssetStatus] DROP CONSTRAINT [UQ__tblAsset__737584F6465B49EC]
GO
