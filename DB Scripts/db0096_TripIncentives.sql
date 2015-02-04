alter table [Companies] drop column [TripIncentive]
go

alter table [Projects] add [TerritoryID] [int] null CONSTRAINT [fkProject_Territory] FOREIGN KEY REFERENCES [Territories] ([TerritoryID])
go

update [Projects] set [TerritoryID] = [c].[TerritoryID]
	from [Projects] [p]
		inner join [Companies] [c] on [p].[DealerID] = [c].[CompanyID] 
go
