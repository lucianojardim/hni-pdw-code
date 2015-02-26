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