IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__300424B4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF__tblProduc__IM_Is__300424B4]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__ExFac__41BFE6AA]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF__tblProduc__ExFac__41BFE6AA]
END
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__2E1BDC42]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF__tblProduc__IM_Da__2E1BDC42]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__2F10007B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF__tblProduc__IM_Da__2F10007B]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblProduct_Capacity]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF_tblProduct_Capacity]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_St__66BC612F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProduct] DROP CONSTRAINT [DF__tblProduc__IM_St__66BC612F]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__2AF18FEB]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductBrand] DROP CONSTRAINT [DF__tblProduc__IM_Da__2AF18FEB]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__2BE5B424]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductBrand] DROP CONSTRAINT [DF__tblProduc__IM_Da__2BE5B424]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__2CD9D85D]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductBrand] DROP CONSTRAINT [DF__tblProduc__IM_Is__2CD9D85D]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_St__4FD8FBD7]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductDiscountGroup] DROP CONSTRAINT [DF__tblProduc__IM_St__4FD8FBD7]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_St__50CD2010]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductDiscountGroupItem] DROP CONSTRAINT [DF__tblProduc__IM_St__50CD2010]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__33D4B598]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Da__33D4B598]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__34C8D9D1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Da__34C8D9D1]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__35BCFE0A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Is__35BCFE0A]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7B5B524B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Da__7B5B524B]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7C4F7684]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Da__7C4F7684]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__7D439ABD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Is__7D439ABD]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__33D4B598]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Da__33D4B598]
END
GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__34C8D9D1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Da__34C8D9D1]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__35BCFE0A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductFlavour] DROP CONSTRAINT [DF__tblProduc__IM_Is__35BCFE0A]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7B5B524B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Da__7B5B524B]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7C4F7684]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Da__7C4F7684]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__7D439ABD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackaging] DROP CONSTRAINT [DF__tblProduc__IM_Is__7D439ABD]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7E37BEF6]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackagingType] DROP CONSTRAINT [DF__tblProduc__IM_Da__7E37BEF6]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__7F2BE32F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackagingType] DROP CONSTRAINT [DF__tblProduc__IM_Da__7F2BE32F]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__00200768]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductPackagingType] DROP CONSTRAINT [DF__tblProduc__IM_Is__00200768]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__3C69FB99]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductType] DROP CONSTRAINT [DF__tblProduc__IM_Da__3C69FB99]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Da__3D5E1FD2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductType] DROP CONSTRAINT [DF__tblProduc__IM_Da__3D5E1FD2]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblProduc__IM_Is__3E52440B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblProductType] DROP CONSTRAINT [DF__tblProduc__IM_Is__3E52440B]
END

GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblRegion__IM_Is__412EB0B6]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblRegion] DROP CONSTRAINT [DF__tblRegion__IM_Is__412EB0B6]
END

GO


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__tblRoutes__IM_Is__440B1D61]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[tblRoutes] DROP CONSTRAINT [DF__tblRoutes__IM_Is__440B1D61]
END

GO



















































