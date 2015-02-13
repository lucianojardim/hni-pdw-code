alter table [Companies] drop column [TripIncentive]
go

alter table [Projects] add [TerritoryID] [int] null CONSTRAINT [fkProject_Territory] FOREIGN KEY REFERENCES [Territories] ([TerritoryID])
go

update [Projects] set [TerritoryID] = [c].[TerritoryID]
	from [Projects] [p]
		inner join [Companies] [c] on [p].[DealerID] = [c].[CompanyID] 
go

create table [CompanyTripData] (
	[CompanyID] [int] not null CONSTRAINT [fkTripData_Company] FOREIGN KEY REFERENCES [Companies] ([CompanyID]),
	[TotalSalesYTD] [money] null,
	[TotalTripsYTD] [int] null,
	[SalesToNextTrip] [money] null,
	[ShakaThreshold] [money] null,
	[ShakaPercentGrowthYTD] [money] null,
	[ShakaPercentGrowthRank] [int] null,
	[ShakaDollarGrowthYTD] [money] null,
	[ShakaDollarGrowthRank] [int] null,
	[AlohaThreshold] [money] null,
	[AlohaPercentGrowthYTD] [money] null,
	[AlohaPercentGrowthRank] [int] null,
	[AlohaDollarGrowthYTD] [money] null,
	[AlohaDollarGrowthRank] [int] null,
	[MahaloThreshold] [money] null,
	[MahaloPercentGrowthYTD] [money] null,
	[MahaloPercentGrowthRank] [int] null,
	[MahaloDollarGrowthYTD] [money] null,
	[MahaloDollarGrowthRank] [int] null,
	[ImportDate] [datetime] not null,
	constraint [pkCompanyTripData] primary key clustered ([CompanyID])	
) on [primary]
go
