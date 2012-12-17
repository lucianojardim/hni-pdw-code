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
