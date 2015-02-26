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