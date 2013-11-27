/*
drop table [SpecRequestFile]
drop table [SpecRequests]
go
*/
create table [SpecRequests] (
	[RequestID] [int] identity(1,1) not null,
	[Name] [nvarchar](10) null,
	[ProjectName] [nvarchar](50) null,
	[PaoliSalesRepGroupID] [int] null CONSTRAINT [fkSpec_RepGroup] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[CompanyID] [int] null CONSTRAINT [fkSpec_Company] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[DealerSalesRep] [int] null CONSTRAINT [fkSpec_DealerSalesRep] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[RequestDate] [datetime] null,
	[IsGSA] [bit] null,
	[SavedLocation] [nvarchar](500) null,
	[ListPrice] [int] null,
	[FeaturedSeries] [nvarchar](50) null,
	[SeriesList] [ntext] null,
	[Received] [bit] null,
	[SPLQuote] [int] null,
	[PaoliSpecTeamMember] [int] null CONSTRAINT [fkSpec_PaoliSpecTeam] FOREIGN KEY REFERENCES [Users] ([UserID]),
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

create table [SpecRequestFile] (
	[FileID] [int] identity(1,1) not null,
	[RequestID] [int] not null CONSTRAINT [fkSpecFile_Request] FOREIGN KEY REFERENCES [SpecRequests] ([RequestID]),
	[Extension] [nvarchar](10) not null,
	[Name] [nvarchar](100) not null,
	[VersionNumber] [int] not null,
	[UploadDate] [datetime] not null,
	CONSTRAINT [pkSpecRequestFile] PRIMARY KEY CLUSTERED ([FileID])
) on [primary]
go