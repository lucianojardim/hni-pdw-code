create table [ZipCodeInfo] (
	[ZipCode] [nvarchar](10) not null,
	[TerritoryID] [int] not null constraint [fkZip_Territory] foreign key references [Territories](TerritoryID),
	[Longitude] [float] not null,
	[Latitude] [float] not null,
	CONSTRAINT [pkZipCodeInfo] PRIMARY KEY CLUSTERED ([ZipCode])
) on [primary]
go

delete from [ImportZipCodes] where [State] in ('GU')
update [ImportZipCodes] set [Territory ID] = 6227 where [Territory ID] = 6627
go

insert into [ZipCodeInfo] ([ZipCode] , [TerritoryID] , [Latitude] , [Longitude])
	(select [Zip], [Territory ID], [Latitude], [Longitude] from [ImportZipCodes])
go
