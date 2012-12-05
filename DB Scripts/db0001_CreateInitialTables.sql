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
