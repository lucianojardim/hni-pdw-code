set nocount on
go

/**********************************
 * db0001_CreateInitialTables.sql *
 **********************************/
create table [Categories] (
	[CategoryID] [int] not null identity(1,1),
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkCategories] PRIMARY KEY CLUSTERED ([CategoryID])
) on [primary]
go

create table [Serieses] (
	[SeriesID] [int] not null identity(1,1),
	[Name] [nvarchar](50) not null,
	[CreatedDate] [datetime] not null,
	[CategoryID] [int] not null CONSTRAINT fkSeries_Category FOREIGN KEY REFERENCES [Categories]([CategoryID]),
	CONSTRAINT [pkSerieses] PRIMARY KEY CLUSTERED ([SeriesID])
) on [primary]
go

create table [Attributes] (
	[AttributeID] [int] not null identity(1,1),
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkAttributes] PRIMARY KEY CLUSTERED ([AttributeID])
) on [primary]
go

create table [AttributeOptions] (
	[OptionID] [int] not null identity(1,1),
	[Name] [nvarchar](100) not null,
	[AttributeID] [int] not null CONSTRAINT fkOption_Attribute FOREIGN KEY REFERENCES [Attributes]([AttributeID]),
	CONSTRAINT [pkAttributeOptions] PRIMARY KEY CLUSTERED ([OptionID])
) on [primary]
go

create table [SeriesIntAttributes] (
	[ValueID] [int] not null identity(1,1),
	[Value] [int] not null,
	[AttributeID] [int] not null CONSTRAINT fkSIA_Attribute FOREIGN KEY REFERENCES [Attributes]([AttributeID]),
	[SeriesID] [int] not null CONSTRAINT fkSIA_Series FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkSeriesIntAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [SeriesTextAttributes] (
	[ValueID] [int] not null identity(1,1),
	[Value] [nvarchar](500) not null,
	[AttributeID] [int] not null CONSTRAINT fkSTA_Attribute FOREIGN KEY REFERENCES [Attributes]([AttributeID]),
	[SeriesID] [int] not null CONSTRAINT fkSTA_Series FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkSeriesTextAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [SeriesOptionAttributes] (
	[ValueID] [int] not null identity(1,1),
	[OptionID] [int] not null CONSTRAINT fkSOA_AttributeOption FOREIGN KEY REFERENCES [AttributeOptions]([OptionID]),
	[AttributeID] [int] not null CONSTRAINT fkSOA_Attribute FOREIGN KEY REFERENCES [Attributes]([AttributeID]),
	[SeriesID] [int] not null CONSTRAINT fkSOA_Series FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkSeriesOptionAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [ImageFiles] (
	[ImageID] [int] not null identity(1,1),
	[Name] [nvarchar](100) not null,
	[OriginalExtension] [nvarchar](10) not null,
	[Caption] [nvarchar](1000) not null, 
	[HasPeople] [bit] not null, 
	[ImageType] [nvarchar](10) not null,
	CONSTRAINT [pkImageFiles] PRIMARY KEY CLUSTERED ([ImageID])
) on [primary]
go

create table [SeriesImageFiles] (
	[AssociationID] [int] not null identity(1,1),
	[IsFeatured] [bit] not null,
	[ImageID] [int] not null CONSTRAINT fkSIF_Image FOREIGN KEY REFERENCES [ImageFiles]([ImageID]),
	[SeriesID] [int] not null CONSTRAINT fkSIF_Series FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkSeriesImageFiles] PRIMARY KEY CLUSTERED ([AssociationID])
) on [primary]
go

go
/*******************************
 * db0002_AddImageKeywords.sql *
 *******************************/
alter table [ImageFiles] add [Keyword] [nvarchar](1000) not null constraint [defkeyword] default('')
alter table [ImageFiles] drop constraint [defkeyword]
go
go
/************************************
 * db0003_ImageFilesCreatedDate.sql *
 ************************************/
alter table [ImageFiles] add [CreatedDate] [datetime] not null constraint [defCD] default(getDate())
alter table [ImageFiles] drop constraint [defCD]
go
go
/************************************
 * db0004_ImageFilesDescription.sql *
 ************************************/
/*
alter table [ImageFiles] add [Description] [nvarchar](1000) not null constraint [defDesc] default('')
alter table [ImageFiles] drop constraint [defDesc]
go
alter table [ImageFiles] drop column [Description]
go
*/
go
/*********************************
 * db0005_RelatedSeriesTable.sql *
 *********************************/
create table [RelatedSeries] (
	[ParentSeriesID] [int] not null CONSTRAINT fkRelatedSeries_Parent FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	[ChildSeriesID] [int] not null CONSTRAINT fkRelatedSeries_Child FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	CONSTRAINT [pkRelatedSeries] PRIMARY KEY CLUSTERED ([ParentSeriesID], [ChildSeriesID])

) on [primary]
go
go
/***********************************
 * db0006_AttributesDetailItem.sql *
 ***********************************/
alter table [Attributes] add [DetailItem] [bit] not null constraint [defDI] default(1)
alter table [Attributes] drop constraint [defDI]
go
go
/****************************
 * db0007_TypicalTables.sql *
 ****************************/
create table [Typicals] (
	[TypicalID] [int] not null identity(1,1),
	[Name] [nvarchar](50) not null,
	[CreatedDate] [datetime] not null,
	CONSTRAINT [pkTypicals] PRIMARY KEY CLUSTERED ([TypicalID])
) on [primary]
go

create table [SeriesTypicals] (
	[AssociationID] [int] not null identity(1,1),
	[IsPrimary] [bit] not null,
	[SeriesID] [int] not null CONSTRAINT fkST_Series FOREIGN KEY REFERENCES [Serieses]([SeriesID]),
	[TypicalID] [int] not null CONSTRAINT fkST_Typical FOREIGN KEY REFERENCES [Typicals]([TypicalID]),
	CONSTRAINT [pkSeriesTypicals] PRIMARY KEY CLUSTERED ([AssociationID])
) on [primary]
go

create table [TAttributes] (
	[AttributeID] [int] not null identity(1,1),
	[Name] [nvarchar](50) not null,
	[DetailItem] [bit] not null,
	CONSTRAINT [pkTAttributes] PRIMARY KEY CLUSTERED ([AttributeID])
) on [primary]
go

create table [TAttributeOptions] (
	[OptionID] [int] not null identity(1,1),
	[Name] [nvarchar](100) not null,
	[AttributeID] [int] not null CONSTRAINT fkTOption_Attribute FOREIGN KEY REFERENCES [TAttributes]([AttributeID]),
	CONSTRAINT [pkTAttributeOptions] PRIMARY KEY CLUSTERED ([OptionID])
) on [primary]
go

create table [TypicalIntAttributes] (
	[ValueID] [int] not null identity(1,1),
	[Value] [int] not null,
	[AttributeID] [int] not null CONSTRAINT fkTIA_Attribute FOREIGN KEY REFERENCES [TAttributes]([AttributeID]),
	[TypicalID] [int] not null CONSTRAINT fkTIA_Typical FOREIGN KEY REFERENCES [Typicals]([TypicalID]),
	CONSTRAINT [pkTypicalIntAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [TypicalTextAttributes] (
	[ValueID] [int] not null identity(1,1),
	[Value] [nvarchar](500) not null,
	[AttributeID] [int] not null CONSTRAINT fkTTA_Attribute FOREIGN KEY REFERENCES [TAttributes]([AttributeID]),
	[TypicalID] [int] not null CONSTRAINT fkTTA_Typical FOREIGN KEY REFERENCES [Typicals]([TypicalID]),
	CONSTRAINT [pkTypicalTextAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [TypicalOptionAttributes] (
	[ValueID] [int] not null identity(1,1),
	[OptionID] [int] not null CONSTRAINT fkTOA_AttributeOption FOREIGN KEY REFERENCES [TAttributeOptions]([OptionID]),
	[AttributeID] [int] not null CONSTRAINT fkTOA_Attribute FOREIGN KEY REFERENCES [TAttributes]([AttributeID]),
	[TypicalID] [int] not null CONSTRAINT fkTOA_Typical FOREIGN KEY REFERENCES [Typicals]([TypicalID]),
	CONSTRAINT [pkTypicalOptionAttributes] PRIMARY KEY CLUSTERED ([ValueID])
) on [primary]
go

create table [TypicalImageFiles] (
	[AssociationID] [int] not null identity(1,1),
	[IsFeatured] [bit] not null,
	[ImageID] [int] not null CONSTRAINT fkTIF_Image FOREIGN KEY REFERENCES [ImageFiles]([ImageID]),
	[TypicalID] [int] not null CONSTRAINT fkTIF_Typical FOREIGN KEY REFERENCES [Typicals]([TypicalID]),
	CONSTRAINT [pkTypicalImageFiles] PRIMARY KEY CLUSTERED ([AssociationID])
) on [primary]
go

go
/*********************************
 * db0008_ImageFilesMimeType.sql *
 *********************************/
alter table [ImageFiles] add [MIMEType] [nvarchar](100) not null constraint [defMT] default('')
alter table [ImageFiles] drop constraint [defMT]
go
update [ImageFiles] set [MIMEType] = 'image/jpeg' where OriginalExtension = '.jpg'
go

go
/***************************
 * db0009_BiggerFields.sql *
 ***************************/
alter table AttributeOptions alter column [Name] [nvarchar](500) not null
alter table TAttributeOptions alter column [Name] [nvarchar](500) not null
go

alter table SeriesTextAttributes alter column [Value] [nvarchar](2000) not null
alter table TypicalTextAttributes alter column [Value] [nvarchar](2000) not null
go

go
/*******************************
 * db0010_PriceGuideChange.sql *
 *******************************/
update Attributes set Name = 'Price List' where Name = 'price guide'
go
/******************************
 * db0011_SIFDisplayOrder.sql *
 ******************************/
alter table SeriesImageFiles add [DisplayOrder] [int] not null constraint [defOrder] default(0)
alter table SeriesImageFiles drop constraint [defOrder]
go
update SeriesImageFiles 
	set DisplayOrder = (select count(*) + 1 from SeriesImageFiles sif 
							where SeriesImageFiles.SeriesID = sif.SeriesID and sif.AssociationID < SeriesImageFiles.AssociationID and sif.IsFeatured = 0)
	where SeriesImageFiles.IsFeatured = 0
go
select * from SeriesImageFiles order by SeriesID, IsFeatured desc, DisplayOrder
go

go
/*******************************
 * db0012_SearchResultsLog.sql *
 *******************************/
create table [SearchResultsLog] (
	[LogID] [int] not null identity(1,1),
	[SearchDateTime] [datetime] not null,
	[SeriesCount] [int] not null,
	[ImageCount] [int] not null,
	[TypicalCount] [int] not null,
	[PageCount] [int] not null,
	[SearchTerm] [ntext] not null,
	CONSTRAINT [pkSearchResultsLog] PRIMARY KEY CLUSTERED ([LogID])		
) on [primary]
go
go
/*********************************
 * db0013_FeaturedImageTypes.sql *
 *********************************/
alter table [ImageFiles] add [FeaturedEdge] [nvarchar](500) null, [FeaturedPull] [nvarchar](500) null
go
go
/********************************
 * db0014_ImageContentField.sql *
 ********************************/
alter table [ImageFiles] add [ImageContent] [int] not null constraint [defImageType] default(1)
go
alter table [ImageFiles] drop constraint [defImageType]
go
go
/********************************
 * db0015_ImageFinishFields.sql *
 ********************************/
alter table [ImageFiles] add 
	[FeaturedFinish] [nvarchar](500) null,
	[FinishType] [int] null,
	[FinishSubType] [int] null	
go

go
/*******************************
 * db0016_DBKeywordsFields.sql *
 *******************************/
alter table [Serieses] add [DBKeywords] [ntext] null
go
alter table [Typicals] add [DBKeywords] [ntext] null
go
alter table [ImageFiles] add [DBKeywords] [ntext] null
go

go
/***************************************
 * db0017_PopulateImageFileDetails.sql *
 ***************************************/
update ImageFiles set ImageContent = 3, FeaturedPull='Arc (AR)' where Name='A0449'
update ImageFiles set ImageContent = 3, FeaturedPull='Bevel (BL)' where Name='A0450'
update ImageFiles set ImageContent = 3, FeaturedPull='Classic (CL)' where Name='A0453'
update ImageFiles set ImageContent = 3, FeaturedPull='Traditional (TD)' where Name='A0457'
update ImageFiles set ImageContent = 3, FeaturedPull='Premier (PE)' where Name='A0455'
update ImageFiles set ImageContent = 3, FeaturedPull='Traditional Knob (TK)' where Name='A0459'
update ImageFiles set ImageContent = 3, FeaturedPull='Chamfer (CH)' where Name='A0465'
update ImageFiles set ImageContent = 3, FeaturedPull='Rod (RD)' where Name='A0469'
update ImageFiles set ImageContent = 3, FeaturedPull='Shark (SH)' where Name='A0473'
update ImageFiles set ImageContent = 3, FeaturedPull='Keyhole (KH)' where Name='A0480'
update ImageFiles set ImageContent = 3, FeaturedPull='Rainbow (RB)' where Name='A0481'
update ImageFiles set ImageContent = 3, FeaturedPull='Waterfall (WF)' where Name='A0482'
update ImageFiles set ImageContent = 3, FeaturedPull='Bead (BD)' where Name='A0438'
update ImageFiles set ImageContent = 3, FeaturedPull='Cable (CB)' where Name='A0440'
update ImageFiles set ImageContent = 3, FeaturedPull='Slot (ST)' where Name='A0492'
update ImageFiles set ImageContent = 3, FeaturedPull='Wave (WV)' where Name='A0493'
update ImageFiles set ImageContent = 3, FeaturedPull='Ramp (RP)' where Name='A0443'
update ImageFiles set ImageContent = 3, FeaturedPull='Spline (SP)' where Name='A0441'
update ImageFiles set ImageContent = 3, FeaturedPull='Sweep (SW)' where Name='A0442'
update ImageFiles set ImageContent = 3, FeaturedPull='Beam (BM)' where Name='A0439'
update ImageFiles set ImageContent = 3, FeaturedPull='Arc (AC)' where Name='A0791'
update ImageFiles set ImageContent = 3, FeaturedPull='Bar (BR)' where Name='A0789'
update ImageFiles set ImageContent = 3, FeaturedPull='Crescent (CS)' where Name='A0790'
update ImageFiles set ImageContent = 3, FeaturedPull='Axis (AS)' where Name='A0786'
update ImageFiles set ImageContent = 3, FeaturedPull='Radius (RS)' where Name='A0787'
update ImageFiles set ImageContent = 3, FeaturedPull='Wing (WG)' where Name='A0788'
update ImageFiles set ImageContent = 3, FeaturedPull='Prominence Pull' where Name='A0487'
go
update ImageFiles set ImageContent = 2, FeaturedEdge='I-Beam (IB)' where Name='A0445'
update ImageFiles set ImageContent = 2, FeaturedEdge='Ridge (RG)' where Name='A0446'
update ImageFiles set ImageContent = 2, FeaturedEdge='Ridge (RG)' where Name='A0446'
update ImageFiles set ImageContent = 2, FeaturedEdge='PVC Soft (PS)' where Name='A0447'
update ImageFiles set ImageContent = 2, FeaturedEdge='Soft (SO)' where Name='A0447'
update ImageFiles set ImageContent = 2, FeaturedEdge='Soft (SO)' where Name='A0447'
update ImageFiles set ImageContent = 2, FeaturedEdge='Softenend (SF)' where Name='A0447'
update ImageFiles set ImageContent = 2, FeaturedEdge='Altamont Edge' where Name='A0452'
update ImageFiles set ImageContent = 2, FeaturedEdge='Coronado (CE)' where Name='A0461'
update ImageFiles set ImageContent = 2, FeaturedEdge='Knife (KF)' where Name='A0474'
update ImageFiles set ImageContent = 2, FeaturedEdge='Shark (SH)' where Name='A0476'
update ImageFiles set ImageContent = 2, FeaturedEdge='Angle (AG)' where Name='A0478'
update ImageFiles set ImageContent = 2, FeaturedEdge='Kerf (KF)' where Name='A0479'
update ImageFiles set ImageContent = 2, FeaturedEdge='Chamfer (CF)' where Name='A0483'
update ImageFiles set ImageContent = 2, FeaturedEdge='Quarter Bullnose (QB)' where Name='A0484'
update ImageFiles set ImageContent = 2, FeaturedEdge='Cove (CV)' where Name='A0494'
update ImageFiles set ImageContent = 2, FeaturedEdge='Crescent (CS)' where Name='A0495'
update ImageFiles set ImageContent = 2, FeaturedEdge='Double Bead (DB)' where Name='A0496'
update ImageFiles set ImageContent = 2, FeaturedEdge='Reeded (RD)' where Name='A0497'
update ImageFiles set ImageContent = 2, FeaturedEdge='Butterfly (BF)' where Name='A0635'
update ImageFiles set ImageContent = 2, FeaturedEdge='Reflect (RE)' where Name='A0636'
go
update ImageFiles set ImageContent = 4, FeaturedFinish='White Laminate (LWH)', FinishType=2, FinishSubType=4 where Name='A0753'
update ImageFiles set ImageContent = 4, FeaturedFinish='Shaker Cherry Laminate (LKC)', FinishType=2, FinishSubType=5 where Name='A0754'
update ImageFiles set ImageContent = 4, FeaturedFinish='Platinum Laminate (LPN)', FinishType=2, FinishSubType=4 where Name='A0755'
update ImageFiles set ImageContent = 4, FeaturedFinish='Natural Maple Laminate (LNM)', FinishType=2, FinishSubType=5 where Name='A0756'
update ImageFiles set ImageContent = 4, FeaturedFinish='Light Cherry Laminate (LLC)', FinishType=2, FinishSubType=5 where Name='A0757'
update ImageFiles set ImageContent = 4, FeaturedFinish='Java Cherry Laminate (LJC)', FinishType=2, FinishSubType=5 where Name='A0758'
update ImageFiles set ImageContent = 4, FeaturedFinish='Huntington Maple Laminate (LUM)', FinishType=2, FinishSubType=5 where Name='A0759'
update ImageFiles set ImageContent = 4, FeaturedFinish='Harvest Maple Laminate (LVM)', FinishType=2, FinishSubType=5 where Name='A0760'
update ImageFiles set ImageContent = 4, FeaturedFinish='Harbor Teak Laminate (LHT)', FinishType=2, FinishSubType=5 where Name='A0761'
update ImageFiles set ImageContent = 4, FeaturedFinish='Golden Cherry Laminate (LGC)', FinishType=2, FinishSubType=5 where Name='A0762'
update ImageFiles set ImageContent = 4, FeaturedFinish='Empire Walnut Laminate (LEW)', FinishType=2, FinishSubType=5 where Name='A0763'
update ImageFiles set ImageContent = 4, FeaturedFinish='Cordovan Cherry Laminate (LCC)', FinishType=2, FinishSubType=5 where Name='A0764'
update ImageFiles set ImageContent = 4, FeaturedFinish='Columbian Walnut Laminate (LCW)', FinishType=2, FinishSubType=5 where Name='A0765'
update ImageFiles set ImageContent = 4, FeaturedFinish='Clear Maple Laminate (LCM)', FinishType=2, FinishSubType=5 where Name='A0766'
update ImageFiles set ImageContent = 4, FeaturedFinish='Brighton Walnut Laminate (LBW)', FinishType=2, FinishSubType=5 where Name='A0767'
update ImageFiles set ImageContent = 4, FeaturedFinish='Traditional Walnut Veneer (VTW)', FinishType=1, FinishSubType=3 where Name='A0768'
update ImageFiles set ImageContent = 4, FeaturedFinish='Empire Walnut Veneer (VEW)', FinishType=1, FinishSubType=3 where Name='A0769'
update ImageFiles set ImageContent = 4, FeaturedFinish='Columbian Walnut Veneer (VCW)', FinishType=1, FinishSubType=3 where Name='A0770'
update ImageFiles set ImageContent = 4, FeaturedFinish='Brighton Walnut Veneer (VBW)', FinishType=1, FinishSubType=3 where Name='A0771'
update ImageFiles set ImageContent = 4, FeaturedFinish='American Walnut Veneer (VAW)', FinishType=1, FinishSubType=3 where Name='A0772'
update ImageFiles set ImageContent = 4, FeaturedFinish='Natural Maple Veneer (VNM)', FinishType=1, FinishSubType=2 where Name='A0773'
update ImageFiles set ImageContent = 4, FeaturedFinish='Huntington Maple Veneer (VUM)', FinishType=1, FinishSubType=2 where Name='A0774'
update ImageFiles set ImageContent = 4, FeaturedFinish='Harvest Maple Veneer (VVM)', FinishType=1, FinishSubType=2 where Name='A0775'
update ImageFiles set ImageContent = 4, FeaturedFinish='Golden Maple Veneer (VGM)', FinishType=1, FinishSubType=2 where Name='A0776'
update ImageFiles set ImageContent = 4, FeaturedFinish='Clear Maple Veneer (VCM)', FinishType=1, FinishSubType=2 where Name='A0777'
update ImageFiles set ImageContent = 4, FeaturedFinish='Traditional Cherry Veneer (VTC)', FinishType=1, FinishSubType=1 where Name='A0778'
update ImageFiles set ImageContent = 4, FeaturedFinish='Shaker Cherry Veneer (VKC)', FinishType=1, FinishSubType=1 where Name='A0779'
update ImageFiles set ImageContent = 4, FeaturedFinish='Natural Cherry Veneer (VNC)', FinishType=1, FinishSubType=1 where Name='A0780'
update ImageFiles set ImageContent = 4, FeaturedFinish='Mahogany Cherry Veneer (VHC)', FinishType=1, FinishSubType=1 where Name='A0781'
update ImageFiles set ImageContent = 4, FeaturedFinish='Light Cherry Veneer (VLC)', FinishType=1, FinishSubType=1 where Name='A0782'
update ImageFiles set ImageContent = 4, FeaturedFinish='Java Cherry Veneer (VJC)', FinishType=1, FinishSubType=1 where Name='A0783'
update ImageFiles set ImageContent = 4, FeaturedFinish='Golden Cherry Veneer (VGC)', FinishType=1, FinishSubType=1 where Name='A0784'
update ImageFiles set ImageContent = 4, FeaturedFinish='Cordovan Cherry Veneer (VCC)', FinishType=1, FinishSubType=1 where Name='A0785'
update ImageFiles set ImageContent = 4, FeaturedFinish='Monterey Beige Laminate (LMB)', FinishType=2, FinishSubType=6 where Name='A0792'
update ImageFiles set ImageContent = 4, FeaturedFinish='Desert Zephyr Laminate (LDZ)', FinishType=2, FinishSubType=6 where Name='A0793'
update ImageFiles set ImageContent = 4, FeaturedFinish='Canyon Zephyr Laminate (LCZ)', FinishType=2, FinishSubType=6 where Name='A0794'
update ImageFiles set ImageContent = 4, FeaturedFinish='Twilight Zephyr Laminate (LTZ)', FinishType=2, FinishSubType=6 where Name='A0795'
update ImageFiles set ImageContent = 4, FeaturedFinish='Sheer Mesh Laminate (LSH)', FinishType=2, FinishSubType=6 where Name='A0796'
update ImageFiles set ImageContent = 4, FeaturedFinish='Steel Mesh Laminate (LST)', FinishType=2, FinishSubType=6 where Name='A0797'
update ImageFiles set ImageContent = 4, FeaturedFinish='Charcoal Laminate (LCR)', FinishType=2, FinishSubType=4 where Name='A0798'
update ImageFiles set ImageContent = 4, FeaturedFinish='Gray Cloud Laminate (LYC)', FinishType=2, FinishSubType=4 where Name='A0800'
go
go
/********************************
 * db0019_PublicationTables.sql *
 ********************************/
create table [Publications] (
	[PublicationID] [int] identity(1,1) not null,
	[Name] [nvarchar](100) not null,
	[PubDate] [datetime] not null,
	[FilterVisible] [bit] not null,
	CONSTRAINT [pkPublications] PRIMARY KEY CLUSTERED ([PublicationID])	
) on [Primary]
go

create table [PublicationImages] (
	[PublicationID] [int] not null CONSTRAINT fkPI_Publication FOREIGN KEY REFERENCES [Publications]([PublicationID]),
	[ImageID] [int] not null CONSTRAINT fkPI_Image FOREIGN KEY REFERENCES [ImageFiles]([ImageID]),
	[PageNumber] [int] null,
	CONSTRAINT [pkPublicationImages] PRIMARY KEY CLUSTERED ([PublicationID], [ImageID])	
) on [primary]
go

go
/*****************************
 * db0020_SeriesIsActive.sql *
 *****************************/
alter table [Serieses] add [IsActive] bit not null constraint [defactive] default(1)
go
alter table [Serieses] drop constraint [defactive]
go
go
/*******************************
 * db0021_MoreImageDetails.sql *
 *******************************/
alter table [ImageFiles] add 
	[TableShape] [nvarchar](500) null,
	[TableBase] [nvarchar](500) null,
	[ControlMechanism] [nvarchar](500) null,
	[ControlDescription] [ntext] null
go

go
/********************************
 * db0022_DealerInformation.sql *
 ********************************/
CREATE TABLE [VideoLinks](
	[VideoID] [int] IDENTITY(1,1) NOT NULL,
	[Display] [nvarchar](200) NOT NULL,
	[YouTubeID] [nvarchar](20) NOT NULL,
	CONSTRAINT [pkVideoLinks] PRIMARY KEY CLUSTERED ([VideoID])
) ON [PRIMARY]
GO

CREATE TABLE [Dealers](
	[DealerID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[URL] [nvarchar](200) NOT NULL,
	[MainContent] [ntext] NOT NULL,
	[FeaturedVideoID] [int] NOT NULL CONSTRAINT [fkDealer_VideoLink] FOREIGN KEY REFERENCES [VideoLinks] ([VideoID]),
	CONSTRAINT [pkDealers] PRIMARY KEY CLUSTERED ([DealerID])
) ON [PRIMARY]
GO

CREATE TABLE [PageLinks](
	[PageID] [int] IDENTITY(1,1) NOT NULL,
	[Display] [nvarchar](200) NOT NULL,
	[URLLocation] [nvarchar](200) NOT NULL,
	CONSTRAINT [pkPageLinks] PRIMARY KEY CLUSTERED ([PageID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerVideoLinks](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDVL_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[VideoID] [int] NOT NULL CONSTRAINT [fkDVL_VideoLink] FOREIGN KEY REFERENCES [VideoLinks] ([VideoID]),
	CONSTRAINT [pkDealerVideoLinks] PRIMARY KEY CLUSTERED ([DealerID], [VideoID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerPageLinks](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDPL_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[PageID] [int] NOT NULL CONSTRAINT [fkDPL_PageLink] FOREIGN KEY REFERENCES [PageLinks] ([PageID]),
	CONSTRAINT [pkDealerPageLinks] PRIMARY KEY CLUSTERED ([DealerID], [PageID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerFeaturedProducts](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDFP_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[SeriesName] [nvarchar](50) NOT NULL,
	CONSTRAINT [pkDealerFeaturedProducts] PRIMARY KEY CLUSTERED ([DealerID], [SeriesName])
) ON [PRIMARY]
GO

go
/************************************
 * db0023_MoreDealerInformation.sql *
 ************************************/
alter table [Dealers] add 
	[ProductsHeadline] [nvarchar](200) null,
	[PagesHeadline] [nvarchar](200) null,
	[VideosHeadline] [nvarchar](200) null
go
update [Dealers] set [ProductsHeadline] = N'Products We Discussed', PagesHeadline = N'Pages We Discussed', VideosHeadline = N'Videos You Missed'
go
go
/******************************
 * db0024_GoToGuideImages.sql *
 ******************************/
alter table [ImageFiles] add [GoToGuidePage] [int] null
go
go
/*************************
 * db0025_UserTables.sql *
 *************************/
create table [Users] (
	[UserID] [int] identity(1,1) not null,
	[FirstName] [nvarchar](50) not null,
	[LastName] [nvarchar](50) not null, 
	[CompanyName] [nvarchar](50) not null, 
	[Address1] [nvarchar](100) null, 
	[Address2] [nvarchar](100) null, 
	[City] [nvarchar](50) null, 
	[State] [nvarchar](5) null, 
	[Zip] [nvarchar](12) null, 
	[Email] [nvarchar](50) not null, 
	[Password] [nvarchar](200) not null,
	[BusinessPhone] [nvarchar](20) null, 
	[CellPhone] [nvarchar](20) null, 
	[Title] [nvarchar](50) null, 
	[UserType] [nvarchar](50) not null, 
	[Role] [nvarchar](50) not null
	CONSTRAINT [pkUsers] PRIMARY KEY CLUSTERED ([UserID])
) on [PRIMARY]
go


go
/********************************
 * db0026_UserTable_Enabled.sql *
 ********************************/
alter table [Users] add [Enabled] bit not null constraint [defEnabled] default(1)
alter table [Users] drop constraint [defEnabled]
go
go
/************************************
 * db0027_UserTable_AccountType.sql *
 ************************************/
alter table [Users] drop column [UserType]
go
alter table [Users] add [AccountType] [int] not null constraint [defAcctType] default(0)
go
update [Users] set [AccountType] = 1 where [Role] = 'Super Admin'
update [Users] set [AccountType] = 3 where [Role] = 'Paoli Staff Marketing'
update [Users] set [AccountType] = 4 where [Role] = 'Paoli Staff Spec Team'
update [Users] set [AccountType] = 6 where [Role] = 'Paoli Staff Support'
update [Users] set [AccountType] = 7 where [Role] = 'Paoli Staff Sales Rep'
update [Users] set [AccountType] = 9 where [Role] = 'Dealer Sales Rep'
update [Users] set [AccountType] = 8 where [Role] = 'Dealer Principle'
go
alter table [Users] drop column [Role]
go

go
/***************************
 * db0028_SpecRequests.sql *
 ***************************/
/*
drop table [SpecRequestFile]
drop table [SpecRequests]
go
*/
create table [Companies] (
	[CompanyID] [int] identity(1,1) not null,
	[Name] [nvarchar](50) not null,
	[Address1] [nvarchar](100) null, 
	[Address2] [nvarchar](100) null, 
	[City] [nvarchar](50) null, 
	[State] [nvarchar](5) null, 
	[Zip] [nvarchar](12) null, 
	[Phone] [nvarchar](20) null, 
	[FAX] [nvarchar](20) null, 
	[MasterID] [nvarchar](20) null,
	[SubCompanyIDs] [nvarchar](200) null,
	[TripIncentive] [bit] not null,
	[CompanyType] [int] not null,
	CONSTRAINT [pkCompanies] PRIMARY KEY CLUSTERED ([CompanyID])
) on [primary]
go

create table [SpecRequests] (
	[RequestID] [int] identity(1,1) not null,
	[Name] [nvarchar](10) null,
	[ProjectName] [nvarchar](50) null,
	[PaoliSalesRepGroupID] [int] null CONSTRAINT [fkSpec_RepGroup] FOREIGN KEY REFERENCES [Companies] ([CompanyID]),
	[PrimaryCompanyID] [int] null CONSTRAINT [fkSpec_Company] FOREIGN KEY REFERENCES [Companies] ([CompanyID]),
	[DealerSalesRepID] [int] null CONSTRAINT [fkSpec_DealerSalesRep] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[PaoliSpecTeamMemberID] [int] null CONSTRAINT [fkSpec_PaoliSpecTeam] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[RequestDate] [datetime] null,
	[IsGSA] [bit] null,
	[SavedLocation] [nvarchar](500) null,
	[ListPrice] [int] null,
	[FeaturedSeries] [nvarchar](50) null,
	[SeriesList] [ntext] null,
	[Received] [bit] null,
	[SPLQuote] [int] null,
	[LastModifiedDate] [datetime] null,
	[Material] [nvarchar](50) null,
	[Finish] [nvarchar](50) null,
	[IsGoodForWeb] [bit] null,
	[AvailableForIn2] [bit] null,
	[Footprint] [nvarchar](50) null,
	[Notes] [ntext] null,
	CONSTRAINT [pkSpecRequests] PRIMARY KEY CLUSTERED ([RequestID])
) on [primary]
go

create table [SpecRequestFiles] (
	[FileID] [int] identity(1,1) not null,
	[RequestID] [int] not null CONSTRAINT [fkSpecFile_Request] FOREIGN KEY REFERENCES [SpecRequests] ([RequestID]),
	[Extension] [nvarchar](10) not null,
	[Name] [nvarchar](100) not null,
	[VersionNumber] [int] not null,
	[UploadDate] [datetime] not null,
	CONSTRAINT [pkSpecRequestFile] PRIMARY KEY CLUSTERED ([FileID])
) on [primary]
go

insert into [Companies] (
	[Name], [Address1], [City], [State], [Zip], [Phone], [FAX], [TripIncentive], [CompanyType]
) values (
	'Paoli', '201 E Martin St.', 'Orleans', 'IN', '47452', '800-472-8669', '812-865-7080', 0, 1
)
go
alter table [Users] drop column [CompanyName]
go
alter table [Users] add [CompanyID] [int] not null 
	CONSTRAINT [fkUser_Company] FOREIGN KEY REFERENCES [Companies] ([CompanyID])
	constraint [defUserCompany] default(1)
go
alter table [Users] drop constraint [defUserCompany]
go
delete from [Users] where [AccountType] not in (1,2,3,4,5,6)	-- get rid of non-paoli users
go
go
/*******************************
 * db0029_NewTypicalLayout.sql *
 *******************************/
alter table [SpecRequests] alter column [Material] [nvarchar](100) null
alter table [SpecRequests] alter column [Finish] [nvarchar](100) null
alter table [SpecRequests] alter column [Footprint] [nvarchar](100) null
go
alter table [Typicals] add
	[Notes] [ntext] null,
	[SpecRequestID] [int] null CONSTRAINT [fkTypical_Spec] FOREIGN KEY REFERENCES [SpecRequests] ([RequestID]),
	[FeaturedSeries] [nvarchar](50) null,
	[SeriesList] [ntext] null,
	[AvailableForIn2] [bit] null
go
update [Typicals] set [FeaturedSeries] = (Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 1
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main])
go
update [Typicals] set [SeriesList] = (Select 
			   Left(Main.[FullSeriesList] ,Len(Main.[FullSeriesList])-1)
		From(Select distinct 
				   (Select s.Name + ',' AS [text()]
					From Serieses s
						inner join SeriesTypicals st on st.SeriesID = s.SeriesID and st.IsPrimary = 0
					Where st.TypicalID = typicals.TypicalID
					For XML PATH ('')) [FullSeriesList]
			 From Serieses S2) [Main])
go

go
/************************************
 * db0034_Users_RecWelcomeEmail.sql *
 ************************************/
alter table [Users] add [RecWelcomeEmail] [bit] not null constraint [defRWE] default(1)
alter table [Users] drop constraint [defRWE]
go
go
/***************************************
 * db0035_SpecRequests_IsCompleted.sql *
 ***************************************/
alter table [SpecRequests] add [IsCompleted] [bit] not null constraint [defIC] default(0)
alter table [SpecRequests] drop constraint [defIC]
go
go
/**************************
 * db0036_Territories.sql *
 **************************/
create table [Territories] (
	[TerritoryID] [int] not null,
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkTerritories] PRIMARY KEY CLUSTERED ([TerritoryID])
) on [primary]
go

insert into [Territories] ([TerritoryID], [Name]) values 
(6101, ''),
(6102, ''),
(6105, ''),
(6107, ''),
(6110, ''),
(6111, ''),
(6112, ''),
(6113, ''),
(6114, ''),
(6115, ''),
(6140, ''),
(6202, ''),
(6203, ''),
(6204, ''),
(6209, ''),
(6210, ''),
(6212, ''),
(6214, ''),
(6217, ''),
(6221, ''),
(6222, ''),
(6227, ''),
(6228, ''),
(6301, ''),
(6303, ''),
(6305, ''),
(6306, ''),
(6308, ''),
(7005, '')
go
update [Territories] set [Name] = CAST([TerritoryID] as nvarchar(50))
go

alter table [Companies] add [TerritoryID] [int] null CONSTRAINT [fkCompany_Territory] FOREIGN KEY REFERENCES [Territories] ([TerritoryID])
go
/*
update [Companies] set [Companies].[TerritoryID] = [t].[TerritoryID]
	from [Companies] [c]
		left join [ImportCompanies] [ic] on [c].[Name] = [ic].[Company Name] 
		left join [Territories] [t] on [ic].[Territory ID] = [t].[Name] 
*/
go
/********************************
 * db0037_UserSubscriptions.sql *
 ********************************/
create table [UserSubscriptions] (
	[UserID] [int] not null CONSTRAINT [fkUSub_User] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[ProductIntroductions] [bit] not null,
	[BehindTheScenes] [bit] not null,
	[MeetOurMembers] [bit] not null,
	[ProgramChanges] [bit] not null,
	[PricelistUpdates] [bit] not null,
	[QuoteRequests] [bit] not null,
	[SMSAlerts] [bit] not null,
	[SMSPhoneNumber] [nvarchar](20) null,
	CONSTRAINT [pkUserSubscriptions] PRIMARY KEY CLUSTERED ([UserID])
) on [primary]
go

go
/***********************************
 * db0038_SpecRequest_SPLQuote.sql *
 ***********************************/
alter table [SpecRequests] alter column [SPLQuote] [nvarchar](20) null
go
/*
update [SpecRequests] set [SPLQuote] = [SPL Quote] 
	from [SpecRequests] inner join [ImportSpecRequests] on [SpecRequests].[Name] = [ImportSpecRequests].[Typical Name]
where ISNUMERIC([SPL Quote]) = 0 and [Spl quote] is not null
go
*/
/*
select * from [SpecRequests] inner join [ImportSpecRequests] on [SpecRequests].[Name] = [ImportSpecRequests].[Project Number]
where ISNUMERIC([SPL Quote]) = 0 and [Spl quote] is not null
*/
go
/******************************
 * db0039_ImagePopularity.sql *
 ******************************/
create function ImagePopularity( @imgId int )
returns int
as
begin
	declare @nRet int
	set @nRet = 0
	
	if exists(select * from SeriesImageFiles sif 
			inner join SeriesIntAttributes sia on sif.SeriesID = sia.SeriesID and sif.ImageID = @imgId
			inner join Attributes a on sia.AttributeID = a.AttributeID and a.Name = 'Ranking'
		where sia.Value = 2)
	begin
		set @nRet = @nRet + 1
	end
	
	if exists( select * from ImageFiles where ImageID = @imgId and ImageType = 'Env' )
	begin
		set @nRet = @nRet + 2
	end
	
	return @nRet
end
go

alter table [ImageFiles] add [Popularity] as dbo.ImagePopularity(ImageID)
go

go
/******************************
 * db0040_ImagePopularity.sql *
 ******************************/
alter table [ImageFiles] drop column [Popularity]
go

alter function ImagePopularity( @imgId int )
returns int
as
begin
	declare @nRet int
	set @nRet = 0
	
	if exists(select * from SeriesImageFiles sif 
			inner join SeriesIntAttributes sia on sif.SeriesID = sia.SeriesID and sif.ImageID = @imgId
			inner join Attributes a on sia.AttributeID = a.AttributeID and a.Name = 'Ranking'
		where sia.Value = 2)
	begin
		set @nRet = @nRet + 1
	end
	
	if exists( select * from ImageFiles where ImageID = @imgId and ImageType = 'Env' )
	begin
		set @nRet = @nRet + 2
	end
	
	if exists( select * from SeriesImageFiles sif where sif.ImageID = @imgId )
	begin
		set @nRet = @nRet + 4
	end
	
	return @nRet
end
go

alter table [ImageFiles] add [Popularity] as dbo.ImagePopularity(ImageID)
go

go
/**************************
 * db0041_ZipCodeInfo.sql *
 **************************/
create table [ZipCodeInfo] (
	[ZipCode] [nvarchar](10) not null,
	[TerritoryID] [int] not null constraint [fkZip_Territory] foreign key references [Territories](TerritoryID),
	[Longitude] [float] not null,
	[Latitude] [float] not null,
	CONSTRAINT [pkZipCodeInfo] PRIMARY KEY CLUSTERED ([ZipCode])
) on [primary]
go
/*
delete from [ImportZipCodes] where [State] in ('GU')
update [ImportZipCodes] set [Territory ID] = 6227 where [Territory ID] = 6627
go

insert into [ZipCodeInfo] ([ZipCode] , [TerritoryID] , [Latitude] , [Longitude])
	(select [Zip], [Territory ID], [Latitude], [Longitude] from [ImportZipCodes])
go
*/
go
/************************************
 * db0042_CompaniesAndShowrooms.sql *
 ************************************/
alter table [Companies] add 
	[PublicAddress1] [nvarchar](100) null, 
	[PublicAddress2] [nvarchar](100) null, 
	[PublicCity] [nvarchar](50) null, 
	[PublicState] [nvarchar](5) null, 
	[PublicZip] [nvarchar](12) null, 
	[PublicPhone] [nvarchar](20) null, 
	[PublicFAX] [nvarchar](20) null 
go

alter table [Companies] add
	[ContactEmail] [nvarchar](50) null,
	[WebSite] [nvarchar](100) null,
	[PublicContactEmail] [nvarchar](50) null,
	[PublicWebSite] [nvarchar](100) null,
	[PublicDisplayName] [nvarchar](100) null
go

create table [Showrooms] (
	[CompanyID] [int] not null constraint [fkShowroom_Company] foreign key references [Companies](CompanyID),
	[DisplayName] [nvarchar](100) null,
	[Address1] [nvarchar](100) null, 
	[Address2] [nvarchar](100) null, 
	[City] [nvarchar](50) null, 
	[State] [nvarchar](5) null, 
	[Zip] [nvarchar](12) null, 
	[Phone] [nvarchar](20) null, 
	[FAX] [nvarchar](20) null,
	[WebSite] [nvarchar](100) null,
	[Description] [ntext] null,
	[Hours] [ntext] null,
	CONSTRAINT [pkShowrooms] PRIMARY KEY CLUSTERED ([CompanyID])
) on [primary]
go

create table [ShowroomImages] (
	[CompanyID] [int] not null constraint [fkSRI_Company] foreign key references [Companies](CompanyID),
	[ImageID] [int] not null constraint [fkSRI_Image] foreign key references [ImageFiles](ImageID),
	CONSTRAINT [pkShowroomImages] PRIMARY KEY CLUSTERED ([CompanyID], [ImageID])
) on [primary]
go

go
/***********************************
 * db0043_DistanceCalculations.sql *
 ***********************************/
create function DistanceBetweenPoints( @Latitude1 float,@Longitude1 float, @Latitude2 float, @Longitude2 float )
returns float
as
begin
	declare @radius float

	declare @lon1  float
	declare @lon2  float
	declare @lat1  float
	declare @lat2  float

	declare @a float
	declare @distance float

	-- Sets average radius of Earth in Kilometers
	set @radius = 3959.0E

	-- Convert degrees to radians
	set @lon1 = radians( @Longitude1 )
	set @lon2 = radians( @Longitude2 )
	set @lat1 = radians( @Latitude1 )
	set @lat2 = radians( @Latitude2 )

	set @a = sqrt(square(sin((@lat2-@lat1)/2.0E)) + 
		(cos(@lat1) * cos(@lat2) * square(sin((@lon2-@lon1)/2.0E))) )

	set @distance =
		@radius * ( 2.0E *asin(case when 1.0E < @a then 1.0E else @a end ))

	return @distance

end
go

create procedure ClosestShowroom( @lat float, @long float ) as
begin
	select Showrooms.* from Showrooms 
		inner join ZipCodeInfo on Showrooms.Zip = ZipCodeInfo.ZipCode
	order by dbo.DistanceBetweenPoints( @lat, @long, ZipCodeInfo.Latitude, ZipCodeInfo.Longitude )
end
go
/*
exec ClosestShowroom 38.093604, -78.561139 

select * from zipcodeinfo where zipcode in ('46074', '22901')

select dbo.DistanceBetweenPoints( 38.093604, -78.561139, 40.097028, -86.125716 )

select sqrt(square(-78.561139+86.125716) + square(40.097028-38.093604)) * 69
*/
go
/**********************************
 * db0044_Users_ImageFileName.sql *
 **********************************/
alter table [Users] add [ImageFileName] [nvarchar](50) null
go

go
/*******************************
 * db0045_CollateralTables.sql *
 *******************************/
create table [CollateralTypes] (
	[CollateralTypeID] [int] identity(1,1) not null,
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkCollateralTypes] PRIMARY KEY CLUSTERED ([CollateralTypeID])
) on [primary]
go

create table [CollateralItems] (
	[CollateralID] [int] identity(1,1) not null,
	[Name] [nvarchar](100) not null,
	[CollateralTypeID] [int] null constraint [fkCollateral_Type] foreign key references [CollateralTypes]([CollateralTypeID]),
	[Description] [ntext] null,
	[LeadTime] [nvarchar](20) null,
	[Status] [int] not null,
	[StatusDate] [datetime] null,
	[Quantity] [int] not null,
	[ImageFileName] [nvarchar](100) null,
	[Price] [float] null,
	[Shipping] [float] null,
	[Weight] [float] null,
	[IsGroup] [bit] not null constraint [defIsGroup] default(0),
	CONSTRAINT [pkCollateralItems] PRIMARY KEY CLUSTERED ([CollateralID])
) on [primary]
go

create table [CollateralGroupItems] (
	[GroupID] [int] not null constraint [fkCGI_Parent] foreign key references [CollateralItems]([CollateralID]),
	[CollateralID] [int] not null constraint [fkCGI_Children] foreign key references [CollateralItems]([CollateralID]),
	[Quantity] [int] not null,
	CONSTRAINT [pkCollateralGroupItems] PRIMARY KEY CLUSTERED ([GroupID], [CollateralID])
) on [primary]
go

insert into [CollateralTypes] ([Name]) values 
	('Catalog'),
	('Finish Sample'),
	('Fabric Sample'),
	('Brochure'),
	('Pull Sample'),
	('Pricelist'),
	('Other')
go
go
/*******************************
 * db0046_MoreSpecRequests.sql *
 *******************************/
alter table [SpecRequests] add
	[PaoliSalesRepMemberID] [int] null constraint [fkSpecRequest_PSRMember] foreign key references [Users](UserID),
	
	[EndCustomer] [nvarchar](100) null,
	[ProjectSize] [nvarchar](100) null,
	[QuoteDueDate] [datetime] null,
	
	[NeedFloorplanSpecs] [bit] not null constraint [defNeedFloorplanSpecs] default(0),
	[Need3DDrawing] [bit] not null constraint [defNeed3DDrawing] default(0),
	[NeedValueEng] [bit] not null constraint [defNeedValueEng] default(0),
	[NeedPhotoRendering] [bit] not null constraint [defNeedPhotoRendering] default(0),
	[Need2DDrawing] [bit] not null constraint [defNeed2DDrawing] default(0),
	[NeedAuditSpecs] [bit] not null constraint [defNeedAuditSpecs] default(0),
	
	[Casegoods] [nvarchar](max) null,
	[Conferencing] [nvarchar](max) null,
	[Seating] [nvarchar](max) null,
	[Finishes] [nvarchar](max) null,
	[OtherFinishDetails] [nvarchar](max) null,
	[Grommets] [bit] not null constraint [defGrommets] default(0),
	[GrommetDetails] [ntext] null,
	[DrawerOption] [nvarchar](100) null,
	[FabricGrade] [nvarchar](5) null,
	[FabricDetails] [nvarchar](100) null,
	
	[SpecialRequests] [ntext] null
go

alter table [SpecRequests] drop constraint [defNeedFloorplanSpecs]
alter table [SpecRequests] drop constraint [defNeed3DDrawing]
alter table [SpecRequests] drop constraint [defNeedValueEng]
alter table [SpecRequests] drop constraint [defNeedPhotoRendering]
alter table [SpecRequests] drop constraint [defNeed2DDrawing]
alter table [SpecRequests] drop constraint [defNeedAuditSpecs]
alter table [SpecRequests] drop constraint [defGrommets]
go

update SpecRequests set [Casegoods] = null, [Conferencing] = null, [Seating] = null 
go
set nocount on
go
declare @catCasegoods int, @catSeating int, @catTables int
select @catCasegoods = CategoryId from Categories where Name = 'Casegood'
select @catSeating = CategoryId from Categories where Name = 'Seating'
select @catTables = CategoryId from Categories where Name = 'Tables'

declare @reqId int, @idx int, @newIdx int, @newCat int
declare @fSeries nvarchar(100), @seriesList nvarchar(max), @fullList nvarchar(max), @newSeries nvarchar(100)
declare @fullCasegoods nvarchar(max), @fullSeating nvarchar(max), @fullTables nvarchar(max)
declare c1 cursor local for select RequestID, FeaturedSeries, SeriesList from SpecRequests 
open c1
fetch next from c1 into @reqId, @fSeries, @seriesList
while @@fetch_status = 0
begin
	set @fullList = @fSeries + ',' + ISNULL(@seriesList,N'')
	set @idx = 0
	select @fullCasegoods = null, @fullSeating = null, @fullTables = null
	
	while( @idx < LEN(@fullList) )
	begin
		set @newCat = null
		set @newIdx = CHARINDEX( ',', @fullList, @idx )
		if( @newIdx = 0 )
		begin
			set @newIdx = LEN(@fullList) + 1
		end
--		print 'do substring ['+cast(LEN(@fullList) as nvarchar(10))+'] ['+cast(@idx as nvarchar(10))+'] ['+cast(@newIdx as nvarchar(10))+']' + @fullList
		set @newSeries = SUBSTRING(@fullList, @idx, @newIdx - @idx)
		if( LEN(@newSeries) > 0 )
		begin
			select @newCat = CategoryId from Serieses where Name = rtrim(ltrim(@newSeries))
			if( @newCat = @catCasegoods )
			begin
				set @fullCasegoods = isnull(@fullCasegoods, '') + (case when @fullCasegoods <> '' then ',' else '' end) + @newSeries
			end
			else if( @newCat = @catSeating )
			begin
				set @fullSeating = isnull(@fullSeating, '') + (case when @fullSeating <> '' then ',' else '' end) + @newSeries
			end
			else if( @newCat = @catTables )
			begin
				set @fullTables = isnull(@fullTables, '') + (case when @fullTables <> '' then ',' else '' end) + @newSeries
			end
			else
			begin
				print 'unable to find [' + @newseries + ']'
			end
		end
		set @idx = @newIdx + 1
	end
	
	update SpecRequests set [Casegoods] = @fullCasegoods, [Conferencing] = @fullTables, [Seating] = @fullSeating 
		where RequestID = @reqId

	fetch next from c1 into @reqId, @fSeries, @seriesList
end
close c1
deallocate c1
go
go
/**************************************
 * db0047_CollateralItemsLeadTime.sql *
 **************************************/
alter table [CollateralItems] alter column [LeadTime] [nvarchar](max) null
go
go
/*********************
 * db0048_Orders.sql *
 *********************/
alter table [CollateralItems] drop column [Status]
go

create table [CollateralOrders] (
	[OrderID] [int] not null identity(1,1),
	[RequestingParty] [int] not null,
	[MemberFirstName] [nvarchar](50) null,
	[MemberLastName] [nvarchar](50) null,
	[PaoliRepGroupID] [int] null constraint [fkCOrder_SalesRep] foreign key references [Companies](CompanyID),
	[PaoliRepGroupMemberID] [int] null constraint [fkCOrder_SalesRepMember] foreign key references [Users](UserID),
	[DealerID] [int] null constraint [fkCOrder_Dealer] foreign key references [Companies](CompanyID),
	[DealerMemberID] [int] null constraint [fkCOrder_DealerMember] foreign key references [Users](UserID),
	[EndUserFirstName] [nvarchar](50) null,
	[EndUserLastName] [nvarchar](50) null,
	[EndUserPhoneNumber] [nvarchar](20) null,
	[EndUserEMailAddress] [nvarchar](50) null,
	[ShippingType] [int] not null,
	[ShippingAddressType] [int] not null,
	[ShippingFedexAccount] [nvarchar](50) null,
	[ShippingFirstName] [nvarchar](50) null,
	[ShippingLastName] [nvarchar](50) null,
	[ShippingCompanyName] [nvarchar](50) null,
	[ShippingAddress1] [nvarchar](100) null,
	[ShippingAddress2] [nvarchar](100) null,
	[ShippingCity] [nvarchar](50) null,
	[ShippingState] [nvarchar](5) null,
	[ShippingZip] [nvarchar](15) null,
	[ShippingPhoneNumber] [nvarchar](20) null,
	[ShippingEmailAddress] [nvarchar](50) null,
	[OrderDate] [datetime] not null,
	CONSTRAINT [pkCollateralOrders] PRIMARY KEY CLUSTERED ([OrderID])
) on [primary]
go


create table [CollateralOrderDetails] (
	[DetailID] [int] not null identity(1,1),
	[OrderID] [int] not null constraint [fkCODetail_Order] foreign key references [CollateralOrders]([OrderID]),
	[CollateralID] [int] not null constraint [fkCODetail_Collateral] foreign key references [CollateralItems]([CollateralID]),
	[Quantity] [int] not null,
	CONSTRAINT [pkCollateralOrderDetails] PRIMARY KEY CLUSTERED ([DetailID])	
) on [primary]
go
go
/*********************************
 * db0049_Orders_PaoliMember.sql *
 *********************************/
alter table [CollateralOrders] add 	
	[PaoliMemberID] [int] null constraint [fkCOrder_PaoliMember] foreign key references [Users](UserID)
go
alter table [CollateralOrders] drop column [MemberFirstName]
alter table [CollateralOrders] drop column [MemberLastName]
go
DBCC CHECKIDENT ('CollateralOrders', RESEED, 10000)
go
alter table [CollateralOrders] add [Status] [int] not null constraint [defmj1] default(1)
alter table [CollateralOrders] drop constraint [defmj1]
go
go
/**********************************
 * db0050_Orders_ShippingAttn.sql *
 **********************************/
alter table [CollateralOrders] add [ShippingAttn] [nvarchar](200) null
go
update [CollateralOrders] set [ShippingAttn] = [ShippingFirstName] + ' ' + [ShippingLastName]
go
alter table [CollateralOrders] drop column [ShippingFirstName]
alter table [CollateralOrders] drop column [ShippingLastName]
go

go
/************************************
 * db0051_SpecRequest_CreatedBy.sql *
 ************************************/
alter table [SpecRequests] add 	
	[CreatedByUserId] [int] null constraint [fkSRequest_CreatedBy] foreign key references [Users](UserID)
go
alter table [SpecRequests] drop column [QuoteDueDate]
go
go
/*****************************
 * db0052_OrderShipments.sql *
 *****************************/
alter table [CollateralOrderDetails] add [GroupID] [int] null constraint [fkCOD_Group] foreign key references [CollateralItems](CollateralID)
go

insert into [CollateralOrderDetails] ([CollateralID], [OrderID], [Quantity], [GroupID])
	(select cgi.[CollateralID], cod.[OrderID], cod.[Quantity] * cgi.Quantity, cod.CollateralID from CollateralOrderDetails cod
		inner join CollateralGroupItems cgi on cod.CollateralID = cgi.[GroupID])

delete from CollateralOrderDetails where CollateralID in (select CollateralID from CollateralItems where IsGroup = 1)
go

alter table [CollateralOrders] add [CreatedByUserID] [int] null
go
update [CollateralOrders] set [CreatedByUserID] = (select top 1 UserID from Users where Email like '%vitaminj%')
go
alter table [CollateralOrders] alter column [CreatedByUserID] [int] not null 
alter table [CollateralOrders] add constraint [fkCO_CreatedUser] foreign key ([CreatedByUserID]) references [Users](UserID)
go

create table [CollateralOrderShipments] (
	[ShipmentID] [int] not null identity(1, 1),
	[OrderID] [int] not null constraint [fkShipment_COrder] foreign key references [CollateralOrders]([OrderID]),
	[Vendor] nvarchar(100) null,
	[TrackingNumber] nvarchar(100) null,
	[GLCode] nvarchar(100) null,
	[ShippingType] nvarchar(100) null,
	[ShippingFedexAccount] nvarchar(50) null,
	[ShippingAttn] nvarchar(200) null,
	[ShippingCompanyName] nvarchar(50) null,
	[ShippingAddress1] nvarchar(100) null,
	[ShippingAddress2] nvarchar(100) null,
	[ShippingCity] nvarchar(50) null,
	[ShippingState] nvarchar(5) null,
	[ShippingZip] nvarchar(15) null,
	[ShippingPhoneNumber] nvarchar(20) null,
	[ShippingEmailAddress] nvarchar(100) null,
	CONSTRAINT [pkCollateralOrderShipments] PRIMARY KEY CLUSTERED ([ShipmentID])	
) on [primary]
go

create table [CollateralOrderShipmentDetails] (
	[DetailID] [int] not null identity(1, 1),
	[ShipmentID] [int] not null constraint [fkShipmentDetail_Shipment] foreign key references [CollateralOrderShipments]([ShipmentID]),
	[OrderDetailID] [int] not null constraint [fkShipmentDetail_CODetail] foreign key references [CollateralOrderDetails]([DetailID]),
	[Quantity] [int] not null,
	CONSTRAINT [pkCollateralOrderShipmentDetails] PRIMARY KEY CLUSTERED ([DetailID])	
) on [primary]
go

alter table [CollateralOrders] add [CanceledByUserID] [int] null constraint [fkCO_CanceledUser] foreign key references [Users](UserID)
alter table [CollateralOrders] add [CanceledOnDateTime] [datetime] null
go

go
/**************************************
 * db0053_OrderAndRequest_Updates.sql *
 **************************************/
alter table [SpecRequests] add [LaminatePreference] [nvarchar](10) null
go

alter table [CollateralOrderShipments] add [ShipmentDate] [datetime] not null constraint [defshipmentdate] default(getutcdate())
alter table [CollateralOrderShipments] drop constraint [defshipmentdate]
go

alter table [SpecRequestFiles] add [IsSpecTeam] [bit] not null constraint [defIsSpecTeam] default(0)
alter table [SpecRequestFiles] drop constraint [defIsSpecTeam]
go

alter table [SpecRequests] add [DealerPOCText] [nvarchar](max) null
go

alter table [CollateralOrders] add
	[ShippingParty] [int] not null constraint [defShippingParty] default(0),
	[SPPaoliMemberID] [int] null constraint [fkCOrder_SPPaoliMember] foreign key references [Users](UserID),
	[SPPaoliRepGroupID] [int] null constraint [fkCOrder_SPSalesRep] foreign key references [Companies](CompanyID),
	[SPPaoliRepGroupMemberID] [int] null constraint [fkCOrder_SPSalesRepMember] foreign key references [Users](UserID),
	[SPDealerID] [int] null constraint [fkCOrder_SPDealer] foreign key references [Companies](CompanyID),
	[SPDealerMemberID] [int] null constraint [fkCOrder_SPDealerMember] foreign key references [Users](UserID),
	[SPEndUserFirstName] [nvarchar](50) null,
	[SPEndUserLastName] [nvarchar](50) null,
	[SPEndUserPhoneNumber] [nvarchar](20) null,
	[SPEndUserEMailAddress] [nvarchar](50) null
alter table [CollateralOrders] drop constraint [defShippingParty]
go

select [OrderID], [RequestingParty], [PaoliMemberID], [PaoliRepGroupID], [DealerID], [EndUserFirstName] from [CollateralOrders]
go
/*
update [CollateralOrders] set [ShippingParty] = 2, [SPDealerID] = [DealerID] where OrderID in (10000, 10001, 10010, 10011)
update [CollateralOrders] set [ShippingParty] = 1, [SPPaoliRepGroupID] = [PaoliRepGroupID], [SPPaoliRepGroupMemberID] = [PaoliRepGroupMemberID] where OrderID in (10002, 10007)
*/

create function dbo.GetPartyName( @partyType int, @paoliMemberID int, @salesRepID int, @salesRepMemberID int,
	@dealerID int, @dealerMemberID int, @endUserFirst nvarchar(100), @endUserLast nvarchar(100) )
returns nvarchar(300)
begin
	declare @companyID int, @userID int
	select @companyID = null, @userID = null
	
	if( @partyType = 0 )
	begin
		set @userID = @paoliMemberID
	end
	else if( @partyType = 1 )
	begin
		set @companyID = @salesRepID
		set @userID = @salesRepMemberID
	end
	else if( @partyType = 2 )
	begin
		set @companyID = @dealerID
		set @userID = @dealerMemberID
	end
	else if( @partyType = 3 )
	begin
		return @endUserFirst + ' ' + @endUserLast
	end
	
	declare @displayName nvarchar(300)
	if( @userID is not null )
	begin
		select @displayName = c.Name + ' - ' + u.FirstName + ' ' + u.LastName
			from Users u inner join Companies c on u.CompanyID = c.CompanyID
			where u.UserID = @userID
	end
	else if( @companyID is not null )
	begin
		select @displayName = Name 
			from Companies 
			where CompanyID = @companyID
	end
	return @displayName
end
go

alter table [CollateralOrders] add [RequestingPartyName] as [dbo].GetPartyName( RequestingParty, PaoliMemberID, PaoliRepGroupID, PaoliRepGroupMemberID,
	DealerID, DealerMemberID, EndUserFirstName, EndUserLastName )
alter table [CollateralOrders] add [ShippingPartyName] as [dbo].GetPartyName( ShippingParty, SPPaoliMemberID, SPPaoliRepGroupID, SPPaoliRepGroupMemberID,
	SPDealerID, SPDealerMemberID, SPEndUserFirstName, SPEndUserLastName )
go
go
/*****************************************
 * db0054_Companies_BusinessUnitName.sql *
 *****************************************/
alter table [Companies] add [BusinessUnitName] [nvarchar](200) null
go

alter table [CollateralOrders] drop column RequestingPartyName
alter table [CollateralOrders] drop column ShippingPartyName
go

alter function dbo.GetPartyName( @partyType int, @paoliMemberID int, @salesRepID int, @salesRepMemberID int,
	@dealerID int, @dealerMemberID int, @endUserFirst nvarchar(100), @endUserLast nvarchar(100) )
returns nvarchar(300)
begin
	declare @companyID int, @userID int
	select @companyID = null, @userID = null
	
	if( @partyType = 0 )
	begin
		set @userID = @paoliMemberID
	end
	else if( @partyType = 1 )
	begin
		set @companyID = @salesRepID
		set @userID = @salesRepMemberID
	end
	else if( @partyType = 2 )
	begin
		set @companyID = @dealerID
		set @userID = @dealerMemberID
	end
	else if( @partyType = 3 )
	begin
		return @endUserFirst + ' ' + @endUserLast
	end
	
	declare @displayName nvarchar(300)
	if( @userID is not null )
	begin
		select @displayName = c.Name + case when c.BusinessUnitName is not null then ' - ' + c.BusinessUnitName else '' end + ' - ' + u.FirstName + ' ' + u.LastName
			from Users u inner join Companies c on u.CompanyID = c.CompanyID
			where u.UserID = @userID
	end
	else if( @companyID is not null )
	begin
		select @displayName = Name + case when BusinessUnitName is not null then ' - ' + BusinessUnitName else '' end
			from Companies 
			where CompanyID = @companyID
	end
	return @displayName
end
go

alter table [CollateralOrders] add [RequestingPartyName] as [dbo].GetPartyName( RequestingParty, PaoliMemberID, PaoliRepGroupID, PaoliRepGroupMemberID,
	DealerID, DealerMemberID, EndUserFirstName, EndUserLastName )
alter table [CollateralOrders] add [ShippingPartyName] as [dbo].GetPartyName( ShippingParty, SPPaoliMemberID, SPPaoliRepGroupID, SPPaoliRepGroupMemberID,
	SPDealerID, SPDealerMemberID, SPEndUserFirstName, SPEndUserLastName )
go
go
/***********************************
 * db0055_Typicals_IsPublished.sql *
 ***********************************/
alter table [Typicals] add [IsPublished] [bit] not null constraint [defIsPub] default(1)
alter table [Typicals] drop constraint [defIsPub]
go
go
/************************************************
 * db0056_CollateralOrders_MultipleTracking.sql *
 ************************************************/
alter table CollateralOrderShipments add 
	[TrackingNumber1] [nvarchar](100) null,
	[TrackingNumber2] [nvarchar](100) null,
	[TrackingNumber3] [nvarchar](100) null,
	[TrackingNumber4] [nvarchar](100) null
go
update CollateralOrderShipments set [TrackingNumber1] = [TrackingNumber]
go
alter table CollateralOrderShipments
	drop column [TrackingNumber]
go

go
/************************************
 * db0057_Companies_PaoliMember.sql *
 ************************************/
alter table [Companies] add [PaoliMemberID] [int] null constraint [fkCompany_PaoliMember] foreign key references [Users](UserID)
go

go
/**********************************
 * db0058_User_IsTempPassword.sql *
 **********************************/
alter table [Users] add [IsTempPassword] [bit] not null constraint [defITP] default(0)
alter table [Users] drop constraint [defITP]
go
go
/******************************************
 * db0059_CollateralOrderDetailsSplit.sql *
 ******************************************/
alter table CollateralOrderDetails drop constraint fkCOD_Group
alter table CollateralOrderDetails drop constraint fkCODetail_Collateral
go
alter table CollateralOrderDetails add 
	[ItemName] [nvarchar](100) null,
	[GroupName] [nvarchar](100) null,
	[GroupQuantity] [int] null,
	[CollateralTypeNKID] [int] not null constraint [defCOD1] default(0),
	[CollateralTypeName] [nvarchar](100) null,
	[CollateralNKID] [int] not null constraint [defCOD2] default(0)
alter table CollateralOrderDetails drop constraint [defCOD1]
alter table CollateralOrderDetails drop constraint [defCOD2]
go
update CollateralOrderDetails set [CollateralNKID] = c.CollateralID, [ItemName] = c.Name, [CollateralTypeNKID] = ct.CollateralTypeID, 
	[CollateralTypeName] = ct.Name, [GroupName] = g.Name, [GroupQuantity] = cgi.Quantity
from CollateralOrderDetails cod
	inner join CollateralItems c on cod.CollateralID = c.CollateralID
	inner join CollateralTypes ct on c.CollateralTypeID = ct.CollateralTypeID
	left join CollateralItems g on cod.GroupID = g.CollateralID
	left join CollateralGroupItems cgi on cgi.CollateralID = c.CollateralID and cgi.GroupID = cod.GroupID
go
alter table CollateralOrderDetails drop column CollateralID
go
alter table CollateralOrderDetails add [GroupNKID] [int] null
go
update CollateralOrderDetails set [GroupNKID] = [GroupID]
go
alter table CollateralOrderDetails drop column [GroupID]
go

alter table CollateralItems add [IsActive] [bit] not null constraint [defCIActive] default(1)
alter table CollateralItems drop constraint [defCIActive]
go
--select * from CollateralOrderDetails
go
/*************************************
 * db0060_SpecRequests_NeedFiles.sql *
 *************************************/
alter table [SpecRequests] add 
	[NeedDWGFiles] [bit] not null constraint [defSRDWG] default(0),
	[NeedSP4Files] [bit] not null constraint [defSRSP4] default(0),
	[NeedSIFFiles] [bit] not null constraint [defSRSIF] default(0),
	[NeedXLSFiles] [bit] not null constraint [defSRXLS] default(0),
	[NeedPDFFiles] [bit] not null constraint [defSRPDF] default(0)
go

alter table [SpecRequests] drop constraint [defSRDWG]
alter table [SpecRequests] drop constraint [defSRSP4]
alter table [SpecRequests] drop constraint [defSRSIF]
alter table [SpecRequests] drop constraint [defSRXLS]
alter table [SpecRequests] drop constraint [defSRPDF]
go
go
/*******************************************
 * db0061_SpecRequests_GrommetPosition.sql *
 *******************************************/
alter table [SpecRequests] add [GrommetPosition] [ntext] null
go
go
/********************************************
 * db0062_Companies_PaoliSalesRepMember.sql *
 ********************************************/
alter table [Companies] add [PaoliSalesRepMemberID] [int] null constraint [fkCompany_PaoliSalesRepMember] foreign key references [Users](UserID)
go

go
/***********************************
 * db0063_SpecRequest_Canceled.sql *
 ***********************************/
alter table [SpecRequests] add
	[CompletedByUserID] [int] null CONSTRAINT fkSpecRequest_CompletedUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[CompletedDateTime] [datetime] null,
	[IsCanceled] [bit] not null constraint [defIsC] default(0),
	[CanceledByUserID] [int] null CONSTRAINT fkSpecRequest_CanceledUser FOREIGN KEY REFERENCES [Users]([UserID]),
	[CanceledDateTime] [datetime] null
go

alter table [SpecRequests] drop constraint [defIsC]
go
go
/****************************************
 * db0064_SpecRequest_SpecTeamNotes.sql *
 ****************************************/
alter table [SpecRequests] add [SpecTeamNotes] [ntext] null
go
go
/***************************
 * db0065_GSAContracts.sql *
 ***************************/
create table [GSAContracts] (
	[ContractID] [int] not null identity(1,1),
	[Name] [nvarchar](100) not null,
	CONSTRAINT [pkGSAContracts] PRIMARY KEY CLUSTERED ([ContractID])
) on [primary]
go

insert into [GSAContracts] ([Name]) values
	('Federal - GS-28F-0006T'),
	('Alabama: 4008429 T390 Office Furniture'),
	('California: CMAS 4-13-71-0104B'),
	('Connecticut: 07PSX0285'),
	('Florida: 425-001-12-1  (View Complete Contract Info)'),
	('Kentucky: MA-758 1100000660 1'),
	('Mississippi: 5-420-24820-12'),
	('New Jersey: 81636'),
	('New Mexico: Price Agreement Number 30-000-00-00015'),
	('North Carolina: 425-B Wood Desks, Credenzas, Conference Tables, etc.'),
	('Pennsylvania: 4400009465'),
	('South Carolina: 5000011597'),
	('Texas: TXMAS-7-711080 (View Complete Contract Info)'),
	('Wisconsin: 15-42594-900'),
	('Baltimore Metro Contract 2008-01'),
	('BuyBoard Cooperative Contract: Buyboard'),
	('IUCPG (Ohio State University) 05-Office Furniture'),
	('Lockheed Martin: NPG020'),
	('Miami-Dade County: 1072-1/16-1'),
	('Navy BPA: N00189-11-A-0126'),
	('NCPA: NCPA07-14')
go

alter table [SpecRequests] add [ContractID] [int] null CONSTRAINT fkSpecRequest_GSAContract FOREIGN KEY REFERENCES [GSAContracts]([ContractID])
go
go
/*****************************
 * db0066_Users_IsActive.sql *
 *****************************/
alter table [users] add [IsActive] [bit] not null constraint [defUIA] default(1)
go
alter table [Users] drop constraint [defUIA]
go
go
/*********************************************
 * db0067_CollateralOrders_CorrectStatus.sql *
 *********************************************/

declare @bHasFulfilled bit, @bHasUnfulfilled bit, @bHasPartial bit
declare @OrderID int, @Quantity int, @ShippedQuantity int, @Status int

declare @SPending int, @SPartial int, @SFulfilled int, @SCanceled int
select @SPending = 1, @SPartial = 2, @SFulfilled = 3, @SCanceled = 4

declare c1 cursor local for select OrderID from CollateralOrders where CanceledByUserID is null
open c1
fetch next from c1 into @OrderID
while @@fetch_status = 0
begin
	select @bHasFulfilled = 0, @bHasUnfulfilled = 0, @bHasPartial = 0

	declare c2 cursor local for select co.Quantity, coalesce((select SUM(cosd.Quantity) from CollateralOrderShipmentDetails cosd where cosd.OrderDetailID = co.DetailID), 0) from CollateralOrderDetails co where co.OrderID = @OrderID
	open c2
	fetch next from c2 into @Quantity, @ShippedQuantity
	while @@fetch_status = 0
	begin
		set @bHasFulfilled = case when (@Quantity = @ShippedQuantity) or (@bHasFulfilled = 1) then 1 else 0 end
		set @bHasUnfulfilled = case when (0 = @ShippedQuantity) or (@bHasUnfulfilled = 1) then 1 else 0 end
		set @bHasPartial = case when ( (@ShippedQuantity > 0) and (@Quantity > @ShippedQuantity) ) or (@bHasPartial = 1) then 1 else 0 end
		
		fetch next from c2 into @Quantity, @ShippedQuantity
	end
	close c2
	deallocate c2
	
	set @Status = @SPartial
	if( @bHasFulfilled = 0 and @bHasPartial = 0 )
	begin
		set @Status = @SPending
	end
	else if( @bHasUnfulfilled = 0 and @bHasPartial = 0 )
	begin
		set @Status = @SFulfilled
	end
	
	update CollateralOrders set [Status] = @Status where OrderID = @OrderID

	fetch next from c1 into @OrderID
end

close c1
deallocate c1

go
/*************************
 * db0068_UserLogins.sql *
 *************************/
create table [UserLogins] (
	[LoginRecID] [int] identity(1,1) not null,
	[UserID] [int] not null CONSTRAINT fkLogin_User FOREIGN KEY REFERENCES [Users]([UserID]),
	[LoginDate] [datetime] not null,
	CONSTRAINT [pkUserLogins] PRIMARY KEY CLUSTERED ([LoginRecID])	
) on [PRIMARY]
go

go
/**************************************
 * db0069_ImageFiles_FinishFields.sql *
 **************************************/
alter table [ImageFiles] add
	[LaminatePattern] [int] null,
	[LaminateType] [int] null,
	[VeneerGrade] [int] null,
	[VeneerSpecies] [int] null,
	[VeneerColorTone] [int] null
go

update [ImageFiles] set [LaminatePattern] = [FinishSubType] where [FinishType] = 2
update [ImageFiles] set [VeneerSpecies] = [FinishSubType] where [FinishType] = 1
go

alter table [ImageFiles] drop column [FinishSubType]
go

alter table [ImageFiles] drop column [LaminateType]
go

alter table [ImageFiles] add 
	[LaminateIsHPL] [bit] not null constraint [defLHPL] default(0),
	[LaminateIsLPL] [bit] not null constraint [defLLPL] default(0)
go

alter table [ImageFiles] drop constraint [defLHPL]
alter table [ImageFiles] drop constraint [defLLPL]
go

alter table [ImageFiles] drop column [VeneerColorTone]
go

go
/*************************************
 * db0070_SpecRequests_DealerPOC.sql *
 *************************************/
alter table [SpecRequests] add
	[DealerPOCEmail] [nvarchar](200) null,
	[DealerPOCPhone] [nvarchar](20) null
go

go
