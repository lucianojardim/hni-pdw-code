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
