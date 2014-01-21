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

update [Companies] set [Companies].[TerritoryID] = [t].[TerritoryID]
	from [Companies] [c]
		left join [ImportCompanies] [ic] on [c].[Name] = [ic].[Company Name] 
		left join [Territories] [t] on [ic].[Territory ID] = [t].[Name] 
