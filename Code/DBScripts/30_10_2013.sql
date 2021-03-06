
GO
/****** Object:  Table [dbo].[tblPricingTier]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPricingTier](
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[Description] [nvarchar](450) NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblPricingTier] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSupplier]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSupplier](
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](250) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](350) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblSupplier] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductBrand]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductBrand](
	[id] [uniqueidentifier] NOT NULL,
	[SupplierId] [uniqueidentifier] NULL,
	[code] [nvarchar](50) NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](4000) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_ProductBrand] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductFlavour]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductFlavour](
	[id] [uniqueidentifier] NOT NULL,
	[BrandId] [uniqueidentifier] NULL,
	[code] [nvarchar](50) NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](4000) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_ProductFlavour] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductPackagingType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductPackagingType](
	[id] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](250) NULL,
	[name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](4000) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_ProductPackagingType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductType](
	[id] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](250) NULL,
	[name] [nvarchar](250) NULL,
	[Description] [nvarchar](400) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProduct](
	[id] [uniqueidentifier] NOT NULL,
	[DomainTypeId] [int] NOT NULL,
	[BrandId] [uniqueidentifier] NULL,
	[FlavourId] [uniqueidentifier] NULL,
	[PackagingTypeId] [uniqueidentifier] NULL,
	[PackagingId] [uniqueidentifier] NULL,
	[ProductTypeId] [uniqueidentifier] NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Returnable] [uniqueidentifier] NULL,
	[UnitCases] [money] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[ReturnableType] [int] NULL,
	[Capacity] [int] NOT NULL,
	[VatClassId] [uniqueidentifier] NULL,
	[ExFactoryPrice] [decimal](20, 2) NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPricing]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPricing](
	[id] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[Tier] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblPricing] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPricingItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPricingItem](
	[id] [uniqueidentifier] NOT NULL,
	[PricingId] [uniqueidentifier] NOT NULL,
	[Exfactory] [money] NOT NULL,
	[SellingPrice] [money] NOT NULL,
	[EffecitiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblPricingItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPromotionDiscount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPromotionDiscount](
	[id] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblPromotionDiscount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPromotionDiscountItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPromotionDiscountItem](
	[id] [uniqueidentifier] NOT NULL,
	[PromotionDiscountId] [uniqueidentifier] NOT NULL,
	[ParentProductQuantity] [int] NOT NULL,
	[FreeOfChargeProductRef] [uniqueidentifier] NULL,
	[FreeOfChargeQuantity] [int] NULL,
	[DiscountRate] [money] NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblFreeOfChargeDiscountItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblContainment]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContainment](
	[id] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[ReturnableProduct] [uniqueidentifier] NOT NULL,
	[ProductPackagingType] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblContainment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductPackaging]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductPackaging](
	[Id] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](250) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[description] [nvarchar](4000) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Containment] [uniqueidentifier] NULL,
	[ReturnableProduct] [uniqueidentifier] NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_ProductPackaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTerritory]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTerritory](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_dbo.tblTerritory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCountry]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCountry](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Code] [nvarchar](150) NULL,
	[Currency] [nvarchar](150) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Territory_ID] [uniqueidentifier] NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_dbo.tlbCountry] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRegion]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRegion](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[Description] [varchar](50) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Country] [uniqueidentifier] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblRegion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRoutes]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRoutes](
	[RouteID] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[RegionId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Code] [varchar](50) NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblRoutes] PRIMARY KEY CLUSTERED 
(
	[RouteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSocioEconomicStatus]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSocioEconomicStatus](
	[id] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblSocioEconomicStatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblVATClass]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblVATClass](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Class] [nvarchar](50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblVATClass] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblOutletCategory]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOutletCategory](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Description] [varchar](max) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[Code] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblOutletCategory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblOutletType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOutletType](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[Code] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblOutletType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDiscountGroup]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDiscountGroup](
	[id] [uniqueidentifier] NOT NULL,
	[Code] [varchar](250) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblDiscountGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCostCentre](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[CostCentreType] [int] NULL,
	[ParentCostCentreId] [uniqueidentifier] NULL,
	[RouteId] [uniqueidentifier] NULL,
	[StandardWH_Latitude] [nvarchar](50) NULL,
	[StandardWH_Longtitude] [nvarchar](50) NULL,
	[StandardWH_VatRegistrationNo] [nvarchar](50) NULL,
	[Transporter_Drivername] [nvarchar](50) NULL,
	[Transporter_VehicleRegistrationNo] [nvarchar](50) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Outlet_Category_Id] [uniqueidentifier] NULL,
	[Outlet_Type_Id] [uniqueidentifier] NULL,
	[Distributor_RegionId] [uniqueidentifier] NULL,
	[Distributor_Owner] [nvarchar](500) NULL,
	[Distributor_PIN] [nvarchar](50) NULL,
	[Distributor_ASM_Id] [uniqueidentifier] NULL,
	[SalesRep_Id] [uniqueidentifier] NULL,
	[Surveyor_Id] [uniqueidentifier] NULL,
	[Tier_Id] [uniqueidentifier] NULL,
	[Disabled] [bit] NULL,
	[Cost_Centre_Code] [nvarchar](50) NULL,
	[SocioEconomicStatus_id] [uniqueidentifier] NULL,
	[Owner_Id] [uniqueidentifier] NULL,
	[Revenue_PIN] [nvarchar](50) NULL,
	[VATClass_Id] [uniqueidentifier] NULL,
	[Outlet_DiscountGroupId] [uniqueidentifier] NULL,
	[PaybillNumber] [nvarchar](50) NULL,
	[MerchantNumber] [nvarchar](50) NULL,
	[IM_Status] [int] NOT NULL,
	[CostCentreType2] [int] NOT NULL,
	[JoinDate] [datetime] NULL,
	[SpecialPricingTierId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_tblCostCentre] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomerDiscount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomerDiscount](
	[id] [uniqueidentifier] NOT NULL,
	[Outlet] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblCustomerDiscount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomerDiscountItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomerDiscountItem](
	[id] [uniqueidentifier] NOT NULL,
	[CustomerDiscountId] [uniqueidentifier] NOT NULL,
	[DiscountRate] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblCustomerDiscountItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblInventory]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblInventory](
	[id] [uniqueidentifier] NOT NULL,
	[WareHouseId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Balance] [decimal](20, 2) NULL,
	[Value] [money] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[UnavailableBalance] [decimal](18, 2) NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblInventory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblInventoryTransaction]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblInventoryTransaction](
	[Id] [uniqueidentifier] NOT NULL,
	[InventoryId] [uniqueidentifier] NOT NULL,
	[NoItems] [decimal](20, 2) NULL,
	[NetValue] [decimal](18, 2) NOT NULL,
	[GrossValue] [decimal](18, 2) NOT NULL,
	[DocumentType] [int] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DateInserted] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblInventoryTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblFreeOfChargeDiscount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblFreeOfChargeDiscount](
	[id] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[IsChecked] [bit] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblFreeOfChargeDiscount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblConsolidatedProductProducts]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblConsolidatedProductProducts](
	[ConsolidatedProductId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[QtyPerConsolidatedProduct] [int] NOT NULL,
 CONSTRAINT [PK_tblConsolidatedProductProducts] PRIMARY KEY CLUSTERED 
(
	[ConsolidatedProductId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblArea]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblArea](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[Description] [varchar](50) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Region] [uniqueidentifier] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCertainValueCertainProductDiscount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCertainValueCertainProductDiscount](
	[id] [uniqueidentifier] NOT NULL,
	[Value] [money] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblCertainValueCertainProductDiscount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCertainValueCertainProductDiscountItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCertainValueCertainProductDiscountItem](
	[id] [uniqueidentifier] NOT NULL,
	[CertainValueCertainDiscountID] [uniqueidentifier] NOT NULL,
	[Product] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Value] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblCertainValueCertainProductDiscountItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCentreType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCentreType](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblCentr__3214EC07595B4002] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBank]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBank](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](150) NULL,
	[Code] [varchar](50) NULL,
	[Description] [varchar](150) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblBank] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAuditLog]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAuditLog](
	[Id] [uniqueidentifier] NOT NULL,
	[OwnerId] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[Module] [varchar](100) NOT NULL,
	[Action] [varchar](max) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_tblAuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAssetType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAssetType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblAssetType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAssetStatus]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAssetStatus](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblAsset__3214EC073FE0292B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCompetitor]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCompetitor](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[PostaAddress] [nvarchar](250) NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[Telephone] [nvarchar](150) NULL,
	[ContactPerson] [nvarchar](250) NULL,
	[City] [nvarchar](250) NULL,
	[Longitude] [nvarchar](150) NULL,
	[Lattitude] [nvarchar](150) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblCompetitor] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommodityType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodityType](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommodityProducer]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodityProducer](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Acrage] [nvarchar](50) NOT NULL,
	[RegNo] [nvarchar](50) NOT NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[Description] [nvarchar](250) NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommodityOwnerType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodityOwnerType](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblContactType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContactType](
	[id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](250) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](350) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblContactType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblClientMasterDataTracker]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblClientMasterDataTracker](
	[CostCentreApplicationId] [uniqueidentifier] NOT NULL,
	[MasterDataId] [int] NOT NULL,
	[DateTimePushed] [datetime] NOT NULL,
	[DateTimePushConfirmed] [datetime] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblClientMasterDataTracker] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblFiles]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblFiles](
	[Id] [uniqueidentifier] NOT NULL,
	[FileData] [varbinary](max) NOT NULL,
	[FileExtension] [varchar](20) NOT NULL,
	[FileType] [int] NOT NULL,
	[FileDescription] [varchar](200) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblFiles__3214EC07501690F4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblExportImportAudit]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblExportImportAudit](
	[DocumentId] [uniqueidentifier] NOT NULL,
	[IntegrationModule] [int] NOT NULL,
	[DocumentAuditStatus] [int] NOT NULL,
	[DocumentReference] [nvarchar](50) NULL,
	[ExternalDocumentReference] [nvarchar](50) NULL,
	[DocumentType] [int] NULL,
	[DateUploaded] [datetime] NULL,
 CONSTRAINT [PK_tblExportImportAudit] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDocument]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDocument](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_IsActive] [bit] NOT NULL,
	[DocumentReference] [varchar](250) NULL,
	[DocumentIssuerCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentIssuerUserId] [uniqueidentifier] NOT NULL,
	[DocumentDateIssued] [datetime] NOT NULL,
	[DocumentStatusId] [int] NOT NULL,
	[DocumentRecipientCostCentre] [uniqueidentifier] NOT NULL,
	[OrderDateRequired] [date] NULL,
	[OrderIssuedOnBehalfOfCC] [uniqueidentifier] NULL,
	[DocumentIssuerCostCentreApplicationId] [uniqueidentifier] NULL,
	[IRNOrderReferences] [nvarchar](200) NULL,
	[IRNLoadNo] [nvarchar](50) NULL,
	[IRNGoodsReceivedFromCC] [uniqueidentifier] NULL,
	[InvoiceOrderId] [uniqueidentifier] NULL,
	[OrderOrderTypeId] [int] NULL,
	[Note] [text] NULL,
	[Longitude] [float] NULL,
	[Latitude] [float] NULL,
	[SaleDiscount] [money] NULL,
	[PaymentDocId] [uniqueidentifier] NULL,
	[DocumentParentId] [uniqueidentifier] NULL,
	[SendDateTime] [datetime] NULL,
	[OrderStatusId] [int] NULL,
	[OrderParentId] [uniqueidentifier] NULL,
	[ShipToAddress] [varchar](250) NULL,
	[ExtDocumentReference] [varchar](250) NULL,
 CONSTRAINT [PK_tblOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMasterDataAllocation]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMasterDataAllocation](
	[Id] [uniqueidentifier] NOT NULL,
	[EntityAId] [uniqueidentifier] NOT NULL,
	[EntityBId] [uniqueidentifier] NOT NULL,
	[AllocationType] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblMasterDataAllocation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblMarketAudit]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMarketAudit](
	[Id] [uniqueidentifier] NOT NULL,
	[Question] [varchar](max) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblMarketAudit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRecollection]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRecollection](
	[Id] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[FromCostCentreId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](50) NULL,
	[Amount] [decimal](16, 2) NOT NULL,
	[IsReceived] [bit] NOT NULL,
	[DateInserted] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRetireDocumentSetting]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRetireDocumentSetting](
	[Id] [uniqueidentifier] NOT NULL,
	[RetireTypeId] [int] NOT NULL,
	[Duration] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSettings]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSettings](
	[Id] [uniqueidentifier] NOT NULL,
	[Key] [int] NOT NULL,
	[Value] [nvarchar](250) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[AppId] [int] NULL,
 CONSTRAINT [PK_[tblSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblOutletVisitDay]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblOutletVisitDay](
	[Id] [uniqueidentifier] NOT NULL,
	[OutletId] [uniqueidentifier] NOT NULL,
	[VistDay] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblOutle__3214EC0755CF6A4A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblChannelPackaging]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblChannelPackaging](
	[id] [uniqueidentifier] NOT NULL,
	[PackagingId] [uniqueidentifier] NOT NULL,
	[OutletTypeId] [uniqueidentifier] NOT NULL,
	[IsChecked] [bit] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblChannelPackaging] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblOutletPriority]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblOutletPriority](
	[Id] [uniqueidentifier] NOT NULL,
	[OutletId] [uniqueidentifier] NOT NULL,
	[RouteId] [uniqueidentifier] NOT NULL,
	[OutletPriority] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblOutle__3214EC075B8843A0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblOutletAudit]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOutletAudit](
	[Id] [uniqueidentifier] NOT NULL,
	[Question] [varchar](max) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblOutletAudit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblUserGroup]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUserGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblUserG__3214EC0712A9974E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTargetPeriod]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTargetPeriod](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblTargetPeriod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblUserTypes]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblUserTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Description] [text] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblUserTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSourcingDocument]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSourcingDocument](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[DocumentStatusId] [int] NOT NULL,
	[DocumentParentId] [uniqueidentifier] NULL,
	[DocumentReference] [varchar](250) NULL,
	[DocumentIssuerCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentIssuerUserId] [uniqueidentifier] NOT NULL,
	[DocumentRecipientCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentOnBehalfOfCostCentreId] [uniqueidentifier] NULL,
	[DocumentIssuerCostCentreApplicationId] [uniqueidentifier] NULL,
	[DeliveredBy] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
	[Note] [nvarchar](250) NULL,
	[CommodityOwnerId] [uniqueidentifier] NOT NULL,
	[CommodityProducerId] [uniqueidentifier] NULL,
	[DateIssued] [datetime] NOT NULL,
	[DocumentDate] [datetime] NOT NULL,
	[DateSent] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[DriverName] [varchar](50) NULL,
	[VehicleRegNo] [varchar](50) NULL,
	[RouteId] [uniqueidentifier] NULL,
	[CentreId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[test]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[test](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[DocumentReference] [nvarchar](50) NULL,
	[DocumentIssuerCostCentreId] [uniqueidentifier] NOT NULL,
	[DocumentIssuerUserId] [uniqueidentifier] NOT NULL,
	[DocumentDateIssued] [date] NOT NULL,
	[DocumentStatusId] [int] NOT NULL,
	[DocumentRecipientCostCentre] [uniqueidentifier] NOT NULL,
	[OrderDateRequired] [date] NULL,
	[OrderIssuedOnBehalfOfCC] [uniqueidentifier] NULL,
	[DocumentIssuerCostCentreApplicationId] [uniqueidentifier] NULL,
	[IRNOrderReferences] [nvarchar](200) NULL,
	[IRNLoadNo] [nvarchar](50) NULL,
	[IRNGoodsReceivedFromCC] [uniqueidentifier] NULL,
	[InvoiceOrderId] [uniqueidentifier] NULL,
	[OrderOrderTypeId] [int] NULL,
	[IM_Status] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUserGroupRoles]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUserGroupRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [int] NOT NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[CanAccess] [bit] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblUserG__3214EC074905A7FF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblVATClassItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblVATClassItem](
	[id] [uniqueidentifier] NOT NULL,
	[VATClassID] [uniqueidentifier] NOT NULL,
	[Rate] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblVATClassItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSourcingLineItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSourcingLineItem](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[CommodityId] [uniqueidentifier] NULL,
	[GradeId] [uniqueidentifier] NULL,
	[ContainerId] [uniqueidentifier] NULL,
	[Weight] [decimal](18, 2) NULL,
	[TareWeight] [decimal](18, 2) NULL,
	[Description] [nvarchar](250) NULL,
	[Note] [nvarchar](250) NULL,
	[ContainerNo] [varchar](50) NULL,
	[LineItemStatusId] [int] NULL,
	[NoOfContainer] [decimal](16, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblOrderPaymentInfo]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblOrderPaymentInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ConfirmedAmount] [decimal](18, 2) NOT NULL,
	[PaymentMode] [int] NOT NULL,
	[MMoneyPaymentType] [varchar](50) NULL,
	[PaymentRefId] [varchar](50) NULL,
	[IsConfirmed] [bit] NULL,
	[NotificationId] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
	[TransactionDate] [datetime] NULL,
	[IsProcessed] [bit] NULL,
	[BankCode] [varchar](250) NULL,
	[BranchCode] [varchar](250) NULL,
	[ChequeDueDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblProductDiscountGroup]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductDiscountGroup](
	[id] [uniqueidentifier] NOT NULL,
	[DiscountGroup] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblProductDiscountGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProductDiscountGroupItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProductDiscountGroupItem](
	[id] [uniqueidentifier] NOT NULL,
	[ProductDiscountGroup] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[DiscountRate] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblProductDiscountGroupItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCompetitorProducts]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCompetitorProducts](
	[id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](350) NULL,
	[CompetitorId] [uniqueidentifier] NOT NULL,
	[BrandId] [uniqueidentifier] NOT NULL,
	[FlavourId] [uniqueidentifier] NOT NULL,
	[PackagingId] [uniqueidentifier] NOT NULL,
	[PackagingTypeId] [uniqueidentifier] NOT NULL,
	[ProductTypeId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblCompetitorProducts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSaleValueDiscount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSaleValueDiscount](
	[id] [uniqueidentifier] NOT NULL,
	[TierId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblSaleValueDiscount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRecollectionItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRecollectionItem](
	[Id] [uniqueidentifier] NOT NULL,
	[RecollectionId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](16, 2) NOT NULL,
	[DateInserted] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[IsComfirmed] [bit] NOT NULL,
	[CollectionModeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblLineItems]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLineItems](
	[id] [uniqueidentifier] NOT NULL,
	[DocumentID] [uniqueidentifier] NULL,
	[ProductID] [uniqueidentifier] NULL,
	[Description] [nvarchar](250) NULL,
	[LineItemSequenceNo] [int] NULL,
	[Quantity] [decimal](20, 2) NULL,
	[Value] [money] NULL,
	[Vat] [money] NULL,
	[OrderLineItemType] [int] NULL,
	[DiscountLineItemTypeId] [int] NULL,
	[IAN_Actual] [decimal](18, 2) NULL,
	[PaymentDocLineItemId] [uniqueidentifier] NULL,
	[Receipt_AccountType] [int] NULL,
	[Receipt_PaymentTypeId] [int] NULL,
	[Receipt_PaymentReference] [nvarchar](50) NULL,
	[Receipt_MMoneyPaymentType] [nvarchar](50) NULL,
	[ProductDiscount] [money] NULL,
	[ReturnsNoteReason] [nvarchar](250) NULL,
	[Other] [nvarchar](250) NULL,
	[NotificationId] [nvarchar](1000) NULL,
	[LineItemStatusId] [int] NULL,
	[ApprovedQuantity] [decimal](18, 2) NULL,
	[LostSaleQuantity] [decimal](18, 2) NULL,
	[BackOrderQuantity] [decimal](18, 2) NULL,
	[DispatchedQuantity] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblLineItems] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDiscounts]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDiscounts](
	[id] [uniqueidentifier] NOT NULL,
	[ProductRef] [uniqueidentifier] NOT NULL,
	[TierId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblDiscounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommodity]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodity](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[CommodityTypeId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBankBranch]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBankBranch](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](150) NULL,
	[Code] [varchar](50) NULL,
	[BankId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](150) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblBankBranch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblContact]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContact](
	[id] [uniqueidentifier] NOT NULL,
	[CostCenterId] [uniqueidentifier] NOT NULL,
	[Firstname] [nvarchar](250) NULL,
	[Lastname] [nvarchar](250) NULL,
	[DateOfBirth] [datetime] NULL,
	[ContactType] [uniqueidentifier] NULL,
	[ContactOwner] [int] NOT NULL,
	[SpouseName] [nvarchar](250) NULL,
	[Company] [nvarchar](250) NULL,
	[JobTitle] [nvarchar](250) NULL,
	[City] [nvarchar](250) NULL,
	[HomeTown] [nvarchar](250) NULL,
	[PhysicalAddress] [nvarchar](350) NULL,
	[PostalAddress] [nvarchar](350) NULL,
	[BusinessPhone] [nvarchar](350) NULL,
	[MobilePhone] [nvarchar](250) NULL,
	[HomePhone] [nvarchar](250) NULL,
	[WorkExtPhone] [nvarchar](250) NULL,
	[Fax] [nvarchar](250) NULL,
	[Email] [nvarchar](350) NULL,
	[ChildrenNames] [nvarchar](500) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[ContactClassification] [int] NULL,
	[IM_Status] [int] NOT NULL,
	[MaritalStatusId] [int] NULL,
 CONSTRAINT [PK_tblContact] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAssetCategory]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAssetCategory](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[AssetTypeId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblAsset__3214EC076ACA8730] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAsset]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAsset](
	[Id] [uniqueidentifier] NOT NULL,
	[AssetTypeId] [uniqueidentifier] NOT NULL,
	[Code] [varchar](50) NULL,
	[Capacity] [varchar](50) NULL,
	[AssetNo] [varchar](50) NULL,
	[SerialNo] [varchar](50) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[Name] [nvarchar](250) NULL,
	[AssetCategoryId] [uniqueidentifier] NULL,
	[AssetStatusId] [uniqueidentifier] NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblAsset] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCommodityGrade]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodityGrade](
	[Id] [uniqueidentifier] NOT NULL,
	[CommodityId] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[UsageTypeId] [int] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDiscountItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDiscountItem](
	[id] [uniqueidentifier] NOT NULL,
	[DiscountId] [uniqueidentifier] NOT NULL,
	[DiscountRate] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblDiscountItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProvince]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblProvince](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](150) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblProvince] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSaleValueDiscountItems]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSaleValueDiscountItems](
	[id] [uniqueidentifier] NOT NULL,
	[SaleValueId] [uniqueidentifier] NOT NULL,
	[DiscountRate] [money] NOT NULL,
	[SaleValue] [money] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_tblSaleValueDiscountItems] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDistrict]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDistrict](
	[Id] [uniqueidentifier] NOT NULL,
	[ProvinceId] [uniqueidentifier] NOT NULL,
	[District] [varchar](250) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblDistrict] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblContainerType]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblContainerType](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[LoadCariage] [decimal](18, 2) NULL,
	[TareWeight] [decimal](18, 2) NULL,
	[Lenght] [decimal](18, 2) NULL,
	[Width] [decimal](18, 2) NULL,
	[Height] [decimal](18, 2) NULL,
	[BubbleSpace] [decimal](18, 2) NULL,
	[Volume] [decimal](18, 2) NULL,
	[FreezerTemp] [decimal](18, 2) NULL,
	[Make] [nvarchar](50) NOT NULL,
	[Model] [nvarchar](50) NOT NULL,
	[CommodityGradeId] [uniqueidentifier] NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[ContainerUseId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblReOrderLevel]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblReOrderLevel](
	[id] [uniqueidentifier] NOT NULL,
	[DistributorId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductReOrderLevel] [money] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblReOrderLevel] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblInventorySerials]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblInventorySerials](
	[Id] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[From] [nvarchar](150) NOT NULL,
	[To] [nvarchar](150) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblInventorySerials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSalemanRoute]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSalemanRoute](
	[Id] [uniqueidentifier] NOT NULL,
	[RouteId] [uniqueidentifier] NOT NULL,
	[SalemanId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblSalem__3214EC0709202D14] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPurchasingClerkRoute]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPurchasingClerkRoute](
	[Id] [uniqueidentifier] NOT NULL,
	[RouteId] [uniqueidentifier] NOT NULL,
	[PurchasingClerkId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUsers]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[CostCenterId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[PIN] [nvarchar](250) NULL,
	[UserType] [int] NOT NULL,
	[Mobile] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NULL,
	[GroupId] [uniqueidentifier] NULL,
	[TillNumber] [nvarchar](250) NULL,
	[IM_Status] [int] NOT NULL,
	[Code] [varchar](250) NULL,
	[FirstName] [varchar](250) NULL,
	[LastName] [varchar](250) NULL,
 CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTarget]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTarget](
	[id] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[PeriodId] [uniqueidentifier] NOT NULL,
	[TargetValue] [decimal](20, 2) NOT NULL,
	[IsQuantityTarget] [bit] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblTarget] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTargetItem]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTargetItem](
	[Id] [uniqueidentifier] NOT NULL,
	[TargetId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK__tblTarge__3214EC072BD9307E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblShipToAddress]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblShipToAddress](
	[Id] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NULL,
	[Description] [nvarchar](250) NULL,
	[PostalAddress] [nvarchar](250) NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[Longitude] [decimal](18, 4) NULL,
	[Latitude] [decimal](18, 4) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[Code] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblPaymentTracker]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPaymentTracker](
	[id] [uniqueidentifier] NOT NULL,
	[CostCenterId] [uniqueidentifier] NOT NULL,
	[PaymentModeId] [int] NOT NULL,
	[Balance] [money] NULL,
	[PendingConfirmBalance] [money] NULL,
 CONSTRAINT [PK_tblPaymentTracker] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblEquipment]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblEquipment](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[EquipmentNumber] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Make] [nvarchar](50) NULL,
	[Model] [nvarchar](50) NULL,
	[EquipmentType] [int] NOT NULL,
	[Description] [nvarchar](250) NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[ContainerTypeId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCostCentreApplication]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCostCentreApplication](
	[id] [uniqueidentifier] NOT NULL,
	[costcentreid] [uniqueidentifier] NOT NULL,
	[description] [nvarchar](50) NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
 CONSTRAINT [PK_tblCostCentreApplication] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommodityOwner]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommodityOwner](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NULL,
	[Gender] [int] NOT NULL,
	[IdNo] [nvarchar](50) NOT NULL,
	[PINNo] [nvarchar](50) NOT NULL,
	[DOB] [datetime] NOT NULL,
	[MaritalStatus] [int] NULL,
	[MaritalStatusId] [uniqueidentifier] NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[PostalAddress] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[PhoneNo] [nvarchar](20) NULL,
	[BusinessNo] [nvarchar](20) NULL,
	[FaxNo] [nvarchar](20) NULL,
	[OfficeNo] [nvarchar](20) NULL,
	[Description] [nvarchar](250) NULL,
	[CommodityOwnerTypeId] [uniqueidentifier] NOT NULL,
	[CostCentreId] [uniqueidentifier] NOT NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCentre]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCentre](
	[Id] [uniqueidentifier] NOT NULL,
	[CentreTypeId] [uniqueidentifier] NOT NULL,
	[HubId] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] [int] NOT NULL,
	[RouteId] [uniqueidentifier] NULL,
 CONSTRAINT [PK__tblCentr__3214EC075D2BD0E6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAccount]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAccount](
	[id] [uniqueidentifier] NOT NULL,
	[CostCenterId] [uniqueidentifier] NOT NULL,
	[AccountType] [int] NOT NULL,
	[Balance] [money] NULL,
 CONSTRAINT [PK_tblAccount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAccountTransaction]    Script Date: 10/30/2013 09:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAccountTransaction](
	[Id] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DocumentType] [int] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DateInserted] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF__tblArea__IM_Stat__6497E884]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblArea] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblAsset__IM_Sta__658C0CBD]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAsset] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblAssetC__IM_Da__0672A1A5]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetCategory] ADD  CONSTRAINT [DF__tblAssetC__IM_Da__0672A1A5]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblAssetC__IM_Da__0766C5DE]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetCategory] ADD  CONSTRAINT [DF__tblAssetC__IM_Da__0766C5DE]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblAssetC__IM_St__68687968]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetCategory] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblAssetS__IM_Da__085AEA17]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetStatus] ADD  CONSTRAINT [DF__tblAssetS__IM_Da__085AEA17]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblAssetS__IM_Da__094F0E50]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetStatus] ADD  CONSTRAINT [DF__tblAssetS__IM_Da__094F0E50]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblAssetS__IM_St__6B44E613]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetStatus] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblAssetT__IM_St__6C390A4C]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetType] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblBank__IM_Stat__6D2D2E85]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblBank] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblBankBr__IM_St__6E2152BE]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblBankBranch] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCertai__IM_St__6F1576F7]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscount] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCertai__IM_St__70099B30]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscountItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblChanne__IM_St__70FDBF69]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblChannelPackaging] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblClient__IM_St__71F1E3A2]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblClientMasterDataTracker] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCompet__IM_St__72E607DB]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitor] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCompet__IM_St__73DA2C14]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblContac__IM_St__74CE504D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContact] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblContac__Marit__75C27486]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContact] ADD  DEFAULT ((0)) FOR [MaritalStatusId]
GO
/****** Object:  Default [DF__tblContac__IM_St__76B698BF]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContactType] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblContai__IM_St__77AABCF8]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContainment] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCostCe__IM_St__5649C92D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre] ADD  CONSTRAINT [DF__tblCostCe__IM_St__5649C92D]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCostCe__CostC__573DED66]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre] ADD  CONSTRAINT [DF__tblCostCe__CostC__573DED66]  DEFAULT ((0)) FOR [CostCentreType2]
GO
/****** Object:  Default [DF__tblCostCe__IM_St__7A8729A3]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentreApplication] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCountr__IM_St__592635D8]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCountry] ADD  CONSTRAINT [DF__tblCountr__IM_St__592635D8]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCustom__IM_St__7C6F7215]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCustomerDiscount] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblCustom__IM_St__7D63964E]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCustomerDiscountItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblDiscou__IM_St__5C02A283]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDiscountGroup] ADD  CONSTRAINT [DF__tblDiscou__IM_St__5C02A283]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblDiscou__IM_St__7F4BDEC0]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDiscountItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblDiscou__IM_St__004002F9]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDiscounts] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblDistri__IM_St__01342732]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDistrict] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblDocume__Order__02284B6B]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDocument] ADD  DEFAULT ((0)) FOR [OrderStatusId]
GO
/****** Object:  Default [DF__tblFiles__IM_Dat__0B3756C2]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblFiles] ADD  CONSTRAINT [DF__tblFiles__IM_Dat__0B3756C2]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblFiles__IM_Dat__0C2B7AFB]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblFiles] ADD  CONSTRAINT [DF__tblFiles__IM_Dat__0C2B7AFB]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblFiles__IM_Sta__0504B816]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblFiles] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblFreeOf__IM_St__05F8DC4F]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblFreeOfChargeDiscount] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblInvent__Unava__02C769E9]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventory] ADD  CONSTRAINT [DF__tblInvent__Unava__02C769E9]  DEFAULT ((0)) FOR [UnavailableBalance]
GO
/****** Object:  Default [DF__tblInvent__IM_St__07E124C1]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventory] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblInvent__IM_St__08D548FA]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventoryTransaction] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblLineIt__LineI__09C96D33]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems] ADD  DEFAULT ((0)) FOR [LineItemStatusId]
GO
/****** Object:  Default [DF__tblLineIt__Appro__0ABD916C]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems] ADD  DEFAULT ((0)) FOR [ApprovedQuantity]
GO
/****** Object:  Default [DF__tblLineIt__LostS__0BB1B5A5]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems] ADD  DEFAULT ((0)) FOR [LostSaleQuantity]
GO
/****** Object:  Default [DF__tblLineIt__BackO__0CA5D9DE]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems] ADD  DEFAULT ((0)) FOR [BackOrderQuantity]
GO
/****** Object:  Default [DF__tblLineIt__Dispa__0D99FE17]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems] ADD  DEFAULT ((0)) FOR [DispatchedQuantity]
GO
/****** Object:  Default [DF__tblMarket__IM_St__0E8E2250]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblMarketAudit] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblOutlet__IM_St__0F824689]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletAudit] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblOutlet__IM_St__6F1576F7]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletCategory] ADD  CONSTRAINT [DF__tblOutlet__IM_St__6F1576F7]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblOutlet__IM_Da__0D1F9F34]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletPriority] ADD  CONSTRAINT [DF__tblOutlet__IM_Da__0D1F9F34]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblOutlet__IM_Da__0E13C36D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletPriority] ADD  CONSTRAINT [DF__tblOutlet__IM_Da__0E13C36D]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblOutlet__IM_St__1352D76D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletPriority] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblOutlet__IM_Da__0F07E7A6]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletVisitDay] ADD  CONSTRAINT [DF__tblOutlet__IM_Da__0F07E7A6]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblOutlet__IM_Da__0FFC0BDF]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletVisitDay] ADD  CONSTRAINT [DF__tblOutlet__IM_Da__0FFC0BDF]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblOutlet__IM_St__162F4418]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOutletVisitDay] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblPaymen__Balan__05A3D694]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPaymentTracker] ADD  CONSTRAINT [DF__tblPaymen__Balan__05A3D694]  DEFAULT ((0)) FOR [Balance]
GO
/****** Object:  Default [DF__tblPaymen__Pendi__0697FACD]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPaymentTracker] ADD  CONSTRAINT [DF__tblPaymen__Pendi__0697FACD]  DEFAULT ((0)) FOR [PendingConfirmBalance]
GO
/****** Object:  Default [DF__tblPricin__IM_St__190BB0C3]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricing] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblPricin__IM_St__19FFD4FC]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricingItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblPricin__IM_St__1AF3F935]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricingTier] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__ExFac__14C0C0FC]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct] ADD  CONSTRAINT [DF__tblProduc__ExFac__14C0C0FC]  DEFAULT ((0)) FOR [ExFactoryPrice]
GO
/****** Object:  Default [DF__tblProduc__IM_St__1CDC41A7]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__1DD065E0]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductBrand] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__1EC48A19]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductDiscountGroup] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__1FB8AE52]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductDiscountGroupItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__20ACD28B]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductFlavour] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__21A0F6C4]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductPackaging] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__22951AFD]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductPackagingType] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProduc__IM_St__23893F36]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductType] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblPromot__IM_St__247D636F]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPromotionDiscount] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblPromot__IM_St__257187A8]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPromotionDiscountItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblProvin__IM_St__2665ABE1]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProvince] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblRecoll__IsCom__2759D01A]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRecollectionItem] ADD  DEFAULT ((0)) FOR [IsComfirmed]
GO
/****** Object:  Default [DF__tblRecoll__Colle__1E8F7FEF]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRecollectionItem] ADD  DEFAULT ((0)) FOR [CollectionModeId]
GO
/****** Object:  Default [DF__tblRegion__IM_Da__3F466844]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRegion] ADD  CONSTRAINT [DF__tblRegion__IM_Da__3F466844]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblRegion__IM_Da__403A8C7D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRegion] ADD  CONSTRAINT [DF__tblRegion__IM_Da__403A8C7D]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblRegion__IM_St__08D548FA]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRegion] ADD  CONSTRAINT [DF__tblRegion__IM_St__08D548FA]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblReOrde__IM_St__2B2A60FE]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblReOrderLevel] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblRetire__IM_St__2C1E8537]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRetireDocumentSetting] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblRoutes__IM_Da__4222D4EF]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRoutes] ADD  CONSTRAINT [DF__tblRoutes__IM_Da__4222D4EF]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblRoutes__IM_Da__4316F928]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRoutes] ADD  CONSTRAINT [DF__tblRoutes__IM_Da__4316F928]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblRoutes__IM_St__0D99FE17]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRoutes] ADD  CONSTRAINT [DF__tblRoutes__IM_St__0D99FE17]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSalema__IM_St__2FEF161B]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSalemanRoute] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSaleVa__IM_St__30E33A54]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSaleValueDiscount] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSaleVa__IM_St__31D75E8D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSaleValueDiscountItems] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSettin__IM_St__32CB82C6]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSettings] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSocioE__IM_St__125EB334]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSocioEconomicStatus] ADD  CONSTRAINT [DF__tblSocioE__IM_St__125EB334]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblSuppli__IM_St__34B3CB38]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSupplier] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblTarget__IM_St__35A7EF71]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTarget] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblTarget__IM_Da__29BBDDE2]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetItem] ADD  CONSTRAINT [DF__tblTarget__IM_Da__29BBDDE2]  DEFAULT (getdate()) FOR [IM_DateCreated]
GO
/****** Object:  Default [DF__tblTarget__IM_Da__2AB0021B]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetItem] ADD  CONSTRAINT [DF__tblTarget__IM_Da__2AB0021B]  DEFAULT (getdate()) FOR [IM_DateLastUpdated]
GO
/****** Object:  Default [DF__tblTarget__IM_St__38845C1C]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblTarget__IM_St__39788055]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetPeriod] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblTerrit__IM_St__190BB0C3]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTerritory] ADD  CONSTRAINT [DF__tblTerrit__IM_St__190BB0C3]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblUserGr__IM_St__3B60C8C7]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUserGroup] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblUserGr__CanAc__6E372CAE]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUserGroupRoles] ADD  CONSTRAINT [DF__tblUserGr__CanAc__6E372CAE]  DEFAULT ((0)) FOR [CanAccess]
GO
/****** Object:  Default [DF__tblUserGr__IM_St__3D491139]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUserGroupRoles] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblUsers__IM_Sta__3E3D3572]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUsers] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblUserTy__IM_St__3F3159AB]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUserTypes] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblVATCla__IM_St__1EC48A19]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblVATClass] ADD  CONSTRAINT [DF__tblVATCla__IM_St__1EC48A19]  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__tblVATCla__IM_St__4119A21D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblVATClassItem] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  Default [DF__test__IM_Status__420DC656]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[test] ADD  DEFAULT ((1)) FOR [IM_Status]
GO
/****** Object:  ForeignKey [FK_tblAccount_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAccount]  WITH CHECK ADD  CONSTRAINT [FK_tblAccount_tblCostCentre] FOREIGN KEY([CostCenterId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblAccount] CHECK CONSTRAINT [FK_tblAccount_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_AccountTransaction_AccountTransaction]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAccountTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AccountTransaction_AccountTransaction] FOREIGN KEY([AccountId])
REFERENCES [dbo].[tblAccount] ([id])
GO
ALTER TABLE [dbo].[tblAccountTransaction] CHECK CONSTRAINT [FK_AccountTransaction_AccountTransaction]
GO
/****** Object:  ForeignKey [FK__tblAsset__AssetC__2E8092FF]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAsset]  WITH CHECK ADD  CONSTRAINT [FK__tblAsset__AssetC__2E8092FF] FOREIGN KEY([AssetCategoryId])
REFERENCES [dbo].[tblAssetCategory] ([Id])
GO
ALTER TABLE [dbo].[tblAsset] CHECK CONSTRAINT [FK__tblAsset__AssetC__2E8092FF]
GO
/****** Object:  ForeignKey [FK__tblAsset__AssetS__2F74B738]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAsset]  WITH CHECK ADD  CONSTRAINT [FK__tblAsset__AssetS__2F74B738] FOREIGN KEY([AssetStatusId])
REFERENCES [dbo].[tblAssetStatus] ([Id])
GO
ALTER TABLE [dbo].[tblAsset] CHECK CONSTRAINT [FK__tblAsset__AssetS__2F74B738]
GO
/****** Object:  ForeignKey [FK__tblAsset__AssetT__3068DB71]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAsset]  WITH CHECK ADD  CONSTRAINT [FK__tblAsset__AssetT__3068DB71] FOREIGN KEY([AssetTypeId])
REFERENCES [dbo].[tblAssetType] ([Id])
GO
ALTER TABLE [dbo].[tblAsset] CHECK CONSTRAINT [FK__tblAsset__AssetT__3068DB71]
GO
/****** Object:  ForeignKey [FK_tblAsset_tblAssetType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAsset]  WITH CHECK ADD  CONSTRAINT [FK_tblAsset_tblAssetType] FOREIGN KEY([AssetTypeId])
REFERENCES [dbo].[tblAssetType] ([Id])
GO
ALTER TABLE [dbo].[tblAsset] CHECK CONSTRAINT [FK_tblAsset_tblAssetType]
GO
/****** Object:  ForeignKey [FK__tblAssetC__Asset__325123E3]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblAssetCategory]  WITH CHECK ADD  CONSTRAINT [FK__tblAssetC__Asset__325123E3] FOREIGN KEY([AssetTypeId])
REFERENCES [dbo].[tblAssetType] ([Id])
GO
ALTER TABLE [dbo].[tblAssetCategory] CHECK CONSTRAINT [FK__tblAssetC__Asset__325123E3]
GO
/****** Object:  ForeignKey [FK_tblBankBranch_tblBank]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblBankBranch]  WITH CHECK ADD  CONSTRAINT [FK_tblBankBranch_tblBank] FOREIGN KEY([BankId])
REFERENCES [dbo].[tblBank] ([Id])
GO
ALTER TABLE [dbo].[tblBankBranch] CHECK CONSTRAINT [FK_tblBankBranch_tblBank]
GO
/****** Object:  ForeignKey [Centre_CentreType_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCentre]  WITH CHECK ADD  CONSTRAINT [Centre_CentreType_FK] FOREIGN KEY([CentreTypeId])
REFERENCES [dbo].[tblCentreType] ([Id])
GO
ALTER TABLE [dbo].[tblCentre] CHECK CONSTRAINT [Centre_CentreType_FK]
GO
/****** Object:  ForeignKey [Centre_Hub_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCentre]  WITH CHECK ADD  CONSTRAINT [Centre_Hub_FK] FOREIGN KEY([HubId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblCentre] CHECK CONSTRAINT [Centre_Hub_FK]
GO
/****** Object:  ForeignKey [Centre_Route_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCentre]  WITH CHECK ADD  CONSTRAINT [Centre_Route_FK] FOREIGN KEY([RouteId])
REFERENCES [dbo].[tblRoutes] ([RouteID])
GO
ALTER TABLE [dbo].[tblCentre] CHECK CONSTRAINT [Centre_Route_FK]
GO
/****** Object:  ForeignKey [FK_tblCertainValueCertainProductDiscountItem_tblCertainValueCertainProductDiscount]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscountItem]  WITH CHECK ADD  CONSTRAINT [FK_tblCertainValueCertainProductDiscountItem_tblCertainValueCertainProductDiscount] FOREIGN KEY([CertainValueCertainDiscountID])
REFERENCES [dbo].[tblCertainValueCertainProductDiscount] ([id])
GO
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscountItem] CHECK CONSTRAINT [FK_tblCertainValueCertainProductDiscountItem_tblCertainValueCertainProductDiscount]
GO
/****** Object:  ForeignKey [FK_tblCertainValueCertainProductDiscountItem_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscountItem]  WITH CHECK ADD  CONSTRAINT [FK_tblCertainValueCertainProductDiscountItem_tblProduct] FOREIGN KEY([Product])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblCertainValueCertainProductDiscountItem] CHECK CONSTRAINT [FK_tblCertainValueCertainProductDiscountItem_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblChannelPackaging_tblOutletType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblChannelPackaging]  WITH CHECK ADD  CONSTRAINT [FK_tblChannelPackaging_tblOutletType] FOREIGN KEY([OutletTypeId])
REFERENCES [dbo].[tblOutletType] ([id])
GO
ALTER TABLE [dbo].[tblChannelPackaging] CHECK CONSTRAINT [FK_tblChannelPackaging_tblOutletType]
GO
/****** Object:  ForeignKey [FK_tblChannelPackaging_tblProductPackaging]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblChannelPackaging]  WITH CHECK ADD  CONSTRAINT [FK_tblChannelPackaging_tblProductPackaging] FOREIGN KEY([PackagingId])
REFERENCES [dbo].[tblProductPackaging] ([Id])
GO
ALTER TABLE [dbo].[tblChannelPackaging] CHECK CONSTRAINT [FK_tblChannelPackaging_tblProductPackaging]
GO
/****** Object:  ForeignKey [Commodity_CommodityType_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCommodity]  WITH CHECK ADD  CONSTRAINT [Commodity_CommodityType_FK] FOREIGN KEY([CommodityTypeId])
REFERENCES [dbo].[tblCommodityType] ([Id])
GO
ALTER TABLE [dbo].[tblCommodity] CHECK CONSTRAINT [Commodity_CommodityType_FK]
GO
/****** Object:  ForeignKey [CommodityGrade_Commodity_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCommodityGrade]  WITH CHECK ADD  CONSTRAINT [CommodityGrade_Commodity_FK] FOREIGN KEY([CommodityId])
REFERENCES [dbo].[tblCommodity] ([Id])
GO
ALTER TABLE [dbo].[tblCommodityGrade] CHECK CONSTRAINT [CommodityGrade_Commodity_FK]
GO
/****** Object:  ForeignKey [CommodityOwner_CommodityOwnerSupplier_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCommodityOwner]  WITH CHECK ADD  CONSTRAINT [CommodityOwner_CommodityOwnerSupplier_FK] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblCommodityOwner] CHECK CONSTRAINT [CommodityOwner_CommodityOwnerSupplier_FK]
GO
/****** Object:  ForeignKey [CommodityOwner_CommodityOwnerType_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCommodityOwner]  WITH CHECK ADD  CONSTRAINT [CommodityOwner_CommodityOwnerType_FK] FOREIGN KEY([CommodityOwnerTypeId])
REFERENCES [dbo].[tblCommodityOwnerType] ([Id])
GO
ALTER TABLE [dbo].[tblCommodityOwner] CHECK CONSTRAINT [CommodityOwner_CommodityOwnerType_FK]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblCompetitor]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblCompetitor] FOREIGN KEY([CompetitorId])
REFERENCES [dbo].[tblCompetitor] ([id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblCompetitor]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblProductBrand]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblProductBrand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[tblProductBrand] ([id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblProductBrand]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblProductFlavour]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblProductFlavour] FOREIGN KEY([FlavourId])
REFERENCES [dbo].[tblProductFlavour] ([id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblProductFlavour]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblProductPackaging]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblProductPackaging] FOREIGN KEY([PackagingId])
REFERENCES [dbo].[tblProductPackaging] ([Id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblProductPackaging]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblProductPackagingType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblProductPackagingType] FOREIGN KEY([PackagingTypeId])
REFERENCES [dbo].[tblProductPackagingType] ([id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblProductPackagingType]
GO
/****** Object:  ForeignKey [FK_tblCompetitorProducts_tblProductType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCompetitorProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblCompetitorProducts_tblProductType] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[tblProductType] ([id])
GO
ALTER TABLE [dbo].[tblCompetitorProducts] CHECK CONSTRAINT [FK_tblCompetitorProducts_tblProductType]
GO
/****** Object:  ForeignKey [FK_tblConsolidatedProductProducts_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblConsolidatedProductProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblConsolidatedProductProducts_tblProduct] FOREIGN KEY([ConsolidatedProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblConsolidatedProductProducts] CHECK CONSTRAINT [FK_tblConsolidatedProductProducts_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblConsolidatedProductProducts_tblProduct1]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblConsolidatedProductProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblConsolidatedProductProducts_tblProduct1] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblConsolidatedProductProducts] CHECK CONSTRAINT [FK_tblConsolidatedProductProducts_tblProduct1]
GO
/****** Object:  ForeignKey [FK_tblContact_tblContactType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContact]  WITH CHECK ADD  CONSTRAINT [FK_tblContact_tblContactType] FOREIGN KEY([ContactType])
REFERENCES [dbo].[tblContactType] ([id])
GO
ALTER TABLE [dbo].[tblContact] CHECK CONSTRAINT [FK_tblContact_tblContactType]
GO
/****** Object:  ForeignKey [ContainerType_CommodityGrade_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContainerType]  WITH CHECK ADD  CONSTRAINT [ContainerType_CommodityGrade_FK] FOREIGN KEY([CommodityGradeId])
REFERENCES [dbo].[tblCommodityGrade] ([Id])
GO
ALTER TABLE [dbo].[tblContainerType] CHECK CONSTRAINT [ContainerType_CommodityGrade_FK]
GO
/****** Object:  ForeignKey [FK_tblContainment_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContainment]  WITH CHECK ADD  CONSTRAINT [FK_tblContainment_tblProduct] FOREIGN KEY([ReturnableProduct])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblContainment] CHECK CONSTRAINT [FK_tblContainment_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblContainment_tblProductPackagingType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblContainment]  WITH CHECK ADD  CONSTRAINT [FK_tblContainment_tblProductPackagingType] FOREIGN KEY([ProductPackagingType])
REFERENCES [dbo].[tblProductPackagingType] ([id])
GO
ALTER TABLE [dbo].[tblContainment] CHECK CONSTRAINT [FK_tblContainment_tblProductPackagingType]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblCostCentre] FOREIGN KEY([Id])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblDiscountGroup]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblDiscountGroup] FOREIGN KEY([Outlet_DiscountGroupId])
REFERENCES [dbo].[tblDiscountGroup] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblDiscountGroup]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblOutletCategory]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblOutletCategory] FOREIGN KEY([Outlet_Category_Id])
REFERENCES [dbo].[tblOutletCategory] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblOutletCategory]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblOutletType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblOutletType] FOREIGN KEY([Outlet_Type_Id])
REFERENCES [dbo].[tblOutletType] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblOutletType]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblRegion]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblRegion] FOREIGN KEY([Distributor_RegionId])
REFERENCES [dbo].[tblRegion] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblRegion]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblRoutes]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblRoutes] FOREIGN KEY([RouteId])
REFERENCES [dbo].[tblRoutes] ([RouteID])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblRoutes]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblSocioEconomicStatus]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblSocioEconomicStatus] FOREIGN KEY([SocioEconomicStatus_id])
REFERENCES [dbo].[tblSocioEconomicStatus] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblSocioEconomicStatus]
GO
/****** Object:  ForeignKey [FK_tblCostCentre_tblVATClass]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentre]  WITH NOCHECK ADD  CONSTRAINT [FK_tblCostCentre_tblVATClass] FOREIGN KEY([VATClass_Id])
REFERENCES [dbo].[tblVATClass] ([id])
GO
ALTER TABLE [dbo].[tblCostCentre] CHECK CONSTRAINT [FK_tblCostCentre_tblVATClass]
GO
/****** Object:  ForeignKey [FK_tblCostCentreApplication_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCostCentreApplication]  WITH CHECK ADD  CONSTRAINT [FK_tblCostCentreApplication_tblCostCentre] FOREIGN KEY([costcentreid])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblCostCentreApplication] CHECK CONSTRAINT [FK_tblCostCentreApplication_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblCountry_tblTerritory]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCountry]  WITH CHECK ADD  CONSTRAINT [FK_tblCountry_tblTerritory] FOREIGN KEY([Territory_ID])
REFERENCES [dbo].[tblTerritory] ([id])
GO
ALTER TABLE [dbo].[tblCountry] CHECK CONSTRAINT [FK_tblCountry_tblTerritory]
GO
/****** Object:  ForeignKey [FK_tblCustomerDiscount_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCustomerDiscount]  WITH CHECK ADD  CONSTRAINT [FK_tblCustomerDiscount_tblCostCentre] FOREIGN KEY([Outlet])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblCustomerDiscount] CHECK CONSTRAINT [FK_tblCustomerDiscount_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblCustomerDiscount_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCustomerDiscount]  WITH CHECK ADD  CONSTRAINT [FK_tblCustomerDiscount_tblProduct] FOREIGN KEY([ProductRef])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblCustomerDiscount] CHECK CONSTRAINT [FK_tblCustomerDiscount_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblCustomerDiscountItem_tblCustomerDiscount]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblCustomerDiscountItem]  WITH CHECK ADD  CONSTRAINT [FK_tblCustomerDiscountItem_tblCustomerDiscount] FOREIGN KEY([CustomerDiscountId])
REFERENCES [dbo].[tblCustomerDiscount] ([id])
GO
ALTER TABLE [dbo].[tblCustomerDiscountItem] CHECK CONSTRAINT [FK_tblCustomerDiscountItem_tblCustomerDiscount]
GO
/****** Object:  ForeignKey [FK_tblDiscountItem_tblDiscounts]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDiscountItem]  WITH CHECK ADD  CONSTRAINT [FK_tblDiscountItem_tblDiscounts] FOREIGN KEY([DiscountId])
REFERENCES [dbo].[tblDiscounts] ([id])
GO
ALTER TABLE [dbo].[tblDiscountItem] CHECK CONSTRAINT [FK_tblDiscountItem_tblDiscounts]
GO
/****** Object:  ForeignKey [FK_tblDiscounts_tblPricingTier]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDiscounts]  WITH CHECK ADD  CONSTRAINT [FK_tblDiscounts_tblPricingTier] FOREIGN KEY([TierId])
REFERENCES [dbo].[tblPricingTier] ([id])
GO
ALTER TABLE [dbo].[tblDiscounts] CHECK CONSTRAINT [FK_tblDiscounts_tblPricingTier]
GO
/****** Object:  ForeignKey [FK_tblDistrict_tblProvince]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblDistrict]  WITH CHECK ADD  CONSTRAINT [FK_tblDistrict_tblProvince] FOREIGN KEY([ProvinceId])
REFERENCES [dbo].[tblProvince] ([Id])
GO
ALTER TABLE [dbo].[tblDistrict] CHECK CONSTRAINT [FK_tblDistrict_tblProvince]
GO
/****** Object:  ForeignKey [Equipment_ContainerType_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblEquipment]  WITH CHECK ADD  CONSTRAINT [Equipment_ContainerType_FK] FOREIGN KEY([ContainerTypeId])
REFERENCES [dbo].[tblContainerType] ([Id])
GO
ALTER TABLE [dbo].[tblEquipment] CHECK CONSTRAINT [Equipment_ContainerType_FK]
GO
/****** Object:  ForeignKey [Equipment_CostCentre_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblEquipment]  WITH CHECK ADD  CONSTRAINT [Equipment_CostCentre_FK] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblEquipment] CHECK CONSTRAINT [Equipment_CostCentre_FK]
GO
/****** Object:  ForeignKey [FK_tblFreeOfChargeDiscount_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblFreeOfChargeDiscount]  WITH CHECK ADD  CONSTRAINT [FK_tblFreeOfChargeDiscount_tblProduct] FOREIGN KEY([ProductRef])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblFreeOfChargeDiscount] CHECK CONSTRAINT [FK_tblFreeOfChargeDiscount_tblProduct]
GO
/****** Object:  ForeignKey [fk_costcentre_pid]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventory]  WITH CHECK ADD  CONSTRAINT [fk_costcentre_pid] FOREIGN KEY([WareHouseId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblInventory] CHECK CONSTRAINT [fk_costcentre_pid]
GO
/****** Object:  ForeignKey [FK_tblInventory_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventory]  WITH CHECK ADD  CONSTRAINT [FK_tblInventory_tblProduct] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblInventory] CHECK CONSTRAINT [FK_tblInventory_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblInventorySerials_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventorySerials]  WITH CHECK ADD  CONSTRAINT [FK_tblInventorySerials_tblCostCentre] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblInventorySerials] CHECK CONSTRAINT [FK_tblInventorySerials_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblInventorySerials_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventorySerials]  WITH CHECK ADD  CONSTRAINT [FK_tblInventorySerials_tblProduct] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblInventorySerials] CHECK CONSTRAINT [FK_tblInventorySerials_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblInventoryTransaction_tblInventory]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblInventoryTransaction]  WITH CHECK ADD  CONSTRAINT [FK_tblInventoryTransaction_tblInventory] FOREIGN KEY([InventoryId])
REFERENCES [dbo].[tblInventory] ([id])
GO
ALTER TABLE [dbo].[tblInventoryTransaction] CHECK CONSTRAINT [FK_tblInventoryTransaction_tblInventory]
GO
/****** Object:  ForeignKey [FK_tblLineItems_tblDocument]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblLineItems]  WITH CHECK ADD  CONSTRAINT [FK_tblLineItems_tblDocument] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[tblDocument] ([Id])
GO
ALTER TABLE [dbo].[tblLineItems] CHECK CONSTRAINT [FK_tblLineItems_tblDocument]
GO
/****** Object:  ForeignKey [OrderPayment_Order_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblOrderPaymentInfo]  WITH CHECK ADD  CONSTRAINT [OrderPayment_Order_FK] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[tblDocument] ([Id])
GO
ALTER TABLE [dbo].[tblOrderPaymentInfo] CHECK CONSTRAINT [OrderPayment_Order_FK]
GO
/****** Object:  ForeignKey [FK_tblPaymentTracker_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPaymentTracker]  WITH CHECK ADD  CONSTRAINT [FK_tblPaymentTracker_tblCostCentre] FOREIGN KEY([CostCenterId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblPaymentTracker] CHECK CONSTRAINT [FK_tblPaymentTracker_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblPricing_tblPricingTier]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricing]  WITH CHECK ADD  CONSTRAINT [FK_tblPricing_tblPricingTier] FOREIGN KEY([Tier])
REFERENCES [dbo].[tblPricingTier] ([id])
GO
ALTER TABLE [dbo].[tblPricing] CHECK CONSTRAINT [FK_tblPricing_tblPricingTier]
GO
/****** Object:  ForeignKey [FK_tblPricing_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricing]  WITH CHECK ADD  CONSTRAINT [FK_tblPricing_tblProduct] FOREIGN KEY([ProductRef])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblPricing] CHECK CONSTRAINT [FK_tblPricing_tblProduct]
GO
/****** Object:  ForeignKey [FK__tblPricin__Prici__19218AB3]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricingItem]  WITH CHECK ADD  CONSTRAINT [FK__tblPricin__Prici__19218AB3] FOREIGN KEY([PricingId])
REFERENCES [dbo].[tblPricing] ([id])
GO
ALTER TABLE [dbo].[tblPricingItem] CHECK CONSTRAINT [FK__tblPricin__Prici__19218AB3]
GO
/****** Object:  ForeignKey [FK__tblPricin__Prici__1A15AEEC]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPricingItem]  WITH CHECK ADD  CONSTRAINT [FK__tblPricin__Prici__1A15AEEC] FOREIGN KEY([PricingId])
REFERENCES [dbo].[tblPricing] ([id])
GO
ALTER TABLE [dbo].[tblPricingItem] CHECK CONSTRAINT [FK__tblPricin__Prici__1A15AEEC]
GO
/****** Object:  ForeignKey [FK_Product_ProductBrand]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductBrand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[tblProductBrand] ([id])
GO
ALTER TABLE [dbo].[tblProduct] CHECK CONSTRAINT [FK_Product_ProductBrand]
GO
/****** Object:  ForeignKey [FK_Product_ProductFlavour]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductFlavour] FOREIGN KEY([FlavourId])
REFERENCES [dbo].[tblProductFlavour] ([id])
GO
ALTER TABLE [dbo].[tblProduct] CHECK CONSTRAINT [FK_Product_ProductFlavour]
GO
/****** Object:  ForeignKey [FK_Product_ProductPackaging]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductPackaging] FOREIGN KEY([PackagingId])
REFERENCES [dbo].[tblProductPackaging] ([Id])
GO
ALTER TABLE [dbo].[tblProduct] CHECK CONSTRAINT [FK_Product_ProductPackaging]
GO
/****** Object:  ForeignKey [FK_Product_ProductPackagingType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductPackagingType] FOREIGN KEY([PackagingTypeId])
REFERENCES [dbo].[tblProductPackagingType] ([id])
GO
ALTER TABLE [dbo].[tblProduct] CHECK CONSTRAINT [FK_Product_ProductPackagingType]
GO
/****** Object:  ForeignKey [FK_Product_ProductType]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProduct]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductType] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[tblProductType] ([id])
GO
ALTER TABLE [dbo].[tblProduct] CHECK CONSTRAINT [FK_Product_ProductType]
GO
/****** Object:  ForeignKey [FK_tblProductBrand_tblSupplier]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductBrand]  WITH CHECK ADD  CONSTRAINT [FK_tblProductBrand_tblSupplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[tblSupplier] ([id])
GO
ALTER TABLE [dbo].[tblProductBrand] CHECK CONSTRAINT [FK_tblProductBrand_tblSupplier]
GO
/****** Object:  ForeignKey [FK_tblProductDiscountGroup_tblDiscountGroup]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductDiscountGroup]  WITH CHECK ADD  CONSTRAINT [FK_tblProductDiscountGroup_tblDiscountGroup] FOREIGN KEY([DiscountGroup])
REFERENCES [dbo].[tblDiscountGroup] ([id])
GO
ALTER TABLE [dbo].[tblProductDiscountGroup] CHECK CONSTRAINT [FK_tblProductDiscountGroup_tblDiscountGroup]
GO
/****** Object:  ForeignKey [FK_tblProductDiscountGroupItem_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductDiscountGroupItem]  WITH CHECK ADD  CONSTRAINT [FK_tblProductDiscountGroupItem_tblProduct] FOREIGN KEY([ProductRef])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblProductDiscountGroupItem] CHECK CONSTRAINT [FK_tblProductDiscountGroupItem_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblProductDiscountGroupItem_tblProductDiscountGroup]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductDiscountGroupItem]  WITH CHECK ADD  CONSTRAINT [FK_tblProductDiscountGroupItem_tblProductDiscountGroup] FOREIGN KEY([ProductDiscountGroup])
REFERENCES [dbo].[tblProductDiscountGroup] ([id])
GO
ALTER TABLE [dbo].[tblProductDiscountGroupItem] CHECK CONSTRAINT [FK_tblProductDiscountGroupItem_tblProductDiscountGroup]
GO
/****** Object:  ForeignKey [FK_tblProductPackaging_tblContainment]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductPackaging]  WITH NOCHECK ADD  CONSTRAINT [FK_tblProductPackaging_tblContainment] FOREIGN KEY([Containment])
REFERENCES [dbo].[tblContainment] ([id])
GO
ALTER TABLE [dbo].[tblProductPackaging] CHECK CONSTRAINT [FK_tblProductPackaging_tblContainment]
GO
/****** Object:  ForeignKey [FK_tblProductPackaging_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProductPackaging]  WITH NOCHECK ADD  CONSTRAINT [FK_tblProductPackaging_tblProduct] FOREIGN KEY([ReturnableProduct])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblProductPackaging] CHECK CONSTRAINT [FK_tblProductPackaging_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblPromotionDiscount_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPromotionDiscount]  WITH CHECK ADD  CONSTRAINT [FK_tblPromotionDiscount_tblProduct] FOREIGN KEY([ProductRef])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblPromotionDiscount] CHECK CONSTRAINT [FK_tblPromotionDiscount_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblPromotionDiscountItem_tblPromotionDiscount]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPromotionDiscountItem]  WITH CHECK ADD  CONSTRAINT [FK_tblPromotionDiscountItem_tblPromotionDiscount] FOREIGN KEY([PromotionDiscountId])
REFERENCES [dbo].[tblPromotionDiscount] ([id])
GO
ALTER TABLE [dbo].[tblPromotionDiscountItem] CHECK CONSTRAINT [FK_tblPromotionDiscountItem_tblPromotionDiscount]
GO
/****** Object:  ForeignKey [FK_tblProvince_tblProvince]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblProvince]  WITH CHECK ADD  CONSTRAINT [FK_tblProvince_tblProvince] FOREIGN KEY([CountryId])
REFERENCES [dbo].[tblCountry] ([id])
GO
ALTER TABLE [dbo].[tblProvince] CHECK CONSTRAINT [FK_tblProvince_tblProvince]
GO
/****** Object:  ForeignKey [PurchasingClerkRoute_Route_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblPurchasingClerkRoute]  WITH CHECK ADD  CONSTRAINT [PurchasingClerkRoute_Route_FK] FOREIGN KEY([RouteId])
REFERENCES [dbo].[tblRoutes] ([RouteID])
GO
ALTER TABLE [dbo].[tblPurchasingClerkRoute] CHECK CONSTRAINT [PurchasingClerkRoute_Route_FK]
GO
/****** Object:  ForeignKey [RecollectionItem_Recollection_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRecollectionItem]  WITH CHECK ADD  CONSTRAINT [RecollectionItem_Recollection_FK] FOREIGN KEY([RecollectionId])
REFERENCES [dbo].[tblRecollection] ([Id])
GO
ALTER TABLE [dbo].[tblRecollectionItem] CHECK CONSTRAINT [RecollectionItem_Recollection_FK]
GO
/****** Object:  ForeignKey [FK_tblRegion_tblCountry]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRegion]  WITH CHECK ADD  CONSTRAINT [FK_tblRegion_tblCountry] FOREIGN KEY([Country])
REFERENCES [dbo].[tblCountry] ([id])
GO
ALTER TABLE [dbo].[tblRegion] CHECK CONSTRAINT [FK_tblRegion_tblCountry]
GO
/****** Object:  ForeignKey [FK_tblReOrderLevel_tblProduct]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblReOrderLevel]  WITH CHECK ADD  CONSTRAINT [FK_tblReOrderLevel_tblProduct] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblReOrderLevel] CHECK CONSTRAINT [FK_tblReOrderLevel_tblProduct]
GO
/****** Object:  ForeignKey [FK_tblReOrderLevel_tblReOrderLevel]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblReOrderLevel]  WITH CHECK ADD  CONSTRAINT [FK_tblReOrderLevel_tblReOrderLevel] FOREIGN KEY([DistributorId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblReOrderLevel] CHECK CONSTRAINT [FK_tblReOrderLevel_tblReOrderLevel]
GO
/****** Object:  ForeignKey [FK_tblRoutes_tblRoute]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblRoutes]  WITH CHECK ADD  CONSTRAINT [FK_tblRoutes_tblRoute] FOREIGN KEY([RegionId])
REFERENCES [dbo].[tblRegion] ([id])
GO
ALTER TABLE [dbo].[tblRoutes] CHECK CONSTRAINT [FK_tblRoutes_tblRoute]
GO
/****** Object:  ForeignKey [FK__tblSalema__Route__2C345F27]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSalemanRoute]  WITH CHECK ADD  CONSTRAINT [FK__tblSalema__Route__2C345F27] FOREIGN KEY([RouteId])
REFERENCES [dbo].[tblRoutes] ([RouteID])
GO
ALTER TABLE [dbo].[tblSalemanRoute] CHECK CONSTRAINT [FK__tblSalema__Route__2C345F27]
GO
/****** Object:  ForeignKey [FK__tblSalema__Route__2D288360]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSalemanRoute]  WITH CHECK ADD  CONSTRAINT [FK__tblSalema__Route__2D288360] FOREIGN KEY([RouteId])
REFERENCES [dbo].[tblRoutes] ([RouteID])
GO
ALTER TABLE [dbo].[tblSalemanRoute] CHECK CONSTRAINT [FK__tblSalema__Route__2D288360]
GO
/****** Object:  ForeignKey [FK_tblSaleValueDiscount_tblSaleValueDiscount]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSaleValueDiscount]  WITH CHECK ADD  CONSTRAINT [FK_tblSaleValueDiscount_tblSaleValueDiscount] FOREIGN KEY([TierId])
REFERENCES [dbo].[tblPricingTier] ([id])
GO
ALTER TABLE [dbo].[tblSaleValueDiscount] CHECK CONSTRAINT [FK_tblSaleValueDiscount_tblSaleValueDiscount]
GO
/****** Object:  ForeignKey [FK_tblSaleValueDiscountItems_tblSaleValueDiscount]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSaleValueDiscountItems]  WITH CHECK ADD  CONSTRAINT [FK_tblSaleValueDiscountItems_tblSaleValueDiscount] FOREIGN KEY([SaleValueId])
REFERENCES [dbo].[tblSaleValueDiscount] ([id])
GO
ALTER TABLE [dbo].[tblSaleValueDiscountItems] CHECK CONSTRAINT [FK_tblSaleValueDiscountItems_tblSaleValueDiscount]
GO
/****** Object:  ForeignKey [ShipToAdress_CostCentre_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblShipToAddress]  WITH CHECK ADD  CONSTRAINT [ShipToAdress_CostCentre_FK] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblShipToAddress] CHECK CONSTRAINT [ShipToAdress_CostCentre_FK]
GO
/****** Object:  ForeignKey [SourcingLineItem_SourcingDocument_FK]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblSourcingLineItem]  WITH CHECK ADD  CONSTRAINT [SourcingLineItem_SourcingDocument_FK] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[tblSourcingDocument] ([Id])
GO
ALTER TABLE [dbo].[tblSourcingLineItem] CHECK CONSTRAINT [SourcingLineItem_SourcingDocument_FK]
GO
/****** Object:  ForeignKey [FK_tblTarget_tblCostCentre]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTarget]  WITH CHECK ADD  CONSTRAINT [FK_tblTarget_tblCostCentre] FOREIGN KEY([CostCentreId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblTarget] CHECK CONSTRAINT [FK_tblTarget_tblCostCentre]
GO
/****** Object:  ForeignKey [FK_tblTarget_tblTargetPeriod]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTarget]  WITH CHECK ADD  CONSTRAINT [FK_tblTarget_tblTargetPeriod] FOREIGN KEY([PeriodId])
REFERENCES [dbo].[tblTargetPeriod] ([Id])
GO
ALTER TABLE [dbo].[tblTarget] CHECK CONSTRAINT [FK_tblTarget_tblTargetPeriod]
GO
/****** Object:  ForeignKey [FK__tblTarget__Produ__732AC307]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetItem]  WITH CHECK ADD  CONSTRAINT [FK__tblTarget__Produ__732AC307] FOREIGN KEY([ProductId])
REFERENCES [dbo].[tblProduct] ([id])
GO
ALTER TABLE [dbo].[tblTargetItem] CHECK CONSTRAINT [FK__tblTarget__Produ__732AC307]
GO
/****** Object:  ForeignKey [FK__tblTarget__Targe__741EE740]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblTargetItem]  WITH CHECK ADD  CONSTRAINT [FK__tblTarget__Targe__741EE740] FOREIGN KEY([TargetId])
REFERENCES [dbo].[tblTarget] ([id])
GO
ALTER TABLE [dbo].[tblTargetItem] CHECK CONSTRAINT [FK__tblTarget__Targe__741EE740]
GO
/****** Object:  ForeignKey [FK__tblUserGr__Group__31ED387D]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUserGroupRoles]  WITH CHECK ADD  CONSTRAINT [FK__tblUserGr__Group__31ED387D] FOREIGN KEY([GroupId])
REFERENCES [dbo].[tblUserGroup] ([Id])
GO
ALTER TABLE [dbo].[tblUserGroupRoles] CHECK CONSTRAINT [FK__tblUserGr__Group__31ED387D]
GO
/****** Object:  ForeignKey [FK__tblUsers__GroupI__34C9A528]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUsers]  WITH CHECK ADD  CONSTRAINT [FK__tblUsers__GroupI__34C9A528] FOREIGN KEY([GroupId])
REFERENCES [dbo].[tblUserGroup] ([Id])
GO
ALTER TABLE [dbo].[tblUsers] CHECK CONSTRAINT [FK__tblUsers__GroupI__34C9A528]
GO
/****** Object:  ForeignKey [FK_tblUsers_tblUsers]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblUsers]  WITH CHECK ADD  CONSTRAINT [FK_tblUsers_tblUsers] FOREIGN KEY([CostCenterId])
REFERENCES [dbo].[tblCostCentre] ([Id])
GO
ALTER TABLE [dbo].[tblUsers] CHECK CONSTRAINT [FK_tblUsers_tblUsers]
GO
/****** Object:  ForeignKey [FK_tblVATClassItem_tblVATClass]    Script Date: 10/30/2013 09:46:30 ******/
ALTER TABLE [dbo].[tblVATClassItem]  WITH CHECK ADD  CONSTRAINT [FK_tblVATClassItem_tblVATClass] FOREIGN KEY([VATClassID])
REFERENCES [dbo].[tblVATClass] ([id])
GO
ALTER TABLE [dbo].[tblVATClassItem] CHECK CONSTRAINT [FK_tblVATClassItem_tblVATClass]
GO
