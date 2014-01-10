create table [CollateralTypes] (
	[CollateralTypeID] [int] identity(1,1) not null,
	[Name] [nvarchar](50) not null,
	CONSTRAINT [pkCollateralTypes] PRIMARY KEY CLUSTERED ([CollateralTypeID])
) on [primary]
go

create table [CollateralItems] (
	[CollateralID] [int] identity(1,1) not null,
	[Name] [nvarchar](100) not null,
	[CollateralTypeID] [int] not null constraint [fkCollateral_Type] foreign key references [CollateralTypes]([CollateralTypeID]),
	[Description] [ntext] null,
	[LeadTime] [nvarchar](20) null,
	[WaitTime] [nvarchar](20) null,
	[Status] [int] not null,
	[StatusDate] [datetime] null,
	[Quantity] [int] not null,
	[ImageFileName] [nvarchar](100) null,
	[Price] [float] null,
	[Shipping] [float] null,
	CONSTRAINT [pkCollateralItems] PRIMARY KEY CLUSTERED ([CollateralID])
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