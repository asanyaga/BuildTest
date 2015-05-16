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

ALTER TABLE dbo.tblArea DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblArea ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblAsset DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblAsset ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblAssetCategory DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblAssetCategory ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblAssetStatus DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblAssetStatus ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblAssetType DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblAssetType ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblBank DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblBank ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblBankBranch DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblBankBranch ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCertainValueCertainProductDiscount DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCertainValueCertainProductDiscount ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCertainValueCertainProductDiscountItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCertainValueCertainProductDiscountItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblChannelPackaging DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblChannelPackaging ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblClientMasterDataTracker DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblClientMasterDataTracker ADD  IM_Status INT NOT NULL Default 1;
GO --dddddddd
ALTER TABLE dbo.tblCompetitor DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCompetitor ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCompetitorProducts DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCompetitorProducts ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblContact DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblContact ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblContactType DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblContactType ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblContainment DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblContainment ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCostCentre DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCostCentre ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCostCentreApplication DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCostCentreApplication ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCountry DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCountry ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblCustomerDiscount DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCustomerDiscount ADD  IM_Status INT NOT NULL Default 1;;
GO
ALTER TABLE dbo.tblCustomerDiscountItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblCustomerDiscountItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblDiscountGroup DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblDiscountGroup ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblDiscountItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblDiscountItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblDiscounts DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblDiscounts ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblDistrict DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblDistrict ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblFiles DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblFiles ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblFreeOfChargeDiscount DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblFreeOfChargeDiscount ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblInventory DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblInventory ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblInventoryTransaction DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblInventoryTransaction ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblMaritalStatus DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblMaritalStatus ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblMarketAudit DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblMarketAudit ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblOutletAudit DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblOutletAudit ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblOutletCategory DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblOutletCategory ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblOutletPriority DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblOutletPriority ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblOutletType DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblOutletType ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblPricing DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblPricing ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblPricingItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblPricingItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblPricingTier DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblPricingTier ADD  IM_Status INT NOT NULL Default 1;
GO


ALTER TABLE dbo.tblProduct DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProduct ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductBrand DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductBrand ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductDiscountGroup DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductDiscountGroup ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductDiscountGroupItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductDiscountGroupItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductFlavour DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductFlavour ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductPackaging DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductPackaging ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductPackagingType DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductPackagingType ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProductType DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProductType ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblPromotionDiscount DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblPromotionDiscount ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblPromotionDiscountItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblPromotionDiscountItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblProvince DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblProvince ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblRegion DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblRegion ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblReOrderLevel DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblReOrderLevel ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblRetireDocumentSetting DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblRetireDocumentSetting ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblRoutes DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblRoutes ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSalemanRoute DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSalemanRoute ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSaleValueDiscount DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSaleValueDiscount ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSaleValueDiscountItems DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSaleValueDiscountItems ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSettings DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSettings ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSocioEconomicStatus DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSocioEconomicStatus ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblSupplier DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblSupplier ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblTarget DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblTarget ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblTargetItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblTargetItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblTargetPeriod DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblTargetPeriod ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblTerritory DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblTerritory ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblUserGroup DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblUserGroup ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblUserGroupRoles DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblUserGroupRoles ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblUsers DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblUsers ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblUserTypes DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblUserTypes ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblVATClass DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblVATClass ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblVATClassItem DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblVATClassItem ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.test DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.test ADD  IM_Status INT NOT NULL Default 1;
GO
ALTER TABLE dbo.tblOutletVisitDay DROP COLUMN IM_IsActive ;
ALTER TABLE dbo.tblOutletVisitDay ADD  IM_Status INT NOT NULL Default 1;
GO